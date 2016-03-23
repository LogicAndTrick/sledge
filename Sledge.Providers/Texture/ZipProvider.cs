using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using Sledge.Common;
using Sledge.Graphics.Helpers;
using System.Drawing;
using Sledge.Packages;
using Sledge.Packages.Zip;

namespace Sledge.Providers.Texture
{
    public class ZipProvider : TextureProvider
    {
        public static bool ReplaceTransparentPixels = true;

        private static Bitmap PostProcessBitmap(string packageName, string name, Bitmap bmp, out bool hasTransparency)
        {
            hasTransparency = false;
            return bmp;
        }

        private const char NullCharacter = (char)0;

        private bool LoadFromCache(TexturePackage package)
        {
            if (CachePath == null || !Directory.Exists(CachePath)) return false;

            var fi = new FileInfo(package.PackageRoot);
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
                    items.Add(new TextureItem(package, spl[0], GetFlags(spl[0]), int.Parse(spl[1], CultureInfo.InvariantCulture), int.Parse(spl[2], CultureInfo.InvariantCulture)));
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

        // TODO: Figure out how id Tech 3 handles this stuff.
        private TextureFlags GetFlags(string name)
        {
            var flags = TextureFlags.None;
            //var tp = ReplaceTransparentPixels && name.StartsWith("{");
            //if (tp) flags |= TextureFlags.Transparent;
            return flags;
        }

        private void SaveToCache(TexturePackage package)
        {
            if (CachePath == null || !Directory.Exists(CachePath)) return;
            var fi = new FileInfo(package.PackageRoot);
            var cacheFile = Path.Combine(CachePath, fi.Name + "_" + (fi.LastWriteTime.Ticks));
            var lines = new List<string>();
            lines.Add(fi.FullName);
            lines.Add(fi.LastWriteTime.ToFileTime().ToString(CultureInfo.InvariantCulture));
            lines.Add(fi.Length.ToString(CultureInfo.InvariantCulture));
            foreach (var ti in package.Items.Values)
            {
                lines.Add(ti.Name + NullCharacter + ti.Width.ToString(CultureInfo.InvariantCulture) + NullCharacter + ti.Height.ToString(CultureInfo.InvariantCulture));
            }
            File.WriteAllLines(cacheFile, lines);
        }

        private readonly Dictionary<TexturePackage, ZipStream> _roots = new Dictionary<TexturePackage, ZipStream>();

        private TexturePackage CreatePackage(string package)
        {
            try
            {
                var fi = new FileInfo(package);
                if (!fi.Exists) return null;

                var tp = new TexturePackage(package, Path.GetFileNameWithoutExtension(package), this);
                if (LoadFromCache(tp)) return tp;

                var list = new List<TextureItem>();

                var pack = _roots.Values.FirstOrDefault(x => x.Package.PackageFile.FullName == fi.FullName);
                if (pack == null) _roots.Add(tp, pack = new ZipStream(new ZipPackage(fi)));

                list.AddRange(pack.Package.GetEntries().OfType<ZipEntry>().Where(x => x.ContentType != ZipEntry.ContentTypes.None).Select(x => new TextureItem(tp, x.Name, GetFlags(x.Name), x.Width, x.Height)));
                foreach (var ti in list)
                {
                    tp.AddTexture(ti);
                }
                SaveToCache(tp);
                return tp;
            }
            catch
            {
                return null;
            }
        }

        public override IEnumerable<TexturePackage> CreatePackages(IEnumerable<string> sourceRoots, IEnumerable<string> additionalPackages, IEnumerable<string> blacklist, IEnumerable<string> whitelist)
        {
            var blist = blacklist.Select(x => x.EndsWith(".pk3") ? x.Substring(0, x.Length - 4) : x).Where(x => !String.IsNullOrWhiteSpace(x)).ToList();
            var wlist = whitelist.Select(x => x.EndsWith(".pk3") ? x.Substring(0, x.Length - 4) : x).Where(x => !String.IsNullOrWhiteSpace(x)).ToList();
            var pk3s = sourceRoots.Union(additionalPackages)
                .Where(Directory.Exists)
                .SelectMany(x => Directory.GetFiles(x, "*.pk3", SearchOption.TopDirectoryOnly))
                .Union(additionalPackages.Where(x => x.EndsWith(".pk3") && File.Exists(x)))
                .GroupBy(Path.GetFileNameWithoutExtension)
                .Select(x => x.First())
                .Where(x => !blist.Any(b => String.Equals(Path.GetFileNameWithoutExtension(x) ?? x, b, StringComparison.InvariantCultureIgnoreCase)));
            if (wlist.Any())
            {
                pk3s = pk3s.Where(x => wlist.Contains(Path.GetFileNameWithoutExtension(x) ?? x, StringComparer.InvariantCultureIgnoreCase));
            }
            return pk3s.AsParallel().Select(CreatePackage).Where(x => x != null);
        }

        public override void DeletePackages(IEnumerable<TexturePackage> packages)
        {
            var packs = packages.ToList();
            var roots = _roots.Where(x => packs.Contains(x.Key)).Select(x => x.Value).ToList();
            foreach (var tp in packs)
            {
                _roots.Remove(tp);
            }
            foreach (var root in roots.Where(x => !_roots.ContainsValue(x)))
            {
                root.Dispose();
            }
        }

        public override void LoadTextures(IEnumerable<TextureItem> items)
        {
            var list = items.ToList();
            var packages = list.Select(x => x.Package).Distinct().ToList();
            var packs = packages.Select(x =>
            {
                if (!_roots.ContainsKey(x))
                {
                    var wp = new ZipStream(new ZipPackage(new FileInfo(x.PackageRoot)));
                    _roots.Add(x, wp);
                }
                return _roots[x];

            }).ToList();
            var streams = packs.Select(x => x.StreamSource).ToList();

            // Process the bitmaps in parallel
            var bitmaps = list.AsParallel().Select(ti =>
            {
                var stream = streams.FirstOrDefault(x => PackageHasTexture(x, ti.Name));
                if (stream == null) return null;

                var open = stream.OpenFile($"textures/{ti.Name.ToLowerInvariant()}");
                if (open == null) return null;

                var bmp = new Bitmap(open);
                bool hasTransparency;
                bmp = PostProcessBitmap(ti.Package.PackageRelativePath, ti.Name.ToLowerInvariant(), bmp, out hasTransparency);
                open.Dispose();

                return new
                {
                    Bitmap = bmp,
                    Name = ti.Name.ToLowerInvariant(),
                    ti.Width,
                    ti.Height,
                    ti.Flags
                };
            }).Where(x => x != null);

            // TextureHelper.Create must run on the UI thread
            foreach (var bmp in bitmaps)
            {
                TextureHelper.Create(bmp.Name, bmp.Bitmap, bmp.Width, bmp.Height, bmp.Flags);
                bmp.Bitmap.Dispose();
            }
        }

	    private static bool PackageHasTexture(IPackageStreamSource package, string name)
	    {
		    return package.HasFile($"textures/{name.ToLowerInvariant()}");
	    }

	    public override ITextureStreamSource GetStreamSource(int maxWidth, int maxHeight, IEnumerable<TexturePackage> packages)
        {
            var packs = packages.Select(x =>
            {
                if (!_roots.ContainsKey(x))
                {
                    var wp = new ZipStream(new ZipPackage(new FileInfo(x.PackageRoot)));
                    _roots.Add(x, wp);
                }
                return _roots[x];

            }).ToList();
            var streams = packs.Select(x => x.StreamSource).ToList();
            return new ZipStreamSource(streams);
        }

        private class ZipStreamSource : ITextureStreamSource
        {
            private readonly List<IPackageStreamSource> _streams;

            public ZipStreamSource(IEnumerable<IPackageStreamSource> streams)
            {
                _streams = streams.ToList();
            }

            public bool HasImage(TextureItem item)
            {
                return _streams.Any(x => PackageHasTexture(x, item.Name));
            }

            public Bitmap GetImage(TextureItem item)
            {
                using (var stream = _streams.First(x => PackageHasTexture(x, item.Name)).OpenFile($"textures/{item.Name.ToLowerInvariant()}"))
                {
                    bool hasTransparency;
                    return PostProcessBitmap(item.Package.PackageRelativePath, item.Name.ToLowerInvariant(), new Bitmap(stream), out hasTransparency);
                }
            }

            public void Dispose()
            {

            }
        }

        private class ZipStream : IDisposable
        {
            public ZipPackage Package { get; set; }
            public IPackageStreamSource StreamSource { get; private set; }

            public ZipStream(ZipPackage package)
            {
                Package = package;
                StreamSource = package.GetStreamSource();
            }

            public void Dispose()
            {
                StreamSource.Dispose();
                Package.Dispose();
            }
        }
    }
}
