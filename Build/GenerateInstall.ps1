$msbuild = Join-Path (Get-ItemProperty 'HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0').MSBuildToolsPath "MSBuild.exe"
if (Test-Path 'HKLM:\SOFTWARE\7-Zip') {
    $7zip = Join-Path (Get-ItemProperty 'HKLM:\SOFTWARE\7-Zip').Path "7z.exe"
} else {
    $7zip = Join-Path (Get-ItemProperty 'HKLM:\SOFTWARE\WOW6432Node\7-Zip').Path "7z.exe"
}
if (Test-Path 'HKLM:\SOFTWARE\NSIS') {
    $nsis = Join-Path (Get-ItemProperty 'HKLM:\SOFTWARE\NSIS').'(default)' "makensis.exe"
} else {
    $nsis = Join-Path (Get-ItemProperty 'HKLM:\SOFTWARE\WOW6432Node\NSIS').'(default)' "makensis.exe"
}

If (Test-Path './Out') { Remove-Item './Out' -recurse }

(New-Item './Out' -ItemType directory) | Out-Null
$log = './Out/Build.log'
(Set-Content $log '')

echo 'Building Solution...'
(& $msbuild '../Sledge.sln' '/p:Configuration=Release') | Add-Content $log

echo 'Copying Files...'
(& 'robocopy.exe' '../Sledge.Editor/bin/Release/' 'Out/Build' '/S' '/XF' '*.pdb' '*.xml' '*.vshost.*' 'Settings.vdf' '*.ico') | Add-Content $log

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