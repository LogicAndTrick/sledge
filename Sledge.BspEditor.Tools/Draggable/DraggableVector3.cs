using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Tools.Draggable
{
    public class DraggableVector3 : BaseDraggable
    {
        public bool Highlighted { get; protected set; }
        public Vector3 Position { get; set; }
        public int Width { get; protected set; }

        public override Vector3 Origin => Position;

        public DraggableVector3()
        {
            Width = 5;
            Position = Vector3.Zero;
        }

        public override void Click(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
        {
            
        }

        public override bool CanDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
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

        public override void Highlight(MapDocument document, MapViewport viewport)
        {
            Highlighted = true;
            SetMoveCursor(viewport);
        }

        public override void Unhighlight(MapDocument document, MapViewport viewport)
        {
            Highlighted = false;
            viewport.Control.Cursor = Cursors.Default;
        }

        public override void Drag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 lastPosition, Vector3 position)
        {
            Position = camera.Expand(position) + camera.GetUnusedCoordinate(Position);
            base.Drag(document, viewport, camera, e, lastPosition, position);
        }

        public override void Render(MapDocument document, BufferBuilder builder)
        {
            //
        }

        public override void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im)
        {
            var spos = camera.WorldToScreen(Position);

            const int size = 4;

            var col = Highlighted ? Color.Red : Color.Green;
            im.AddRectOutlineOpaque(spos.ToVector2() - new Vector2(size, size), spos.ToVector2() + new Vector2(size, size), Color.Black, col);
        }

        public override void Render(IViewport viewport, PerspectiveCamera camera, I2DRenderer im)
        {
            //
        }
    }
}