using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
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
        private static Bitmap PostProcessBitmap(string name, Bitmap bmp)
        {
            // Transparent textures are named like: {Name
            if (name.StartsWith("{"))
            {
                var palette = bmp.Palette;

                // Two transparency types: "blue" transparency and "decal" transparency
                // Decal transparency is all greyscale and doesn't contain any of palette #255 colour
                var blueTransparency = false;
                for (var i = 0; i < palette.Entries.Length - 1; i++)
                {
                    var color = palette.Entries[i];
                    if (color.R != color.B || color.R != color.G || color.B != color.G)
                    {
                        blueTransparency = true;
                        break;
                    }
                }

                if (blueTransparency)
                {
                    // We found the last index, therefore it should be transparent
                    palette.Entries[palette.Entries.Length - 1] = Color.Transparent;
                }
                else
                {
                    // If we didn't find the last index, we have a decal
                    var last = palette.Entries[palette.Entries.Length - 1];
                    for (var i = 0; i < palette.Entries.Length - 1; i++)
                    {
                        palette.Entries[i] = Color.FromArgb(255 - palette.Entries[i].R, last);
                    }
                }
                bmp.Palette = palette;

                // Copy the bitmap to a new one with a proper alpha channel
                var clone = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb);
                using (var g = System.Drawing.Graphics.FromImage(clone))
                {
                    g.DrawImage(bmp, new Rectangle(0, 0, clone.Width, clone.Height));
                }
                bmp.Dispose();
                return clone;
            }
            return bmp;
        }

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
                        var bmp = new Bitmap(new MemoryStream(stream.ReadAll()));
                        return PostProcessBitmap(item.Name, bmp);
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
                        var bmp = new Bitmap(new MemoryStream(stream.ReadAll()));
                        bmp = PostProcessBitmap(name, bmp);
                        TextureHelper.Create(name, bmp);
                        bmp.Dispose();
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
                            var bmp = new Bitmap(new MemoryStream(stream.ReadAll()));
                            bmp = PostProcessBitmap(name, bmp);
                            TextureHelper.Create(name, bmp);
                            bmp.Dispose();
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
