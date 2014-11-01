using Sledge.Gui.Events;
using Sledge.Gui.Interfaces.Controls;

namespace Sledge.Gui.Controls
{
    public class Viewport : ControlBase<IViewport>, IViewport
    {
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

        public void Run()
        {
            Control.Run();
        }

        public void UpdateNextFrame()
        {
            Control.UpdateNextFrame();
        }
    }
}