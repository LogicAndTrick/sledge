using System.Collections.Generic;

namespace Sledge.BspEditor.Rendering.Resources
{
    public class ResourceCollector
    {
        public HashSet<string> Textures { get; }

        public ResourceCollector()
        {
            Textures = new HashSet<string>();
        }

        public void RequireTexture(string name) => Textures.Add(name);
    }
}
