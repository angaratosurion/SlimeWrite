dotnet nuget locals all --clear
# File: Build-MAUI.ps1

# Correct path of the .csproj
$projectPath = "D:\Cloud Folder\NextCloud\My Programs\dotNet\Open Source\SlimeWrite\SlimeWrite\SlimeWrite\SlimeWrite.csproj"

if (-Not (Test-Path $projectPath)) {
    Write-Host "The project file was not found:" $projectPath -ForegroundColor Red
    return
}

# Clean bin and obj folders
Write-Host "Cleaning bin and obj folders..." -ForegroundColor Cyan
$projectFolder = Split-Path $projectPath -Parent
Remove-Item -LiteralPath (Join-Path $projectFolder "bin") -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -LiteralPath (Join-Path $projectFolder "obj") -Recurse -Force -ErrorAction SilentlyContinue

# Restore NuGet packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Cyan
dotnet restore "`"$projectPath`""

# List of all MAUI targets
$targets = @(
    "net10.0",
    "net10.0-android",
    "net10.0-ios",
    "net10.0-maccatalyst",
    "net10.0-windows10.0.19041.0"
)

# Build for each target
foreach ($target in $targets) {
    Write-Host "Rebuilding for target $target ..." -ForegroundColor Green
    dotnet build "`"$projectPath`"" -f $target -c Debug
}

Write-Host "Completed!" -ForegroundColor Yellow
