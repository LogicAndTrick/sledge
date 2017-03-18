using System.Collections.Generic;
using System.IO;
using System.Linq;
using LogicAndTrick.Gimme.Providers;
using Sledge.Packages.Wad;
using Sledge.Rendering.Materials;

namespace Sledge.Providers.Texture.Wad
{
    public class WadTextureItemProvider : SyncResourceProvider<TextureItem>
    {
        public override bool CanProvide(string location)
        {
            return File.Exists(location) && location.EndsWith(".wad");
        }

        public override IEnumerable<TextureItem> Fetch(string location, List<string> resources)
        {
            using (var wad = new WadPackage(new FileInfo(location)))
            {
                if (resources == null || !resources.Any())
                {
                    resources = wad.GetEntries().Select(x => x.Name).ToList();
                }
                foreach (var res in resources)
                {
                    var entry = (WadEntry) wad.GetEntry(res);
                    if (entry == null) continue;
                    yield return new TextureItem(entry.Name, TextureFlags.None, (int) entry.Width, (int) entry.Height);
                }
            }
        }
    }
}
