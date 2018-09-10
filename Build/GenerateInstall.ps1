# Find app install locations

# MSBuild
$vswhere = [System.Environment]::ExpandEnvironmentVariables("%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe")
$vspath = & $vswhere -latest -requires Microsoft.Component.MSBuild -property installationPath
$msbuild = join-path $vspath 'MSBuild\15.0\Bin\MSBuild.exe'

# 7-zip
if (Test-Path 'HKLM:\SOFTWARE\7-Zip') {
    $7zip = Join-Path (Get-ItemProperty 'HKLM:\SOFTWARE\7-Zip').Path "7z.exe"
} else {
    $7zip = Join-Path (Get-ItemProperty 'HKLM:\SOFTWARE\WOW6432Node\7-Zip').Path "7z.exe"
}

# NSIS
if (Test-Path 'HKLM:\SOFTWARE\NSIS') {
    $nsis = Join-Path (Get-ItemProperty 'HKLM:\SOFTWARE\NSIS').'(default)' "makensis.exe"
} else {
    $nsis = Join-Path (Get-ItemProperty 'HKLM:\SOFTWARE\WOW6432Node\NSIS').'(default)' "makensis.exe"
}

# Ensure we're in the build folder
$scriptpath = $MyInvocation.MyCommand.Path
$scriptdir = Split-Path $scriptpath
Set-Location $scriptdir

# Delete existing out directory
If (Test-Path './Out') { Remove-Item './Out' -recurse }

# Create out directories and log file
(New-Item './Out' -ItemType directory) | Out-Null
(New-Item './Out/Build' -ItemType directory) | Out-Null
$log = './Out/Build.log'
(Set-Content $log '')

$outputPath = Resolve-Path './Out/Build'

# Compile the shaders
$content = & "..\Sledge.Rendering\Shaders\compile-shaders.ps1"
$content | Add-Content $log

# Build the project
echo 'Building Solution...'
(& $msbuild '../Sledge.sln' '/p:Configuration=Release' "/p:OutputPath=$outputPath") | Add-Content $log

#(& del '.\Out\Build\*.pdb') | Add-Content $log
(& del '.\Out\Build\*.xml') | Add-Content $log

$version = (Get-Command './Out/Build/Sledge.Editor.exe').FileVersionInfo.ProductVersion
$zipfile = './Out/Sledge.Editor.' + $version + '.zip'
$exefile = './Out/Sledge.Editor.' + $version + '.zip'
$nsifile = './Out/Sledge.Editor.Installer.' + $version + '.nsi'
$verfile = './Out/version.txt'

echo ('Version is ' + $version + '.')
echo 'Creating Archive...'
(& $7zip 'a' '-tzip' '-r' $zipfile './Out/Build/*.*') | Add-Content $log

echo 'Creating Installer...'
Set-Content $nsifile ((Get-Content '.\Sledge.Editor.Installer.nsi') -replace "\{version\}", $version)

(& $nsis $nsifile) | Add-Content $log

echo 'Creating Version File...'
$date = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
#https://github.com/LogicAndTrick/sledge/releases/download/pre-alpha/Sledge.Editor.0.1.0.0.zip
$url ='https://github.com/LogicAndTrick/sledge/releases/download/' + $version + '/Sledge.Editor.' + $version + '.zip'
Set-Content $verfile $version, $date, $url

echo 'Done.'