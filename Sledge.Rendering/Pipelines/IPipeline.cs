using System;
using System.Collections.Generic;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;
using Veldrid;

namespace Sledge.Rendering.Pipelines
{
    public interface IPipeline : IDisposable
    {
        PipelineType Type { get; }
        PipelineGroup Group { get; }
        float Order { get; }

        void Create(RenderContext context);
        void SetupFrame(RenderContext context, IViewport target);
        void Render(RenderContext context, IViewport target, CommandList cl, IEnumerable<IRenderable> renderables);
        void Render(RenderContext context, IViewport target, CommandList cl, IRenderable renderable, ILocation locationObject);
        void Bind(RenderContext context, CommandList cl, string binding);
    }
}
