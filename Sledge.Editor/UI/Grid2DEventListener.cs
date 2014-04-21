using System;
using System.Windows.Forms;
using Sledge.DataStructures.Geometric;
using Sledge.UI;
using Sledge.Editor.Rendering;

namespace Sledge.Editor.UI
{
    public class Grid2DEventListener : IViewport2DEventListener
    {
        public ViewportBase Viewport
        {
            get { return Viewport2D; }
            set { Viewport2D = (Viewport2D) value; }
        }

        private Viewport2D Viewport2D { get; set; }

        public Grid2DEventListener(Viewport2D viewport)
        {
            Viewport = viewport;
            Viewport2D = viewport;
        }

        public void ZoomChanged(decimal oldZoom, decimal newZoom)
        {
            var doc = Documents.DocumentManager.CurrentDocument;
            if (doc != null)
            {
                doc.Renderer.UpdateGrid(doc.Map.GridSpacing, doc.Map.Show2DGrid, doc.Map.Show3DGrid, false);
            }
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

        public void MouseEnter(ViewportEvent e)
        {
            // Not used
        }

        public void MouseLeave(ViewportEvent e)
        {
            // Not used
        }

        public void UpdateFrame(FrameInfo frame)
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
