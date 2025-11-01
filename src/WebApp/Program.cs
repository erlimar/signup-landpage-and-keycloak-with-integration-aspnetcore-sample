// Copyright (c) 2025 Erlimar Silva Campos. All Rights Reserved.
// This file is a part of SignUpKeycloakGoogleIntegration

using Duende.AccessTokenManagement;
using Keycloak.AuthServices.Common;
using Keycloak.AuthServices.Sdk;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using SignUpKeycloakGoogleIntegration.Application;
using SignUpKeycloakGoogleIntegration.Application.UserSignUp;
using SignUpKeycloakGoogleIntegration.KeycloakAdminAdapter;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IKeycloakAdminGateway, KeycloakAdminGatewayAdapter>();
builder.Services.AddTransient<UserSignUpHandler>();

KeycloakAdminClientOptions? keycloakAdminOptions =
    builder.Configuration.GetKeycloakOptions<KeycloakAdminClientOptions>()!;

builder.Services.AddDistributedMemoryCache();
builder
    .Services.AddClientCredentialsTokenManagement()
    .AddClient(
        ClientCredentialsClientName.Parse(keycloakAdminOptions.Resource),
        client =>
        {
            client.ClientId = ClientId.Parse(keycloakAdminOptions.Resource);
            client.ClientSecret = ClientSecret.Parse(keycloakAdminOptions.Credentials.Secret);
            client.TokenEndpoint = new Uri(keycloakAdminOptions.KeycloakTokenEndpoint);
        }
    );

builder
    .Services.AddKeycloakAdminHttpClient(keycloakAdminOptions)
    .AddClientCredentialsTokenHandler(
        ClientCredentialsClientName.Parse(keycloakAdminOptions.Resource)
    );

// Add services to the container.
builder.Services.AddRazorPages();

IConfigurationSection config = builder.Configuration.GetSection("Authentication:Google");

string clientId = config["ClientId"]!;
string clientSecret = config["ClientSecret"]!;

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(
        GoogleDefaults.AuthenticationScheme,
        options =>
        {
            options.ClientId = clientId;
            options.ClientSecret = clientSecret;
            options.SaveTokens = true;
            options.CallbackPath = "/signin/external/google";
        }
    );

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    _ = app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
