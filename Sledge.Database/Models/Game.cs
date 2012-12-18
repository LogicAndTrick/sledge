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
        public string GameDir { get; set; }
        public string ModDir { get; set; }
        public int BuildID { get; set; }

        public Engine Engine { get; set; }
        public Build Build { get; set; }
        public IList<Fgd> Fgds { get; set; }
        public IList<Wad> Wads { get; set; }
    }
}
