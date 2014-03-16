using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Extensions;
using Sledge.Settings;
using Sledge.UI;

namespace Sledge.Editor.Tools.Widgets
{
    public abstract class Widget : BaseTool
    {
        private Action<Matrix4?> _transformedCallback = null;
        private Action<Matrix4?> _transformingCallback = null;

        public Action<Matrix4?> OnTransformed
        {
            get
            {
                return _transformedCallback ?? (x => { });
            }
            set
            {
                _transformedCallback = value;
            }
        }

        public Action<Matrix4?> OnTransforming
        {
            get
            {
                return _transformingCallback ?? (x => { });
            }
            set
            {
                _transformingCallback = value;
            }
        }
        /*
        protected void OnTransformed(Matrix4? transformation)
        {
            if (transformation.HasValue)
            {
                ExecuteTransform(Document, transformation.Value);
            }

            Document.EndSelectionTransform();
        }

        private void ExecuteTransform(Document document, Matrix4 matrix)
        {
            var objects = document.Selection.GetSelectedParents().ToList();
            var name = String.Format("Rotate {0} object{1}", objects.Count, (objects.Count == 1 ? "" : "s"));

            var tform = new UnitMatrixMult(matrix);

            var action = new Edit(objects, (d, x) => x.Transform(tform, d.Map.GetTransformFlags()));
            document.PerformAction(name, action);
        }

        protected void OnTransforming(Matrix4? tform)
        {
            if (tform.HasValue) Document.SetSelectListTransform(tform.Value);
        }*/


        public override Image GetIcon() { return null; }
        public override string GetName() { return "Widget"; }
        public override HotkeyTool? GetHotkeyToolType() { return null; }
        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage) { return HotkeyInterceptResult.Continue; }
        public override void KeyUp(ViewportBase viewport, ViewportEvent e) { }
        public override void KeyDown(ViewportBase viewport, ViewportEvent e) { }
        public override void KeyPress(ViewportBase viewport, ViewportEvent e) { }
        public override void MouseClick(ViewportBase viewport, ViewportEvent e) { }
        public override void MouseDoubleClick(ViewportBase viewport, ViewportEvent e) { }
        public override void MouseEnter(ViewportBase viewport, ViewportEvent e) { }
        public override void MouseLeave(ViewportBase viewport, ViewportEvent e) { }
        public override void UpdateFrame(ViewportBase viewport, FrameInfo frame) { }
    }

    public class RotationWidget : Widget
    {
        private enum CircleType
        {
            None,
            Outer,
            X,
            Y,
            Z
        }

        public RotationWidget(Document document)
        {
            Document = document;
        }

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

        public override void MouseMove(ViewportBase viewport, ViewportEvent e)
        {
            var vp = viewport as Viewport3D;
            if (vp == null) return;

            if (Document.Selection.IsEmpty() || !vp.IsUnlocked(this)) return;

            if (_mouseDown != CircleType.None)
            {
                _mouseMovePoint = new Coordinate(e.X, vp.Height - e.Y, 0);
                e.Handled = true;
                var tform = GetTransformationMatrix(vp);
                OnTransforming(tform);
            }
            else
            {
                UpdateCache(vp, Document);

                if (MouseOver(CircleType.Z, e, vp)) _mouseOver = CircleType.Z;
                else if (MouseOver(CircleType.Y, e, vp)) _mouseOver = CircleType.Y;
                else if (MouseOver(CircleType.X, e, vp)) _mouseOver = CircleType.X;
                else if (MouseOver(CircleType.Outer, e, vp)) _mouseOver = CircleType.Outer;
                else _mouseOver = CircleType.None;
            }
        }

        public override void MouseDown(ViewportBase viewport, ViewportEvent ve)
        {
            var vp = viewport as Viewport3D;
            if (vp == null) return;

            if (ve.Button != MouseButtons.Left || _mouseOver == CircleType.None) return;
            _mouseDown = _mouseOver;
            _mouseDownPoint = new Coordinate(ve.X, vp.Height - ve.Y, 0);
            _mouseMovePoint = null;
            ve.Handled = true;
            vp.AquireInputLock(this);
        }

        public override void MouseUp(ViewportBase viewport, ViewportEvent ve)
        {
            var vp = viewport as Viewport3D;
            if (vp == null) return;

            if (_mouseDown != CircleType.None && _mouseMovePoint != null) ve.Handled = true;

            var transformation = GetTransformationMatrix(vp);
            OnTransformed(transformation);
            _mouseDown = CircleType.None;
            _mouseMovePoint = null;
            vp.ReleaseInputLock(this);
        }

        public override void MouseWheel(ViewportBase viewport, ViewportEvent ve)
        {
            if (_mouseDown != CircleType.None) ve.Handled = true;
        }

        public override void Render(ViewportBase viewport)
        {
            var vp = viewport as Viewport3D;
            if (vp == null) return;

            if (Document.Selection.IsEmpty()) return;
            switch (_mouseMovePoint == null ? CircleType.None : _mouseDown)
            {
                case CircleType.None:
                    RenderCircleTypeNone(vp, Document);
                    break;
                case CircleType.Outer:
                case CircleType.X:
                case CircleType.Y:
                case CircleType.Z:
                    RenderAxisRotating(vp, Document);
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

                var zero = new Vector3((float)_cachedPivotPoint.DX, (float)_cachedPivotPoint.DY, (float)_cachedPivotPoint.DZ);

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
            var origin = new Vector3((float)center.DX, (float)center.DY, (float)center.DZ);
            var distance = (viewport.Camera.Location - origin).Length;

            if (distance <= 1) return;

            var radius = 0.15f * distance;

            var normal = Vector3.Subtract(viewport.Camera.Location, origin).Normalized();
            var right = Vector3.Cross(normal, Vector3.UnitZ).Normalized();
            var up = Vector3.Cross(normal, right).Normalized();


            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Texture2D);

            const int sides = 32;
            const float diff = (float)(2 * Math.PI) / sides;

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
            GL.ClipPlane(ClipPlaneName.ClipPlane0, new double[] { normal.X, normal.Y, normal.Z, -Vector3.Dot(origin, normal) });

            GL.LineWidth(2);
            GL.Begin(PrimitiveType.Lines);
            for (var i = 0; i < sides; i++)
            {
                var cos1 = (float)Math.Cos(diff * i) * radius;
                var sin1 = (float)Math.Sin(diff * i) * radius;
                var cos2 = (float)Math.Cos(diff * (i + 1)) * radius;
                var sin2 = (float)Math.Sin(diff * (i + 1)) * radius;

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
