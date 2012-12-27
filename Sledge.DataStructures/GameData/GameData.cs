using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.DataStructures.GameData
{
    public class GameData
    {
        public int MapSizeLow { get; set; }
        public int MapSizeHigh { get; set; }
        public List<GameDataObject> Classes { get; private set; }
        public List<string> Includes { get; private set; }

        public GameData()
        {
            MapSizeHigh = 4096;
            MapSizeLow = -4096;
            Classes = new List<GameDataObject>();
            Includes = new List<string>();
        }

        public void CreateDependencies()
        {
            var resolved = new List<string>();
            var unresolved = new List<GameDataObject>(Classes);
            while (unresolved.Any())
            {
                var resolve = unresolved.Where(x => x.BaseClasses.All(resolved.Contains)).ToList();
                if (!resolve.Any()) throw new Exception("Circular dependencies: " + String.Join(", ", unresolved.Select(x => x.Name)));
                resolve.ForEach(x => x.Inherit(Classes.Where(y => x.BaseClasses.Contains(y.Name))));
                unresolved.RemoveAll(resolve.Contains);
                resolved.AddRange(resolve.Select(x => x.Name));
            }
        }
    }
}
