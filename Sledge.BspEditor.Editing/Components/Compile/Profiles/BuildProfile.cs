using System.Collections.Generic;

namespace Sledge.BspEditor.Editing.Components.Compile.Profiles
{
    public class BuildProfile
    {
        public string Name { get; set; }
        public string SpecificationName { get; set; }
        public Dictionary<string, string> Arguments { get; set; }

        public BuildProfile()
        {
            Name = "";
            SpecificationName = "";
            Arguments = new Dictionary<string, string>();
        }
    }
}