// Copyright (c) 2025 Erlimar Silva Campos. All Rights Reserved.
// This file is a part of SignUpKeycloakGoogleIntegration

namespace SignUpKeycloakGoogleIntegration.Application.UserSignUp;

public enum UserSignUpResponseType
{
    /// <summary>
    /// Quando o usuário já está cadastrado
    /// </summary>
    AlreadyRegistered,

    /// <summary>
    /// Quando o usuário já está cadastrado, porém sem vínculo com Google
    /// </summary>
    RegisteredWithoutLink,

    /// <summary>
    /// Quando o usuário não existia mas foi cadastrado
    /// </summary>
    NewRegistration,

    /// <summary>
    /// Quando ocorre alguma falha
    /// </summary>
    Failed,
}