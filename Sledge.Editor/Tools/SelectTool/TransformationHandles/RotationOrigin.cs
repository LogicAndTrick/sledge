using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Extensions;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools.DraggableTool;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.Editor.Tools.SelectTool.TransformationHandles
{
    public class RotationOrigin : DraggableCoordinate
    {
        protected override void SetMoveCursor(MapViewport viewport)
        {
            viewport.Control.Cursor = Cursors.Cross;
        }

        public override bool CanDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            const int width = 8;
            var screenPosition = viewport.ProperWorldToScreen(Position);
            var diff = (e.Location - screenPosition).Absolute();
            return diff.X < width && diff.Y < width;
        }

        public override IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            yield return new HandleElement(PositionType.World, HandleElement.HandleType.Circle, new Position(Position.ToVector3()), 4)
            {
                Color = Color.Transparent,
                LineColor = Color.Cyan
            };
            yield return new HandleElement(PositionType.World, HandleElement.HandleType.Circle, new Position(Position.ToVector3()), 8)
            {
                Color = Color.Transparent,
                LineColor = Highlighted ? Color.Red : Color.White
            };
        }
    }
}