using System.Collections.Generic;
using System.Linq;
using Sledge.Rendering.Renderables;

namespace Sledge.BspEditor.Rendering.Resources
{
    public class ResourceCollector
    {
        public HashSet<string> Textures { get; }
        public HashSet<IRenderable> AddedRenderables { get; set; }
        public HashSet<IRenderable> RemovedRenderables { get; set; }

        public ResourceCollector()
        {
            Textures = new HashSet<string>();
            AddedRenderables = new HashSet<IRenderable>();
            RemovedRenderables = new HashSet<IRenderable>();
        }

        public void RequireTexture(string name) => Textures.Add(name);
        public void AddRenderables(IEnumerable<IRenderable> renderables)
        {
            var arr = renderables.ToArray();
            AddedRenderables.UnionWith(arr);
            RemovedRenderables.ExceptWith(arr);
        }

        public void RemoveRenderables(IEnumerable<IRenderable> renderables)
        {
            var arr = renderables.ToArray();
            RemovedRenderables.UnionWith(arr);
            AddedRenderables.ExceptWith(arr);
        }
    }
}
