#!/bin/env bash

dotnet user-secrets --project src/WebApp/SignUpKeycloakGoogleIntegration.WebApp.csproj set "$1" "$2"
