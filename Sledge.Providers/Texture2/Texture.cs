using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicAndTrick.Gimme.Providers;
using Sledge.Packages.Vpk;
using Sledge.Packages.Wad;

namespace Sledge.Providers.Texture2
{
    /// <summary>
    /// A list of textures. We don't care what they are at this point, just that they exist.
    /// </summary>
    public class TexturePackage
    {
        public string Root { get; private set; }
        public string RelativePath { get; private set; }
        public HashSet<string> Items { get; private set; }

        public TexturePackage(string root, string relativePath, IEnumerable<string> items)
        {
            Root = root;
            RelativePath = relativePath;
            Items = new HashSet<string>(items);
        }
    }

    /// <summary>
    /// A texture item. Now that we are at this point, we care about the width, height, flags and other properties of the texture.
    /// </summary>
    public class TextureItem
    {
        public TexturePackage Package { get; private set; }
        public string Name { get; private set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class WadTexturePackageProvider : SyncResourceProvider<TexturePackage>
    {
        public override bool CanProvide(string location)
        {
            return Directory.Exists(location) && Directory.EnumerateFiles(location, "*.wad").Any();
        }

        public override IEnumerable<TexturePackage> Fetch(string location, List<string> resources)
        {
            if (resources == null || resources.Count == 0)
            {
                resources = Directory.EnumerateFiles(location, "*.wad").Select(Path.GetFileName).ToList();
            }
            foreach (var resource in resources)
            {
                var path = Path.Combine(location, resource);
                if (!File.Exists(path)) continue;
                yield return new TexturePackage(path, Path.GetFileNameWithoutExtension(path), WadPackage.GetEntryNames(new FileInfo(path)));
            }
        }
    }

    public class VmtTexturePackageProvider : SyncResourceProvider<TexturePackage>
    {
        public override bool CanProvide(string location)
        {
            return Directory.Exists(location) && (Directory.EnumerateFiles(location, "*_dir.vpk").Any() || Directory.Exists(Path.Combine(location, "materials")));
        }

        public override IEnumerable<TexturePackage> Fetch(string location, List<string> resources)
        {
            if (resources == null || resources.Count == 0)
            {
                resources = new List<string>();
                // resources = Directory.EnumerateFiles(location, "*.wad").Select(Path.GetFileName).ToList();
            }
            foreach (var file in Directory.EnumerateFiles(location, "*_dir.vpk"))
            {
                var vpk = new VpkDirectory(new FileInfo(file));
                if (!vpk.HasDirectory("materials")) continue;
                var dirs = new Queue<string>();
                dirs.Enqueue("materials");
                while (dirs.Count > 0)
                {
                    var dir = dirs.Dequeue();
                    foreach (var subDir in vpk.GetDirectories(dir))
                    {
                        dirs.Enqueue(subDir);
                    }
                    var files = vpk.GetFiles(dir).Where(x => x.EndsWith(".vmt")).ToList();
                    if (files.Any()) yield return new TexturePackage(file, dir, files);
                }
            }
        }
    }
}
