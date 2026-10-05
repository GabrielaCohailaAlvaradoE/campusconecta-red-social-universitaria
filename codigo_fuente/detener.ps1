$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$pidFile = Join-Path $root '.campusconecta-pids.json'
if (Test-Path -LiteralPath $pidFile) {
    $processes = Get-Content -Raw -LiteralPath $pidFile | ConvertFrom-Json
    foreach ($processId in @($processes.ApiPid, $processes.WebPid)) {
        if ($processId -and (Get-Process -Id $processId -ErrorAction SilentlyContinue)) { Stop-Process -Id $processId -Force }
    }
    Remove-Item -LiteralPath $pidFile -Force
}
Push-Location $root
try { docker compose stop postgres } finally { Pop-Location }
Write-Host 'CampusConecta detenido.'
