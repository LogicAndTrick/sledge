using System;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Viewports;
using Veldrid;

namespace Sledge.Rendering.Renderables
{
    public interface IRenderable : IDisposable
    {
        bool ShouldRender(IPipeline pipeline, IViewport viewport);
        void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl);
        void RenderTransparent(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl);
    }
}
