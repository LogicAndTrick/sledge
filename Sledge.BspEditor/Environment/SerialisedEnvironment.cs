using System.Collections.Generic;

namespace Sledge.BspEditor.Environment
{
    public class SerialisedEnvironment
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}