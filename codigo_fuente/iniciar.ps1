$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
Push-Location $root
try {
    docker compose up -d postgres
    $healthy = $false
    for ($attempt = 1; $attempt -le 30; $attempt++) {
        $status = docker inspect --format '{{.State.Health.Status}}' campusconecta-postgres 2>$null
        if ($status -eq 'healthy') { $healthy = $true; break }
        Start-Sleep -Seconds 1
    }
    if (-not $healthy) { throw 'PostgreSQL no alcanzó el estado healthy.' }

    $dotnet = (Get-Command dotnet).Source
    $npm = (Get-Command npm.cmd).Source
    & $dotnet build 'backend/CampusConecta.Api/CampusConecta.Api.csproj' --nologo | Out-Host
    $apiDll = Join-Path $root 'backend/CampusConecta.Api/bin/Debug/net8.0/CampusConecta.Api.dll'
    $api = Start-Process $dotnet -ArgumentList $apiDll,'--urls','http://localhost:5050' -WorkingDirectory $root -WindowStyle Hidden -PassThru
    $web = Start-Process $npm -ArgumentList 'run','dev','--','--host','127.0.0.1' -WorkingDirectory (Join-Path $root 'frontend') -WindowStyle Hidden -PassThru
    @{ ApiPid = $api.Id; WebPid = $web.Id } | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $root '.campusconecta-pids.json')
    Write-Host 'CampusConecta iniciado en http://localhost:5173'
    Write-Host 'Swagger disponible en http://localhost:5050/swagger'
}
finally { Pop-Location }
