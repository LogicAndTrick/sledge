using System;

namespace Sledge.Rendering.OpenGL.Vertices
{
    [Flags]
    public enum VertexFlags : uint
    {
        None = 0,

        InvisibleOrthographic = 1 << 0,
        InvisiblePerspective = 1 << 1,

        Selected = 1 << 2,
        Normalised = 1 << 3,
    }
}