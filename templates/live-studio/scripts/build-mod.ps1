$ErrorActionPreference = "Stop"

$workspaceRoot = Split-Path -Parent $PSScriptRoot
$csproj = Join-Path $workspaceRoot "ModProject/ModProject.csproj"

if (-not (Test-Path -LiteralPath $csproj)) {
  throw "Cannot find ModProject/ModProject.csproj. Run this script from the generated mod workspace."
}

dotnet build $csproj -c Release -clp:NoSummary

if ($LASTEXITCODE -ne 0) {
  exit $LASTEXITCODE
}

$dist = Join-Path $workspaceRoot "dist"
$dlls = Get-ChildItem -LiteralPath $dist -Filter "*.dll" -File -ErrorAction SilentlyContinue

if (-not $dlls -or $dlls.Count -eq 0) {
  throw "Build succeeded but no dll was found in dist/."
}

Write-Host ""
Write-Host "Build succeeded. Output:"
foreach ($dll in $dlls) {
  Write-Host ("- " + $dll.FullName)
}
