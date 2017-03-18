using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using LogicAndTrick.Gimme;
using Sledge.Providers.Texture;

namespace Sledge.Editor.Documents
{
    public class TextureCollection
    {
        public IEnumerable<TexturePackage> Packages => _packages;

        private readonly ConcurrentDictionary<string, TextureItem> _itemCache;

        private string _selectedTexture;
        private readonly List<string> _recentTextures;
        private readonly List<TexturePackage> _packages;

        public string SelectedTexture
        {
            get { return _selectedTexture; }
            set
            {
                _selectedTexture = value;
                if (_selectedTexture != null)
                {
                    _recentTextures.RemoveAll(x => String.Equals(x, _selectedTexture, StringComparison.InvariantCultureIgnoreCase));
                    _recentTextures.Insert(0, _selectedTexture);
                    while (_recentTextures.Count > 25) _recentTextures.RemoveAt(_recentTextures.Count - 1);
                }
            }
        }

        public TextureCollection(List<TexturePackage> packages)
        {
            _packages = packages;
            _recentTextures = new List<string>();
            _itemCache = new ConcurrentDictionary<string, TextureItem>();
            SelectedTexture = GetDefaultSelection();
        }

        public bool HasTexture(string name)
        {
            return Packages.Any(x => x.HasTexture(name));
        }

        private string GetDefaultSelection()
        {
            return
            (from item in _packages.SelectMany(x => x.Textures).OrderBy(x => x, StringComparer.CurrentCultureIgnoreCase)
                where item.Length > 0
                let c = Char.ToLower(item[0])
                where c >= 'a' && c <= 'z'
                select item).FirstOrDefault();
        }

        public IEnumerable<string> GetRecentTextures()
        {
            return _recentTextures;
        }

        public async Task Precache(IEnumerable<string> textures)
        {
            var tex = new HashSet<string>(textures);
            tex.ExceptWith(_itemCache.Keys);
            if (!tex.Any()) return;

            var tasks = new List<Task>();

            foreach (var pack in _packages)
            {
                var found = new HashSet<string>(tex.Where(x => pack.HasTexture(x)));
                tex.ExceptWith(found);
                if (found.Any())
                {
                    var t = Gimme.Fetch<TextureItem>(pack.Location, found.ToList(), x =>
                    {
                        _itemCache[x.Name] = x;
                    });
                    tasks.Add(t);
                }
            }
            if (tasks.Any()) await Task.WhenAll(tasks);
        }

        public IEnumerable<string> GetAllTextures()
        {
            var hs = new HashSet<string>();
            foreach (var pack in _packages) hs.UnionWith(pack.Textures);
            return hs;
        }

        public async Task<TextureItem> GetTextureItem(string name)
        {
            if (_itemCache.ContainsKey(name)) return _itemCache[name];
            var packs = _packages.Where(x => x.HasTexture(name)).ToList();
            foreach (var tp in packs)
            {
                var r = await Gimme.FetchOne<TextureItem>(tp.Location, name);
                if (r != null)
                {
                    _itemCache[r.Name] = r;
                    return r;
                }
            }
            return null;
        }

        public TextureItem TryGetTextureItem(string name)
        {
            if (_itemCache.ContainsKey(name)) return _itemCache[name];
            GetTextureItem(name);
            return null;
        }

        public async Task<IEnumerable<TextureItem>> GetTextureItems(IEnumerable<string> names)
        {
            var n = names.ToList();
            var missing = n.Where(x => !_itemCache.ContainsKey(x));
            await Precache(missing);
            return n.Where(x => _itemCache.ContainsKey(x)).Select(x => _itemCache[x]);
        }

        public Task<ITextureStreamSource> GetStreamSource()
        {
            return GetStreamSource(_packages);
        }

        public async Task<ITextureStreamSource> GetStreamSource(IEnumerable<TexturePackage> packages)
        {
            return await packages
                .Select(x => Gimme.Fetch<ITextureStreamSource>(x.Location, null))
                .Merge()
                .ToList()
                .ToTask()
                .ContinueWith(x => new MultiTextureStreamSource(x.Result));
        }
    }
}
