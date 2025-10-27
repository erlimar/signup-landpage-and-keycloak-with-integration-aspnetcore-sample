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

    public static IEnumerable<object[]> InvalidCommands =>
        [
            [
                new UserSignUpCommand
                {
                    Id = "",
                    Name = "ValidName",
                    Email = "valid@email",
                },
            ],
            [
                new UserSignUpCommand
                {
                    Id = "ValidId",
                    Name = "",
                    Email = "valid@email",
                },
            ],
            [
                new UserSignUpCommand
                {
                    Id = "ValidId",
                    Name = "ValidName",
                    Email = "",
                },
            ],
        ];
}
