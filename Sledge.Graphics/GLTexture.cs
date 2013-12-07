using System.Drawing;
using Sledge.Common;
using Sledge.Graphics.Helpers;

namespace Sledge.Graphics
{
    public class GLTexture : ITexture
    {

        public int Reference { get; private set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool HasTransparency { get; set; }
        public Bitmap BitmapImage { get; set; }

        internal GLTexture(int reference, string name)
        {
            Name = name;
            Reference = reference;
        }

        public void Bind()
        {
            TextureHelper.Bind(Reference);
        }

        public void Unbind()
        {
            TextureHelper.Unbind();
        }

        public void Dispose()
        {
            BitmapImage.Dispose();
            TextureHelper.DeleteTexture(Reference);
            TextureHelper.Textures.Remove(Name);
        }
    }
}
