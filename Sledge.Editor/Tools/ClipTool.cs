using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using Sledge.Editor.Properties;
using Sledge.UI;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Editing;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Rendering;
using KeyPressEventArgs = System.Windows.Forms.KeyPressEventArgs;
using OpenTK.Graphics.OpenGL;
using Sledge.Graphics.Helpers;
using Matrix = Sledge.Graphics.Helpers.Matrix;

namespace Sledge.Editor.Tools
{
    public class ClipTool : BaseBothTool
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

        private Coordinate _clipPlanePoint1;
        private Coordinate _clipPlanePoint2;
        private Coordinate _clipPlanePoint3;
        private Coordinate _drawingPoint;
        private ClipState _prevState;
        private ClipState _state;

        public ClipTool()
        {
            _clipPlanePoint1 = _clipPlanePoint2 = _clipPlanePoint3 = _drawingPoint = null;
            _state = _prevState = ClipState.None;
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Clip;
        }

        public override string GetName()
        {
            return "Clip Tool";
        }

        protected static Coordinate SnapIfNeeded(Coordinate c)
        {
            return KeyboardState.Alt ? c : c.Snap(Document.GridSpacing);
        }

        private ClipState GetStateAtPoint(int x, int y, Viewport2D viewport)
        {
            if (_clipPlanePoint1 == null || _clipPlanePoint2 == null || _clipPlanePoint3 == null) return ClipState.None;

            var p = viewport.ScreenToWorld(x, y);
            var p1 = viewport.Flatten(_clipPlanePoint1);
            var p2 = viewport.Flatten(_clipPlanePoint2);
            var p3 = viewport.Flatten(_clipPlanePoint3);

            var d = 5 / viewport.Zoom;

            if (p.X >= p1.X - d && p.X <= p1.X + d && p.Y >= p1.Y - d && p.Y <= p1.Y + d) return ClipState.MovingPoint1;
            if (p.X >= p2.X - d && p.X <= p2.X + d && p.Y >= p2.Y - d && p.Y <= p2.Y + d) return ClipState.MovingPoint2;
            if (p.X >= p3.X - d && p.X <= p3.X + d && p.Y >= p3.Y - d && p.Y <= p3.Y + d) return ClipState.MovingPoint3;

            return ClipState.None;
        }

        public override void MouseDown(ViewportBase vp, MouseEventArgs e)
        {
            if (!(vp is Viewport2D)) return;
            var viewport = (Viewport2D) vp;
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
        }

        public override void MouseUp(ViewportBase vp, MouseEventArgs e)
        {
            if (!(vp is Viewport2D)) return;
            var viewport = (Viewport2D)vp;

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

            Document.CaptureAltPresses = false;
        }

        public override void MouseMove(ViewportBase vp, MouseEventArgs e)
        {
            if (!(vp is Viewport2D)) return;
            var viewport = (Viewport2D)vp;

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
                _clipPlanePoint1 = viewport.GetUnusedCoordinate(_clipPlanePoint1) + point;
            }
            else if (_state == ClipState.MovingPoint2)
            {
                // Move point 2
                _clipPlanePoint2 = viewport.GetUnusedCoordinate(_clipPlanePoint2) + point;
            }
            else if (_state == ClipState.MovingPoint3)
            {
                // Move point 3
                _clipPlanePoint3 = viewport.GetUnusedCoordinate(_clipPlanePoint3) + point;
            }

            Document.CaptureAltPresses = _state != ClipState.None && _state != ClipState.Drawn;

            if (st != ClipState.None || (_state != ClipState.None && _state != ClipState.Drawn))
            {
                viewport.Cursor = Cursors.Cross;
            }
            else
            {
                viewport.Cursor = Cursors.Default;
            }
        }

        public override void KeyPress(ViewportBase viewport, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) // Enter
            {
                if (!_clipPlanePoint1.EquivalentTo(_clipPlanePoint2)
                    && !_clipPlanePoint2.EquivalentTo(_clipPlanePoint3)
                    && !_clipPlanePoint1.EquivalentTo(_clipPlanePoint3)) // Don't clip if the points are too close together
                {
                    var plane = new Plane(_clipPlanePoint1, _clipPlanePoint2, _clipPlanePoint3);
                    foreach (var solid in Selection.GetSelectedObjects().OfType<Solid>().ToList())
                    {
                        // Split solid by plane
                        Solid back, front;
                        if (solid.Split(plane, out back, out front))
                        {
                            var parent = solid.Parent;
                            Selection.Deselect(solid);
                            parent.Children.Remove(solid);

                            back.UpdateBoundingBox(false);
                            front.UpdateBoundingBox();

                            parent.Children.Add(back);
                            parent.Children.Add(front);

                            Selection.Select(back);
                            Selection.Select(front);
                        }
                    }
                    Document.UpdateDisplayLists();
                }
            }
            if (e.KeyChar == 27 || e.KeyChar == 13) // Escape cancels, Enter commits and resets
            {
                _clipPlanePoint1 = _clipPlanePoint2 = _clipPlanePoint3 = _drawingPoint = null;
                _state = _prevState = ClipState.None;
            }
        }

        public override void Render(ViewportBase viewport)
        {
            if (viewport is Viewport2D) Render2D((Viewport2D) viewport);
            if (viewport is Viewport3D) Render3D((Viewport3D) viewport);
        }

        private void Render2D(Viewport2D vp)
        {
            if (_state == ClipState.None
                || _clipPlanePoint1 == null
                || _clipPlanePoint2 == null
                || _clipPlanePoint3 == null) return; // Nothing to draw at this point

            var z = (double) vp.Zoom;
            var p1 = vp.Flatten(_clipPlanePoint1);
            var p2 = vp.Flatten(_clipPlanePoint2);
            var p3 = vp.Flatten(_clipPlanePoint3);
            // Draw points
            GL.Begin(BeginMode.Quads);
            GL.Color3(Color.White);
            GLX.Square(new Vector2d(p1.DX, p1.DY), 4, z, true);
            GLX.Square(new Vector2d(p2.DX, p2.DY), 4, z, true);
            GLX.Square(new Vector2d(p3.DX, p3.DY), 4, z, true);
            GL.End();

            GL.Enable(EnableCap.LineSmooth);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);

            // Draw lines between points and point outlines
            GL.Begin(BeginMode.Lines);
            GL.Color3(Color.White);
            GL.Vertex2(p1.DX, p1.DY);
            GL.Vertex2(p2.DX, p2.DY);
            GL.Vertex2(p2.DX, p2.DY);
            GL.Vertex2(p3.DX, p3.DY);
            GL.Vertex2(p3.DX, p3.DY);
            GL.Vertex2(p1.DX, p1.DY);
            GL.Color3(Color.Black);
            GLX.Square(new Vector2d(p1.DX, p1.DY), 4, z);
            GLX.Square(new Vector2d(p2.DX, p2.DY), 4, z);
            GLX.Square(new Vector2d(p3.DX, p3.DY), 4, z);
            GL.End();

            // Draw the clipped brushes
            if (!_clipPlanePoint1.EquivalentTo(_clipPlanePoint2)
                    && !_clipPlanePoint2.EquivalentTo(_clipPlanePoint3)
                    && !_clipPlanePoint1.EquivalentTo(_clipPlanePoint3))
            {
                var plane = new Plane(_clipPlanePoint1, _clipPlanePoint2, _clipPlanePoint3);
                var faces = new List<Face>();
                foreach (var solid in Selection.GetSelectedObjects().OfType<Solid>().ToList())
                {
                    Solid back, front;
                    if (solid.Split(plane, out back, out front))
                    {
                        faces.AddRange(back.Faces);
                        faces.AddRange(front.Faces);
                    }
                }
                GL.LineWidth(2);
                GL.Color3(Color.White);
                Matrix.Push();
                var mat = DisplayListGroup.GetMatrixFor(vp.Direction);
                GL.MultMatrix(ref mat);
                DataStructures.Rendering.Rendering.DrawWireframe(faces, true);
                Matrix.Pop();
                GL.LineWidth(1);
            }

            GL.Hint(HintTarget.LineSmoothHint, HintMode.Fastest);
            GL.Disable(EnableCap.LineSmooth);

        }

        private void Render3D(Viewport3D vp)
        {
            if (_state == ClipState.None
                || _clipPlanePoint1 == null
                || _clipPlanePoint2 == null
                || _clipPlanePoint3 == null) return; // Nothing to draw at this point

            TextureHelper.DisableTexturing();

            // Draw points

            if (!_clipPlanePoint1.EquivalentTo(_clipPlanePoint2)
                    && !_clipPlanePoint2.EquivalentTo(_clipPlanePoint3)
                    && !_clipPlanePoint1.EquivalentTo(_clipPlanePoint3))
            {
                var plane = new Plane(_clipPlanePoint1, _clipPlanePoint2, _clipPlanePoint3);

                // Draw clipped solids
                GL.Enable(EnableCap.LineSmooth);
                GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);

                var faces = new List<Face>();
                foreach (var solid in Selection.GetSelectedObjects().OfType<Solid>().ToList())
                {
                    Solid back, front;
                    if (solid.Split(plane, out back, out front))
                    {
                        faces.AddRange(back.Faces);
                        faces.AddRange(front.Faces);
                    }
                }
                GL.LineWidth(2);
                GL.Color3(Color.White);
                DataStructures.Rendering.Rendering.DrawWireframe(faces, true);
                GL.LineWidth(1);

                GL.Hint(HintTarget.LineSmoothHint, HintMode.Fastest);
                GL.Disable(EnableCap.LineSmooth);

                // Draw the clipping plane
                var u = plane.Normal.Cross(plane.GetClosestAxisToNormal() == Coordinate.UnitZ ? Coordinate.UnitX : Coordinate.UnitZ);
                var v = plane.Normal.Cross(u);
                var point = (_clipPlanePoint1 + _clipPlanePoint2 + _clipPlanePoint3) / 3;
                var dx = u * 10000;
                var dy = v * 10000;
                GL.Disable(EnableCap.CullFace);
                GL.Begin(BeginMode.Quads);
                GL.Color4(Color.FromArgb(100, Color.Turquoise));
                Action<Coordinate> render = c => GL.Vertex3(c.DX, c.DY, c.DZ);
                render(point - dx - dy);
                render(point + dx - dy);
                render(point + dx + dy);
                render(point - dx + dy);
                GL.End();

                GL.Enable(EnableCap.CullFace);
            }

            TextureHelper.EnableTexturing();
        }

        public override void MouseEnter(ViewportBase viewport, EventArgs e)
        {
            //
        }

        public override void MouseLeave(ViewportBase viewport, EventArgs e)
        {
            //
        }

        public override void MouseWheel(ViewportBase viewport, MouseEventArgs e)
        {
            //
        }

        public override void KeyDown(ViewportBase viewport, KeyEventArgs e)
        {
            //
        }

        public override void KeyUp(ViewportBase viewport, KeyEventArgs e)
        {
            //
        }

        public override void UpdateFrame(ViewportBase viewport)
        {
            //
        }
    }
}
