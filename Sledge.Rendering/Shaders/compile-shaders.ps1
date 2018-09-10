$scriptpath = $MyInvocation.MyCommand.Path
$scriptdir = Split-Path $scriptpath
Push-Location $scriptdir

$fileNames = Get-ChildItem -Filter "*.hlsl" -Recurse

foreach ($file in $fileNames)
{
    if ($file.Name.EndsWith(".frag.hlsl"))
    {
        .\fxc /WX /E main /T ps_4_0 $file /Fo $file".bytes"
    }
    if ($file.Name.EndsWith(".geom.hlsl"))
    {
        .\fxc /WX /E main /T gs_4_0 $file /Fo $file".bytes"
    }
    if ($file.Name.EndsWith(".vert.hlsl"))
    {
        .\fxc /WX /E main /T vs_4_0 $file /Fo $file".bytes"
    }
}

Pop-Location