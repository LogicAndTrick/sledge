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

        public static CompilePreset Parse(GenericStructure gs)
        {
            return new CompilePreset
            {
                Name = gs["Name"] ?? "",
                Description = gs["Description"] ?? "",
                Csg = gs["Csg"] ?? "",
                Bsp = gs["Bsp"] ?? "",
                Vis = gs["Vis"] ?? "",
                Rad = gs["Rad"] ?? ""
            };
        }
    }
}