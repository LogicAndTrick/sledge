using System;

namespace Sledge.Providers.Model.Mdl10.Format
{
    [Flags]
    public enum TextureFlags : int
    {
        Flatshade = 0x0001,
        Chrome = 0x0002,
        Fullbright = 0x0004,
        NoMips = 0x0008,
        Alpha = 0x0010,
        Additive = 0x0020,
        Masked = 0x0040,
    }
}