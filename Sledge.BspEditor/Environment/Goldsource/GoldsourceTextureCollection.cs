using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sledge.Providers.Texture;

namespace Sledge.BspEditor.Environment.Goldsource
{
    public class GoldsourceTextureCollection : TextureCollection
    {
        public GoldsourceTextureCollection(IEnumerable<TexturePackage> packages) : base(packages)
        {
        }

        public override bool IsNullTexture(string name)
        {
            switch (name?.ToLower())
            {
                case "null":
                case "bevel":
                    return true;
                default:
                    return false;
            }
        }

        public override float GetOpacity(string name)
        {
            switch (name?.ToLower())
            {
                case "aaatrigger":
                case "hint":
                case "clip":
                case "origin":
                case "skip":
                    return 0.5f;
                default:
                    return 1;
            }
        }
    }
}
