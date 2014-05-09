using System.Collections.Generic;
using System.Linq;
using Sledge.Providers;

namespace Sledge.Editor.Compiling
{
    public class CompileTool
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public bool Enabled { get; set; }
        public List<CompileParameter> Parameters { get; private set; }

        public CompileTool()
        {
            Parameters = new List<CompileParameter>();
        }

        public static CompileTool Parse(GenericStructure gs)
        {
            var tool = new CompileTool
            {
                Name = gs["Name"] ?? "",
                Description = gs["Description"] ?? "",
                Order = gs.PropertyInteger("Order"),
                Enabled = gs.PropertyBoolean("Enabled", true)
            };
            var parameters = gs.GetChildren("Parameter");
            tool.Parameters.AddRange(parameters.Select(CompileParameter.Parse));
            return tool;
        }
    }
}