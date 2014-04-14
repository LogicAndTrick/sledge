namespace Sledge.UI
{
    public interface IViewportEventListener
    {
        ViewportBase Viewport { get; set; }

        void KeyUp(ViewportEvent e);
        void KeyDown(ViewportEvent e);
        void KeyPress(ViewportEvent e);

        void MouseMove(ViewportEvent e);
        void MouseWheel(ViewportEvent e);
        void MouseUp(ViewportEvent e);
        void MouseDown(ViewportEvent e);
        void MouseClick(ViewportEvent e);
        void MouseDoubleClick(ViewportEvent e);

        void MouseEnter(ViewportEvent e);
        void MouseLeave(ViewportEvent e);

        void UpdateFrame(FrameInfo frame);
        void PreRender();
        void Render3D();
        void Render2D();
        void PostRender();
    }
}
