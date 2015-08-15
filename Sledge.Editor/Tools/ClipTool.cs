using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Extensions;
using Sledge.Editor.Properties;
using Sledge.Editor.Rendering;
using Sledge.Editor.UI;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Settings;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Face = Sledge.DataStructures.MapObjects.Face;
using Line = Sledge.Rendering.Scenes.Renderables.Line;
using Vertex = Sledge.Rendering.Scenes.Renderables.Vertex;

namespace Sledge.Editor.Tools
{
    public class ClipTool : BaseTool
    {
        public enum ClipState
        {
            None,
            Drawing,
            Drawn,
            MovingPoint1,
            MovingPoint2,
            MovingPoint3
        }

        public enum ClipSide
        {
            Both,
            Front,
            Back
        }

        private Coordinate _clipPlanePoint1;
        private Coordinate _clipPlanePoint2;
        private Coordinate _clipPlanePoint3;
        private Coordinate _drawingPoint;
        private ClipState _prevState;
        private ClipState _state;
        private ClipSide _side;

        public ClipTool()
        {
            Usage = ToolUsage.Both;
            _clipPlanePoint1 = _clipPlanePoint2 = _clipPlanePoint3 = _drawingPoint = null;
            _state = _prevState = ClipState.None;
            _side = ClipSide.Both;

            UseValidation = true;
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Clip;
        }

        public override string GetName()
        {
            return "Clip Tool";
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.Clip;
        }

        public override string GetContextualHelp()
        {
            return "*Click* and drag to define the clipping plane.\n" +
                   "*Click* and drag any of the three points to change the orientation of the plane.\n" +
                   "Press *enter* to cut the selected solids along the clipping plane.";
        }

        private ClipState GetStateAtPoint(int x, int y, MapViewport viewport)
        {
            if (_clipPlanePoint1 == null || _clipPlanePoint2 == null || _clipPlanePoint3 == null) return ClipState.None;

            var p = viewport.ScreenToWorld(x, y);
            var p1 = viewport.Flatten(_clipPlanePoint1);
            var p2 = viewport.Flatten(_clipPlanePoint2);
            var p3 = viewport.Flatten(_clipPlanePoint3);

            var d = 5 / (decimal) viewport.Zoom;

            if (p.X >= p1.X - d && p.X <= p1.X + d && p.Y >= p1.Y - d && p.Y <= p1.Y + d) return ClipState.MovingPoint1;
            if (p.X >= p2.X - d && p.X <= p2.X + d && p.Y >= p2.Y - d && p.Y <= p2.Y + d) return ClipState.MovingPoint2;
            if (p.X >= p3.X - d && p.X <= p3.X + d && p.Y >= p3.Y - d && p.Y <= p3.Y + d) return ClipState.MovingPoint3;

            return ClipState.None;
        }

        protected override void MouseDown(MapViewport vp, OrthographicCamera camera, ViewportEvent e)
        {
            var viewport = vp;
            _prevState = _state;

            var point = SnapIfNeeded(viewport.Expand(viewport.ScreenToWorld(e.X, viewport.Height - e.Y)));
            var st = GetStateAtPoint(e.X, viewport.Height - e.Y, viewport);
            if (_state == ClipState.None || st == ClipState.None)
            {
                _state = ClipState.Drawing;
                _drawingPoint = point;
            }
            else if (_state == ClipState.Drawn)
            {
                _state = st;
            }
            Invalidate();
        }

        protected override void MouseUp(MapViewport vp, OrthographicCamera camera, ViewportEvent e)
        {
            var viewport = vp;

            var point = SnapIfNeeded(viewport.Expand(viewport.ScreenToWorld(e.X, viewport.Height - e.Y)));
            if (_state == ClipState.Drawing)
            {
                // Do nothing
                _state = _prevState;
            }
            else
            {
                _state = ClipState.Drawn;
            }

            Editor.Instance.CaptureAltPresses = false;

            Invalidate();
        }

        protected override void MouseMove(MapViewport vp, OrthographicCamera camera, ViewportEvent e)
        {
            var viewport = vp;

            var point = SnapIfNeeded(viewport.Expand(viewport.ScreenToWorld(e.X, viewport.Height - e.Y)));
            var st = GetStateAtPoint(e.X, viewport.Height - e.Y, viewport);
            if (_state == ClipState.Drawing)
            {
                _state = ClipState.MovingPoint2;
                _clipPlanePoint1 = _drawingPoint;
                _clipPlanePoint2 = point;
                _clipPlanePoint3 = _clipPlanePoint1 + SnapIfNeeded(viewport.GetUnusedCoordinate(new Coordinate(128, 128, 128)));
            }
            else if (_state == ClipState.MovingPoint1)
            {
                // Move point 1
                var cp1 = viewport.GetUnusedCoordinate(_clipPlanePoint1) + point;
                if (KeyboardState.Ctrl)
                {
                    var diff = _clipPlanePoint1 - cp1;
                    _clipPlanePoint2 -= diff;
                    _clipPlanePoint3 -= diff;
                }
                _clipPlanePoint1 = cp1;
            }
            else if (_state == ClipState.MovingPoint2)
            {
                // Move point 2
                var cp2 = viewport.GetUnusedCoordinate(_clipPlanePoint2) + point;
                if (KeyboardState.Ctrl)
                {
                    var diff = _clipPlanePoint2 - cp2;
                    _clipPlanePoint1 -= diff;
                    _clipPlanePoint3 -= diff;
                }
                _clipPlanePoint2 = cp2;
            }
            else if (_state == ClipState.MovingPoint3)
            {
                // Move point 3
                var cp3 = viewport.GetUnusedCoordinate(_clipPlanePoint3) + point;
                if (KeyboardState.Ctrl)
                {
                    var diff = _clipPlanePoint3 - cp3;
                    _clipPlanePoint1 -= diff;
                    _clipPlanePoint2 -= diff;
                }
                _clipPlanePoint3 = cp3;
            }

            Editor.Instance.CaptureAltPresses = _state != ClipState.None && _state != ClipState.Drawn;

            if (st != ClipState.None || (_state != ClipState.None && _state != ClipState.Drawn))
            {
                viewport.Control.Cursor = Cursors.Cross;
            }
            else
            {
                viewport.Control.Cursor = Cursors.Default;
            }

            Invalidate();
        }

        public override void KeyDown(MapViewport viewport, ViewportEvent e)
        {
            if (e.KeyCode == Keys.Enter && _state != ClipState.None)
            {
                if (!_clipPlanePoint1.EquivalentTo(_clipPlanePoint2)
                    && !_clipPlanePoint2.EquivalentTo(_clipPlanePoint3)
                    && !_clipPlanePoint1.EquivalentTo(_clipPlanePoint3)) // Don't clip if the points are too close together
                {
                    PerformClip();
                }
            }
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter) // Escape cancels, Enter commits and resets
            {
                _clipPlanePoint1 = _clipPlanePoint2 = _clipPlanePoint3 = _drawingPoint = null;
                _state = _prevState = ClipState.None;
            }

            Invalidate();

            base.KeyDown(viewport, e);
        }

        private void PerformClip()
        {
            var objects = Document.Selection.GetSelectedObjects().OfType<Solid>().ToList();
            var plane = new Plane(_clipPlanePoint1, _clipPlanePoint2, _clipPlanePoint3);
            Document.PerformAction("Perform Clip", new Clip(objects, plane, _side != ClipSide.Back, _side != ClipSide.Front));
        }

        protected override IEnumerable<SceneObject> GetSceneObjects()
        {
            var list = base.GetSceneObjects().ToList();

            if (_state != ClipState.None && _clipPlanePoint1 != null && _clipPlanePoint2 != null && _clipPlanePoint3 != null)
            {
                // Draw the lines
                var p1 = _clipPlanePoint1.ToVector3();
                var p2 = _clipPlanePoint2.ToVector3();
                var p3 = _clipPlanePoint3.ToVector3();

                list.Add(new Line(Color.White, p1, p2, p3, p1));

                if (!_clipPlanePoint1.EquivalentTo(_clipPlanePoint2)
                    && !_clipPlanePoint2.EquivalentTo(_clipPlanePoint3)
                    && !_clipPlanePoint1.EquivalentTo(_clipPlanePoint3)
                    && !Document.Selection.IsEmpty())
                {
                    var plane = new Plane(_clipPlanePoint1, _clipPlanePoint2, _clipPlanePoint3);

                    // Draw the clipped solids
                    var faces = new List<Face>();
                    var idg = new IDGenerator();
                    foreach (var solid in Document.Selection.GetSelectedObjects().OfType<Solid>().ToList())
                    {
                        Solid back, front;
                        if (solid.Split(plane, out back, out front, idg))
                        {
                            if (_side != ClipSide.Front) faces.AddRange(back.Faces);
                            if (_side != ClipSide.Back) faces.AddRange(front.Faces);
                        }
                    }
                    var lines = faces.Select(x => new Line(Color.White, x.Vertices.Select(v => v.Location.ToVector3()).ToArray()) {Width = 2});
                    list.AddRange(lines);

                    // Draw the clipping plane
                    var poly = new Polygon(plane);
                    var bbox = Document.Selection.GetSelectionBoundingBox();
                    var point = bbox.Center;
                    foreach (var boxPlane in bbox.GetBoxPlanes())
                    {
                        var proj = boxPlane.Project(point);
                        var dist = (point - proj).VectorMagnitude() * 0.1m;
                        poly.Split(new Plane(boxPlane.Normal, proj + boxPlane.Normal * Math.Max(dist, 100)));
                    }

                    // Add the face in both directions so it renders on both sides
                    list.Add(new Sledge.Rendering.Scenes.Renderables.Face(Material.Flat(Color.FromArgb(100, Color.Turquoise)),
                        poly.Vertices.Select(x => new Vertex(x.ToVector3(), 0, 0)).ToList()) {CameraFlags = CameraFlags.Perspective});
                    list.Add(new Sledge.Rendering.Scenes.Renderables.Face(Material.Flat(Color.FromArgb(100, Color.Turquoise)),
                        poly.Vertices.Select(x => new Vertex(x.ToVector3(), 0, 0)).Reverse().ToList()) {CameraFlags = CameraFlags.Perspective});
                }
            }

            return list;
        }

        protected override IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            var list = base.GetViewportElements(viewport, camera).ToList();

            if (_state != ClipState.None && _clipPlanePoint1 != null && _clipPlanePoint2 != null && _clipPlanePoint3 != null)
            {
                var p1 = _clipPlanePoint1.ToVector3();
                var p2 = _clipPlanePoint2.ToVector3();
                var p3 = _clipPlanePoint3.ToVector3();

                // Draw the drag handles in 2D only
                list.Add(new HandleElement(PositionType.World, HandleElement.HandleType.Square, new Position(p1), 4));
                list.Add(new HandleElement(PositionType.World, HandleElement.HandleType.Square, new Position(p2), 4));
                list.Add(new HandleElement(PositionType.World, HandleElement.HandleType.Square, new Position(p3), 4));
            }

            return list;
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            switch (hotkeyMessage)
            {
                case HotkeysMediator.OperationsPasteSpecial:
                case HotkeysMediator.OperationsPaste:
                    return HotkeyInterceptResult.SwitchToSelectTool;
                case HotkeysMediator.SwitchTool:
                    if (parameters is HotkeyTool && (HotkeyTool) parameters == GetHotkeyToolType())
                    {
                        CycleClipSide();
                        return HotkeyInterceptResult.Abort;
                    }
                    break;
            }
            return HotkeyInterceptResult.Continue;
        }

        private void CycleClipSide()
        {
            var side = (int) _side;
            side = (side + 1) % (Enum.GetValues(typeof (ClipSide)).Length);
            _side = (ClipSide) side;
            Invalidate();
        }
    }
}
