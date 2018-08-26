namespace Sledge.Providers.Texture.Spr
{
    public enum SpriteRenderMode
    {
        Normal = 0,     // No transparency
        Additive = 1,   // R/G/B = R/G/B, A = (R+G+B)/3
        IndexAlpha = 2, // R/G/B = Palette index 255, A = (R+G+B)/3
        AlphaTest = 3   // R/G/B = R/G/B, Palette index 255 = transparent
    }
}