# Simple dev startup — API + Blazor Web, no Aspire.
# Usage: .\run-dev.ps1

$root = Split-Path -Parent $MyInvocation.MyCommand.Path

Get-NetTCPConnection -LocalPort 5022,5102 -ErrorAction SilentlyContinue |
    ForEach-Object { Stop-Process -Id $_.OwningProcess -Force -ErrorAction SilentlyContinue }

Start-Sleep -Seconds 2

Write-Host "Starting LunchNear API on http://localhost:5022 ..."
Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run --project `"$root\LunchNear.Api\LunchNear.Api.csproj`" --launch-profile http"

Start-Sleep -Seconds 4

Write-Host "Starting LunchNear Web on http://localhost:5102 ..."
Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run --project `"$root\LunchNear.Web\LunchNear.Web.csproj`" --launch-profile http"

Write-Host ""
Write-Host "Open: http://localhost:5102/restaurants"
Write-Host "If page was stuck before: Ctrl+Shift+R in browser once."
