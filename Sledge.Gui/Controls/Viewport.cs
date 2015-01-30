using System;
using OpenTK.Graphics;
using Sledge.Gui.Events;
using Sledge.Gui.Interfaces.Controls;

namespace Sledge.Gui.Controls
{
    public class Viewport : ControlBase<IViewport>, IViewport
    {
        public IGraphicsContext Context
        {
            get { return Control.Context; }
        }

        public event EventHandler Initialised
        {
            add { Control.Initialised += value; }
            remove { Control.Initialised -= value; }
        }

        public event FrameEventHandler Update
        {
            add { Control.Update += value; }
            remove { Control.Update -= value; }
        }

        public event FrameEventHandler Render
        {
            add { Control.Render += value; }
            remove { Control.Render -= value; }
        }

        public event RenderExceptionEventHandler RenderException
        {
            add { Control.RenderException += value; }
            remove { Control.RenderException -= value; }
        }

        public void MakeCurrent()
        {
            Control.MakeCurrent();
        }

        public virtual void Run()
        {
            Control.Run();
        }

        public virtual void UpdateNextFrame()
        {
            Control.UpdateNextFrame();
        }
    }
}