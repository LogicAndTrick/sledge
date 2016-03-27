using System.Drawing;
using Sledge.Rendering.Materials;

namespace Sledge.Rendering.Interfaces
{
    public class TextureDetails
    {
        public string Name { get; private set; }
        public Bitmap Bitmap { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public TextureFlags Flags { get; private set; }

        public TextureDetails(string name, Bitmap bitmap, int width, int height, TextureFlags flags)
        {
            Name = name;
            Bitmap = bitmap;
            Width = width;
            Height = height;
            Flags = flags;
        }
    }
}