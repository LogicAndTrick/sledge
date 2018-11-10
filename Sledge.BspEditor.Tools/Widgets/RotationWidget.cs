using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;
using Sledge.Shell.Input;
using Plane = Sledge.DataStructures.Geometric.Plane;

namespace Sledge.BspEditor.Tools.Widgets
{
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

        public RotationWidget(MapDocument document)
        {
            SetDocument(document);
        }

        private class CachedLines
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public Vector3 CameraLocation { get; set; }
            public Vector3 CameraLookAt { get; set; }
            public Vector3 PivotPoint { get; set; }
            public IViewport Viewport { get; set; }
            public Dictionary<CircleType, List<Line>> Cache { get; set; }

            public CachedLines(IViewport viewport)
            {
                Viewport = viewport;
                Cache = new Dictionary<CircleType, List<Line>>
                {
                    {CircleType.Outer, new List<Line>()},
                    {CircleType.X, new List<Line>()},
                    {CircleType.Y, new List<Line>()},
                    {CircleType.Z, new List<Line>()}
                };
            }
        }

        private readonly List<CachedLines> _cachedLines = new List<CachedLines>();

        private bool _autoPivot = true;

        private Vector3 _pivotPoint = Vector3.Zero;
        private CircleType _mouseOver;
        private CircleType _mouseDown;
        private Vector3? _mouseDownPoint;
        private Vector3? _mouseMovePoint;

        public Vector3 GetPivotPoint()
        {
            return _pivotPoint;
        }

        public void SetPivotPoint(Vector3 point)
        {
            _pivotPoint = point;
        }

        public override bool IsUniformTransformation => true;
        public override bool IsScaleTransformation => false;

        public override void SelectionChanged()
        {
            var document = GetDocument();
            if (document != null && document.Selection.IsEmpty) _autoPivot = true;
            if (!_autoPivot) return;

            var bb = document?.Selection.GetSelectionBoundingBox();
            _pivotPoint = bb?.Center ?? Vector3.Zero;
        }

        #region Line cache

        private void AddLine(CircleType type, Vector3 start, Vector3 end, Plane test, CachedLines cache)
        {
            var line = new Line(start, end);
            var cls = line.ClassifyAgainstPlane(test);
            if (cls == PlaneClassification.Back) return;
            if (cls == PlaneClassification.Spanning)
            {
                var isect = test.GetIntersectionPoint(line, true);
                var first = test.OnPlane(line.Start) > 0 ? line.Start : line.End;
                if (isect.HasValue) line = new Line(first, isect.Value);
            }
            cache.Cache[type].Add(new Line(cache.Viewport.Camera.WorldToScreen(line.Start), cache.Viewport.Camera.WorldToScreen(line.End)));
        }

        private void UpdateCache(IViewport viewport, PerspectiveCamera camera)
        {
            var ccl = camera.EyeLocation;
            var ccla = camera.Position + camera.Direction;

            var cache = _cachedLines.FirstOrDefault(x => x.Viewport == viewport);
            if (cache == null)
            {
                cache = new CachedLines(viewport);
                _cachedLines.Add(cache);
            }
            if (ccl == cache.CameraLocation && ccla == cache.CameraLookAt && cache.PivotPoint == _pivotPoint && cache.Width == viewport.Width && cache.Height == viewport.Height) return;

            var origin = _pivotPoint;
            var distance = (ccl - origin).Length();

            if (distance <= 1) return;

            cache.CameraLocation = ccl;
            cache.CameraLookAt = ccla;
            cache.PivotPoint = _pivotPoint;
            cache.Width = viewport.Width;
            cache.Height = viewport.Height;

            var normal = (ccl - origin).Normalise();
            var right = normal.Cross(Vector3.UnitZ).Normalise();
            var up = normal.Cross(right).Normalise();

            var plane = new Plane(normal, origin.Dot(normal));

            const float sides = 32;
            var diff = (2 * Math.PI) / sides;

            var radius = 0.15f * distance;

            cache.Cache[CircleType.Outer].Clear();
            cache.Cache[CircleType.X].Clear();
            cache.Cache[CircleType.Y].Clear();
            cache.Cache[CircleType.Z].Clear();

            for (var i = 0; i < sides; i++)
            {
                var cos1 = (float) Math.Cos(diff * i);
                var sin1 = (float) Math.Sin(diff * i);
                var cos2 = (float) Math.Cos(diff * (i + 1));
                var sin2 = (float) Math.Sin(diff * (i + 1));

                // outer circle
                AddLine(CircleType.Outer,
                    origin + right * cos1 * radius * 1.2f + up * sin1 * radius * 1.2f,
                    origin + right * cos2 * radius * 1.2f + up * sin2 * radius * 1.2f,
                    plane, cache);

                cos1 *= radius;
                sin1 *= radius;
                cos2 *= radius;
                sin2 *= radius;

                // X/Y plane = Z axis
                AddLine(CircleType.Z,
                    origin + Vector3.UnitX * cos1 + Vector3.UnitY * sin1,
                    origin + Vector3.UnitX * cos2 + Vector3.UnitY * sin2,
                    plane, cache);

                // Y/Z plane = X axis
                AddLine(CircleType.X,
                    origin + Vector3.UnitY * cos1 + Vector3.UnitZ * sin1,
                    origin + Vector3.UnitY * cos2 + Vector3.UnitZ * sin2,
                    plane, cache);

                // X/Z plane = Y axis
                AddLine(CircleType.Y,
                    origin + Vector3.UnitZ * cos1 + Vector3.UnitX * sin1,
                    origin + Vector3.UnitZ * cos2 + Vector3.UnitX * sin2,
                    plane, cache);
            }
        }

        #endregion

        private Matrix4x4? GetTransformationMatrix(MapViewport viewport)
        {
            if (_mouseMovePoint == null || _mouseDownPoint == null) return null;

            var originPoint = viewport.Viewport.Camera.WorldToScreen(_pivotPoint);
            var origv = Vector3.Normalize(_mouseDownPoint.Value - originPoint);
            var newv = Vector3.Normalize(_mouseMovePoint.Value - originPoint);
            var angle = Math.Acos(Math.Max(-1, Math.Min(1, origv.Dot(newv))));
            if ((origv.Cross(newv).Z < 0)) angle = 2 * Math.PI - angle;

            var shf = KeyboardState.Shift;
            // var def = Select.RotationStyle;
            var snap = true; // (def == RotationStyle.SnapOnShift && shf) || (def == RotationStyle.SnapOffShift && !shf);
            if (snap)
            {
                var deg = angle * (180 / Math.PI);
                var rnd = Math.Round(deg / 15) * 15;
                angle = rnd * (Math.PI / 180);
            }

            Vector3 axis;
            var dir = Vector3.Normalize(viewport.Viewport.Camera.Location - _pivotPoint);
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

            var rotm = Matrix4x4.CreateFromAxisAngle(axis, (float) -angle);
            var mov = Matrix4x4.CreateTranslation(-_pivotPoint);
            var rot = Matrix4x4.Multiply(mov, rotm);
            var inv = Matrix4x4.Invert(mov, out var i) ? i : Matrix4x4.Identity;
            return Matrix4x4.Multiply(rot, inv);
        }

        private bool MouseOver(CircleType type, ViewportEvent ev, MapViewport viewport)
        {
            var cache = _cachedLines.FirstOrDefault(x => x.Viewport == viewport.Viewport);
            if (cache == null) return false;
            var lines = cache.Cache[type];
            var point = new Vector3(ev.X, ev.Y, 0);
            return lines.Any(x => (x.ClosestPoint(point) - point).Length() <= 8);
        }

        protected override void MouseLeave(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            viewport.Control.Cursor = Cursors.Default;
        }

        protected override void MouseMove(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            if (viewport != ActiveViewport) return;

            if (document.Selection.IsEmpty || !viewport.IsUnlocked(this)) return;

            if (_mouseDown != CircleType.None)
            {
                _mouseMovePoint = new Vector3(e.X, e.Y, 0);
                e.Handled = true;
                var tform = GetTransformationMatrix(viewport);
                OnTransforming(tform);
            }
            else
            {
                UpdateCache(viewport.Viewport, camera);

                if (MouseOver(CircleType.Z, e, viewport)) _mouseOver = CircleType.Z;
                else if (MouseOver(CircleType.Y, e, viewport)) _mouseOver = CircleType.Y;
                else if (MouseOver(CircleType.X, e, viewport)) _mouseOver = CircleType.X;
                else if (MouseOver(CircleType.Outer, e, viewport)) _mouseOver = CircleType.Outer;
                else _mouseOver = CircleType.None;
            }
        }

        protected override void MouseDown(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            if (viewport != ActiveViewport) return;

            if (e.Button != MouseButtons.Left || _mouseOver == CircleType.None) return;
            _mouseDown = _mouseOver;
            _mouseDownPoint = new Vector3(e.X, e.Y, 0);
            _mouseMovePoint = null;
            e.Handled = true;
            viewport.AquireInputLock(this);
        }

        protected override void MouseUp(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            if (viewport != ActiveViewport) return;

            if (_mouseDown != CircleType.None && _mouseMovePoint != null) e.Handled = true;

            var transformation = GetTransformationMatrix(viewport);
            OnTransformed(transformation);
            _mouseDown = CircleType.None;
            _mouseMovePoint = null;
            viewport.ReleaseInputLock(this);
        }

        protected override void MouseWheel(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            if (viewport != ActiveViewport) return;
            if (_mouseDown != CircleType.None) e.Handled = true;
        }

        protected override void Render(MapDocument document, BufferBuilder builder, ResourceCollector resourceCollector)
        {
            if (_mouseMovePoint.HasValue && _mouseDown != CircleType.None)
            {
                var axis = Vector3.One;
                var c = Color.White;

                switch (_mouseDown)
                {
                    case CircleType.X:
                        axis = Vector3.UnitX;
                        c = Color.Red;
                        break;
                    case CircleType.Y:
                        axis = Vector3.UnitY;
                        c = Color.Lime;
                        break;
                    case CircleType.Z:
                        axis = Vector3.UnitZ;
                        c = Color.Blue;
                        break;
                    case CircleType.Outer:
                        if (ActiveViewport == null || !(ActiveViewport.Viewport.Camera is PerspectiveCamera pc)) return;
                        axis = pc.Direction;
                        c = Color.White;
                        break;
                }

                var start = _pivotPoint - axis * 1024 * 1024;
                var end = _pivotPoint + axis * 1024 * 1024;

                var col = new Vector4(c.R, c.G, c.B, c.A) / 255;

                builder.Append(
                    new[]
                    {
                        new VertexStandard {Position = start, Colour = col, Tint = Vector4.One},
                        new VertexStandard {Position = end, Colour = col, Tint = Vector4.One},
                    },
                    new uint[] { 0, 1 },
                    new[]
                    {
                        new BufferGroup(PipelineType.Wireframe, CameraType.Perspective, 0, 2)
                    }
                );
            }

            base.Render(document, builder, resourceCollector);
        }

        protected override void Render(MapDocument document, IViewport viewport, PerspectiveCamera camera, I2DRenderer im)
        {
            if (!document.Selection.IsEmpty)
            {
                switch (_mouseMovePoint == null ? CircleType.None : _mouseDown)
                {
                    case CircleType.None:
                        RenderCircleTypeNone(camera, im);
                        break;
                    case CircleType.Outer:
                    case CircleType.X:
                    case CircleType.Y:
                    case CircleType.Z:
                        RenderAxisRotating(viewport, camera, im);
                        break;
                }
            }
            base.Render(document, viewport, camera, im);
        }

        private void RenderAxisRotating(IViewport viewport, PerspectiveCamera camera, I2DRenderer im)
        {
            if (ActiveViewport.Viewport != viewport || !_mouseDownPoint.HasValue || !_mouseMovePoint.HasValue) return;

            var st = camera.WorldToScreen(_pivotPoint);
            var en = _mouseDownPoint.Value;
            im.AddLine(st.ToVector2(), en.ToVector2(), Color.Gray);

            en = _mouseMovePoint.Value;
            im.AddLine(st.ToVector2(), en.ToVector2(), Color.LightGray);
        }

        private void RenderCircleTypeNone(PerspectiveCamera camera, I2DRenderer im)
        {
            var center = _pivotPoint;
            var origin = new Vector3(center.X, center.Y, center.Z);

            var distance = (camera.EyeLocation - origin).Length();
            if (distance <= 1) return;

            // Ensure points that can't be projected properly don't get rendered
            var screenOrigin = camera.WorldToScreen(origin);
            var sop = new PointF(screenOrigin.X, screenOrigin.Y);
            var rec = new RectangleF(-200, -200, camera.Width + 400, camera.Height + 400);
            if (!rec.Contains(sop)) return;

            var radius = 0.15f * distance;

            var normal = Vector3.Normalize(Vector3.Subtract(camera.EyeLocation, origin));
            var right = Vector3.Normalize(Vector3.Cross(normal, Vector3.UnitZ));
            var up = Vector3.Normalize(Vector3.Cross(normal, right));

            const int sides = 32;
            const float diff = (float)(2 * Math.PI) / sides;

            for (var i = 0; i < sides; i++)
            {
                var cos1 = (float)Math.Cos(diff * i);
                var sin1 = (float)Math.Sin(diff * i);
                var cos2 = (float)Math.Cos(diff * (i + 1));
                var sin2 = (float)Math.Sin(diff * (i + 1));

                var line = new Line(
                    origin + right * cos1 * radius + up * sin1 * radius,
                    origin + right * cos2 * radius + up * sin2 * radius
                );

                var st = camera.WorldToScreen(line.Start);
                var en = camera.WorldToScreen(line.End);

                im.AddLine(st.ToVector2(), en.ToVector2(), Color.DarkGray);

                line = new Line(
                    origin + right * cos1 * radius * 1.2f + up * sin1 * radius * 1.2f,
                    origin + right * cos2 * radius * 1.2f + up * sin2 * radius * 1.2f
                );

                st = camera.WorldToScreen(line.Start);
                en = camera.WorldToScreen(line.End);

                var c = _mouseOver == CircleType.Outer ? Color.White : Color.LightGray;
                im.AddLine(st.ToVector2(), en.ToVector2(), c);
            }

            var plane = new Plane(normal, Vector3.Dot(origin, normal));

            for (var i = 0; i < sides; i++)
            {
                var cos1 = (float)Math.Cos(diff * i) * radius;
                var sin1 = (float)Math.Sin(diff * i) * radius;
                var cos2 = (float)Math.Cos(diff * (i + 1)) * radius;
                var sin2 = (float)Math.Sin(diff * (i + 1)) * radius;

                RenderLine(
                    (origin + Vector3.UnitX * cos1 + Vector3.UnitY * sin1),
                    (origin + Vector3.UnitX * cos2 + Vector3.UnitY * sin2),
                    plane,
                    _mouseOver == CircleType.Z ? Color.Blue : Color.DarkBlue,
                    camera, im);

                RenderLine(
                    (origin + Vector3.UnitY * cos1 + Vector3.UnitZ * sin1),
                    (origin + Vector3.UnitY * cos2 + Vector3.UnitZ * sin2),
                    plane,
                    _mouseOver == CircleType.X ? Color.Red : Color.DarkRed,
                    camera, im);

                RenderLine(
                    (origin + Vector3.UnitZ * cos1 + Vector3.UnitX * sin1),
                    (origin + Vector3.UnitZ * cos2 + Vector3.UnitX * sin2),
                    plane,
                    _mouseOver == CircleType.Y ? Color.Lime : Color.LimeGreen,
                    camera, im);
            }
        }

        private void RenderLine(Vector3 start, Vector3 end, Plane plane, Color color, ICamera camera, I2DRenderer im)
        {
            var line = new Line(start, end);
            var cls = line.ClassifyAgainstPlane(plane);
            if (cls == PlaneClassification.Back) return;
            if (cls == PlaneClassification.Spanning)
            {
                var isect = plane.GetIntersectionPoint(line, true);
                var first = plane.OnPlane(line.Start) > 0 ? line.Start : line.End;
                if (!isect.HasValue) return;
                line = new Line(first, isect.Value);
            }

            var st = camera.WorldToScreen(line.Start);
            var en = camera.WorldToScreen(line.End);

            im.AddLine(st.ToVector2(), en.ToVector2(), color, 2);
        }
    }
}
