using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Sledge.Providers;

namespace Sledge.Editor.Compiling
{
    public class CompileSpecification
    {
        public string ID { get; set; }
        public string Name { get; set; }
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

        public static CompileSpecification Parse(GenericStructure gs)
        {
            var spec = new CompileSpecification { ID = gs["ID"] ?? "", Name = gs["Name"] ?? ""};
            var tools = gs.GetChildren("Tool");
            spec.Tools.AddRange(tools.Select(CompileTool.Parse));
            var presets = gs.GetChildren("Preset");
            spec.Presets.AddRange(presets.Select(CompilePreset.Parse));
            return spec;
        }

        static CompileSpecification()
        {
            Specifications = new List<CompileSpecification>();
            var specFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Specifications");
            if (Directory.Exists(specFolder))
            {
                foreach (var file in Directory.GetFiles(specFolder, "*.vdf"))
                {
                    try
                    {
                        var gs = GenericStructure.Parse(file);
                        foreach (var spec in gs.Where(x => x.Name == "Specification"))
                        {
                            Specifications.Add(Parse(spec));
                        }
                    }
                    catch
                    {
                        // Not a valid GS
                    }
                }
            }
            if (!Specifications.Any())
            {
                Specifications.Add(new CompileSpecification {ID = "None", Name = "No Specifications Found"});
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public static List<CompileSpecification> Specifications { get; private set; }

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
