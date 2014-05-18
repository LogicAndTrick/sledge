using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Sledge.FileSystem;
using Sledge.Graphics.Helpers;
using Sledge.Packages;
using Sledge.Packages.Vpk;

namespace Sledge.Providers.Texture
{
    public class NewVtfProvider : TextureProvider
    {
        public override IEnumerable<TexturePackage> CreatePackages(IEnumerable<string> sourceRoots)
        {
            var roots = sourceRoots.ToList();

            // Scan for VPKs in the roots
            var streams = roots
                .Where(Directory.Exists)
                .SelectMany(x => Directory.GetFiles(x, "*_dir.vpk"))
                .Select(x => new FileInfo(x))
                .Where(x => x.Exists)
                .Select(x => new VpkDirectory(x))
                .Select(x => new { Directory = x, Stream = x.GetStreamSource() })
                .ToList();

            var packages = new Dictionary<string, TexturePackage>();

            // Grab all the loose VTF files (todo: this should be VMT)
            var loose = roots
                .Select(x => new {Dir = Path.GetFullPath(Path.Combine(x, "materials")), Source = x})
                .Where(x => Directory.Exists(x.Dir))
                .SelectMany(x => Directory.GetFiles(x.Dir, "*.vtf", SearchOption.AllDirectories)
                    .Select(f => new {Path = Path.GetFullPath(f).Substring(x.Dir.Length).TrimStart('\\', '/').Replace('\\', '/'), File = f, x.Source}))
                .ToDictionary(x => x.Path, x => Tuple.Create(x.File, x.Source));


            foreach (var kv in loose)
            {
                var idx = kv.Key.LastIndexOf('/');
                var dir = idx >= 0 ? kv.Key.Substring(0, idx) : "";
                var name = kv.Key.EndsWith(".vtf") ? kv.Key.Substring(0, kv.Key.Length - 4) : kv.Key;
                if (!packages.ContainsKey(dir)) packages.Add(dir, new TexturePackage(kv.Value.Item2, dir, this));
                var size = Vtf.VtfProvider.GetSize(new NativeFile(kv.Value.Item1));
                packages[dir].AddTexture(new TextureItem(packages[dir], name, size.Width, size.Height));
            }

            var packaged = streams
                .SelectMany(x => x.Stream.SearchFiles("materials", "\\.vtf$", true)
                    .Select(f => new { Path = f.Substring("materials/".Length), x.Stream, x.Directory}))
                .ToDictionary(x => x.Path, x => Tuple.Create(x.Stream, x.Directory));

            foreach (var kv in packaged)
            {
                var idx = kv.Key.LastIndexOf('/');
                var dir = idx >= 0 ? kv.Key.Substring(0, idx) : "";
                var name = kv.Key.EndsWith(".vtf") ? kv.Key.Substring(0, kv.Key.Length - 4) : kv.Key;
                if (!packages.ContainsKey(dir)) packages.Add(dir, new TexturePackage(kv.Value.Item2.PackageFile.FullName, dir, this));
                if (packages[dir].HasTexture(name)) continue;
                var size = Vtf.VtfProvider.GetSize(kv.Value.Item1.OpenFile("materials/" + kv.Key));
                packages[dir].AddTexture(new TextureItem(packages[dir], name, size.Width, size.Height));
            }

            foreach (var stream in streams)
            {
                stream.Stream.Dispose();
                stream.Directory.Dispose();
            }

            return packages.Values;
        }

        public override void DeletePackages(IEnumerable<TexturePackage> packages)
        {
            
        }

        private bool QuickCheckTransparent(Bitmap img)
        {
            if (((ImageFlags) img.Flags).HasFlag(ImageFlags.HasTranslucent)) return true;
            // Sample some pixels (edges, center, quarter midpoints)
            return img.GetPixel(0, 0).A < 255 ||
                   img.GetPixel(img.Width - 1, 0).A < 255 ||
                   img.GetPixel(img.Width - 1, img.Height - 1).A < 255 ||
                   img.GetPixel(0, img.Height - 1).A < 255 ||
                   img.GetPixel(img.Width / 2, img.Height / 2).A < 255 ||
                   img.GetPixel(img.Width / 4, img.Height / 2).A < 255 ||
                   img.GetPixel(3 * img.Width / 4, img.Height / 2).A < 255 ||
                   img.GetPixel(img.Width / 2, img.Height / 4).A < 255 ||
                   img.GetPixel(img.Width / 2, 3 * img.Height / 4).A < 255;
        }

        public override void LoadTextures(IEnumerable<TextureItem> items)
        {
            var groups = items.GroupBy(x => x.Package.PackageRoot);
            foreach (var group in groups)
            {
                if (group.Key.EndsWith(".vpk") && File.Exists(group.Key))
                {
                    using (var dir = new VpkDirectory(new FileInfo(group.Key)))
                    {
                        using (var stream = dir.GetStreamSource())
                        {
                            foreach (var ti in group)
                            {
                                var name = "materials/" + ti.Name + ".vtf";
                                if (!stream.HasFile(name)) continue;
                                using (var bmp = Vtf.VtfProvider.GetImage(stream.OpenFile(name)))
                                {
                                    TextureHelper.Create(ti.Name.ToLowerInvariant(), bmp, QuickCheckTransparent(bmp));
                                }
                            }
                        }
                    }
                }
                else if (Directory.Exists(group.Key))
                {
                    foreach (var ti in group)
                    {
                        var file = Path.Combine(group.Key, "materials", ti.Name + ".vtf");
                        if (!File.Exists(file)) continue;
                        using (var bmp = Vtf.VtfProvider.GetImage(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                        {
                            TextureHelper.Create(ti.Name.ToLowerInvariant(), bmp, QuickCheckTransparent(bmp));
                        }
                    }
                }
            }
        }

        public override ITextureStreamSource GetStreamSource(int maxWidth, int maxHeight, IEnumerable<TexturePackage> packages)
        {
            return new NewVtfStreamSource(maxWidth, maxHeight, packages);
        }

        private class NewVtfStreamSource : ITextureStreamSource
        {
            private readonly int _maxWidth;
            private readonly int _maxHeight;
            private readonly List<VpkDirectory> _directories;
            private readonly List<IPackageStreamSource> _streams;
            private readonly List<string> _folders;
            
            public NewVtfStreamSource(int maxWidth, int maxHeight, IEnumerable<TexturePackage> packages)
            {
                _maxWidth = maxWidth;
                _maxHeight = maxHeight;
                _directories = new List<VpkDirectory>();
                _streams = new List<IPackageStreamSource>();
                _folders = new List<string>();
                var groups = packages.GroupBy(x => x.PackageRoot);
                foreach (var group in groups)
                {
                    if (group.Key.EndsWith(".vpk") && File.Exists(group.Key))
                    {
                        var vpk = new VpkDirectory(new FileInfo(group.Key));
                        _directories.Add(vpk);
                        _streams.Add(vpk.GetStreamSource());
                    }
                    else if (Directory.Exists(group.Key))
                    {
                        _folders.Add(group.Key);
                    }
                }
            }
            
            public bool HasImage(TextureItem item)
            {
                var name = item.Name + ".vtf";
                return _folders.Any(x => File.Exists(Path.Combine(x, "materials", name)))
                       || _streams.Any(x => x.HasFile("materials/" + name));
            }

            public Bitmap GetImage(TextureItem item)
            {
                var name = item.Name + ".vtf";

                var native = _folders.Select(x => new FileInfo(Path.Combine(x, "materials", name))).FirstOrDefault(x => x.Exists);
                var strm = _streams.FirstOrDefault(x => x.HasFile("materials/" + name));

                Stream stream;
                if (native != null) stream = native.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                else if (strm != null) stream = strm.OpenFile("materials/" + name);
                else return null;

                using (stream)
                {
                    return Vtf.VtfProvider.GetImage(stream, _maxWidth, _maxHeight);
                }
            }
            
            public void Dispose()
            {
                _streams.ForEach(x => x.Dispose());
                _directories.ForEach(x => x.Dispose());
            }
        }
    }
}
