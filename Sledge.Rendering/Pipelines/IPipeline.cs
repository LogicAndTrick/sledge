using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;
using Veldrid;

namespace Sledge.Rendering.Pipelines
{
    public interface IPipeline : IDisposable
    {
        PipelineType Type { get; }
        int Group { get; }
        float Order { get; }

        void Create(RenderContext context);
        void SetupFrame(RenderContext context, IViewport target);
        void Render(RenderContext context, IViewport target, CommandList cl, IEnumerable<IRenderable> renderables);
        void RenderTransparent(RenderContext context, IViewport target, CommandList cl, IEnumerable<IRenderable> renderables);
        void Bind(RenderContext context, CommandList cl, string binding);
    }
}
