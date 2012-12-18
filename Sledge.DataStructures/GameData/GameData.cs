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
    }
}
