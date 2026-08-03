# Simple dev startup — API + Blazor Web, no Aspire.
# Usage: .\run-dev.ps1

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$apiProject = Join-Path $root "LunchNear.API\LunchNear.API.csproj"
$webProject = Join-Path $root "LunchNear.Web\LunchNear.Web.csproj"

# Stop anything already on dev ports
Get-NetTCPConnection -LocalPort 5022,5102 -ErrorAction SilentlyContinue |
    ForEach-Object { Stop-Process -Id $_.OwningProcess -Force -ErrorAction SilentlyContinue }

Start-Sleep -Seconds 1

Write-Host "Building API + Web (once, no AppHost) ..."
Push-Location $root
try {
    dotnet build $apiProject -c Debug --nologo
    if ($LASTEXITCODE -ne 0) { throw "API build failed." }

    dotnet build $webProject -c Debug --nologo
    if ($LASTEXITCODE -ne 0) { throw "Web build failed." }
}
catch {
    Write-Host ""
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host "Tip: close Visual Studio debug session or stop dotnet processes, then retry." -ForegroundColor Yellow
    exit 1
}
finally {
    Pop-Location
}

Write-Host "Starting LunchNear API on http://localhost:5022 ..."
Start-Process powershell -ArgumentList "-NoExit", "-Command", @"
Set-Location '$root'
dotnet run --no-build --project '$apiProject' --launch-profile http
"@

Start-Sleep -Seconds 3

Write-Host "Starting LunchNear Web on http://localhost:5102 ..."
Start-Process powershell -ArgumentList "-NoExit", "-Command", @"
Set-Location '$root'
dotnet run --no-build --project '$webProject' --launch-profile http
"@

Write-Host ""
Write-Host "Open: http://localhost:5102"
Write-Host "Admin: http://localhost:5102/admin"
Write-Host "If page was stuck before: Ctrl+Shift+R in browser once."
