using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Threading;
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

        private const char UnitSeparator = (char) 31;

        private bool LoadFromCache(TexturePackage package)
        {
            if (CachePath == null || !Directory.Exists(CachePath)) return false;
            var fi = new FileInfo(package.PackageFile);
            var cacheFile = Path.Combine(CachePath, fi.Name + "_" + (fi.LastWriteTime.Ticks));
            if (!File.Exists(cacheFile)) return false;
            var lines = File.ReadAllLines(cacheFile);
            if (lines.Length < 3) return false;
            if (lines[0] != fi.FullName) return false;
            if (lines[1] != fi.LastWriteTime.ToFileTime().ToString(CultureInfo.InvariantCulture)) return false;
            if (lines[2] != fi.Length.ToString(CultureInfo.InvariantCulture)) return false;
            foreach (var line in lines.Skip(3))
            {
                var spl = line.Split(UnitSeparator);
                package.AddTexture(new TextureItem(package, spl[0], int.Parse(spl[1], CultureInfo.InvariantCulture), int.Parse(spl[2], CultureInfo.InvariantCulture)));
            }
            return true;
        }

        private void SaveToCache(TexturePackage package)
        {
            if (CachePath == null || !Directory.Exists(CachePath)) return;
            var fi = new FileInfo(package.PackageFile);
            var cacheFile = Path.Combine(CachePath, fi.Name + "_" + (fi.LastWriteTime.Ticks));
            var lines = new List<string>();
            lines.Add(fi.FullName);
            lines.Add(fi.LastWriteTime.ToFileTime().ToString(CultureInfo.InvariantCulture));;
            lines.Add(fi.Length.ToString(CultureInfo.InvariantCulture));
            foreach (var ti in package.Items.Values)
            {
                lines.Add(ti.Name + UnitSeparator + ti.Width.ToString(CultureInfo.InvariantCulture) + UnitSeparator + ti.Height.ToString(CultureInfo.InvariantCulture));
            }
            File.WriteAllLines(cacheFile, lines);
        }

        public override TexturePackage CreatePackage(string package)
        {
            var tp = new TexturePackage(package, this);
            if (LoadFromCache(tp)) return tp;

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
                        .Select(wad => new TextureItem(tp, StripExtension(wad.Name), wad.Width, wad.Height)));
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
            SaveToCache(tp);
            return tp;
        }

        public static string StripExtension(string path)
        {
            // WAD seems to support texture names that aren't valid in Windows, so Path.GetFileNameWithoutExtension throws an error.
            // Replicate the results with StripExtension.
            // Input = FILE_NAME.bmp
            var idx = path.LastIndexOf('.');
            return idx < 0 ? path : path.Substring(0, idx);
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
