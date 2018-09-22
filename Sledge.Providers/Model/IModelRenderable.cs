using Sledge.Rendering.Engine;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Renderables;

namespace Sledge.Providers.Model
{
    public interface IModelRenderable : IRenderable, IUpdateable
    {
        /// <summary>
        /// Create resources needed by the model renderable, such as transformation matrices.
        /// </summary>
        /// <param name="engine">The engine interface</param>
        /// <param name="context">The render context</param>
        void CreateResources(EngineInterface engine, RenderContext context);
    }
}