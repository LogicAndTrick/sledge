using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Editing.Components.Compile.Specification
{
    public class CompileSpecification
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Engine { get; set; }
        public List<CompileTool> Tools { get; private set; }
        public List<CompilePreset> Presets { get; private set; }

        public CompileSpecification()
        {
            Tools = new List<CompileTool>();
            Presets = new List<CompilePreset>();
        }

        public CompileTool GetTool(string name)
        {
            return Tools.FirstOrDefault(x => String.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
        }

        public static CompileSpecification Parse(SerialisedObject gs)
        {
            var spec = new CompileSpecification
            {
                ID = gs.Get("ID", ""),
                Name = gs.Get("Name", ""),
                Engine = gs.Get("Engine", "")
            };
            var tools = gs.Children.Where(x => x.Name == "Tool");
            spec.Tools.AddRange(tools.Select(CompileTool.Parse));
            var presets = gs.Children.Where(x => x.Name == "Preset");
            spec.Presets.AddRange(presets.Select(CompilePreset.Parse));
            return spec;
        }

        public override string ToString()
        {
            return Name;
        }

        public string GetDefaultParameters(string name)
        {
            var tool = GetTool(name);
            return tool == null
                ? ""
                : String.Join(" ", tool.Parameters.Select(x => x.GetDefaultArgumentString()).Where(x => !String.IsNullOrWhiteSpace(x)));
        }

        public bool GetDefaultRun(string name)
        {
            var tool = GetTool(name);
            return tool != null && tool.Enabled;
        }
    }
}
