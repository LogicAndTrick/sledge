using System;
using System.Collections.Generic;
using System.Linq;

namespace Sledge.Providers.Texture
{
    public class TexturePackage : IDisposable
    {
        public string Location { get; }
        public HashSet<string> Textures { get; }

        public TexturePackage(string location, IEnumerable<string> textures)
        {
            Textures = new HashSet<string>(textures.Select(x => x.ToLowerInvariant()));
            Location = location;
        }

        public bool HasTexture(string name)
        {
            return Textures.Contains(name.ToLowerInvariant());
        }
        
        public override string ToString()
        {
            return Location;
        }

        public void Dispose()
        {
            Textures.Clear();
        }
    }
}
