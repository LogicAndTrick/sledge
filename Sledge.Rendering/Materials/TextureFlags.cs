using System;

namespace Sledge.Rendering.Materials
{
    [Flags]
    public enum TextureFlags : uint
    {
        None = 1u << 0,
        Transparent = 1u << 1,
        PixelPerfect = 1u << 2,

        Missing = 1u << 31
    }
}