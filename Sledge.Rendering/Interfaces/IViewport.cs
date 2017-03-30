using System;
using System.Windows.Forms;
using Sledge.Rendering.Cameras;

namespace Sledge.Rendering.Interfaces
{
    public interface IViewport : IDisposable
    {
        // IGraphicsContext Context { get; }
        //event EventHandler Initialised;
        Control Control { get; }
        Camera Camera { get; set; }
        bool IsFocused { get; }
        string ViewportHandle { get; }

        event FrameEventHandler Update;
        event FrameEventHandler Render;
        event RenderExceptionEventHandler RenderException;
        void MakeCurrent();
        void Run();
        void UpdateNextFrame();
    }
}