using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace Sledge.Providers.GameData
{
    [Export("Fgd", typeof(IGameDataProvider))]
    public class FgdGameDataProvider : IGameDataProvider
    {
        public DataStructures.GameData.GameData GetGameDataFromFiles(IEnumerable<string> files)
        {
            var gd = new DataStructures.GameData.GameData();
            foreach (var f in files.Where(IsValidForFile))
            {
                var provider = new FgdProvider();
                var d = provider.OpenFile(f);

                gd.MapSizeHigh = d.MapSizeHigh;
                gd.MapSizeLow = d.MapSizeLow;
                gd.Classes.AddRange(d.Classes);
                gd.MaterialExclusions.AddRange(d.MaterialExclusions);
            }
            gd.CreateDependencies();
            gd.RemoveDuplicates();
            return gd;
        }
        
        public bool IsValidForFile(string filename)
        {
            return File.Exists(filename) && filename.EndsWith(".fgd");
        }
    }
}
