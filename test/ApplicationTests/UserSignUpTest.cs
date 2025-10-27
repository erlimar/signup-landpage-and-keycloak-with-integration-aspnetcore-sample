// Copyright (c) 2025 Erlimar Silva Campos. All Rights Reserved.
// This file is a part of SignUpKeycloakGoogleIntegration

using FluentValidation;
using SignUpKeycloakGoogleIntegration.Application;

namespace SignUpKeycloakGoogleIntegration.ApplicationTests;

public class UserSignUpTest
{
    /// <summary>
    /// O identificador do usuário deve ser informado, e não pode ser
    /// uma string vazia ou apenas com espaços em branco.
    /// </summary>
    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("target", nameof(UserSignUpCommand))]
    public void IdDeveSerInformado(string invalidId)
    {
        UserSignUpCommand command = new()
        {
            Id = invalidId,
            Name = "ValidUserName",
            Email = "valid@email.com",
        };

        UserSignUpValidator validator = new();

        ValidationException ex = Assert.Throws<ValidationException>(() =>
            validator.ValidateAndThrow(command)
        );

        Assert.Single(ex.Errors);
        Assert.Contains("Id:", ex.Message);
    }

    /// <summary>
    /// O nome do usuário deve ser informado, e não pode ser uma string
    /// vazia ou apenas com espaços em branco.
    /// </summary>
    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("target", nameof(UserSignUpCommand))]
    public void NameDeveSerInformado(string invalidName)
    {
        UserSignUpCommand command = new()
        {
            Name = invalidName,
            Id = "ValidUserId",
            Email = "valid@email.com",
        };

        UserSignUpValidator validator = new();

        ValidationException ex = Assert.Throws<ValidationException>(() =>
            validator.ValidateAndThrow(command)
        );

        Assert.Single(ex.Errors);
        Assert.Contains("Name:", ex.Message);
    }

    /// <summary>
    /// O e-mail do usuário deve estar em um formato válido.
    /// </summary>
    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("no-email-valid")]
    [InlineData("'invalid' at 'email'")]
    [InlineData("invalid@")]
    [InlineData("@email")]
    [Trait("target", nameof(UserSignUpCommand))]
    public void EmailDeveTerFormatoValido(string invalidEmail)
    {
        UserSignUpCommand command = new()
        {
            Email = invalidEmail,
            Id = "ValidUserId",
            Name = "ValidUserName",
        };

        UserSignUpValidator validator = new();

        ValidationException ex = Assert.Throws<ValidationException>(() =>
            validator.ValidateAndThrow(command)
        );

        Assert.InRange(ex.Errors.Count(), 1, 2);
        Assert.Contains("'Email'", ex.Message);
    }
}
