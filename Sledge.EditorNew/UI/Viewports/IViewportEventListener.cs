using Sledge.Gui.Interfaces.Controls;
using Sledge.Gui.Structures;

namespace Sledge.EditorNew.UI.Viewports
{
    public interface IViewportEventListener
    {
        IMapViewport Viewport { get; set; }

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
        void PreRender();
        void Render3D();
        void Render2D();
        void PostRender();
    }
}
