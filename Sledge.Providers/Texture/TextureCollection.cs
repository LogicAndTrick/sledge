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

        private string _selectedTexture;
        private readonly List<string> _recentTextures;
        private readonly List<TexturePackage> _packages;
        private readonly Dictionary<string, TextureItem> _items;

        public TextureItem SelectedTexture
        {
            get
            {
                return _selectedTexture == null ? null : GetItem(_selectedTexture);
            }
            set
            {
                _selectedTexture = value == null ? null : value.Name;
                if (_selectedTexture != null)
                {
                    _recentTextures.Remove(_selectedTexture);
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
            _recentTextures = new List<string>();
            SelectedTexture = GetDefaultSelection();
        }

        private TextureItem GetDefaultSelection()
        {
            var ignored = "{#!~+-0123456789".ToCharArray();
            return GetAllItems()
                .OrderBy(x => new string(x.Name.Where(c => !ignored.Contains(c)).ToArray()) + "Z")
                .FirstOrDefault();
        }

        public IEnumerable<TextureItem> GetRecentTextures()
        {
            return _recentTextures.Select(GetItem);
        }

        public ITextureStreamSource GetStreamSource(int maxWidth, int maxHeight)
        {
            return GetStreamSource(maxWidth, maxHeight, _packages);
        }

        public ITextureStreamSource GetStreamSource(int maxWidth, int maxHeight, IEnumerable<TexturePackage> packages)
        {
            var streams = packages.GroupBy(x => x.Provider).Select(x => x.Key.GetStreamSource(maxWidth, maxHeight, x));
            return new MultiTextureStreamSource(streams);
        }

        public IEnumerable<TextureItem> GetAllItems()
        {
            return _items.Values;
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
