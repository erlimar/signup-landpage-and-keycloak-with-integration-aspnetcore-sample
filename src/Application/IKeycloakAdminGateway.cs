// Copyright (c) 2025 Erlimar Silva Campos. All Rights Reserved.
// This file is a part of SignUpKeycloakGoogleIntegration

namespace SignUpKeycloakGoogleIntegration.Application;

/// <summary>
/// Integração com API administrativa do Keycloak
/// </summary>
public interface IKeycloakAdminGateway
{
    /// <summary>
    /// Obtém o identificador do usuário pelo e-mail
    /// </summary>
    /// <returns>Quando usuário existe, retorna um valor diferente de vazio</returns>
    Task<string?> GetUserIdByEmailAsync(string userEmail);

    /// <summary>
    /// Obtém o identificador Google vinculado a um usuário
    /// </summary>
    /// <returns>Quando há um vínculo Google, retorna um valor diferente de vazio</returns>
    Task<string?> GetGoogleLinkedIdAsync(string userId);

    /// <summary>
    /// Grava um novo usuário
    /// </summary>
    Task<string> WriteNewUser(string name, string email);

    /// <summary>
    /// Vincula um usuário a uma conta Google
    /// </summary>
    Task WriteGoogleLink(string keycloakUserId, string googleUserId);
}