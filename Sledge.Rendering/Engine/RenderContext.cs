using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Sledge.Rendering.Engine
{
    public class RenderContext
    {
        public ResourceLoader ResourceLoader { get; }
        public GraphicsDevice Device { get; }

        public RenderContext(GraphicsDevice device)
        {
            Device = device;
            ResourceLoader = new ResourceLoader(this);
        }
    }
}
