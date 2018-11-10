using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Tools.Selection.TransformationHandles
{
    public class RotationOrigin : DraggableVector3
    {
        private readonly BaseTool _tool;

        public RotationOrigin(BaseTool tool)
        {
            _tool = tool;
            Width = 8;
        }

        protected override void SetMoveCursor(MapViewport viewport)
        {
            viewport.Control.Cursor = Cursors.Cross;
        }

        public override void Drag(MapDocument document, MapViewport viewport, OrthographicCamera camera,
            ViewportEvent e, Vector3 lastPosition, Vector3 position)
        {
            Position = _tool.SnapToSelection(camera.Expand(position) + camera.GetUnusedCoordinate(Position), camera);
            OnDragMoved();
        }

        public override void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im)
        {
            var spos = camera.WorldToScreen(Position);

            const float inner = 4;
            const float outer = 8;

            var col = Highlighted ? Color.Red : Color.White;
            im.AddCircle(spos.ToVector2(), inner, Color.Cyan);
            im.AddCircle(spos.ToVector2(), outer, col);
        }
    }
}