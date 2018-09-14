using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.FileSystem;
using Sledge.Providers.Texture.Wad.Format;

namespace Sledge.Providers.Texture.Wad
{
    public class WadTexturePackage : TexturePackage
    {
        private IFile _file;

        protected override IEqualityComparer<string> GetComparer => StringComparer.InvariantCultureIgnoreCase;

        public WadTexturePackage(TexturePackageReference reference) : base(reference.File.Name, "Wad3")
        {
            _file = reference.File;
            using (var stream = reference.File.Open())
            {
                Textures.UnionWith(WadPackage.GetEntryNames(stream));
            }
        }

        private TextureFlags GetFlags(WadEntry entry)
        {
            return _file.NameWithoutExtension.IndexOf("decal", StringComparison.CurrentCultureIgnoreCase) >= 0 && entry.Name.StartsWith("{")
                ? TextureFlags.Transparent
                : TextureFlags.None;
        }

        public override async Task<IEnumerable<TextureItem>> GetTextures(IEnumerable<string> names)
        {
            var textures = new HashSet<string>(names);
            textures.IntersectWith(Textures);
            if (!textures.Any()) return new TextureItem[0];

            var wp = new WadPackage(_file);
            var list = new List<TextureItem>();

            foreach (var name in textures)
            {
                var entry = wp.GetEntry(name);
                if (entry == null) continue;
                var item = new TextureItem(entry.Name, GetFlags(entry), (int) entry.Width, (int) entry.Height);
                list.Add(item);
            }

            return list;
        }

        public override async Task<TextureItem> GetTexture(string name)
        {
            if (!Textures.Contains(name)) return null;

            var wp = new WadPackage(_file);
            var entry = wp.GetEntry(name);
            if (entry == null) return null;
            return new TextureItem(entry.Name, GetFlags(entry), (int) entry.Width, (int) entry.Height);
        }

        public override ITextureStreamSource GetStreamSource()
        {
            return new WadStreamSource(_file);
        }
    }
}