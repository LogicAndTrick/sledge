using System;

namespace Sledge.Providers.Texture
{
    public enum TextureSubItemType
    {
        Base,     // Primary base texture
        Overlay1, // WorldVertexTransition, UnlitTwoTexture, WorldTwoTextureBlend
        Overlay2, // Lightmapped_4WayBlend, MultiBlend
        Overlay3, // Lightmapped_4WayBlend, MultiBlend
        Detail,   // $detail
        Tool      // %tooltexture
    }
}