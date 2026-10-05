$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
Push-Location $root
try {
    dotnet restore CampusConecta.sln
    Push-Location (Join-Path $root 'frontend')
    try { npm install } finally { Pop-Location }
    Write-Host 'Dependencias instaladas correctamente.'
}
finally { Pop-Location }
