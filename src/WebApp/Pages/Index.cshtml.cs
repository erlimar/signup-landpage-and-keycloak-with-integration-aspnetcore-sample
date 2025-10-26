// Copyright (c) 2025 Erlimar Silva Campos. All Rights Reserved.
// This file is a part of SignUpKeycloakGoogleIntegration

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SignUpKeycloakGoogleIntegration.WebApp.Pages;

[Authorize]
public class IndexModel(ILogger<IndexModel> logger) : PageModel
{
    public string Id { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;

    public void OnGet()
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
    }
}
