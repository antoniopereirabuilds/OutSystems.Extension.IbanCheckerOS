# Builds the ODC External Library upload package (IbanCheckerOS_Asset.zip) from this folder.
Set-ExecutionPolicy -Scope CurrentUser Unrestricted
$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot
if (Test-Path .\publish) { Remove-Item .\publish -Recurse -Force }
if (Test-Path .\IbanCheckerOS_Asset.zip) { Remove-Item .\IbanCheckerOS_Asset.zip -Force }
dotnet publish .\IbanCheckerOS.csproj -c Release -r linux-x64 --self-contained false -o .\publish
Compress-Archive -Path .\publish\* -DestinationPath IbanCheckerOS_Asset.zip
Write-Host "Created $PSScriptRoot\IbanCheckerOS_Asset.zip"
