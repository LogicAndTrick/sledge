using System.Drawing;
using Sledge.Common;
using Sledge.Graphics.Helpers;

namespace Sledge.Graphics
{
    public class GLTexture : ITexture
    {

        public int Reference { get; private set; }
        public TextureFlags Flags { get; private set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool HasTransparency { get; set; }

        internal GLTexture(int reference, string name, TextureFlags flags)
        {
            Name = name;
            Reference = reference;
            Flags = flags;
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
            TextureHelper.DeleteTexture(Reference);
            TextureHelper.Textures.Remove(Name);
        }
    }
}
