using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using Sledge.DataStructures;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.Transformations;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Extensions;
using Sledge.Graphics;
using Sledge.Settings;
using Sledge.UI;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Editor.Tools.TransformationTools
{
    /// <summary>
    /// Allows the selected objects to be rotated
    /// </summary>
    class RotateTool : TransformationTool
    {
        private enum CircleType
        {
            None,
            Outer,
            X,
            Y,
            Z
        }

        public override bool RenderCircleHandles
        {
            get { return true; }
        }

        public override bool FilterHandle(BaseBoxTool.ResizeHandle handle)
        {
            return handle == BaseBoxTool.ResizeHandle.BottomLeft
                   || handle == BaseBoxTool.ResizeHandle.BottomRight
                   || handle == BaseBoxTool.ResizeHandle.TopLeft
                   || handle == BaseBoxTool.ResizeHandle.TopRight;
        }

        public override string GetTransformName()
        {
            return "Rotate";
        }

        public override Cursor CursorForHandle(BaseBoxTool.ResizeHandle handle)
        {
            return SledgeCursors.RotateCursor;
        }

        #region 2D Transformation Matrix
        public override Matrix4? GetTransformationMatrix(Viewport2D viewport, ViewportEvent e, BaseBoxTool.BoxState state, Document doc)
        {
            var origin = viewport.ZeroUnusedCoordinate((state.PreTransformBoxStart + state.PreTransformBoxEnd) / 2);
            var forigin = viewport.Flatten(origin);

            var origv = (state.MoveStart - forigin).Normalise();
            var newv = (viewport.ScreenToWorld(e.X, viewport.Height - e.Y) - forigin).Normalise();

            var angle = DMath.Acos(Math.Max(-1, Math.Min(1, origv.Dot(newv))));
            if ((origv.Cross(newv).Z < 0)) angle = 2 * DMath.PI - angle;

            var shf = KeyboardState.Shift;
            var def = Select.RotationStyle;
            var snap = (def == RotationStyle.SnapOnShift && shf) || (def == RotationStyle.SnapOffShift && !shf);
            if (snap)
            {
                var deg = angle * (180 / DMath.PI);
                var rnd = Math.Round(deg / 15) * 15;
                angle = rnd * (DMath.PI / 180);
            }

            Matrix4 rotm;
            if (viewport.Direction == Viewport2D.ViewDirection.Top) rotm = Matrix4.CreateRotationZ((float)angle);
            else if (viewport.Direction == Viewport2D.ViewDirection.Front) rotm = Matrix4.CreateRotationX((float)angle);
            else rotm = Matrix4.CreateRotationY((float)-angle); // The Y axis rotation goes in the reverse direction for whatever reason

            var mov = Matrix4.CreateTranslation((float)-origin.X, (float)-origin.Y, (float)-origin.Z);
            var rot = Matrix4.Mult(mov, rotm);
            return Matrix4.Mult(rot, Matrix4.Invert(mov));
        }
        #endregion 2D Transformation Matrix

        private readonly Dictionary<CircleType, List<Line>> _cachedLines = new Dictionary<CircleType, List<Line>>
        {
            {CircleType.X, new List<Line>()},
            {CircleType.Y, new List<Line>()},
            {CircleType.Z, new List<Line>()},
            {CircleType.Outer, new List<Line>()},
        };

        private int _cachedWidth;
        private int _cachedHeight;
        private Coordinate _cachedCameraLocation;
        private Coordinate _cachedCameraLookAt;
        private Coordinate _cachedPivotPoint;
        private CircleType _mouseOver;
        private CircleType _mouseDown;
        private Coordinate _mouseDownPoint;
        private Coordinate _mouseMovePoint;

        #region Line cache

        private void AddLine(CircleType type, Coordinate start, Coordinate end, Plane test, Viewport3D viewport)
        {
            var line = new Line(start, end);
            var cls = line.ClassifyAgainstPlane(test);
            if (cls == PlaneClassification.Back) return;
            if (cls == PlaneClassification.Spanning)
            {
                var isect = test.GetIntersectionPoint(line, true);
                var first = test.OnPlane(line.Start) > 0 ? line.Start : line.End;
                line = new Line(first, isect);
            }
            _cachedLines[type].Add(new Line(viewport.WorldToScreen(line.Start), viewport.WorldToScreen(line.End)));
        }

        private void UpdateCache(Viewport3D viewport, Document document)
        {
            var ccl = new Coordinate((decimal)viewport.Camera.Location.X, (decimal)viewport.Camera.Location.Y, (decimal)viewport.Camera.Location.Z);
            var ccla = new Coordinate((decimal)viewport.Camera.LookAt.X, (decimal)viewport.Camera.LookAt.Y, (decimal)viewport.Camera.LookAt.Z);
            var pp = document.Selection.GetSelectionBoundingBox().Center;
            if (ccl == _cachedCameraLocation && ccla == _cachedCameraLookAt && _cachedPivotPoint == pp && _cachedWidth == viewport.Width && _cachedHeight == viewport.Height) return;

            var origin = pp;
            var distance = (ccl - origin).VectorMagnitude();

            if (distance <= 1) return;

            _cachedCameraLocation = ccl;
            _cachedCameraLookAt = ccla;
            _cachedPivotPoint = pp;
            _cachedWidth = viewport.Width;
            _cachedHeight = viewport.Height;

            var normal = (ccl - origin).Normalise();
            var right = normal.Cross(Coordinate.UnitZ).Normalise();
            var up = normal.Cross(right).Normalise();

            var plane = new Plane(normal, origin.Dot(normal));

            const decimal sides = 32;
            var diff = (2 * DMath.PI) / sides;

            var radius = 0.15m * distance;

            _cachedLines[CircleType.Outer].Clear();
            _cachedLines[CircleType.X].Clear();
            _cachedLines[CircleType.Y].Clear();
            _cachedLines[CircleType.Z].Clear();

            for (var i = 0; i < sides; i++)
            {
                var cos1 = DMath.Cos(diff * i);
                var sin1 = DMath.Sin(diff * i);
                var cos2 = DMath.Cos(diff * (i + 1));
                var sin2 = DMath.Sin(diff * (i + 1));

                // outer circle
                AddLine(CircleType.Outer, 
                    origin + right * cos1 * radius * 1.2m + up * sin1 * radius * 1.2m,
                    origin + right * cos2 * radius * 1.2m + up * sin2 * radius * 1.2m,
                    plane, viewport);

                cos1 *= radius;
                sin1 *= radius;
                cos2 *= radius;
                sin2 *= radius;

                // X/Y plane = Z axis
                AddLine(CircleType.Z, 
                    origin + Coordinate.UnitX * cos1 + Coordinate.UnitY * sin1,
                    origin + Coordinate.UnitX * cos2 + Coordinate.UnitY * sin2,
                    plane, viewport);

                // Y/Z plane = X axis
                AddLine(CircleType.X, 
                    origin + Coordinate.UnitY * cos1 + Coordinate.UnitZ * sin1,
                    origin + Coordinate.UnitY * cos2 + Coordinate.UnitZ * sin2,
                    plane, viewport);

                // X/Z plane = Y axis
                AddLine(CircleType.Y, 
                    origin + Coordinate.UnitZ * cos1 + Coordinate.UnitX * sin1,
                    origin + Coordinate.UnitZ * cos2 + Coordinate.UnitX * sin2,
                    plane, viewport);
            }
        }

        #endregion

        private Matrix4? GetTransformationMatrix(Viewport3D viewport)
        {
            if (_mouseMovePoint == null || _mouseDownPoint == null || _cachedPivotPoint == null) return null;

            var originPoint = viewport.WorldToScreen(_cachedPivotPoint);
            var origv = (_mouseDownPoint - originPoint).Normalise();
            var newv = (_mouseMovePoint - originPoint).Normalise();
            var angle = DMath.Acos(Math.Max(-1, Math.Min(1, origv.Dot(newv))));
            if ((origv.Cross(newv).Z < 0)) angle = 2 * DMath.PI - angle;

            var shf = KeyboardState.Shift;
            var def = Select.RotationStyle;
            var snap = (def == RotationStyle.SnapOnShift && shf) || (def == RotationStyle.SnapOffShift && !shf);
            if (snap)
            {
                var deg = angle * (180 / DMath.PI);
                var rnd = Math.Round(deg / 15) * 15;
                angle = rnd * (DMath.PI / 180);
            }

            Vector3 axis;
            var dir = (viewport.Camera.Location - _cachedPivotPoint.ToVector3()).Normalized();
            switch (_mouseDown)
            {
                case CircleType.Outer:
                    axis = dir;
                    break;
                case CircleType.X:
                    axis = Vector3.UnitX;
                    break;
                case CircleType.Y:
                    axis = Vector3.UnitY;
                    break;
                case CircleType.Z:
                    axis = Vector3.UnitZ;
                    break;
                default:
                    return null;
            }
            var dirAng = Math.Acos(Vector3.Dot(dir, axis)) * 180 / Math.PI;
            if (dirAng > 90) angle = -angle;

            var rotm = Matrix4.CreateFromAxisAngle(axis, (float)angle);
            var mov = Matrix4.CreateTranslation(-_cachedPivotPoint.ToVector3());
            var rot = Matrix4.Mult(mov, rotm);
            return Matrix4.Mult(rot, Matrix4.Invert(mov));
        }

        private bool MouseOver(CircleType type, ViewportEvent ev, Viewport3D viewport)
        {
            var lines = _cachedLines[type];
            var point = new Coordinate(ev.X, viewport.Height - ev.Y, 0);
            return lines.Any(x => (x.ClosestPoint(point) - point).VectorMagnitude() <= 8);
        }

        public override void MouseMove3D(Viewport3D viewport, ViewportEvent ve, Document document)
        {
            if (document.Selection.IsEmpty() || !viewport.IsUnlocked(this)) return;

            if (_mouseDown != CircleType.None)
            {
                _mouseMovePoint = new Coordinate(ve.X, viewport.Height - ve.Y, 0);
                ve.Handled = true;
                var tform = GetTransformationMatrix(viewport);
                if (tform.HasValue) document.SetSelectListTransform(tform.Value);
            }
            else
            {
                UpdateCache(viewport, document);

                if (MouseOver(CircleType.Z, ve, viewport)) _mouseOver = CircleType.Z;
                else if (MouseOver(CircleType.Y, ve, viewport)) _mouseOver = CircleType.Y;
                else if (MouseOver(CircleType.X, ve, viewport)) _mouseOver = CircleType.X;
                else if (MouseOver(CircleType.Outer, ve, viewport)) _mouseOver = CircleType.Outer;
                else _mouseOver = CircleType.None;
            }
        }

        public override void MouseDown3D(Viewport3D viewport, ViewportEvent ve, Document document)
        {
            if (ve.Button != MouseButtons.Left || _mouseOver == CircleType.None) return;
            _mouseDown = _mouseOver;
            _mouseDownPoint = new Coordinate(ve.X, viewport.Height - ve.Y, 0);
            _mouseMovePoint = null;
            ve.Handled = true;
            viewport.AquireInputLock(this);
        }

        public override void MouseUp3D(Viewport3D viewport, ViewportEvent ve, Document document)
        {
            if (_mouseDown != CircleType.None && _mouseMovePoint != null) ve.Handled = true;

            var transformation = GetTransformationMatrix(viewport);
            if (transformation.HasValue)
            {
                ExecuteTransform(document, transformation.Value);
            }

            document.EndSelectionTransform();
            _mouseDown = CircleType.None;
            _mouseMovePoint = null;
            viewport.ReleaseInputLock(this);
        }

        private void ExecuteTransform(Document document, Matrix4 matrix)
        {

            var objects = document.Selection.GetSelectedParents().ToList();
            var name = String.Format("Rotate {0} object{1}", objects.Count, (objects.Count == 1 ? "" : "s"));

            var tform = new UnitMatrixMult(matrix);

            var action = new Edit(objects, (d, x) => x.Transform(tform, d.Map.GetTransformFlags()));
            document.PerformAction(name, action);
        }

        public override void MouseWheel3D(Viewport3D viewport, ViewportEvent ve, Document document)
        {
            if (_mouseDown != CircleType.None) ve.Handled = true;
        }

        public override void Render3D(Viewport3D viewport, Document document)
        {
            if (document.Selection.IsEmpty()) return;
            switch (_mouseMovePoint == null ? CircleType.None : _mouseDown)
            {
                case CircleType.None:
                    RenderCircleTypeNone(viewport, document);
                    break;
                case CircleType.Outer:
                case CircleType.X:
                case CircleType.Y:
                case CircleType.Z:
                    RenderAxisRotating(viewport, document);
                    break;
            }
        }

        private void RenderAxisRotating(Viewport3D viewport, Document document)
        {
            var axis = Vector3.UnitX;
            var c = Color.Red;

            if (_mouseDown == CircleType.Y)
            {
                axis = Vector3.UnitY;
                c = Color.Lime;
            }

            if (_mouseDown == CircleType.Z)
            {
                axis = Vector3.UnitZ;
                c = Color.Blue;
            }

            if (_mouseDown != CircleType.Outer)
            {
                GL.Begin(PrimitiveType.Lines);

                var zero = new Vector3((float) _cachedPivotPoint.DX, (float) _cachedPivotPoint.DY, (float) _cachedPivotPoint.DZ);

                GL.Color4(c);
                GL.Vertex3(zero - axis * 100000);
                GL.Vertex3(zero + axis * 100000);

                GL.End();
            }


            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.LineStipple);
            GL.LineStipple(5, 0xAAAA);
            GL.Begin(PrimitiveType.Lines);

            GL.Color4(Color.FromArgb(64, Color.Gray));
            GL.Vertex3(_cachedPivotPoint.ToVector3());
            GL.Vertex3(viewport.ScreenToWorld(_mouseDownPoint).ToVector3());

            GL.Color4(Color.LightGray);
            GL.Vertex3(_cachedPivotPoint.ToVector3());
            GL.Vertex3(viewport.ScreenToWorld(_mouseMovePoint).ToVector3());

            GL.End();
            GL.Disable(EnableCap.LineStipple);
            GL.Enable(EnableCap.DepthTest);
        }

        private void RenderCircleTypeNone(Viewport3D viewport, Document document)
        {
            var center = document.Selection.GetSelectionBoundingBox().Center;
            var origin = new Vector3((float) center.DX, (float) center.DY, (float) center.DZ);
            var distance = (viewport.Camera.Location - origin).Length;

            if (distance <= 1) return;

            var radius = 0.15f * distance;

            var normal = Vector3.Subtract(viewport.Camera.Location, origin).Normalized();
            var right = Vector3.Cross(normal, Vector3.UnitZ).Normalized();
            var up = Vector3.Cross(normal, right).Normalized();


            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Texture2D);

            const int sides = 32;
            const float diff = (float) (2 * Math.PI) / sides;

            GL.Begin(PrimitiveType.Lines);
            for (var i = 0; i < sides; i++)
            {
                var cos1 = (float)Math.Cos(diff * i);
                var sin1 = (float)Math.Sin(diff * i);
                var cos2 = (float)Math.Cos(diff * (i + 1));
                var sin2 = (float)Math.Sin(diff * (i + 1));
                GL.Color4(Color.DarkGray);
                GL.Vertex3(origin + right * cos1 * radius + up * sin1 * radius);
                GL.Vertex3(origin + right * cos2 * radius + up * sin2 * radius);
                GL.Color4(_mouseOver == CircleType.Outer ? Color.White : Color.LightGray);
                GL.Vertex3(origin + right * cos1 * radius * 1.2f + up * sin1 * radius * 1.2f);
                GL.Vertex3(origin + right * cos2 * radius * 1.2f + up * sin2 * radius * 1.2f);
            }
            GL.End();

            GL.Enable(EnableCap.ClipPlane0);
            GL.ClipPlane(ClipPlaneName.ClipPlane0, new double[]{ normal.X, normal.Y, normal.Z, -Vector3.Dot(origin, normal) });

            GL.LineWidth(2);
            GL.Begin(PrimitiveType.Lines);
            for (var i = 0; i < sides; i++)
            {
                var cos1 = (float) Math.Cos(diff * i) * radius;
                var sin1 = (float) Math.Sin(diff * i) * radius;
                var cos2 = (float) Math.Cos(diff * (i + 1)) * radius;
                var sin2 = (float) Math.Sin(diff * (i + 1)) * radius;

                GL.Color4(_mouseOver == CircleType.Z ? Color.Blue : Color.DarkBlue);
                GL.Vertex3(origin + Vector3.UnitX * cos1 + Vector3.UnitY * sin1);
                GL.Vertex3(origin + Vector3.UnitX * cos2 + Vector3.UnitY * sin2);

                GL.Color4(_mouseOver == CircleType.X ? Color.Red : Color.DarkRed);
                GL.Vertex3(origin + Vector3.UnitY * cos1 + Vector3.UnitZ * sin1);
                GL.Vertex3(origin + Vector3.UnitY * cos2 + Vector3.UnitZ * sin2);

                GL.Color4(_mouseOver == CircleType.Y ? Color.Lime : Color.LimeGreen);
                GL.Vertex3(origin + Vector3.UnitZ * cos1 + Vector3.UnitX * sin1);
                GL.Vertex3(origin + Vector3.UnitZ * cos2 + Vector3.UnitX * sin2);
            }
            GL.End();
            GL.LineWidth(1);

            GL.Disable(EnableCap.ClipPlane0);

            GL.Enable(EnableCap.DepthTest);
        }
    }
}