using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sledge.Common.Extensions;
using Sledge.FileSystem;

namespace Sledge.Providers.Texture.Spr
{
    public class SprTexturePackage : TexturePackage
    {
        private readonly IFile _file;
        protected override IEqualityComparer<string> GetComparer => StringComparer.InvariantCultureIgnoreCase;

        public SprTexturePackage(TexturePackageReference reference) : base("sprites", "Spr")
        {
            _file = reference.File;

            var dir = _file.GetChild("sprites");
            if (dir == null) return;
            
            Textures.UnionWith(dir.GetFiles(".*\\.spr", true).Select(x => x.GetRelativePath(dir)));
        }

        private static Size GetSize(IFile file)
        {
            using (var br = new BinaryReader(file.Open()))
            {
                var idst = br.ReadFixedLengthString(Encoding.ASCII, 4);
                if (idst != "IDSP") return Size.Empty;

                var version = br.ReadInt32();
                if (version != 2) return Size.Empty;

                var type = br.ReadInt32();
                var texFormat = br.ReadInt32();
                var boundingRadius = br.ReadSingle();

                var width = br.ReadInt32();
                var height = br.ReadInt32();

                return new Size(width, height);
            }
        }

        public override Task<IEnumerable<TextureItem>> GetTextures(IEnumerable<string> names)
        {
            var textures = new HashSet<string>(names, GetComparer);
            textures.IntersectWith(Textures);

            var list = new List<TextureItem>();
            foreach (var name in textures)
            {
                var entry = _file.TraversePath(name);
                if (entry == null || !entry.Exists) continue;

                var size = GetSize(entry);
                var item = new TextureItem(name, TextureFlags.None, size.Width, size.Height);
                list.Add(item);
            }

            return Task.FromResult<IEnumerable<TextureItem>>(list);
        }

        public override async Task<TextureItem> GetTexture(string name)
        {
            var textures = await GetTextures(new[] {name});
            return textures.FirstOrDefault();
        }

        public override ITextureStreamSource GetStreamSource()
        {
            return new SprStreamSource(_file);
        }
    }
}
