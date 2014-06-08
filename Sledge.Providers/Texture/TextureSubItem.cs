namespace Sledge.Providers.Texture
{
    public class TextureSubItem
    {
        public TextureSubItemType Type { get; private set; }
        public TextureItem Item { get; private set; }
        public string Name { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public TextureSubItem(TextureSubItemType type, TextureItem item, string name, int width, int height)
        {
            Type = type;
            Name = name;
            Width = width;
            Height = height;
            Item = item;
        }
    }
}