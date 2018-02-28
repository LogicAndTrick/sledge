using System.Collections.Generic;
using Sledge.Providers.Texture;

namespace Sledge.BspEditor.Environment.Empty
{
    public class EmptyTextureCollection : TextureCollection
    {
        public EmptyTextureCollection(IEnumerable<TexturePackage> packages) : base(packages)
        {
        }

        public override bool IsNullTexture(string name) => false;

        public override float GetOpacity(string name) => 1;
    }
}
