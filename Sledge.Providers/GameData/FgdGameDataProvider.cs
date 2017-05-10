using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicAndTrick.Gimme.Providers;

namespace Sledge.Providers.GameData
{
    public class FgdGameDataProvider : SyncResourceProvider<DataStructures.GameData.GameData>
    {
        public override bool CanProvide(string location)
        {
            return File.Exists(location) && location.EndsWith(".fgd");
        }

        public override IEnumerable<DataStructures.GameData.GameData> Fetch(string location, List<string> resources)
        {
            var provider = new FgdProvider();
            yield return provider.OpenFile(location);
        }
    }
}
