param(
    [string]$Sts2Path = 'G:\SteamLibrary\steamapps\common\Slay the Spire 2',
    [string]$BaseLibPath = 'G:\SteamLibrary\steamapps\workshop\content\2868840\3737335127\BaseLib'
)

$ErrorActionPreference = 'Stop'
$projectRoot = $PSScriptRoot
$dotnet = Join-Path $projectRoot '.tools\dotnet\dotnet.exe'
$dist = Join-Path $projectRoot 'dist\CosmosColdMusic'
$sourceAudio = Join-Path $projectRoot 'assets\audio'

if (-not (Test-Path -LiteralPath $dotnet)) {
    throw "Missing local .NET SDK: $dotnet"
}

& $dotnet build (Join-Path $projectRoot 'CosmosColdMusic.csproj') `
    --configuration Release `
    --property:Sts2Path="$Sts2Path" `
    --property:BaseLibPath="$BaseLibPath"

if (Test-Path -LiteralPath $dist) {
    Remove-Item -LiteralPath $dist -Recurse -Force
}

New-Item -ItemType Directory -Force -Path $dist | Out-Null
Copy-Item -LiteralPath (Join-Path $projectRoot 'bin\Release\CosmosColdMusic.dll') -Destination $dist
Copy-Item -LiteralPath (Join-Path $projectRoot 'CosmosColdMusic.json') -Destination $dist
Copy-Item -LiteralPath (Join-Path $projectRoot 'README.md') -Destination $dist

$distAudio = Join-Path $dist 'audio'
New-Item -ItemType Directory -Force -Path $distAudio | Out-Null
foreach ($fileName in @('original.mp3', 'refrain.mp3', 'stars.mp3')) {
    $sourceFile = Join-Path $sourceAudio $fileName
    if (-not (Test-Path -LiteralPath $sourceFile)) {
        throw "Missing packaged audio source: $sourceFile"
    }
    Copy-Item -LiteralPath $sourceFile -Destination (Join-Path $distAudio $fileName)
}

$zip = Join-Path $projectRoot 'dist\CosmosColdMusic-v2.1.1.zip'
if (Test-Path -LiteralPath $zip) {
    Remove-Item -LiteralPath $zip -Force
}
Compress-Archive -LiteralPath $dist -DestinationPath $zip

Write-Host "Built: $dist"
Write-Host "Archive: $zip"
