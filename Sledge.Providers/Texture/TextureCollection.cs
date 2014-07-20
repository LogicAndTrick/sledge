using System;
using System.Collections.Generic;
using System.Linq;

namespace Sledge.Providers.Texture
{
    public class TextureCollection
    {
        public IEnumerable<TexturePackage> Packages
        {
            get { return _packages; }
        }

        private TextureItem _selectedTexture;
        private readonly List<TextureItem> _recentTextures;
        private readonly List<TexturePackage> _packages;
        private readonly Dictionary<string, TextureItem> _items;

        public TextureItem SelectedTexture
        {
            get { return _selectedTexture; }
            set
            {
                _selectedTexture = value;
                if (_selectedTexture != null)
                {
                    _recentTextures.RemoveAll(x => String.Equals(x.Name, _selectedTexture.Name, StringComparison.InvariantCultureIgnoreCase));
                    _recentTextures.Insert(0, _selectedTexture);
                    while (_recentTextures.Count > 25) _recentTextures.RemoveAt(_recentTextures.Count - 1);
                }
            }
        }

        public TextureCollection(List<TexturePackage> packages)
        {
            _packages = packages;
            _items = new Dictionary<string, TextureItem>();
            foreach (var item in packages.SelectMany(x => x.Items))
            {
                var k = item.Key.ToLowerInvariant();
                if (!_items.ContainsKey(k)) _items.Add(k, item.Value);
            }
            _recentTextures = new List<TextureItem>();
            SelectedTexture = GetDefaultSelection();
        }

        private TextureItem GetDefaultSelection()
        {
            var ignored = "{#!~+-0123456789".ToCharArray();
            return GetAllBrowsableItems()
                .OrderBy(x => new string(x.Name.Where(c => !ignored.Contains(c)).ToArray()) + "Z")
                .FirstOrDefault();
        }

        public IEnumerable<TextureItem> GetRecentTextures()
        {
            return _recentTextures;
        }

        public ITextureStreamSource GetStreamSource(int maxWidth, int maxHeight)
        {
            return GetStreamSource(maxWidth, maxHeight, _packages);
        }

        public ITextureStreamSource GetStreamSource(int maxWidth, int maxHeight, IEnumerable<TexturePackage> packages)
        {
            var streams = packages.Where(x => x != null && x.Provider != null)
                .GroupBy(x => x.Provider)
                .Select(x => x.Key.GetStreamSource(maxWidth, maxHeight, x))
                .ToList();
            streams.Add(new NullTextureStreamSource(maxWidth, maxHeight));
            return new MultiTextureStreamSource(streams);
        }

        public IEnumerable<TextureItem> GetAllItems()
        {
            return _items.Values;
        }

        public IEnumerable<TextureItem> GetAllBrowsableItems()
        {
            return _items.Values.Where(x => x.Package.IsBrowsable);
        }

        public IEnumerable<TextureItem> GetItems(IEnumerable<string> names)
        {
            return names.Select(x => x.ToLowerInvariant()).Where(x => _items.ContainsKey(x)).Select(x => _items[x]);
        }

        public TextureItem GetItem(string textureName)
        {
            textureName = textureName.ToLowerInvariant();
            return _items.ContainsKey(textureName) ? _items[textureName] : null;
        }
    }
}
