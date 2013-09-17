using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
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
        public static bool ReplaceTransparentPixels = true;

        private static Bitmap PostProcessBitmap(string name, Bitmap bmp, out bool hasTransparency)
        {
            hasTransparency = false;
            // Transparent textures are named like: {Name
            if (ReplaceTransparentPixels && name.StartsWith("{"))
            {
                hasTransparency = true;
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

        private class WadStreamSource : ITextureStreamSource
        {
            private readonly List<TexturePackage> _texturePackages;
            private readonly Dictionary<string, Tuple<HLLib.Package, HLLib.Folder>> _packages;

            public WadStreamSource(IEnumerable<TexturePackage> packages)
            {
                _texturePackages = new List<TexturePackage>();
                _packages = new Dictionary<string, Tuple<HLLib.Package, HLLib.Folder>>();
                HLLib.Initialize();
                foreach (var tp in packages)
                {
                    if (_packages.ContainsKey(tp.PackageFile)) continue;
                    var pack = new HLLib.Package(tp.PackageFile);
                    _texturePackages.Add(tp);
                    _packages.Add(tp.PackageFile, Tuple.Create(pack, pack.GetRootFolder()));
                }
            }

            public bool HasImage(TextureItem item)
            {
                return _texturePackages.Any(x => x.Items.ContainsValue(item));
            }

            public Bitmap GetImage(TextureItem item)
            {
                var root = _packages[item.Package.PackageFile];
                var search = root.Item2.GetItemByName(item.Name + ".bmp", HLLib.FindType.Files);
                if (search.Exists)
                {
                    using (var stream = root.Item1.CreateStream(search))
                    {
                        var bmp = new Bitmap(new MemoryStream(stream.ReadAll()));
                        bool hasTransparency;
                        return PostProcessBitmap(item.Name, bmp, out hasTransparency);
                    }
                }
                return null;
            }

            public void Dispose()
            {
                foreach (var root in _packages)
                {
                    try
                    {
                        root.Value.Item1.Dispose();
                    }
                    catch
                    {
                        // Continue regardless
                    }
                }
                HLLib.Shutdown();
            }
        }

        public override bool IsValidForPackageFile(string package)
        {
            return package.EndsWith(".wad", true, CultureInfo.InvariantCulture) && File.Exists(package);
        }

        private bool IsValidLumpType(uint type)
        {
            return type == 0x42 || type == 0x43;
        }

        public override TexturePackage CreatePackage(string package)
        {
            var tp = new TexturePackage(package, this);
            var list = new List<TextureItem>();
            try
            {
                HLLib.Initialize();
                using (var pack = new HLLib.Package(package))
                {
                    var folder = pack.GetRootFolder();
                    var items = folder.GetItems();
                    list.AddRange(items
                        .Select(item => new HLLib.WADFile(item))
                        .Where(wad => IsValidLumpType(wad.GetLumpType()))
                        .Select(wad => new TextureItem(tp, Path.GetFileNameWithoutExtension(wad.Name), wad.Width, wad.Height)));
                }
            }
            finally
            {
                HLLib.Shutdown();
            }
            foreach (var ti in list)
            {
                tp.AddTexture(ti);
            }
            return tp;
        }

        public override void LoadTexture(TextureItem item)
        {
            LoadTextures(new[] {item});
        }

        public override void LoadTextures(IEnumerable<TextureItem> items)
        {
            var list = items.ToList();
            var packages = list.Select(x => x.Package).Distinct();
            try
            {
                HLLib.Initialize();
                foreach (var package in packages)
                {
                    var p = package;
                    using (var pack = new HLLib.Package(p.PackageFile))
                    {
                        var folder = pack.GetRootFolder();
                        foreach (var ti in list.Where(x => x.Package == p))
                        {
                            var item = folder.GetItemByName(ti.Name + ".bmp", HLLib.FindType.Files);
                            if (!item.Exists) continue;
                            using (var stream = pack.CreateStream(item))
                            {
                                var bmp = new Bitmap(new MemoryStream(stream.ReadAll()));
                                bool hasTransparency;
                                bmp = PostProcessBitmap(ti.Name, bmp, out hasTransparency);
                                TextureHelper.Create(ti.Name.ToLowerInvariant(), bmp, hasTransparency);
                                bmp.Dispose();
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

        public override ITextureStreamSource GetStreamSource(IEnumerable<TexturePackage> packages)
        {
            return new WadStreamSource(packages);
        }
    }
}
