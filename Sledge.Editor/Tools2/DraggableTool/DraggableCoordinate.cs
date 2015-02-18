using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Rendering;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;

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

        public virtual void Click(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position)
        {
            
        }

        public virtual bool CanDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position)
        {
            var pos = viewport.Flatten(Position);
            var diff = (pos - position).Absolute();
            return diff.X < 5 && diff.Y < 5;
        }

        protected virtual void SetMoveCursor(MapViewport viewport, OrthographicCamera camera)
        {
            viewport.Control.Cursor = Cursors.SizeAll;
        }

        public virtual void Highlight(MapViewport viewport, OrthographicCamera camera)
        {
            Highlighted = true;
            SetMoveCursor(viewport, camera);
        }

        public virtual void Unhighlight(MapViewport viewport, OrthographicCamera camera)
        {
            Highlighted = false;
            viewport.Control.Cursor = Cursors.Default;
        }

        public virtual void StartDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position)
        {

        }

        public virtual void Drag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate lastPosition, Coordinate position)
        {
            Position = viewport.Expand(position) + viewport.GetUnusedCoordinate(Position);
        }

        public virtual void EndDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position)
        {

        }

        public IEnumerable<SceneObject> GetSceneObjects()
        {
            // todo 
            yield break;
        }

        public virtual void Render(MapViewport viewport, OrthographicCamera camera)
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