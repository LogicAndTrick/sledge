using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Sledge.FileSystem;
using Sledge.Graphics;
using Sledge.Graphics.Helpers;

namespace Sledge.Providers.Texture
{
    public class TexturePackage : IDisposable
    {
        internal TextureProvider Provider { get; private set; }
        public IFile PackageFile { get; private set; }
        public Dictionary<string, TextureItem> Items { get; private set; }
        private readonly Dictionary<string, TextureItem> _loadedItems;

        public TexturePackage(IFile packageFile, TextureProvider provider)
        {
            Provider = provider;
            PackageFile = packageFile;
            Items = new Dictionary<string, TextureItem>();
            _loadedItems = new Dictionary<string, TextureItem>();
        }

        public void AddTexture(TextureItem item)
        {
            if (Items.ContainsKey(item.Name.ToLowerInvariant())) return;
            Items.Add(item.Name.ToLowerInvariant(), item);
        }

        public void LoadTexture(TextureItem item)
        {
            if (!_loadedItems.ContainsKey(item.Name.ToLowerInvariant()))
            {
                Provider.LoadTexture(item);
                _loadedItems.Add(item.Name.ToLowerInvariant(), item);
            }
        }

        public void LoadTextures(IEnumerable<TextureItem> items)
        {
            var all = items.Where(x => !_loadedItems.ContainsKey(x.Name.ToLowerInvariant())).ToList();
            if (!all.Any()) return;
            Provider.LoadTextures(all);
            foreach (var ti in all)
            {
                if (!_loadedItems.ContainsKey(ti.Name.ToLowerInvariant()))
                {
                    _loadedItems.Add(ti.Name.ToLowerInvariant(), ti);
                }
            }
        }

        public override string ToString()
        {
            return PackageFile.NameWithoutExtension;
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
