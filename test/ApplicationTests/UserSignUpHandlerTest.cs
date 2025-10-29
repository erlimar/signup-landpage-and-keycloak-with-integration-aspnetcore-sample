// Copyright (c) 2025 Erlimar Silva Campos. All Rights Reserved.
// This file is a part of SignUpKeycloakGoogleIntegration

using FluentValidation;
using Moq;
using SignUpKeycloakGoogleIntegration.Application;
using SignUpKeycloakGoogleIntegration.Application.UserSignUp;

namespace SignUpKeycloakGoogleIntegration.ApplicationTests;

public class UserSignUpHandlerTest
{
    /// <summary>
    /// O manipulador requer um <see cref="IKeycloakGateway"/>
    /// </summary>
    [Fact]
    [Trait("target", nameof(UserSignUpHandler))]
    public void KeycloakGatewayEhObrigatorio()
    {
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(static () =>
            new UserSignUpHandler(null!)
        );

        Assert.Equal("keycloakGateway", ex.ParamName);
    }

    /// <summary>
    /// Ao manipular, um <see cref="UserSignUpCommand"/> é obrigatório
    /// </summary>
    [Fact]
    [Trait("target", nameof(UserSignUpHandler))]
    public async Task CommandEhObrigatorio()
    {
        IKeycloakGateway keycloakGateway = new Mock<IKeycloakGateway>().Object;
        UserSignUpHandler handler = new(keycloakGateway);

        ArgumentNullException ex = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            handler.HandleAsync(null!)
        );

        Assert.Equal("command", ex.ParamName);
    }

    /// <summary>
    /// Um <see cref="UserSignUpCommand"/> com dados válidos é necessário
    /// para a manipulação do comando
    /// </summary>
    [Theory]
    [MemberData(nameof(InvalidCommands))]
    [Trait("target", nameof(UserSignUpHandler))]
    public async Task RequerCommandValido(UserSignUpCommand invalidCommand)
    {
        IKeycloakGateway keycloakGateway = new Mock<IKeycloakGateway>().Object;
        UserSignUpHandler handler = new(keycloakGateway);

        _ = await Assert.ThrowsAsync<ValidationException>(() =>
            handler.HandleAsync(invalidCommand)
        );
    }

    /// <summary>
    /// Quando o comando é válido, <see cref="IKeycloakGateway"/> deve ser
    /// consultado para verificar a existência do usuário
    /// </summary>
    [Fact]
    [Trait("target", nameof(UserSignUpHandler))]
    public async Task KeycloakGatewayDeveSerConsultadoQuandoComandoEhValido()
    {
        UserSignUpCommand command = new()
        {
            GoogleId = "user-id",
            Email = "user@email.com",
            Name = "User Name",
        };

        Mock<IKeycloakGateway> keycloakGatewayMock = new();

        UserSignUpHandler handler = new(keycloakGatewayMock.Object);

        try
        {
            _ = await handler.HandleAsync(command);
        }
        catch (Exception) { }

        keycloakGatewayMock.Verify(v => v.GetUserIdByEmailAsync(command.Email), Times.Once);
    }

    /// <summary>
    /// Quando um usuário existe no Keycloak, devemos tentar obter seu link
    /// com o Google
    /// </summary>
    [Fact]
    [Trait("target", nameof(UserSignUpHandler))]
    public async Task QuandoUsuarioExisteNoKeycloakOLinkGoogleDeveSerConsultado()
    {
        string existingUserId = "existing-user-id";

        UserSignUpCommand command = new()
        {
            GoogleId = "user-id",
            Email = "user@email.com",
            Name = "User Name",
        };

        Mock<IKeycloakGateway> keycloakGatewayMock = new();

        _ = keycloakGatewayMock
            .Setup(m => m.GetUserIdByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(existingUserId);

        UserSignUpHandler handler = new(keycloakGatewayMock.Object);

        try
        {
            _ = await handler.HandleAsync(command);
        }
        catch (Exception) { }

        keycloakGatewayMock.Verify(v => v.GetUserIdByEmailAsync(command.Email), Times.Once);
        keycloakGatewayMock.Verify(v => v.GetGoogleLinkedIdAsync(existingUserId), Times.Once);
    }

    /// <summary>
    /// Quando um usuário está cadastrado sem vínculo algum com Google, o retorno deve
    /// informar <see cref="UserSignUpResponseType.RegisteredWithoutLink"/>
    /// </summary>
    [Fact]
    [Trait("target", nameof(UserSignUpHandler))]
    public async Task CadastroSemVinculoDeveSerInformado()
    {
        string existingUserId = "existing-user-id";

        UserSignUpCommand command = new()
        {
            GoogleId = "user-id",
            Email = "user@email.com",
            Name = "User Name",
        };

        Mock<IKeycloakGateway> keycloakGatewayMock = new();

        _ = keycloakGatewayMock
            .Setup(m => m.GetUserIdByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(existingUserId);

        _ = keycloakGatewayMock
            .Setup(m => m.GetGoogleLinkedIdAsync(existingUserId))
            .ReturnsAsync(null! as string);

        UserSignUpHandler handler = new(keycloakGatewayMock.Object);

        UserSignUpCommandResponse response = await handler.HandleAsync(command);

        Assert.NotNull(response);
        Assert.Equal(UserSignUpResponseType.RegisteredWithoutLink, response.ResponseType);
        Assert.Null(response.ResponseMessage);

        keycloakGatewayMock.Verify(v => v.GetUserIdByEmailAsync(command.Email), Times.Once);
        keycloakGatewayMock.Verify(v => v.GetGoogleLinkedIdAsync(existingUserId), Times.Once);
    }

    /// <summary>
    /// Quando um usuário está cadastrado com vínculo Google, mas esse vínculo
    /// é diferente do atual <see cref="UserSignUpResponseType.Failed"/> deve
    /// ser retornado com uma mensagem indicando o problema
    /// </summary>
    [Fact]
    [Trait("target", nameof(UserSignUpHandler))]
    public async Task CadastroComVinculoDiferenteGeraFalha()
    {
        string existingUserId = "existing-user-id";

        UserSignUpCommand command = new()
        {
            GoogleId = "user-id",
            Email = "user@email.com",
            Name = "User Name",
        };

        Mock<IKeycloakGateway> keycloakGatewayMock = new();

        _ = keycloakGatewayMock
            .Setup(m => m.GetUserIdByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(existingUserId);

        _ = keycloakGatewayMock
            .Setup(m => m.GetGoogleLinkedIdAsync(existingUserId))
            .ReturnsAsync("user-id-diferente");

        UserSignUpHandler handler = new(keycloakGatewayMock.Object);

        UserSignUpCommandResponse response = await handler.HandleAsync(command);

        Assert.NotNull(response);
        Assert.Equal(UserSignUpResponseType.Failed, response.ResponseType);
        Assert.NotNull(response.ResponseMessage);
        Assert.Equal("O usuário já está vinculado a outra conta Google", response.ResponseMessage);

        keycloakGatewayMock.Verify(v => v.GetUserIdByEmailAsync(command.Email), Times.Once);
        keycloakGatewayMock.Verify(v => v.GetGoogleLinkedIdAsync(existingUserId), Times.Once);
    }

    /// <summary>
    /// Quando um usuário está cadastrado com vínculo Google igual
    /// ao atual, indica que o usuário já está cadastrado
    /// </summary>
    [Fact]
    [Trait("target", nameof(UserSignUpHandler))]
    public async Task CadastroComVinculoIgualIndicaJaCadastrado()
    {
        string existingUserId = "existing-user-id";

        UserSignUpCommand command = new()
        {
            GoogleId = "user-id",
            Email = "user@email.com",
            Name = "User Name",
        };

        Mock<IKeycloakGateway> keycloakGatewayMock = new();

        _ = keycloakGatewayMock
            .Setup(m => m.GetUserIdByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(existingUserId);

        _ = keycloakGatewayMock
            .Setup(m => m.GetGoogleLinkedIdAsync(existingUserId))
            .ReturnsAsync("user-id");

        UserSignUpHandler handler = new(keycloakGatewayMock.Object);

        UserSignUpCommandResponse response = await handler.HandleAsync(command);

        Assert.NotNull(response);
        Assert.Equal(UserSignUpResponseType.AlreadyRegistered, response.ResponseType);
        Assert.Null(response.ResponseMessage);

        keycloakGatewayMock.Verify(v => v.GetUserIdByEmailAsync(command.Email), Times.Once);
        keycloakGatewayMock.Verify(v => v.GetGoogleLinkedIdAsync(existingUserId), Times.Once);
    }

    /// <summary>
    /// Quando o usuário não existir, esse deve ser cadastrado
    /// </summary>
    [Fact]
    [Trait("target", nameof(UserSignUpHandler))]
    public async Task QuandoUsuarioNaoExistirDeveSerCadastrado()
    {
        UserSignUpCommand command = new()
        {
            GoogleId = "user-id",
            Email = "user@email.com",
            Name = "User Name",
        };

        Mock<IKeycloakGateway> keycloakGatewayMock = new();

        _ = keycloakGatewayMock
            .Setup(m => m.GetUserIdByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(null! as string);

        _ = keycloakGatewayMock
            .Setup(m => m.WriteNewUser(command.Name, command.Email))
            .ReturnsAsync("novo-usuario-id-keycloak");

        UserSignUpHandler handler = new(keycloakGatewayMock.Object);

        UserSignUpCommandResponse response = await handler.HandleAsync(command);

        Assert.NotNull(response);
        Assert.Equal(UserSignUpResponseType.NewRegistration, response.ResponseType);
        Assert.Null(response.ResponseMessage);

        keycloakGatewayMock.Verify(v => v.GetUserIdByEmailAsync(command.Email), Times.Once);
        keycloakGatewayMock.Verify(v => v.GetGoogleLinkedIdAsync(It.IsAny<string>()), Times.Never);
        keycloakGatewayMock.Verify(v => v.WriteNewUser(command.Name, command.Email), Times.Once);
        keycloakGatewayMock.Verify(
            v => v.WriteGoogleLink("novo-usuario-id-keycloak", command.GoogleId),
            Times.Once
        );
    }

    // TODO: Testar quando tentar gravar usuário com o Gateway e não retornar um ID válido FAIL
    // TODO: Testar quando qualquer exceção ocorrer no Gateway FAIL com "Falha interna no gateway do serviço de identidade"

    public static IEnumerable<object[]> InvalidCommands =>
        [
            [
                new UserSignUpCommand
                {
                    GoogleId = "",
                    Name = "ValidName",
                    Email = "valid@email",
                },
            ],
            [
                new UserSignUpCommand
                {
                    GoogleId = "ValidId",
                    Name = "",
                    Email = "valid@email",
                },
            ],
            [
                new UserSignUpCommand
                {
                    GoogleId = "ValidId",
                    Name = "ValidName",
                    Email = "",
                },
            ],
        ];
}
