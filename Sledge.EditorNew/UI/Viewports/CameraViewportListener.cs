using System;
using Sledge.Gui.Structures;

namespace Sledge.EditorNew.UI.Viewports
{
    public class CameraViewportListener : IViewportEventListener
    {
        public IMapViewport Viewport { get; set; }
        private IViewportEventListener _2dListener;
        private IViewportEventListener _3dListener;

        public CameraViewportListener(IMapViewport viewport)
        {
            Viewport = viewport;
            if (viewport is IViewport2D) _2dListener = new Camera2DViewportEventListener(viewport as IViewport2D);
            if (viewport is IViewport3D) _3dListener = new Camera3DViewportListener(viewport as IViewport3D);
        }

        private void Relay(Action<IViewportEventListener> action)
        {
            if (Viewport.Is2D && _2dListener != null) action(_2dListener);
            else if (Viewport.Is3D && _3dListener != null) action(_3dListener);
        }

        public void KeyUp(ViewportEvent e)
        {
            Relay(x => x.KeyUp(e));
        }

        public void KeyDown(ViewportEvent e)
        {
            Relay(x => x.KeyDown(e));
        }

        public void MouseMove(ViewportEvent e)
        {
            Relay(x => x.MouseMove(e));
        }

        public void MouseWheel(ViewportEvent e)
        {
            Relay(x => x.MouseWheel(e));
        }

        public void MouseUp(ViewportEvent e)
        {
            Relay(x => x.MouseUp(e));
        }

        public void MouseDown(ViewportEvent e)
        {
            Relay(x => x.MouseDown(e));
        }

        public void MouseClick(ViewportEvent e)
        {
            Relay(x => x.MouseClick(e));
        }

        public void MouseDoubleClick(ViewportEvent e)
        {
            Relay(x => x.MouseDoubleClick(e));
        }

        public void DragStart(ViewportEvent e)
        {
            Relay(x => x.DragStart(e));
        }

        public void DragMove(ViewportEvent e)
        {
            Relay(x => x.DragMove(e));
        }

        public void DragEnd(ViewportEvent e)
        {
            Relay(x => x.DragEnd(e));
        }

        public void MouseEnter(ViewportEvent e)
        {
            Relay(x => x.MouseEnter(e));
        }

        public void MouseLeave(ViewportEvent e)
        {
            Relay(x => x.MouseLeave(e));
        }

        public void ZoomChanged(ViewportEvent e)
        {
            Relay(x => x.ZoomChanged(e));
        }

        public void PositionChanged(ViewportEvent e)
        {
            Relay(x => x.PositionChanged(e));
        }

        public void UpdateFrame(Frame frame)
        {
            Relay(x => x.UpdateFrame(frame));
        }

        public void PreRender()
        {
            Relay(x => x.PreRender());
        }

        public void Render3D()
        {
            Relay(x => x.Render3D());
        }

        public void Render2D()
        {
            Relay(x => x.Render2D());
        }

        public void PostRender()
        {
            Relay(x => x.PostRender());
        }
    }
}