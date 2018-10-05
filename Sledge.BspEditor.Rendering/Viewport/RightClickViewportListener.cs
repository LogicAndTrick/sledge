using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;

namespace Sledge.BspEditor.Rendering.Viewport
{
    public class RightClickViewportListener : IViewportEventListener
    {
        public string OrderHint => "U";
        public MapViewport Viewport { get; set; }

        private readonly ContextMenuStrip _contextMenu;

        public RightClickViewportListener(MapViewport viewport)
        {
            Viewport = viewport;
            _contextMenu = new ContextMenuStrip();
        }

        public void MouseClick(ViewportEvent e)
        {
            if (e.Button == MouseButtons.Right)
            {
                OpenMenu(e);
            }
        }

        private async Task OpenMenu(ViewportEvent e)
        {
            var mb = new RightClickMenuBuilder(Viewport, e);
            await Oy.Publish("MapViewport:RightClick", mb);
            if (mb.Intercepted || mb.IsEmpty) return;
            mb.Populate(_contextMenu);
            _contextMenu.Show(Viewport.Control, e.X, e.Y);
        }

        public bool IsActive()
        {
            return true;
        }

        #region Not required
        
        public void KeyUp(ViewportEvent e)
        {

        }

        public void KeyDown(ViewportEvent e)
        {

        }

        public void MouseMove(ViewportEvent e)
        {

        }

        public void MouseWheel(ViewportEvent e)
        {

        }

        public void MouseUp(ViewportEvent e)
        {

        }

        public void MouseDown(ViewportEvent e)
        {

        }

        public void MouseDoubleClick(ViewportEvent e)
        {

        }

        public void DragStart(ViewportEvent e)
        {

        }

        public void DragMove(ViewportEvent e)
        {

        }

        public void DragEnd(ViewportEvent e)
        {

        }

        public void MouseEnter(ViewportEvent e)
        {
        }

        public void MouseLeave(ViewportEvent e)
        {
        }

        public bool Filter(string hotkey, int keys)
        {
            return false;
        }

        public void Dispose()
        {
            // 
        }

        public void UpdateFrame(long frame)
        {

        }

        #endregion
    }
}