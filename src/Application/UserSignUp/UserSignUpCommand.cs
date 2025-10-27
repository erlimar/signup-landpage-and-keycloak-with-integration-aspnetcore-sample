// Copyright (c) 2025 Erlimar Silva Campos. All Rights Reserved.
// This file is a part of SignUpKeycloakGoogleIntegration

namespace SignUpKeycloakGoogleIntegration.Application.UserSignUp;

public class UserSignUpCommand
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; }
}
