using System;
using System.Collections.Generic;
using Veldrid;

namespace Sledge.Rendering.Viewports
{
    public interface IRenderTarget : IDisposable
    {
        Swapchain Swapchain { get; }
        bool ShouldRender(long frame);
    }
}