using System.Collections.Generic;
using System.IO;
using System.Linq;
using LogicAndTrick.Gimme.Providers;
using Sledge.Packages.Wad;

namespace Sledge.Providers.Texture.Wad
{
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
                yield return new TexturePackage(path, WadPackage.GetEntryNames(new FileInfo(path)));
            }
        }
    }
}