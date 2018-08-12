using System;
using System.Windows.Forms;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;

namespace Sledge.Rendering.Viewports
{
    public interface IViewport : IRenderTarget
    {
        int ID { get; }

        int Width { get; }
        int Height { get; }
        Control Control { get; }

        ICamera Camera { get; set; }

        void Update(long frame);
        event EventHandler<long> OnUpdate;
    }
}