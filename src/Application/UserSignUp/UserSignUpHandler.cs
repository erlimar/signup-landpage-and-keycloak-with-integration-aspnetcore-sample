// Copyright (c) 2025 Erlimar Silva Campos. All Rights Reserved.
// This file is a part of SignUpKeycloakGoogleIntegration

using FluentValidation;

namespace SignUpKeycloakGoogleIntegration.Application.UserSignUp;

public class UserSignUpHandler(IKeycloakAdminGateway keycloakAdminGateway)
{
    private readonly IKeycloakAdminGateway _keycloakAdminGateway =
        keycloakAdminGateway ?? throw new ArgumentNullException(nameof(keycloakAdminGateway));

    public async Task<UserSignUpCommandResponse> HandleAsync(UserSignUpCommand command)
    {
        _ = command ?? throw new ArgumentNullException(nameof(command));

        new UserSignUpValidator().ValidateAndThrow(command);

        string? keycloakUserId = await _keycloakAdminGateway.GetUserIdByEmailAsync(command.Email);

        if (!string.IsNullOrWhiteSpace(keycloakUserId))
        {
            return await ValidarUsuarioCadastrado(keycloakUserId, command.GoogleId);
        }

        var newUserKeycloakId = await _keycloakAdminGateway.WriteNewUser(
            command.Name,
            command.Email
        );

        // TODO: Falhar se não for um Id válido

        await _keycloakAdminGateway.WriteGoogleLink(newUserKeycloakId, command.GoogleId);

        return new UserSignUpCommandResponse
        {
            ResponseType = UserSignUpResponseType.NewRegistration,
            ResponseMessage = null,
        };
    }

    private async Task<UserSignUpCommandResponse> ValidarUsuarioCadastrado(
        string keycloakUserId,
        string commandUserId
    )
    {
        string? googleLinkId = await _keycloakAdminGateway.GetGoogleLinkedIdAsync(keycloakUserId);

        return string.IsNullOrWhiteSpace(googleLinkId)
                ? new UserSignUpCommandResponse
                {
                    ResponseType = UserSignUpResponseType.RegisteredWithoutLink,
                    ResponseMessage = null,
                }
            : googleLinkId != commandUserId
                ? new UserSignUpCommandResponse
                {
                    ResponseType = UserSignUpResponseType.Failed,
                    ResponseMessage = "O usuário já está vinculado a outra conta Google",
                }
            : new UserSignUpCommandResponse
            {
                ResponseType = UserSignUpResponseType.AlreadyRegistered,
                ResponseMessage = null,
            };
    }
}