// Copyright (c) 2025 Erlimar Silva Campos. All Rights Reserved.
// This file is a part of SignUpKeycloakGoogleIntegration

using FluentValidation;

namespace SignUpKeycloakGoogleIntegration.Application.UserSignUp;

public class UserSignUpValidator : AbstractValidator<UserSignUpCommand>
{
    public UserSignUpValidator()
    {
        _ = RuleFor(static user => user.Id).NotEmpty();
        _ = RuleFor(static user => user.Name).NotEmpty();
        _ = RuleFor(static user => user.Email).NotEmpty().EmailAddress();
    }
}
