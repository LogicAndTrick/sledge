using System;

namespace Sledge.Rendering.OpenGL.Vertices
{
    [Flags]
    public enum VertexFlags : uint
    {
        None = 0,
        Invisible = 1 << 0,

    }
}