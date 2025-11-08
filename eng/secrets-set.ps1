#!/bin/env pwsh

param(
    [Parameter(Mandatory=$true)][string]$Key,
    [Parameter(Mandatory=$true)][string]$Value
)

# Use parameters to set user secrets
dotnet user-secrets --project src/WebApp/SignUpKeycloakGoogleIntegration.WebApp.csproj set $Key $Value
