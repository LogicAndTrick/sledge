using System.Drawing;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.EditorNew.UI.Viewports;
using Sledge.Gui.Components;

namespace Sledge.EditorNew.Tools.DraggableTool
{
    public class DraggableCoordinate : IDraggable
    {
        private bool _highlighted;
        private Coordinate _position;

        public DraggableCoordinate()
        {
            _position = Coordinate.Zero;
        }

        public bool CanDrag(IViewport2D viewport, ViewportEvent e)
        {
            var pos = viewport.Flatten(_position);
            var point = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            var diff = (pos - point).Absolute();
            return diff.X < 5 && diff.Y < 5;
        }

        public void Highlight(IViewport2D viewport)
        {
            _highlighted = true;
            Cursor.SetCursor(viewport, CursorType.SizeAll);
        }

        public void Unhighlight(IViewport2D viewport)
        {
            _highlighted = false;
            Cursor.SetCursor(viewport, CursorType.Default);
        }

        public void StartDrag(IViewport2D viewport, ViewportEvent e)
        {

        }

        public void Drag(IViewport2D viewport, ViewportEvent e)
        {
            _position = viewport.Expand(viewport.ScreenToWorld(e.X, viewport.Height - e.Y)) + viewport.GetUnusedCoordinate(_position);
        }

        public void EndDrag(IViewport2D viewport, ViewportEvent e)
        {

        }

        public void Render(IViewport2D viewport)
        {
            var pos = viewport.Flatten(_position);
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(_highlighted ? Color.Red : Color.Green);
            GL.Vertex2((double)(pos.X - 2), (double)(pos.Y - 2));
            GL.Vertex2((double)(pos.X + 2), (double)(pos.Y - 2));
            GL.Vertex2((double)(pos.X + 2), (double)(pos.Y + 2));
            GL.Vertex2((double)(pos.X - 2), (double)(pos.Y + 2));
            GL.End();
        }
    }
}