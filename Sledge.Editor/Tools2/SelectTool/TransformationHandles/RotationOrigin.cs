using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools2.DraggableTool;
using Sledge.Graphics;
using Sledge.Rendering.Cameras;

namespace Sledge.Editor.Tools2.SelectTool.TransformationHandles
{
    public class RotationOrigin : DraggableCoordinate
    {
        protected override void SetMoveCursor(MapViewport viewport, OrthographicCamera camera)
        {
            viewport.Control.Cursor = Cursors.Cross;
        }

        public override bool CanDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position)
        {
            var pos = viewport.Flatten(Position);
            var diff = (pos - position).Absolute();
            return diff.X < 8 && diff.Y < 8;
        }

        public override void Render(MapViewport viewport, OrthographicCamera camera)
        {
            var pp = viewport.Flatten(Position);
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Cyan);
            GLX.Circle(new Vector2d(pp.DX, pp.DY), 4, (double)viewport.Zoom);
            GL.Color3(Highlighted ? Color.Red : Color.White);
            GLX.Circle(new Vector2d(pp.DX, pp.DY), 8, (double)viewport.Zoom);
            GL.End();
        }
    }
}