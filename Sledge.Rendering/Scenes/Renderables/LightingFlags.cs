using System;

namespace Sledge.Rendering.Scenes.Renderables
{
    [Flags]
    public enum LightingFlags : uint
    {
        None = 0,
        Ambient = 1 << 0,
        Dynamic = 1 << 1,
    }
}