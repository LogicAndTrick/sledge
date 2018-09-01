using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sledge.Providers.Texture
{
    public abstract class TexturePackage : IDisposable
    {
        public string Location { get; }
        public string Type { get; }
        public HashSet<string> Textures { get; }
        
        protected virtual IEqualityComparer<string> GetComparer => StringComparer.InvariantCultureIgnoreCase;

        public TexturePackage(string location, string type)
        {
            Textures = new HashSet<string>(GetComparer);
            Location = location;
            Type = type;
        }

        public bool HasTexture(string name)
        {
            return Textures.Contains(name.ToLowerInvariant());
        }

        public abstract Task<IEnumerable<TextureItem>> GetTextures(IEnumerable<string> names);

        public abstract Task<TextureItem> GetTexture(string name);

        public abstract ITextureStreamSource GetStreamSource();
        
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
