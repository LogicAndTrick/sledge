using System;
using System.Collections.Generic;
using Sledge.Graphics.Helpers;
using Sledge.DataStructures.GameData;

namespace Sledge.Providers.Texture
{
    public class TexturePackage : IDisposable
    {
        internal TextureProvider Provider { get; private set; }
        public string PackageRoot { get; private set; }
        public string PackageRelativePath { get; private set; }
        public Dictionary<string, TextureItem> Items { get; private set; }
        private readonly Dictionary<string, TextureItem> _loadedItems;
        public bool IsBrowsable { get; set; }
        public Palette Palette { get; private set; }

        public TexturePackage(string packageRoot, string packageRelativePath, TextureProvider provider, Palette pal)
        {
            Provider = provider;
            PackageRoot = packageRoot;
            PackageRelativePath = packageRelativePath;
            Items = new Dictionary<string, TextureItem>();
            _loadedItems = new Dictionary<string, TextureItem>();
            IsBrowsable = true;
            Palette = pal;
        }

        public void AddTexture(TextureItem item)
        {
            if (Items.ContainsKey(item.Name.ToLowerInvariant())) return;
            Items.Add(item.Name.ToLowerInvariant(), item);
        }

        public bool HasTexture(string name)
        {
            return Items.ContainsKey(name.ToLowerInvariant());
        }

        public override string ToString()
        {
            return PackageRelativePath;
        }

        public void Dispose()
        {
            foreach (var kv in _loadedItems)
            {
                TextureHelper.Delete(kv.Value.Name.ToLowerInvariant());
            }
        }
    }
}
