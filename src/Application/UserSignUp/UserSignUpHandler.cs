// Copyright (c) 2025 Erlimar Silva Campos. All Rights Reserved.
// This file is a part of SignUpKeycloakGoogleIntegration

using FluentValidation;

namespace SignUpKeycloakGoogleIntegration.Application.UserSignUp;

public class UserSignUpHandler(IKeycloakGateway keycloakGateway)
{
    private readonly IKeycloakGateway _keycloakGateway =
        keycloakGateway ?? throw new ArgumentNullException(nameof(keycloakGateway));

    public Task<UserSignUpCommandResponse> HandleAsync(UserSignUpCommand command)
    {
        _ = command ?? throw new ArgumentNullException(nameof(command));

        new UserSignUpValidator().ValidateAndThrow(command);

        throw new NotImplementedException();
    }
}
