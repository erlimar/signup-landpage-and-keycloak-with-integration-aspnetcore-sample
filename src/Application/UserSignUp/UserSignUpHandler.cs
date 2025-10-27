// Copyright (c) 2025 Erlimar Silva Campos. All Rights Reserved.
// This file is a part of SignUpKeycloakGoogleIntegration

using FluentValidation;

namespace SignUpKeycloakGoogleIntegration.Application.UserSignUp;

public class UserSignUpHandler(IKeycloakGateway keycloakGateway)
{
    private readonly IKeycloakGateway _keycloakGateway =
        keycloakGateway ?? throw new ArgumentNullException(nameof(keycloakGateway));

    public async Task<UserSignUpCommandResponse> HandleAsync(UserSignUpCommand command)
    {
        _ = command ?? throw new ArgumentNullException(nameof(command));

        new UserSignUpValidator().ValidateAndThrow(command);

        string keycloakUserId = await _keycloakGateway.GetUserIdByEmailAsync(command.Email);

        if (!string.IsNullOrWhiteSpace(keycloakUserId))
        {
            return await ValidarUsuarioCadastrado(keycloakUserId);
        }

        throw new NotImplementedException();
    }

    private async Task<UserSignUpCommandResponse> ValidarUsuarioCadastrado(string keycloakUserId)
    {
        string googleLinkId = await _keycloakGateway.GetGoogleLinkedIdAsync(keycloakUserId);

        if (string.IsNullOrWhiteSpace(googleLinkId))
        {
            return new UserSignUpCommandResponse
            {
                ResponseType = UserSignUpResponseType.RegisteredWithoutLink,
                ResponseMessage = null,
            };
        }

        throw new NotImplementedException();
    }
}
