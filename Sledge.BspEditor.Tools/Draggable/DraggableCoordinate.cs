using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.Rendering.Cameras;
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

        public override void Click(MapViewport viewport, ViewportEvent e, Vector3 position)
        {
            
        }

        public override bool CanDrag(MapViewport viewport, ViewportEvent e, Vector3 position)
        {
            var width = Width;
            var screenPosition = viewport.WorldToScreen(Position);
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

        public override void Drag(MapViewport viewport, ViewportEvent e, Vector3 lastPosition, Vector3 position)
        {
            Position = viewport.Expand(position) + viewport.GetUnusedCoordinate(Position);
            base.Drag(viewport, e, lastPosition, position);
        }

        public override void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, Graphics graphics)
        {
            // yield return new HandleElement(PositionType.World, HandleElement.HandleType.Square, new Position(Position.ToVector3()), Width)
            // {
            //     Color = GetColor()
            // };
        }

        public override void Render(IViewport viewport, PerspectiveCamera camera, Graphics graphics)
        {
            //
        }
        
        protected virtual Color GetColor()
        {
            return Highlighted ? Color.Red : Color.Green;
        }
    }
}