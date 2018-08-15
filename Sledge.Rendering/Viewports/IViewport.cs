using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Pipelines;

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