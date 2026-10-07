#!/usr/bin/env bash
set -euo pipefail
project_root="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.." && pwd)"
workspace_sdk="$project_root/../.dotnet/dotnet"
if command -v dotnet >/dev/null 2>&1; then
    dotnet_bin="$(command -v dotnet)"
elif [[ -x "$workspace_sdk" ]]; then
    dotnet_bin="$workspace_sdk"
else
    echo 'Install the .NET 10 SDK first: https://dotnet.microsoft.com/download/dotnet/10.0' >&2
    exit 1
fi
export DOTNET_CLI_HOME="${DOTNET_CLI_HOME:-$project_root/../.cli}"
export NUGET_PACKAGES="${NUGET_PACKAGES:-$project_root/../.nuget}"
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export ASPNETCORE_ENVIRONMENT=Development
cd "$project_root"
exec "$dotnet_bin" run --project Mudi/Mudi.csproj --launch-profile Mudi "$@"
