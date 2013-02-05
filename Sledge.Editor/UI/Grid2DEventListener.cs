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
            //var gr = Viewport.RenderContext.FindRenderable<GridRenderable>();
            //if (gr != null)
            //{
            //    gr.RebuildGrid(newZoom);
            //}
            if (Documents.DocumentManager.CurrentDocument != null
                && Documents.DocumentManager.CurrentDocument.Renderer.GridRenderables.ContainsKey(Viewport))
            {
                Documents.DocumentManager.CurrentDocument.Renderer.GridRenderables[Viewport].RebuildGrid(newZoom);
            }
        }

        public void KeyUp(KeyEventArgs e)
        {
            // Not used
        }

        public void KeyDown(KeyEventArgs e)
        {
            // Not used
        }

        public void KeyPress(KeyPressEventArgs e)
        {
            // Not used
        }

        public void MouseMove(MouseEventArgs e)
        {
            // Not used
        }

        public void MouseWheel(MouseEventArgs e)
        {
            // Not used
        }

        public void MouseUp(MouseEventArgs e)
        {
            // Not used
        }

        public void MouseDown(MouseEventArgs e)
        {
            // Not used
        }

        public void MouseEnter(EventArgs e)
        {
            // Not used
        }

        public void MouseLeave(EventArgs e)
        {
            // Not used
        }

        public void UpdateFrame()
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

        public void PositionChanged(Coordinate oldPosition, Coordinate newPosition)
        {
            // Not used
        }
    }
}
