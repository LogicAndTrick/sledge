using System.Drawing;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Rendering;

namespace Sledge.Editor.Tools2.DraggableTool
{
    public class DraggableCoordinate : IDraggable
    {
        public bool Highlighted { get; protected set; }
        public Coordinate Position { get; set; }

        public DraggableCoordinate()
        {
            Position = Coordinate.Zero;
        }

        public virtual void Click(IViewport2D viewport, ViewportEvent e, Coordinate position)
        {
            
        }

        public virtual bool CanDrag(IViewport2D viewport, ViewportEvent e, Coordinate position)
        {
            var pos = viewport.Flatten(Position);
            var diff = (pos - position).Absolute();
            return diff.X < 5 && diff.Y < 5;
        }

        protected virtual void SetMoveCursor(IViewport2D viewport)
        {
            Cursor.SetCursor(viewport, CursorType.SizeAll);
        }

        public virtual void Highlight(IViewport2D viewport)
        {
            Highlighted = true;
            SetMoveCursor(viewport);
        }

        public virtual void Unhighlight(IViewport2D viewport)
        {
            Highlighted = false;
            Cursor.SetCursor(viewport, CursorType.Default);
        }

        public virtual void StartDrag(IViewport2D viewport, ViewportEvent e, Coordinate position)
        {

        }

        public virtual void Drag(IViewport2D viewport, ViewportEvent e, Coordinate lastPosition, Coordinate position)
        {
            Position = viewport.Expand(position) + viewport.GetUnusedCoordinate(Position);
        }

        public virtual void EndDrag(IViewport2D viewport, ViewportEvent e, Coordinate position)
        {

        }

        public virtual void Render(IViewport2D viewport)
        {
            var pos = viewport.Flatten(Position);
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(Highlighted ? Color.Red : Color.Green);
            GL.Vertex2((double)(pos.X - 2), (double)(pos.Y - 2));
            GL.Vertex2((double)(pos.X + 2), (double)(pos.Y - 2));
            GL.Vertex2((double)(pos.X + 2), (double)(pos.Y + 2));
            GL.Vertex2((double)(pos.X - 2), (double)(pos.Y + 2));
            GL.End();
        }
    }
}