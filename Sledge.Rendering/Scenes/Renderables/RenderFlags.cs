using System;

namespace Sledge.Rendering.Scenes.Renderables
{
    [Flags]
    public enum RenderFlags : uint
    {
        None = 0,
        Polygon = 1 << 0,
        Wireframe = 1 << 1,
        Point = 1 << 2
    }
}