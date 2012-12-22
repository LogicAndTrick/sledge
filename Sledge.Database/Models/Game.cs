using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Database.Models
{
    public class Game
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int EngineID { get; set; }
        public int BuildID { get; set; }
        public bool SteamInstall { get; set; }
        public string WonGameDir { get; set; }
        public string SteamGameDir { get; set; }
        public string ModDir { get; set; }
        public string MapDir { get; set; }
        public bool Autosave { get; set; }
        public bool UseCustomAutosaveDir { get; set; }
        public string AutosaveDir { get; set; }
        public string DefaultPointEntity { get; set; }
        public string DefaultBrushEntity { get; set; }
        public decimal DefaultTextureScale { get; set; }
        public decimal DefaultLightmapScale { get; set; }

        public List<Fgd> Fgds { get; set; }
        public List<Wad> Wads { get; set; }
    }
}
