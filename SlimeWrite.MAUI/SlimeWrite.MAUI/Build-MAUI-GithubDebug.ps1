# Αρχείο: Build-MAUI.ps1

# Σωστό path του .csproj
$projectPath = "D:\Cloud Folder\NextCloud\My Programs\dotNet\Open Source\SlimeWrite\SlimeWrite.MAUI\SlimeWrite.MAUI\SlimeWrite.MAUI-GitHubRelease.csproj"

if (-Not (Test-Path $projectPath)) {
    Write-Host "Το project file δεν βρέθηκε:" $projectPath -ForegroundColor Red
    return
}

# Καθαρισμός φακέλων bin και obj
Write-Host "Καθαρισμός φακέλων bin και obj..." -ForegroundColor Cyan
$projectFolder = Split-Path $projectPath -Parent
Remove-Item -LiteralPath (Join-Path $projectFolder "bin") -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -LiteralPath (Join-Path $projectFolder "obj") -Recurse -Force -ErrorAction SilentlyContinue

# Restore NuGet packages
Write-Host "Κάνουμε restore τα NuGet packages..." -ForegroundColor Cyan
dotnet restore "`"$projectPath`""

# Λίστα με όλα τα MAUI targets
$targets = @(
    "net10.0",
    "net10.0-android",
    "net10.0-ios",
    "net10.0-maccatalyst",
    "net10.0-windows10.0.19041.0"
)

# Build για κάθε target
foreach ($target in $targets) {
    Write-Host "Κάνουμε rebuild για target $target ..." -ForegroundColor Green
    dotnet build "`"$projectPath`"" -f $target -c GithubGithubDebug
}

Write-Host "Ολοκληρώθηκε!" -ForegroundColor Yellow
