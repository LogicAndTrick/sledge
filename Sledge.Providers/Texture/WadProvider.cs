using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Sledge.Graphics;
using Sledge.Graphics.Helpers;
using Sledge.Libs.HLLib;
using System.Drawing;

namespace Sledge.Providers.Texture
{
    public class WadProvider : TextureProvider
    {
        public class WadStreamSource : TextureStreamSource
        {
            private readonly Dictionary<string, Tuple<HLLib.Package, HLLib.Folder>> _packages;

            public WadStreamSource(IEnumerable<TexturePackage> packages) : base(packages)
            {
                _packages = new Dictionary<string, Tuple<HLLib.Package, HLLib.Folder>>();
                HLLib.Initialize();
                foreach (var tp in Packages)
                {
                    var pack = new HLLib.Package(tp.PackageFile);
                    _packages.Add(tp.PackageFile, Tuple.Create(pack, pack.GetRootFolder()));
                }
            }

            public override Bitmap GetImage(TextureItem item)
            {
                var root = _packages[item.Package.PackageFile];
                var search = root.Item2.GetItemByName(item.Name + ".bmp", HLLib.FindType.Files);
                if (search.Exists)
                {
                    using (var stream = root.Item1.CreateStream(search))
                    {
                        return new Bitmap(new MemoryStream(stream.ReadAll()));
                    }
                }
                return null;
            }

            public override void Dispose()
            {
                foreach (var root in _packages)
                {
                    try
                    {
                        root.Value.Item1.Dispose();
                    }
                    catch (Exception)
                    {
                        // Continue regardless
                    }
                }
                HLLib.Shutdown();
            }
        }

        protected override bool IsValidForPackageFile(string package)
        {
            return package.EndsWith(".wad") && File.Exists(package);
        }

        protected override void LoadTexture(TexturePackage package, string name)
        {
            try
            {
                HLLib.Initialize();
                using (var pack = new HLLib.Package(package.PackageFile))
                {
                    var item = pack.GetRootFolder().GetItemByName(name + ".bmp", HLLib.FindType.Files);
                    using (var stream = pack.CreateStream(item))
                    {
                        using (var bmp = new Bitmap(new MemoryStream(stream.ReadAll())))
                        {
                            TextureHelper.Create(name, bmp);
                        }
                    }
                }
            }
            finally
            {
                HLLib.Shutdown();
            }
        }

        protected override void LoadTextures(TexturePackage package, IEnumerable<string> names)
        {
            try
            {
                HLLib.Initialize();
                using (var pack = new HLLib.Package(package.PackageFile))
                {
                    var folder = pack.GetRootFolder();
                    foreach (var name in names)
                    {
                        var item = folder.GetItemByName(name + ".bmp", HLLib.FindType.Files);
                        if (!item.Exists) continue;
                        using (var stream = pack.CreateStream(item))
                        {
                            using (var bmp = new Bitmap(new MemoryStream(stream.ReadAll())))
                            {
                                TextureHelper.Create(name, bmp);
                            }
                        }
                    }
                }
            }
            finally
            {
                HLLib.Shutdown();
            }
        }

        protected override IEnumerable<TextureItem> GetAllTextureItems(TexturePackage package)
        {
            var ret = new List<TextureItem>();
            try
            {
                HLLib.Initialize();
                using (var pack = new HLLib.Package(package.PackageFile))
                {
                    var folder = pack.GetRootFolder();
                    var items = folder.GetItems();
                    ret.AddRange(items
                        .Select(item => new HLLib.WADFile(item))
                        .Select(wad => new TextureItem(package, Path.GetFileNameWithoutExtension(wad.Name), wad.Width, wad.Height)));
                }
            }
            finally
            {
                HLLib.Shutdown();
            }
            return ret;
        }

        protected override TextureStreamSource GetStreamSource(IEnumerable<TexturePackage> packages)
        {
            return new WadStreamSource(packages);
        }
    }
}
