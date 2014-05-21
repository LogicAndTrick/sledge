using System;

namespace Sledge.Providers.Texture.Vtf
{
    [Flags]
    public enum VtfImageFlag : uint
    {
        Pointsample = 0x00000001,
        Trilinear = 0x00000002,
        Clamps = 0x00000004,
        Clampt = 0x00000008,
        Anisotropic = 0x00000010,
        HintDxt5 = 0x00000020,
        Srgb = 0x00000040,
        DeprecatedNocompress = 0x00000040,
        Normal = 0x00000080,
        Nomip = 0x00000100,
        Nolod = 0x00000200,
        Minmip = 0x00000400,
        Procedural = 0x00000800,
        Onebitalpha = 0x00001000,
        Eightbitalpha = 0x00002000,
        Envmap = 0x00004000,
        Rendertarget = 0x00008000,
        Depthrendertarget = 0x00010000,
        Nodebugoverride = 0x00020000,
        Singlecopy = 0x00040000,
        Unused0 = 0x00080000,
        DeprecatedOneovermiplevelinalpha = 0x00080000,
        Unused1 = 0x00100000,
        DeprecatedPremultcolorbyoneovermiplevel = 0x00100000,
        Unused2 = 0x00200000,
        DeprecatedNormaltodudv = 0x00200000,
        Unused3 = 0x00400000,
        DeprecatedAlphatestmipgeneration = 0x00400000,
        Nodepthbuffer = 0x00800000,
        Unused4 = 0x01000000,
        DeprecatedNicefiltered = 0x01000000,
        Clampu = 0x02000000,
        Vertextexture = 0x04000000,
        Ssbump = 0x08000000,
        Unused5 = 0x10000000,
        DeprecatedUnfilterableOk = 0x10000000,
        Border = 0x20000000,
        DeprecatedSpecvarRed = 0x40000000,
        DeprecatedSpecvarAlpha = 0x80000000,
        Last = 0x20000000
    }
}