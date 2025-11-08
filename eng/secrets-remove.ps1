#!/bin/env pwsh

param(
    [Parameter(Mandatory=$true)][string]$Key
)

dotnet user-secrets --project src/WebApp/SignUpKeycloakGoogleIntegration.WebApp.csproj remove $Key
