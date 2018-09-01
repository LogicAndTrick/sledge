namespace Sledge.Providers.Texture.Wad.Format
{
    // Based off quake's qlumpy code
    public enum WadEntryType : byte
    {
        // Palette = 0x40,
        // ColorMap = 0x41,
        Image = 0x42, // Simple image with any size
        Texture = 0x43, // Power-of-16-sized world textures with 4 mipmaps
        // Raw = 0x44,
        // ColorMap2 = 0x45,
        // Font = 0x46, // Fixed-height font. Contains an image and font data (row, X offset and width of a character) for 256 ASCII characters. 
    }
}