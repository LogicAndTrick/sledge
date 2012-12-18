using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using Sledge.Editor.Properties;
using Sledge.Extensions;
using Sledge.UI;

namespace Sledge.Editor.Tools
{
    /// <summary>
    /// Allows the selected objects to be rotated
    /// </summary>
    class RotateTool : TransformationTool
    {
        public override Image GetIcon()
        {
            return Resources.Tool_Rotate;
        }

        public override string GetName()
        {
            return "Rotate Tool";
        }

        protected override bool RenderCircleHandles
        {
            get { return true; }
        }

        protected override bool AllowCenterHandle
        {
            get { return false; }
        }

        protected override bool FilterHandle(ResizeHandle handle)
        {
            return handle == ResizeHandle.BottomLeft
                   || handle == ResizeHandle.BottomRight
                   || handle == ResizeHandle.TopLeft
                   || handle == ResizeHandle.TopRight;
        }

        protected override Cursor CursorForHandle(ResizeHandle handle)
        {
            return SledgeCursors.RotateCursor;
        }

        protected override Matrix4d? GetTransformationMatrix(Viewport2D viewport, MouseEventArgs e)
        {
            var origin = viewport.ZeroUnusedCoordinate((State.PreTransformBoxStart + State.PreTransformBoxEnd) / 2);
            var forigin = viewport.Flatten(origin);

            var origv = (State.MoveStart - forigin).Normalise();
            var newv = (viewport.ScreenToWorld(e.X, viewport.Height - e.Y) - forigin).Normalise();

            var angle = DMath.Acos(Math.Max(-1, Math.Min(1, origv.Dot(newv))));
            if ((origv.Cross(newv).Z < 0)) angle = 2 * DMath.PI - angle;

            var shf = KeyboardState.Shift;
            var def = Settings.Select.SnapRotationTo15DegreesByDefault;
            if (shf ^ def)
            {
                var deg = angle * (180 / DMath.PI);
                var rnd = Math.Round(deg / 15) * 15;
                angle = rnd * (DMath.PI / 180);
            }

            Matrix4d rotm;
            if (viewport.Direction == Viewport2D.ViewDirection.Top) rotm = Matrix4d.CreateRotationZ((float)angle);
            else if (viewport.Direction == Viewport2D.ViewDirection.Front) rotm = Matrix4d.CreateRotationX((float)angle);
            else rotm = Matrix4d.CreateRotationY((float)-angle); // The Y axis rotation goes in the reverse direction for whatever reason

            var mov = Matrix4d.CreateTranslation((float)-origin.X, (float)-origin.Y, (float)-origin.Z);
            var rot = Matrix4d.Mult(mov, rotm);
            return Matrix4d.Mult(rot, Matrix4d.Invert(mov));
        }
    }
}
