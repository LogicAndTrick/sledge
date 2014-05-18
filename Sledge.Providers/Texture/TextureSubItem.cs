namespace Sledge.Providers.Texture
{
    public class TextureSubItem
    {
        public TextureItem Item { get; private set; }
        public string Name { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public TextureSubItem(TextureItem item, string name, int width, int height)
        {
            Name = name;
            Width = width;
            Height = height;
            Item = item;
        }
    }
}