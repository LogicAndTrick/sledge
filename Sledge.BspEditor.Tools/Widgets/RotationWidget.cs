using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Rendering;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.Common;
using Sledge.DataStructures;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Shell.Input;

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
            public Coordinate CameraLocation { get; set; }
            public Coordinate CameraLookAt { get; set; }
            public Coordinate PivotPoint { get; set; }
            public MapViewport MapViewport { get; set; }
            public Dictionary<CircleType, List<Line>> Cache { get; set; }

            public CachedLines(MapViewport viewport3D)
            {
                MapViewport = viewport3D;
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

        private Coordinate _pivotPoint = Coordinate.Zero;
        private CircleType _mouseOver;
        private CircleType _mouseDown;
        private Coordinate _mouseDownPoint;
        private Coordinate _mouseMovePoint;

        public Coordinate GetPivotPoint()
        {
            return _pivotPoint;
        }

        public void SetPivotPoint(Coordinate point)
        {
            _pivotPoint = point;
        }

        public override bool IsUniformTransformation => true;
        public override bool IsScaleTransformation => false;

        public override void SelectionChanged()
        {
            if (Document.Selection.IsEmpty) _autoPivot = true;
            if (!_autoPivot) return;

            var bb = Document.Selection.GetSelectionBoundingBox();
            _pivotPoint = bb == null ? Coordinate.Zero : bb.Center;
        }

        #region Line cache

        private void AddLine(CircleType type, Coordinate start, Coordinate end, Plane test, CachedLines cache)
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
            cache.Cache[type].Add(new Line(cache.MapViewport.ProperWorldToScreen(line.Start), cache.MapViewport.ProperWorldToScreen(line.End)));
        }

        private void UpdateCache(MapViewport viewport, PerspectiveCamera camera, MapDocument document)
        {
            var ccl = camera.EyeLocation.ToCoordinate();
            var ccla = camera.LookAt.ToCoordinate();

            var cache = _cachedLines.FirstOrDefault(x => x.MapViewport == viewport);
            if (cache == null)
            {
                cache = new CachedLines(viewport);
                _cachedLines.Add(cache);
            }
            if (ccl == cache.CameraLocation && ccla == cache.CameraLookAt && cache.PivotPoint == _pivotPoint && cache.Width == viewport.Width && cache.Height == viewport.Height) return;

            var origin = _pivotPoint;
            var distance = (ccl - origin).VectorMagnitude();

            if (distance <= 1) return;

            cache.CameraLocation = ccl;
            cache.CameraLookAt = ccla;
            cache.PivotPoint = _pivotPoint;
            cache.Width = viewport.Width;
            cache.Height = viewport.Height;

            var normal = (ccl - origin).Normalise();
            var right = normal.Cross(Coordinate.UnitZ).Normalise();
            var up = normal.Cross(right).Normalise();

            var plane = new Plane(normal, origin.Dot(normal));

            const decimal sides = 32;
            var diff = (2 * DMath.PI) / sides;

            var radius = 0.15m * distance;

            cache.Cache[CircleType.Outer].Clear();
            cache.Cache[CircleType.X].Clear();
            cache.Cache[CircleType.Y].Clear();
            cache.Cache[CircleType.Z].Clear();

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
                    plane, cache);

                cos1 *= radius;
                sin1 *= radius;
                cos2 *= radius;
                sin2 *= radius;

                // X/Y plane = Z axis
                AddLine(CircleType.Z,
                    origin + Coordinate.UnitX * cos1 + Coordinate.UnitY * sin1,
                    origin + Coordinate.UnitX * cos2 + Coordinate.UnitY * sin2,
                    plane, cache);

                // Y/Z plane = X axis
                AddLine(CircleType.X,
                    origin + Coordinate.UnitY * cos1 + Coordinate.UnitZ * sin1,
                    origin + Coordinate.UnitY * cos2 + Coordinate.UnitZ * sin2,
                    plane, cache);

                // X/Z plane = Y axis
                AddLine(CircleType.Y,
                    origin + Coordinate.UnitZ * cos1 + Coordinate.UnitX * sin1,
                    origin + Coordinate.UnitZ * cos2 + Coordinate.UnitX * sin2,
                    plane, cache);
            }
        }

        #endregion

        private Matrix4? GetTransformationMatrix(MapViewport viewport)
        {
            if (_mouseMovePoint == null || _mouseDownPoint == null || _pivotPoint == null) return null;

            var originPoint = viewport.ProperWorldToScreen(_pivotPoint);
            var origv = (_mouseDownPoint - originPoint).Normalise();
            var newv = (_mouseMovePoint - originPoint).Normalise();
            var angle = DMath.Acos(Math.Max(-1, Math.Min(1, origv.Dot(newv))));
            if ((origv.Cross(newv).Z < 0)) angle = 2 * DMath.PI - angle;

            var shf = KeyboardState.Shift;
            // var def = Select.RotationStyle;
            var snap = true; // (def == RotationStyle.SnapOnShift && shf) || (def == RotationStyle.SnapOffShift && !shf);
            if (snap)
            {
                var deg = angle * (180 / DMath.PI);
                var rnd = Math.Round(deg / 15) * 15;
                angle = rnd * (DMath.PI / 180);
            }

            Vector3 axis;
            var dir = (viewport.Viewport.Camera.EyeLocation - _pivotPoint.ToVector3()).Normalized();
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
            var mov = Matrix4.CreateTranslation(-_pivotPoint.ToVector3());
            var rot = Matrix4.Mult(mov, rotm);
            return Matrix4.Mult(rot, Matrix4.Invert(mov));
        }

        private bool MouseOver(CircleType type, ViewportEvent ev, MapViewport viewport)
        {
            var cache = _cachedLines.FirstOrDefault(x => x.MapViewport == viewport);
            if (cache == null) return false;
            var lines = cache.Cache[type];
            var point = new Coordinate(ev.X, viewport.Height - ev.Y, 0);
            return lines.Any(x => (x.ClosestPoint(point) - point).VectorMagnitude() <= 8);
        }

        protected override void MouseLeave(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            viewport.Control.Cursor = Cursors.Default;
        }

        protected override void MouseMove(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            if (viewport != _activeViewport) return;

            if (Document.Selection.IsEmpty || !viewport.IsUnlocked(this)) return;

            if (_mouseDown != CircleType.None)
            {
                _mouseMovePoint = new Coordinate(e.X, viewport.Height - e.Y, 0);
                e.Handled = true;
                var tform = GetTransformationMatrix(viewport);
                OnTransforming(tform);
            }
            else
            {
                UpdateCache(viewport, camera, Document);

                if (MouseOver(CircleType.Z, e, viewport)) _mouseOver = CircleType.Z;
                else if (MouseOver(CircleType.Y, e, viewport)) _mouseOver = CircleType.Y;
                else if (MouseOver(CircleType.X, e, viewport)) _mouseOver = CircleType.X;
                else if (MouseOver(CircleType.Outer, e, viewport)) _mouseOver = CircleType.Outer;
                else _mouseOver = CircleType.None;
            }
        }

        protected override void MouseDown(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            if (viewport != _activeViewport) return;

            if (e.Button != MouseButtons.Left || _mouseOver == CircleType.None) return;
            _mouseDown = _mouseOver;
            _mouseDownPoint = new Coordinate(e.X, viewport.Height - e.Y, 0);
            _mouseMovePoint = null;
            e.Handled = true;
            viewport.AquireInputLock(this);
        }

        protected override void MouseUp(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            if (viewport != _activeViewport) return;

            if (_mouseDown != CircleType.None && _mouseMovePoint != null) e.Handled = true;

            var transformation = GetTransformationMatrix(viewport);
            OnTransformed(transformation);
            _mouseDown = CircleType.None;
            _mouseMovePoint = null;
            viewport.ReleaseInputLock(this);
        }

        protected override void MouseWheel(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            if (viewport != _activeViewport) return;
            if (_mouseDown != CircleType.None) e.Handled = true;
        }

        protected override IEnumerable<Element> GetViewportElements(MapViewport viewport, PerspectiveCamera camera)
        {
            if (!Document.Selection.IsEmpty)
            {
                switch (_mouseMovePoint == null ? CircleType.None : _mouseDown)
                {
                    case CircleType.None:
                        return GetElementsCircleTypeNone(viewport, camera, Document);
                    case CircleType.Outer:
                    case CircleType.X:
                    case CircleType.Y:
                    case CircleType.Z:
                        return GetElementsAxisRotating(viewport, camera, Document);
                }
            }
            return new Element[0];
        }

        private IEnumerable<Element> GetElementsAxisRotating(MapViewport viewport, PerspectiveCamera camera, MapDocument document)
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

            if (_mouseDown == CircleType.Outer)
            {
                var vp3 = _activeViewport as MapViewport;
                if (vp3 != null) axis = (camera.LookAt - camera.EyeLocation).Normalized();
                c = Color.White;
            }

            if (_activeViewport != viewport || _mouseDown != CircleType.Outer)
            {
                var zero = new Vector3((float)_pivotPoint.DX, (float)_pivotPoint.DY, (float)_pivotPoint.DZ);

                yield return new LineElement(PositionType.World, c, new List<Position>
                {
                    new Position(zero - axis * 100000),
                    new Position(zero + axis * 100000)
                });
            }

            if (_activeViewport == viewport)
            {
                yield return new LineElement(PositionType.World, Color.FromArgb(64, Color.Gray), new List<Position>
                {
                    new Position(_pivotPoint.ToVector3()),
                    new Position(viewport.ProperScreenToWorld(_mouseDownPoint).ToVector3())
                })
                {
                    Stippled = true
                };

                yield return new LineElement(PositionType.World, Color.LightGray, new List<Position>
                {
                    new Position(_pivotPoint.ToVector3()),
                    new Position(viewport.ProperScreenToWorld(_mouseMovePoint).ToVector3())
                })
                {
                    Stippled = true
                };
            }
        }

        private IEnumerable<Element> GetElementsCircleTypeNone(MapViewport viewport, PerspectiveCamera camera, MapDocument document)
        {
            var center = _pivotPoint;
            var origin = new Vector3((float)center.DX, (float)center.DY, (float)center.DZ);
            var distance = (camera.EyeLocation - origin).Length;

            if (distance <= 1) yield break;

            var radius = 0.15f * distance;

            var normal = Vector3.Subtract(camera.EyeLocation, origin).Normalized();
            var right = Vector3.Cross(normal, Vector3.UnitZ).Normalized();
            var up = Vector3.Cross(normal, right).Normalized();

            const int sides = 32;
            const float diff = (float)(2 * Math.PI) / sides;

            for (var i = 0; i < sides; i++)
            {
                var cos1 = (float)Math.Cos(diff * i);
                var sin1 = (float)Math.Sin(diff * i);
                var cos2 = (float)Math.Cos(diff * (i + 1));
                var sin2 = (float)Math.Sin(diff * (i + 1));

                yield return new LineElement(PositionType.World, Color.DarkGray, new List<Position>
                {
                    new Position(origin + right * cos1 * radius + up * sin1 * radius),
                    new Position(origin + right * cos2 * radius + up * sin2 * radius)
                });

                yield return new LineElement(PositionType.World, _mouseOver == CircleType.Outer ? Color.White : Color.LightGray, new List<Position>
                {
                    new Position(origin + right * cos1 * radius * 1.2f + up * sin1 * radius * 1.2f),
                    new Position(origin + right * cos2 * radius * 1.2f + up * sin2 * radius * 1.2f)
                });
            }

            var plane = new Plane(normal.ToCoordinate(), (decimal) Vector3.Dot(origin, normal));

            for (var i = 0; i < sides; i++)
            {
                var cos1 = (float)Math.Cos(diff * i) * radius;
                var sin1 = (float)Math.Sin(diff * i) * radius;
                var cos2 = (float)Math.Cos(diff * (i + 1)) * radius;
                var sin2 = (float)Math.Sin(diff * (i + 1)) * radius;

                var zline = GetLineElement(
                    (origin + Vector3.UnitX * cos1 + Vector3.UnitY * sin1).ToCoordinate(),
                    (origin + Vector3.UnitX * cos2 + Vector3.UnitY * sin2).ToCoordinate(),
                    plane,
                    _mouseOver == CircleType.Z ? Color.Blue : Color.DarkBlue);

                var xline = GetLineElement(
                    (origin + Vector3.UnitY * cos1 + Vector3.UnitZ * sin1).ToCoordinate(),
                    (origin + Vector3.UnitY * cos2 + Vector3.UnitZ * sin2).ToCoordinate(),
                    plane,
                    _mouseOver == CircleType.X ? Color.Red : Color.DarkRed);

                var yline = GetLineElement(
                    (origin + Vector3.UnitZ * cos1 + Vector3.UnitX * sin1).ToCoordinate(),
                    (origin + Vector3.UnitZ * cos2 + Vector3.UnitX * sin2).ToCoordinate(),
                    plane,
                    _mouseOver == CircleType.Y ? Color.Lime : Color.LimeGreen);

                if (xline != null) yield return xline;
                if (yline != null) yield return yline;
                if (zline != null) yield return zline;
            }
        }

        private LineElement GetLineElement(Coordinate start, Coordinate end, Plane plane, Color color)
        {
            var line = new Line(start, end);
            var cls = line.ClassifyAgainstPlane(plane);
            if (cls == PlaneClassification.Back) return null;
            if (cls == PlaneClassification.Spanning)
            {
                var isect = plane.GetIntersectionPoint(line, true);
                var first = plane.OnPlane(line.Start) > 0 ? line.Start : line.End;
                line = new Line(first, isect);
            }

            return new LineElement(PositionType.World, color, new List<Position>
            {
                new Position(line.Start.ToVector3()),
                new Position(line.End.ToVector3())
            })
            {
                Width = 2
            };
        }
    }
}
