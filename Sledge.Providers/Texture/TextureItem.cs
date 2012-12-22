using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.Common;
using Sledge.Graphics.Helpers;

namespace Sledge.Providers.Texture
{
    public class TextureItem
    {
        public TexturePackage Package { get; private set; }
        public string Name { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public TextureItem(TexturePackage package, string name, int width, int height)
        {
            Package = package;
            Name = name;
            Width = width;
            Height = height;
        }

        public ITexture GetTexture()
        {
            if (!TextureHelper.Exists(Name))
            {
                TextureProvider.LoadTextureFromPackage(Package, Name);
            }
            return TextureHelper.Get(Name);
        }
    }
}
