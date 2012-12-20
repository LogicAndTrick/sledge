using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Database.Models
{
    public class Build
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int EngineID { get; set; }
        public string Path { get; set; }
        public string Bsp { get; set; }
        public string Csg { get; set; }
        public string Vis { get; set; }
        public string Rad { get; set; }
    }
}
