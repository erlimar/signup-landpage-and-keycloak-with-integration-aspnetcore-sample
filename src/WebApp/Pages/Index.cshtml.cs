// Copyright (c) 2025 Erlimar Silva Campos. All Rights Reserved.
// This file is a part of SignUpKeycloakGoogleIntegration

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignUpKeycloakGoogleIntegration.Application.UserSignUp;

namespace SignUpKeycloakGoogleIntegration.WebApp.Pages;

[Authorize]
public class IndexModel(ILogger<IndexModel> logger, UserSignUpHandler userSignUpHandler) : PageModel
{
    private readonly UserSignUpHandler _userSignUpHandler =
        userSignUpHandler ?? throw new ArgumentNullException(nameof(userSignUpHandler));

    public string Id { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;

    public string ResponseType { get; private set; } = string.Empty;
    public string ResponseMessage { get; private set; } = string.Empty;

    public async Task OnGetAsync()
    {
        logger.LogInformation(
            "Usuário autenticado como {Name} via {AuthenticationTYpe}",
            User!.Identity!.Name!,
            User.Identity.AuthenticationType
        );

        Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
        Email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)!.Value;
        FullName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        FirstName =
            User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ?? string.Empty;
        LastName =
            User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value ?? string.Empty;

        UserSignUpCommandResponse response = await _userSignUpHandler.HandleAsync(
            new UserSignUpCommand
            {
                GoogleId = Id,
                Email = Email,
                Name = FullName,
            }
        );

        ResponseType = response?.ResponseType.ToString() ?? string.Empty;
        ResponseMessage = response?.ResponseMessage ?? string.Empty;
    }
}
