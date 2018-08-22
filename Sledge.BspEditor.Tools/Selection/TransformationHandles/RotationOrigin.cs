using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.Rendering.Cameras;
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

        public override void Drag(MapViewport viewport, ViewportEvent e, Vector3 lastPosition, Vector3 position)
        {
            Position = _tool.SnapToSelection(viewport.Expand(position) + viewport.GetUnusedCoordinate(Position), viewport);
            OnDragMoved();
        }

        public override void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, Graphics graphics)
        {
            var spos = camera.WorldToScreen(Position);

            const int inner = 8;
            const int outer = 16;

            var innerRect = new Rectangle((int)spos.X - inner / 2, (int)spos.Y - inner / 2, inner, inner);
            var outerRect = new Rectangle((int)spos.X - outer / 2, (int)spos.Y - outer / 2, outer, outer);

            graphics.DrawEllipse(Pens.Cyan, innerRect);
            graphics.DrawEllipse(Highlighted ? Pens.Red : Pens.White, outerRect);
        }
    }
}