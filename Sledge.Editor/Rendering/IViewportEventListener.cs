using Sledge.Rendering;

namespace Sledge.Editor.Rendering
{
    public interface IViewportEventListener
    {
        MapViewport Viewport { get; set; }

        bool IsActive();

        void KeyUp(ViewportEvent e);
        void KeyDown(ViewportEvent e);

        void MouseMove(ViewportEvent e);
        void MouseWheel(ViewportEvent e);
        void MouseUp(ViewportEvent e);
        void MouseDown(ViewportEvent e);
        void MouseClick(ViewportEvent e);
        void MouseDoubleClick(ViewportEvent e);

        void DragStart(ViewportEvent e);
        void DragMove(ViewportEvent e);
        void DragEnd(ViewportEvent e);

        void MouseEnter(ViewportEvent e);
        void MouseLeave(ViewportEvent e);

        void ZoomChanged(ViewportEvent e);
        void PositionChanged(ViewportEvent e);

        void UpdateFrame(Frame frame);
        // todo: needed? void PreRender();
        // void Render();
        // void PostRender();
    }
}