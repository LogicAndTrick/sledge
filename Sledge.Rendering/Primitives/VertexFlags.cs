using System;

namespace Sledge.Rendering.Primitives
{
    [Flags]
    public enum VertexFlags : uint
    {
        None = 0,
        SelectiveTransformed = 1 << 0,
    }
}