using Sledge.DataStructures.Geometric;
using Sledge.Editor.Rendering;
using Sledge.Rendering;

namespace Sledge.Editor.UI
{
    public class Grid2DEventListener : IViewportEventListener
    {
        public MapViewport Viewport
        {
            get { return MapViewport; }
            set { MapViewport = (MapViewport) value; }
        }

        private MapViewport MapViewport { get; set; }

        public Grid2DEventListener(MapViewport viewport)
        {
            Viewport = viewport;
            MapViewport = viewport;
        }

        // todo update grid
        public void ZoomChanged(decimal oldZoom, decimal newZoom)
        {
            var doc = Documents.DocumentManager.CurrentDocument;
            if (doc != null)
            {
                //doc.Renderer.UpdateGrid(doc.Map.GridSpacing, doc.Map.Show2DGrid, doc.Map.Show3DGrid, false);
            }
        }

        public bool IsActive()
        {
            return Viewport != null && Viewport.Is2D;
        }

        public void KeyUp(ViewportEvent e)
        {
            // Not used
        }

        public void KeyDown(ViewportEvent e)
        {
            // Not used
        }

        public void KeyPress(ViewportEvent e)
        {
            // Not used
        }

        public void MouseMove(ViewportEvent e)
        {
            // Not used
        }

        public void MouseWheel(ViewportEvent e)
        {
            // Not used
        }

        public void MouseUp(ViewportEvent e)
        {
            // Not used
        }

        public void MouseDown(ViewportEvent e)
        {
            // Not used
        }

        public void MouseClick(ViewportEvent e)
        {
            // Not used
        }

        public void MouseDoubleClick(ViewportEvent e)
        {
            // Not used
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
            // Not used
        }

        public void MouseLeave(ViewportEvent e)
        {
            // Not used
        }

        public void ZoomChanged(ViewportEvent e)
        {

        }

        public void PositionChanged(ViewportEvent e)
        {

        }

        public void UpdateFrame(Frame frame)
        {
            // Not used
        }

        public void PreRender()
        {
            // Not used
        }

        public void Render3D()
        {
            // Not used
        }

        public void Render2D()
        {
            // Not used
        }

        public void PostRender()
        {
            // Not used
        }

        public void PositionChanged(Coordinate oldPosition, Coordinate newPosition)
        {
            // Not used
        }
    }
}
