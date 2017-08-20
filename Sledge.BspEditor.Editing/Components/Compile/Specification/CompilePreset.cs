using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Editing.Components.Compile.Specification
{
    public class CompilePreset
    {
        public string Name { get; set; }
        public string Description { get; set; }
        private Dictionary<string, string> Arguments { get; set; }

        public bool ShouldRunTool(string name)
        {
            return Arguments.ContainsKey(name);
        }

        public string GetArguments(string name)
        {
            return Arguments.ContainsKey(name) ? Arguments[name] : "";
        }

        public static CompilePreset Parse(SerialisedObject gs)
        {
            var args = gs.Children.FirstOrDefault(x => x.Name == "Arguments") ?? new SerialisedObject("Arguments");
            return new CompilePreset
            {
                Name = gs.Get("Name", ""),
                Description = gs.Get("Description", ""),
                Arguments = args.Properties.ToDictionary(x => x.Key, x => x.Value)
            };
        }
    }
}