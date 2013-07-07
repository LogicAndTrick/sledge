using System.Globalization;
using Sledge.Providers;

namespace Sledge.Settings.Models
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

        public void Read(GenericStructure gs)
        {
            ID = gs.PropertyInteger("ID");
            Name = gs["Name"];
            EngineID = gs.PropertyInteger("EngineID");
            Path = gs["Path"];
            Bsp = gs["Bsp"];
            Csg = gs["Csg"];
            Vis = gs["Vis"];
            Rad = gs["Rad"];
        }

        public void Write(GenericStructure gs)
        {
            gs["ID"] = ID.ToString(CultureInfo.InvariantCulture);
            gs["Name"] = Name;
            gs["EngineID"] = EngineID.ToString(CultureInfo.InvariantCulture);
            gs["Path"] = Path;
            gs["Bsp"] = Bsp;
            gs["Csg"] = Csg;
            gs["Vis"] = Vis;
            gs["Rad"] = Rad;
        }
    }
}
