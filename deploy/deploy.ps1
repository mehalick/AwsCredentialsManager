$ErrorActionPreference = 'Stop'

$tmpPath = ".\_tmp"
$zipPath = ".\_tmp\AwsCredentialsManager.zip"
$s3Path = "s3://.../AwsCredentialsManager.zip"

#aws s3 ls --profile cli

if(!(Test-Path -PathType container $tmpPath))
{
    New-Item -ItemType Directory -Path $tmpPath
}

dotnet publish `
    ..\src\AwsCredentialsManager.App\AwsCredentialsManager.App.csproj `
    -f net8.0-windows10.0.19041.0 `
    -c Release `
    -p:RuntimeIdentifierOverride=win10-x64 `
    -p:WindowsPackageType=None `
    -p:WindowsAppSDKSelfContained=true `
    --self-contained `
    --output $tmpPath

Compress-Archive -Path $($tmpPath + "\*") -DestinationPath $zipPath -Force

#aws s3 cp $zipPath $s3Path --profile ...

#Remove-Item $tmpPath -Recurse
