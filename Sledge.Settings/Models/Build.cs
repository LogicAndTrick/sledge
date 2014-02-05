using System;
using System.Collections.Generic;
using System.Globalization;
using Sledge.Providers;

namespace Sledge.Settings.Models
{
    public class Build
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Engine Engine { get; set; }
        public string Specification { get; set; }
        public string Path { get; set; }
        public string Bsp { get; set; }
        public string Csg { get; set; }
        public string Vis { get; set; }
        public string Rad { get; set; }
        public List<BuildProfile> Profiles { get; private set; }

        public Build()
        {
            Profiles = new List<BuildProfile>();
        }

        public void Read(GenericStructure gs)
        {
            ID = gs.PropertyInteger("ID");
            Name = gs["Name"];
            Specification = gs["Specification"];
            Engine = (Engine)Enum.Parse(typeof(Engine), gs["EngineID"]);
            Path = gs["Path"];
            Bsp = gs["Bsp"];
            Csg = gs["Csg"];
            Vis = gs["Vis"];
            Rad = gs["Rad"];

            foreach (var prof in gs.GetChildren("Profile"))
            {
                var bp = new BuildProfile();
                bp.Read(prof);
                Profiles.Add(bp);
            }
        }

        public void Write(GenericStructure gs)
        {
            gs["ID"] = ID.ToString(CultureInfo.InvariantCulture);
            gs["Name"] = Name;
            gs["Specification"] = Specification;
            gs["EngineID"] = Engine.ToString();
            gs["Path"] = Path;
            gs["Bsp"] = Bsp;
            gs["Csg"] = Csg;
            gs["Vis"] = Vis;
            gs["Rad"] = Rad;

            foreach (var bp in Profiles)
            {
                var prof = new GenericStructure("Profile");
                bp.Write(prof);
                gs.Children.Add(prof);
            }
        }
    }
}
