using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using LogicAndTrick.Gimme;
using Sledge.Common.Logging;
using Sledge.Providers.Texture;

namespace Sledge.BspEditor.Environment
{
    public class TextureCollection
    {
        public IEnumerable<TexturePackage> Packages => _packages;

        private readonly ConcurrentDictionary<string, TextureItem> _itemCache;

        private readonly List<TexturePackage> _packages;

        public TextureCollection(IEnumerable<TexturePackage> packages)
        {
            _packages = packages.ToList();
            _itemCache = new ConcurrentDictionary<string, TextureItem>();
            
            Log.Debug(nameof(TextureCollection), $"Reading textures from: {String.Join("; ", _packages.Select(x => x.Location))}");
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

        public async Task Precache(IEnumerable<string> textures)
        {
            var tex = new HashSet<string>(textures.Select(x => x.ToLower()));
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
                        _itemCache[x.Name.ToLower()] = x;
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
            name = name.ToLower();
            if (_itemCache.ContainsKey(name)) return _itemCache[name];
            var packs = _packages.Where(x => x.HasTexture(name)).ToList();
            foreach (var tp in packs)
            {
                var r = await Gimme.FetchOne<TextureItem>(tp.Location, name);
                if (r != null)
                {
                    _itemCache[r.Name.ToLower()] = r;
                    return r;
                }
            }
            return null;
        }

        public async Task<IEnumerable<TextureItem>> GetTextureItems(IEnumerable<string> names)
        {
            var n = names.Select(x => x.ToLower()).ToList();
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
