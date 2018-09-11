using System.Collections.Generic;
using System.Linq;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;

namespace Sledge.Rendering.Engine
{
    public class Scene : IUpdateable
    {
        private readonly LinkedList<IRenderable> _renderables;
        private readonly LinkedList<IUpdateable> _updateables;
        private readonly LinkedList<IOverlayRenderable> _overlayRenderables;

        public Scene()
        {
            _renderables = new LinkedList<IRenderable>();
            _updateables = new LinkedList<IUpdateable>();
            _overlayRenderables = new LinkedList<IOverlayRenderable>();
        }

        public void Add(IRenderable renderable) => _renderables.AddLast(renderable);
        public void Add(IUpdateable updateable) => _updateables.AddLast(updateable);
        public void Add(IOverlayRenderable overlayRenderable) => _overlayRenderables.AddLast(overlayRenderable);

        public void Remove(IRenderable renderable) => _renderables.Remove(renderable);
        public void Remove(IUpdateable updateable) => _updateables.Remove(updateable);
        public void Remove(IOverlayRenderable overlayRenderable) => _overlayRenderables.Remove(overlayRenderable);

        public void Update(long frame)
        {
            // We want to be able to update while modifying, if new items don't get the update that's ok
            for (var u = _updateables.First; u != null; u = u.Next)
            {
                u.Value.Update(frame);
            }
        }

        public IEnumerable<IRenderable> GetRenderables(IPipeline pipeline, IViewport target)
        {
            // Since we only addlast/remove from the linked list, we should be able to iterate over it even though it might change in a different thread
            for (var r = _renderables.First; r != null; r = r?.Next)
            {
                if (r.Value.ShouldRender(pipeline, target)) yield return r.Value;
            }
        }

        public IEnumerable<IOverlayRenderable> GetOverlayRenderables()
        {
            for (var r = _overlayRenderables.First; r != null; r = r?.Next)
            {
                yield return r.Value;
            }
        }
    }
}