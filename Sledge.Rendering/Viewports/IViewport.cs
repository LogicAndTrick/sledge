using System;
using System.Windows.Forms;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;

namespace Sledge.Rendering.Viewports
{
    public interface IViewport : IRenderTarget
    {
        int ID { get; }

        int Width { get; }
        int Height { get; }
        Control Control { get; }
        bool IsFocused { get; }

        ICamera Camera { get; set; }
        ViewportOverlay Overlay { get; }

        void Update(long frame);
        event EventHandler<long> OnUpdate;
    }
}