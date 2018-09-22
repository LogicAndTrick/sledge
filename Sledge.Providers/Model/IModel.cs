using System;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Renderables;

namespace Sledge.Providers.Model
{
    /// <summary>
    /// A model must render itself given the standard properties of the model pipeline.
    /// The transforms will be set before the model is rendered.
    /// </summary>
    public interface IModel : IDisposable
    {
        /// <summary>
        /// Create resources needed by the model to render, such as textures and buffers.
        /// </summary>
        /// <param name="engine">The engine interface</param>
        /// <param name="context">The render context</param>
        void CreateResources(EngineInterface engine, RenderContext context);
    }
}