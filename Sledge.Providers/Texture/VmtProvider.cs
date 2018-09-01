using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Sledge.Common;
using Sledge.Packages;
using Sledge.Packages.Vpk;
using Sledge.Rendering.Materials;

namespace Sledge.Providers.Texture
{
    public class VmtProvider : TextureProvider
    {
        private readonly Dictionary<TexturePackage, QuickRoot> _roots = new Dictionary<TexturePackage, QuickRoot>();

        public override IEnumerable<TexturePackage> CreatePackages(IEnumerable<string> sourceRoots, IEnumerable<string> additionalPackages, IEnumerable<string> blacklist, IEnumerable<string> whitelist)
        {
            var blist = blacklist.Select(x => x.TrimEnd('/', '\\')).Where(x => !String.IsNullOrWhiteSpace(x)).ToList();
            var wlist = whitelist.Select(x => x.TrimEnd('/', '\\')).Where(x => !String.IsNullOrWhiteSpace(x)).ToList();

            var roots = sourceRoots.ToList();
            var packages = new Dictionary<string, TexturePackage>();

            var types = new HashSet<string>
            {
                "unlitgeneric",
                "lightmappedgeneric",
                "lightmappedreflective",
                "water",
                "sprite",
                "decalmodulate",
                "modulate",
                "subrect",
                "worldvertextransition",
                "lightmapped_4wayblend",
                "unlittwotexture",
                "worldtwotextureblend",
                "skyfog"
            };

            var packageRoot = String.Join(";", roots);
            var add = additionalPackages.ToList();

            var vmtRoot = new QuickRoot(roots, add, "materials", ".vmt");
            var vtfRoot = new QuickRoot(roots, add, "materials", ".vtf");

            const StringComparison ctype = StringComparison.InvariantCultureIgnoreCase;

            foreach (var vmt in vmtRoot.GetFiles())
            {
                var idx = vmt.LastIndexOf('/');
                var dir = idx >= 0 ? vmt.Substring(0, idx) : "";

                if ((blist.Any(x => dir.Equals(x, ctype) || dir.StartsWith(x + '/', ctype))) ||
                    (wlist.Any() && !wlist.Any(x => dir.Equals(x, ctype) || dir.StartsWith(x + '/', ctype))))
                {
                    continue;
                }

                if (!packages.ContainsKey(dir)) packages.Add(dir, new TexturePackage(packageRoot, dir, this));
                if (packages[dir].HasTexture(vmt)) continue;

                var gs = GenericStructure.Parse(new StreamReader(vmtRoot.OpenFile(vmt))).FirstOrDefault();
                if (gs == null || !types.Contains(gs.Name.ToLowerInvariant())) continue;

                var baseTexture = gs.GetPropertyValue("$basetexture", true);
                if (baseTexture == null) continue;
                baseTexture = baseTexture.ToLowerInvariant().Replace('\\', '/');

                if (!vtfRoot.HasFile(baseTexture)) continue;

                var size = Vtf.VtfProvider.GetSize(vtfRoot.OpenFile(baseTexture));
                packages[dir].AddTexture(new TextureItem(packages[dir], vmt, GetFlags(gs), baseTexture, size.Width, size.Height));
            }

            vmtRoot.Dispose();

            foreach (var tp in packages.Values)
            {
                _roots.Add(tp, vtfRoot);
            }

            return packages.Values;
        }

        private TextureFlags GetFlags(GenericStructure vmt)
        {
            var flags = TextureFlags.None;
            var tp = vmt.PropertyInteger("$translucent") + vmt.PropertyInteger("$alphatest");
            if (tp > 0) flags |= TextureFlags.Transparent;
            return flags;
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

        public override ITextureStreamSource GetStreamSource(int maxWidth, int maxHeight, IEnumerable<TexturePackage> packages)
        {
            return new NewVtfStreamSource(maxWidth, maxHeight, packages, packages.Select(x => _roots[x]));
        }

        private class NewVtfStreamSource : ITextureStreamSource
        {
            private readonly int _maxWidth;
            private readonly int _maxHeight;
            private readonly List<QuickRoot> _roots;
            
            public NewVtfStreamSource(int maxWidth, int maxHeight, IEnumerable<TexturePackage> packages, IEnumerable<QuickRoot> roots)
            {
                _maxWidth = maxWidth;
                _maxHeight = maxHeight;
                _roots = roots.ToList();
            }
            
            public bool HasImage(TextureItem item)
            {
                return _roots.Any(x => x.HasFile(item.PrimarySubItem.Name));
            }

            public Bitmap GetImage(TextureItem item)
            {
                var root = _roots.FirstOrDefault(x => x.HasFile(item.PrimarySubItem.Name));
                if (root == null) return null;
                var stream = root.OpenFile(item.PrimarySubItem.Name);
                if (stream == null) return null;

                using (stream)
                {
                    return Vtf.VtfProvider.GetImage(stream, _maxWidth, _maxHeight);
                }
            }
            
            public void Dispose()
            {

            }
        }

        private class QuickRoot : IDisposable
        {
            private readonly List<string> _roots;
            private readonly List<VpkDirectory> _vpks;
            private readonly List<IPackageStreamSource> _streams;
            private readonly List<string> _files;
            private readonly string _baseFolder;
            private readonly string _extension;
            private List<string> _extras;

            public QuickRoot(IEnumerable<string> roots, IEnumerable<string> additional, string baseFolder, string extension)
            {
                _baseFolder = baseFolder;
                _extension = extension;
                _roots = roots.ToList();
                var streams = _roots
                    .Where(Directory.Exists)
                    .SelectMany(x => Directory.GetFiles(x, "*_dir.vpk"))
                    .Select(x => new FileInfo(x))
                    .Where(x => x.Exists)
                    .AsParallel()
                    .Select(x => new VpkDirectory(x))
                    .Select(x => new { Directory = x, Stream = x.GetStreamSource() })
                    .ToList();
                _vpks = streams.Select(x => x.Directory).ToList();
                _streams = streams.Select(x => x.Stream).ToList();
                _extras = additional.Select(x => Directory.Exists(Path.Combine(x, baseFolder)) ? Path.Combine(x, baseFolder) : x).Where(Directory.Exists).ToList();
                _files = _streams
                    .SelectMany(x => x.SearchFiles(baseFolder, "\\" + extension + "$", true))
                    .Union(_roots.Where(x => Directory.Exists(Path.Combine(x, baseFolder)))
                        .SelectMany(x => Directory.GetFiles(Path.Combine(x, baseFolder), "*" + extension, SearchOption.AllDirectories)
                            .Select(f => MakeRelative(x, f))))
                    .Union(_extras.SelectMany(x => Directory.GetFiles(x, "*" + extension, SearchOption.AllDirectories)
                        .Select(f => MakeRelative(x, f))))
                    .GroupBy(x => x)
                    .Select(x => StripBase(x.First()))
                    .ToList();
            }

            private string MakeRelative(string baseFolder, string file)
            {
                return Path.GetFullPath(file).Substring(Path.GetFullPath(baseFolder).Length).TrimStart('\\', '/').Replace('\\', '/');
            }

            private string StripBase(string path)
            {
                if (path.StartsWith(_baseFolder)) path = path.Substring(_baseFolder.Length);
                if (path.EndsWith(_extension)) path = path.Substring(0, path.Length - _extension.Length);
                return path.TrimStart('/');
            }

            public IEnumerable<string> GetFiles()
            {
                return _files;
            }

            public bool HasFile(string path)
            {
                return _extras.Any(x => File.Exists(Path.Combine(x, path + _extension)))
                       || _roots.Any(x => File.Exists(Path.Combine(x, _baseFolder, path + _extension)))
                       || _streams.Any(x => x.HasFile(_baseFolder + "/" + path + _extension));
            }

            public Stream OpenFile(string path)
            {
                foreach (var extra in _extras)
                {
                    var p = Path.Combine(extra, path + _extension);
                    if (File.Exists(p))
                    {
                        return File.Open(p, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    }
                }
                foreach (var root in _roots)
                {
                    var p = Path.Combine(root, _baseFolder, path + _extension);
                    if (File.Exists(p))
                    {
                        return File.Open(p, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    }
                }
                foreach (var ss in _streams)
                {
                    var p = _baseFolder + "/" + path + _extension;
                    if (ss.HasFile(p)) return ss.OpenFile(p);
                }
                return null;
            }

            public void Dispose()
            {
                _streams.ForEach(x => x.Dispose());
                _vpks.ForEach(x => x.Dispose());
            }
        }
    }
}
