using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.EditorNew.Tools.DraggableTool;
using Sledge.EditorNew.UI.Viewports;
using Sledge.Graphics;
using Sledge.Gui.Components;

namespace Sledge.EditorNew.Tools.SelectTool.TransformationStates
{
    public class RotationOrigin : DraggableCoordinate
    {
        protected override void SetMoveCursor(IViewport2D viewport)
        {
            Cursor.SetCursor(viewport, CursorType.Crosshair);
        }

        public override bool CanDrag(IViewport2D viewport, ViewportEvent e, Coordinate position)
        {
            var pos = viewport.Flatten(Position);
            var diff = (pos - position).Absolute();
            return diff.X < 8 && diff.Y < 8;
        }

        public override void Render(IViewport2D viewport)
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