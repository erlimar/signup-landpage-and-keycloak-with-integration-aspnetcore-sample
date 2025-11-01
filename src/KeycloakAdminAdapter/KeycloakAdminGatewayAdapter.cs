// Copyright (c) 2025 Erlimar Silva Campos. All Rights Reserved.
// This file is a part of SignUpKeycloakGoogleIntegration

using System.Text.RegularExpressions;
using Keycloak.AuthServices.Sdk;
using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;
using Keycloak.AuthServices.Sdk.Admin.Requests.Users;
using Microsoft.Extensions.Options;
using SignUpKeycloakGoogleIntegration.Application;

namespace SignUpKeycloakGoogleIntegration.KeycloakAdminAdapter;

public partial class KeycloakAdminGatewayAdapter(
    IKeycloakUserClient keycloakUserClient,
    IOptions<KeycloakAdminClientOptions> keycloakAdminClientOptions
) : IKeycloakAdminGateway
{
    private readonly IKeycloakUserClient _keycloakUserClient =
        keycloakUserClient ?? throw new ArgumentNullException(nameof(keycloakUserClient));

    private readonly KeycloakAdminClientOptions _keycloakAdminClientOptions =
        keycloakAdminClientOptions?.Value
        ?? throw new ArgumentNullException(nameof(keycloakAdminClientOptions));

    public Task<string?> GetGoogleLinkedIdAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<string?> GetUserIdByEmailAsync(string userEmail)
    {
        IEnumerable<UserRepresentation> users = await _keycloakUserClient.GetUsersAsync(
            _keycloakAdminClientOptions.Realm,
            new GetUsersRequestParameters
            {
                Email = userEmail,
                BriefRepresentation = true,
                Exact = true,
            }
        );

        return users.FirstOrDefault()?.Id;
    }

    public Task WriteGoogleLink(string keycloakUserId, string googleUserId)
    {
        throw new NotImplementedException();
    }

    public async Task<string> WriteNewUser(string name, string email)
    {
        var response = await _keycloakUserClient.CreateUserWithResponseAsync(
            _keycloakAdminClientOptions.Realm,
            new UserRepresentation
            {
                Username = email,
                Email = email,
                // Por estar sendo cadastrado com Google, assumimos a verificação como certa
                EmailVerified = true,
                Enabled = true,
                // TODO: Mudar na origem para user FirstName + LastName ao invés de FullName
                FirstName = name,
            }
        );

        string? newUserId = null;

        if (response?.Headers?.Location != null)
        {
            Match match = IdFromUriRegex().Match(response.Headers.Location.AbsolutePath);

            if (match.Success)
            {
                newUserId = match.Groups[1].Value;
            }
        }

        return string.IsNullOrWhiteSpace(newUserId)
            ? throw new KeyNotFoundException(
                "Nenhum erro relatado ao incluir usuário, porém o mesmo não pode ser recuperado"
            )
            : newUserId!;
    }

    [GeneratedRegex("^.*users/(.*)$")]
    private static partial Regex IdFromUriRegex();
}
