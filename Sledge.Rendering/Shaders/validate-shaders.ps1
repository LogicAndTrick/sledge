$fileNames = Get-ChildItem -Filter "*.hlsl" -Path $scriptPath -Recurse

foreach ($file in $fileNames)
{
    if ($file.Name.EndsWith(".frag.hlsl"))
    {
        .\fxc /WX /E main /T ps_5_0 $file 1> $null
    }
    if ($file.Name.EndsWith(".geom.hlsl"))
    {
        .\fxc /WX /E main /T gs_5_0 $file 1> $null
    }
    if ($file.Name.EndsWith(".vert.hlsl"))
    {
        .\fxc /WX /E main /T vs_5_0 $file 1> $null
    }
}