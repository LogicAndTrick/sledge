using System;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Tools.Draggable
{
    public class DraggableVector3 : BaseDraggable
    {
        public bool Highlighted { get; protected set; }
        public Vector3 Position { get; set; }
        public int Width { get; protected set; }

        public DraggableVector3()
        {
            Width = 5;
            Position = Vector3.Zero;
        }

        public override void Click(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
        {
            
        }

        public override bool CanDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
        {
            var width = Width;
            var screenPosition = camera.WorldToScreen(Position);
            var diff = Vector3.Abs(e.Location - screenPosition);
            return diff.X < width && diff.Y < width;
        }

        protected virtual void SetMoveCursor(MapViewport viewport)
        {
            viewport.Control.Cursor = Cursors.SizeAll;
        }

        public override void Highlight(MapViewport viewport)
        {
            Highlighted = true;
            SetMoveCursor(viewport);
        }

        public override void Unhighlight(MapViewport viewport)
        {
            Highlighted = false;
            viewport.Control.Cursor = Cursors.Default;
        }

        public override void Drag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 lastPosition, Vector3 position)
        {
            Position = camera.Expand(position) + camera.GetUnusedCoordinate(Position);
            base.Drag(viewport, camera, e, lastPosition, position);
        }

        public override void Render(BufferBuilder builder)
        {
            //
        }

        public override void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, Graphics graphics)
        {
            var spos = camera.WorldToScreen(Position);

            const int size = 8;
            var rect = new Rectangle((int)spos.X - size / 2, (int)spos.Y - size / 2, size, size);

            graphics.FillRectangle(Highlighted ? Brushes.Red : Brushes.Green, rect);
            graphics.DrawRectangle(Pens.Black, rect);
        }

        public override void Render(IViewport viewport, PerspectiveCamera camera, Graphics graphics)
        {
            //
        }
    }
}