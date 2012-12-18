using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Database.Models
{
    public class Wad
    {
        public int ID { get; set; }
        public int GameID { get; set; }
        public string Path { get; set; }

        public Game Game { get; set; }
    }
}
