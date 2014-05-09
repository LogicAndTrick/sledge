using Sledge.Providers;

namespace Sledge.Editor.Compiling
{
    public class CompilePreset
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Csg { get; set; }
        public string Bsp { get; set; }
        public string Vis { get; set; }
        public string Rad { get; set; }
        public bool RunCsg { get; set; }
        public bool RunBsp { get; set; }
        public bool RunVis { get; set; }
        public bool RunRad { get; set; }

        public static CompilePreset Parse(GenericStructure gs)
        {
            return new CompilePreset
            {
                Name = gs["Name"] ?? "",
                Description = gs["Description"] ?? "",
                Csg = gs["Csg"] ?? "",
                Bsp = gs["Bsp"] ?? "",
                Vis = gs["Vis"] ?? "",
                Rad = gs["Rad"] ?? "",
                RunCsg = gs.PropertyBoolean("RunCsg", true),
                RunBsp = gs.PropertyBoolean("RunBsp", true),
                RunVis = gs.PropertyBoolean("RunVis", true),
                RunRad = gs.PropertyBoolean("RunRad", true)
            };
        }
    }
}