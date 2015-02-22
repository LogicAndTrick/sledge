using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Extensions;
using Sledge.Editor.Rendering;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;

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

        public virtual void Click(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            
        }

        public virtual bool CanDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            const int width = 5;
            var screenPosition = viewport.ProperWorldToScreen(Position);
            var diff = (e.Location - screenPosition).Absolute();
            return diff.X < width && diff.Y < width;
        }

        protected virtual void SetMoveCursor(MapViewport viewport)
        {
            viewport.Control.Cursor = Cursors.SizeAll;
        }

        public virtual void Highlight(MapViewport viewport)
        {
            Highlighted = true;
            SetMoveCursor(viewport);
        }

        public virtual void Unhighlight(MapViewport viewport)
        {
            Highlighted = false;
            viewport.Control.Cursor = Cursors.Default;
        }

        public virtual void StartDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {

        }

        public virtual void Drag(MapViewport viewport, ViewportEvent e, Coordinate lastPosition, Coordinate position)
        {
            Position = viewport.Expand(position) + viewport.GetUnusedCoordinate(Position);
        }

        public virtual void EndDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {

        }

        public virtual IEnumerable<SceneObject> GetSceneObjects()
        {
            yield break;
        }

        public virtual IEnumerable<Element> GetViewportElements(MapViewport viewport, PerspectiveCamera camera)
        {
            yield break;
        }

        public virtual IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            yield return new HandleElement(PositionType.World, HandleElement.HandleType.Square, new Position(Position.ToVector3()), 2)
            {
                Color = Highlighted ? Color.Red : Color.Green
            };
        }
    }
}