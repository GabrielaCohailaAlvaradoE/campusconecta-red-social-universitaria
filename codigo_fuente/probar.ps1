$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
Push-Location $root
try {
    dotnet test CampusConecta.sln --configuration Release
    Push-Location (Join-Path $root 'frontend')
    try { npm run lint; npm run build } finally { Pop-Location }
}
finally { Pop-Location }
