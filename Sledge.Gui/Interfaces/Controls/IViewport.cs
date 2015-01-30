using System;
using OpenTK.Graphics;
using Sledge.Gui.Attributes;
using Sledge.Gui.Events;

namespace Sledge.Gui.Interfaces.Controls
{
    [ControlInterface]
    public interface IViewport : IControl
    {
        IGraphicsContext Context { get; }
        event EventHandler Initialised;
        event FrameEventHandler Update;
        event FrameEventHandler Render;
        event RenderExceptionEventHandler RenderException;
        void MakeCurrent();
        void Run();
        void UpdateNextFrame();
    }
}