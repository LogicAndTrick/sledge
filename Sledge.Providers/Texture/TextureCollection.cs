using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Providers.Texture
{
    public class TextureCollection
    {
        public List<TexturePackage> Packages { get; set; }

        private string _selectedTexture;
        private List<string> _recentTextures;

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
            Packages = packages;
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

        public ITextureStreamSource GetStreamSource()
        {
            return GetStreamSource(Packages);
        }

        public ITextureStreamSource GetStreamSource(IEnumerable<TexturePackage> packages)
        {
            var streams = packages.GroupBy(x => x.Provider).Select(x => x.Key.GetStreamSource(x));
            return new MultiTextureStreamSource(streams);
        }

        public IEnumerable<TextureItem> GetAllItems()
        {
            return Packages.SelectMany(x => x.Items.Values).ToList();
        }

        public IEnumerable<TextureItem> GetItems(IEnumerable<string> names)
        {
            var all = Packages.SelectMany(x => x.Items.Values).ToList();
            return names.Select(x => all.FirstOrDefault(y => String.Equals(x, y.Name, StringComparison.InvariantCultureIgnoreCase))).Where(x => x != null);
        }

        public TextureItem GetItem(string textureName)
        {
            textureName = textureName.ToLowerInvariant();
            foreach (var package in Packages)
            {
                if (package.Items.ContainsKey(textureName)) return package.Items[textureName];
            }
            return null;
        }

        public void LoadTextureItem(TextureItem item)
        {
            item.Package.LoadTexture(item);
        }

        public void LoadTextureItems(IEnumerable<TextureItem> items)
        {
            foreach (var g in items.GroupBy(x => x.Package))
            {
                g.Key.LoadTextures(g);
            }
        }
    }
}
