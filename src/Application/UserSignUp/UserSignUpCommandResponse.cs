// Copyright (c) 2025 Erlimar Silva Campos. All Rights Reserved.
// This file is a part of SignUpKeycloakGoogleIntegration

namespace SignUpKeycloakGoogleIntegration.Application.UserSignUp;

public class UserSignUpCommandResponse
{
    public required UserSignUpResponseType ResponseType { get; set; }
    public string? ResponseMessage { get; set; }
}
