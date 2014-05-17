using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Sledge.FileSystem;
using Sledge.Graphics.Helpers;
/*
namespace Sledge.Providers.Texture
{
    public class VtfProvider : TextureProvider
    {
        public override bool IsValidForPackageFile(IFile package)
        {
            return package.IsContainer;
        }

        public override TexturePackage CreatePackage(IFile package)
        {
            var entries = package.GetFilesWithExtension("vtf");
            var pack = new TexturePackage(package, this);
            foreach (var file in entries)
            {
                var size = Vtf.VtfProvider.GetSize(file);
                pack.AddTexture(new TextureItem(pack, GetTextureName(file), size.Width, size.Height));
            }
            return pack;
        }

        private string GetTextureName(IFile file)
        {
            var spl = file.FullPathName.Split('/', '\\', '.');
            var name = spl.SkipWhile(x => !String.Equals(x, "materials", StringComparison.CurrentCultureIgnoreCase)).ToArray();
            return name.Length <= 2 ? file.FullPathName : String.Join("/", name.ToArray(), 1, name.Length - 2);
        }

        public override void LoadTexture(TextureItem item)
        {
            LoadTextures(new[] { item });
        }

        public override void LoadTextures(IEnumerable<TextureItem> items)
        {
            var list = items.ToList();
            var folders = list.Select(x => x.Package.PackageFile).GroupBy(x => x.FullPathName).Select(x => x.First());
            var files = folders.SelectMany(x => x.GetFilesWithExtension("vtf")).ToDictionary(GetTextureName, x => x);
            var bitmaps = list.AsParallel().Where(x => files.ContainsKey(x.Name)).Select(ti =>
            {
                return new
                {
                    Bitmap = Vtf.VtfProvider.GetImage(files[ti.Name]),
                    Name = ti.Name.ToLowerInvariant(),
                    HasTransparency = false
                };
            });
            // TextureHelper.Create must run on the UI thread
            foreach (var bmp in bitmaps)
            {
                TextureHelper.Create(bmp.Name, bmp.Bitmap, bmp.HasTransparency);
                bmp.Bitmap.Dispose();
            }
        }

        public override ITextureStreamSource GetStreamSource(IEnumerable<TexturePackage> packages)
        {
            return new VtfStreamSource(packages);
        }

        private class VtfStreamSource : ITextureStreamSource
        {
            private readonly List<IFile> _folders;
            private readonly Dictionary<string, IFile> _files;

            public VtfStreamSource(IEnumerable<TexturePackage> packages)
            {
                _folders = packages.Select(x => x.PackageFile).GroupBy(x => x.FullPathName).Select(x => x.First()).ToList();
                _files = _folders.SelectMany(x => x.GetFilesWithExtension("vtf")).ToDictionary(GetTextureName, x => x);
            }

            private string GetTextureName(IFile file)
            {
                var spl = file.FullPathName.Split('/', '\\', '.');
                var name = spl.SkipWhile(x => !String.Equals(x, "materials", StringComparison.CurrentCultureIgnoreCase)).ToArray();
                return name.Length <= 2 ? file.FullPathName : String.Join("/", name.ToArray(), 1, name.Length - 2);
            }

            public bool HasImage(TextureItem item)
            {
                return _files.ContainsKey(item.Name);
            }

            public Bitmap GetImage(TextureItem item)
            {
                return Vtf.VtfProvider.GetImage(_files[item.Name]);
            }

            public void Dispose()
            {
                _files.Clear();
                _folders.Clear();
            }
        }
    }
}
*/