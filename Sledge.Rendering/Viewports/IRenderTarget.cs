using System;
using Veldrid;

namespace Sledge.Rendering.Viewports
{
    public interface IRenderTarget : IDisposable
    {
        Swapchain Swapchain { get; }
        bool ShouldRender(long frame);
    }
}