$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$output = Join-Path (Split-Path -Parent $root) 'ENTREGA_FINAL.zip'
$staging = Join-Path ([System.IO.Path]::GetTempPath()) ('CampusConecta_' + [guid]::NewGuid().ToString('N'))

New-Item -ItemType Directory -Path $staging | Out-Null
try {
    Get-ChildItem -LiteralPath $root -Force | Where-Object {
        $_.Name -notin '.git', 'entrega_enlaces Gabriela Cohaila y Victoria Lavarello.zip'
    } | ForEach-Object {
        Copy-Item -LiteralPath $_.FullName -Destination $staging -Recurse -Force
    }
    @(
        (Join-Path $staging 'codigo_fuente\.publish'),
        (Join-Path $staging 'codigo_fuente\frontend\node_modules'),
        (Join-Path $staging 'codigo_fuente\frontend\dist'),
        (Join-Path $staging 'codigo_fuente\backend\CampusConecta.Api\wwwroot'),
        (Join-Path $staging 'codigo_fuente\backend\CampusConecta.Api\bin'),
        (Join-Path $staging 'codigo_fuente\backend\CampusConecta.Api\obj'),
        (Join-Path $staging 'codigo_fuente\backend\CampusConecta.Tests\bin'),
        (Join-Path $staging 'codigo_fuente\backend\CampusConecta.Tests\obj')
    ) | Where-Object { Test-Path -LiteralPath $_ } | ForEach-Object {
        Remove-Item -LiteralPath $_ -Recurse -Force
    }
    Compress-Archive -Path (Join-Path $staging '*') -DestinationPath $output -Force
    Write-Host "ZIP actualizado: $output"
}
finally {
    Remove-Item -LiteralPath $staging -Recurse -Force -ErrorAction SilentlyContinue
}
