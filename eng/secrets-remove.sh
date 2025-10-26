#!/bin/env bash

dotnet user-secrets --project src/WebApp/SignUpKeycloakGoogleIntegration.WebApp.csproj remove "$1"
