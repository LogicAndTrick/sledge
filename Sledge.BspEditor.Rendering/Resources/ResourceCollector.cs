using System.Collections.Generic;
using System.Linq;
using Sledge.Rendering.Renderables;

namespace Sledge.BspEditor.Rendering.Resources
{
    public class ResourceCollector
    {
        public HashSet<string> Textures { get; }
        private HashSet<IRenderable> AddedRenderables { get; }
        private HashSet<IRenderable> RemovedRenderables { get; }

        public ResourceCollector()
        {
            Textures = new HashSet<string>();
            AddedRenderables = new HashSet<IRenderable>();
            RemovedRenderables = new HashSet<IRenderable>();
        }

        public void RequireTexture(string name) => Textures.Add(name);
        public void AddRenderables(IEnumerable<IRenderable> renderables) => AddedRenderables.UnionWith(renderables);
        public void RemoveRenderables(IEnumerable<IRenderable> renderables) => RemovedRenderables.UnionWith(renderables);

        public IEnumerable<IRenderable> GetRenderablesToAdd() => AddedRenderables.Except(RemovedRenderables);
        public IEnumerable<IRenderable> GetRenderablesToRemove() => RemovedRenderables.Except(AddedRenderables);

        public void Merge(ResourceCollector collector)
        {
            Textures.UnionWith(collector.Textures);
            AddedRenderables.UnionWith(collector.AddedRenderables);
            RemovedRenderables.UnionWith(collector.RemovedRenderables);
        }
    }
}
