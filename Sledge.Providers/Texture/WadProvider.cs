using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.IO;
using Sledge.FileSystem;
using Sledge.Graphics.Helpers;
using System.Drawing;
using Sledge.Packages;
using Sledge.Packages.Wad;

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
            private readonly List<WadPackage> _packages;
            private readonly List<IPackageStreamSource> _streams;

            public WadStreamSource(IEnumerable<TexturePackage> packages)
            {
                _packages = packages.Select(x => new WadPackage(new FileInfo(x.PackageFile.FullPathName))).ToList();
                _streams = _packages.Select(x => x.GetStreamSource()).ToList();
            }

            public bool HasImage(TextureItem item)
            {
                return _streams.Any(x => x.HasFile(item.Name));
            }

            public Bitmap GetImage(TextureItem item)
            {
                using (var stream = _streams.First(x => x.HasFile(item.Name)).OpenFile(item.Name))
                {
                    bool hasTransparency;
                    return PostProcessBitmap(item.Name, new Bitmap(stream), out hasTransparency);
                }
            }

            public void Dispose()
            {
                _streams.ForEach(x => x.Dispose());
                _packages.ForEach(x => x.Dispose());
            }
        }

        public override bool IsValidForPackageFile(IFile package)
        {
            return package.FullPathName.EndsWith(".wad", true, CultureInfo.InvariantCulture) && File.Exists(package.FullPathName);
        }

        private const char NullCharacter = (char) 0;

        private bool LoadFromCache(TexturePackage package)
        {
            if (CachePath == null || !Directory.Exists(CachePath)) return false;

            var fi = new FileInfo(package.PackageFile.FullPathName);
            var cacheFile = Path.Combine(CachePath, fi.Name + "_" + (fi.LastWriteTime.Ticks));
            if (!File.Exists(cacheFile)) return false;

            var lines = File.ReadAllLines(cacheFile);
            if (lines.Length < 3) return false;
            if (lines[0] != fi.FullName) return false;
            if (lines[1] != fi.LastWriteTime.ToFileTime().ToString(CultureInfo.InvariantCulture)) return false;
            if (lines[2] != fi.Length.ToString(CultureInfo.InvariantCulture)) return false;

            try
            {
                var items = new List<TextureItem>();
                foreach (var line in lines.Skip(3))
                {
                    var spl = line.Split(NullCharacter);
                    items.Add(new TextureItem(package, spl[0], int.Parse(spl[1], CultureInfo.InvariantCulture), int.Parse(spl[2], CultureInfo.InvariantCulture)));
                }
                items.ForEach(package.AddTexture);
            }
            catch
            {
                // Cache file is no good...
                return false;
            }
            return true;
        }

        private void SaveToCache(TexturePackage package)
        {
            if (CachePath == null || !Directory.Exists(CachePath)) return;
            var fi = new FileInfo(package.PackageFile.FullPathName);
            var cacheFile = Path.Combine(CachePath, fi.Name + "_" + (fi.LastWriteTime.Ticks));
            var lines = new List<string>();
            lines.Add(fi.FullName);
            lines.Add(fi.LastWriteTime.ToFileTime().ToString(CultureInfo.InvariantCulture));;
            lines.Add(fi.Length.ToString(CultureInfo.InvariantCulture));
            foreach (var ti in package.Items.Values)
            {
                lines.Add(ti.Name + NullCharacter + ti.Width.ToString(CultureInfo.InvariantCulture) + NullCharacter + ti.Height.ToString(CultureInfo.InvariantCulture));
            }
            File.WriteAllLines(cacheFile, lines);
        }

        public override TexturePackage CreatePackage(IFile package)
        {
            if (!File.Exists(package.FullPathName)) throw new ProviderException("The WAD Provider only supports the native file system.");

            var tp = new TexturePackage(package, this);
            if (LoadFromCache(tp)) return tp;

            var list = new List<TextureItem>();
            using (var pack = new WadPackage(new FileInfo(package.FullPathName)))
            {
                list.AddRange(pack.GetEntries().OfType<WadEntry>().Select(x => new TextureItem(tp, x.Name, (int) x.Width, (int) x.Height)));
            }
            foreach (var ti in list)
            {
                tp.AddTexture(ti);
            }
            SaveToCache(tp);
            return tp;
        }

        public override void LoadTexture(TextureItem item)
        {
            LoadTextures(new[] {item});
        }

        public override void LoadTextures(IEnumerable<TextureItem> items)
        {
            var list = items.ToList();
            var packages = list.Select(x => x.Package).Distinct().ToList();
            var packs = packages.Select(x => new WadPackage(new FileInfo(x.PackageFile.FullPathName))).ToList();
            var streams = packs.Select(x => x.GetStreamSource()).ToList();
            foreach (var ti in list)
            {
                foreach (var stream in streams)
                {
                    var open = stream.OpenFile(ti.Name);
                    if (open == null) continue;
                    var bmp = new Bitmap(open);
                    bool hasTransparency;
                    bmp = PostProcessBitmap(ti.Name, bmp, out hasTransparency);
                    TextureHelper.Create(ti.Name.ToLowerInvariant(), bmp, hasTransparency);
                    bmp.Dispose();
                    open.Dispose();
                    break;
                }
            }
            foreach (var pack in packs) pack.Dispose();
        }

        public override ITextureStreamSource GetStreamSource(IEnumerable<TexturePackage> packages)
        {
            return new WadStreamSource(packages);
        }
    }
}
