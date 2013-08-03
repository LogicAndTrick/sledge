using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Editing;
using Sledge.Editor.Properties;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools.VMTools;
using Sledge.Graphics;
using Sledge.Graphics.Helpers;
using Sledge.Settings;
using Sledge.UI;
using Matrix = Sledge.Graphics.Helpers.Matrix;

namespace Sledge.Editor.Tools
{
    public class VMTool : BaseBoxTool
    {
        private readonly VMForm _form;
        private readonly List<VMSubTool> _tools;
        private VMSubTool _currentTool;

        /// <summary>
        /// Key = copy, Value = original
        /// </summary>
        private Dictionary<Solid, Solid> _copies;

        public List<VMPoint> Points { get; private set; }
        public List<VMPoint> MoveSelection { get; private set; }
        public bool Dirty { get; set; }

        private VMPoint _movingPoint;
        private Coordinate _snapPointOffset;

        public VMTool()
        {
            _form = new VMForm();
            _form.ToolSelected += VMToolSelected;
            _tools = new List<VMSubTool>();

            AddTool(new StandardTool(this));
            AddTool(new ScaleTool(this));
            _currentTool = _tools.FirstOrDefault();
        }

        private void VMToolSelected(object sender, VMSubTool tool)
        {
            if (_currentTool == tool) return;
            if (_currentTool != null) _currentTool.ToolDeselected();
            _currentTool = tool;
            if (_currentTool != null) _currentTool.ToolSelected();
        }

        private void AddTool(VMSubTool tool)
        {
            _form.AddTool(tool);
            _tools.Add(tool);
        }

        public override void DocumentChanged()
        {
            _form.Document = Document;
            _tools.ForEach(x => x.SetDocument(Document));
        }

        public override Image GetIcon()
        {
            return Resources.Tool_VM;
        }

        public override string GetName()
        {
            return "Vertex Manipulation Tool";
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.VM;
        }

        protected override Color BoxColour
        {
            get { return Color.Orange; }
        }

        protected override Color FillColour
        {
            get { return Color.FromArgb(Sledge.Settings.View.SelectionBoxBackgroundOpacity, Color.DodgerBlue); }
        }

        /// <summary>
        /// Get the VM points at the provided coordinate, ordered from top to bottom (for the supplied viewport).
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="viewport">The viewport</param>
        /// <returns>The points ordered from top to bottom, or an empty set if no points were found</returns>
        public List<VMPoint> GetVerticesAtPoint(int x, int y, Viewport2D viewport)
        {
            var p = viewport.ScreenToWorld(x, y);
            var d = 5 / viewport.Zoom; // Tolerance value = 5 pixels

            // Order by the unused coordinate in the view (which is the up axis) descending to get the "closest" point
            return (from point in Points
                    let c = viewport.Flatten(point.Coordinate)
                    where p.X >= c.X - d && p.X <= c.X + d && p.Y >= c.Y - d && p.Y <= c.Y + d
                    let unused = viewport.GetUnusedCoordinate(point.Coordinate)
                    orderby unused.X + unused.Y + unused.Z descending
                    select point).ToList();
        }

        public override void ToolSelected()
        {
            _form.Show(Editor.Instance);
            Editor.Instance.Focus();

            var selectedSolids = Document.Selection.GetSelectedObjects().OfType<Solid>().ToList();
            // Init the points and copy caches
            _copies = new Dictionary<Solid, Solid>();
            Points = new List<VMPoint>();
            foreach (var solid in selectedSolids)
            {
                var copy = (Solid)solid.Clone();
                _copies.Add(copy, solid);

                // Set all the original solids to hidden
                // (do this after we clone it so the clones aren't hidden too)
                solid.IsCodeHidden = true;
            }
            RefreshPoints();
            RefreshMidpoints();
            _snapPointOffset = null;
            _movingPoint = null;
            MoveSelection = null;
            Dirty = false;
            Document.UpdateDisplayLists();

            if (_currentTool != null) _currentTool.ToolSelected();
            _form.SelectionChanged();
        }

        public override void ToolDeselected()
        {
            if (_currentTool != null) _currentTool.ToolDeselected();

            // The solids are no longer hidden
            var selectedSolids = Document.Selection.GetSelectedObjects().OfType<Solid>().ToList();
            foreach (var o in selectedSolids)
            {
                o.IsCodeHidden = false;
            }
            if (Dirty)
            {
                // Commit the changes
                var edit = new Edit(_copies.Values, _copies.Keys);
                Document.PerformAction("Vertex Manipulation", edit);
            }
            _copies = null;
            Points = null;
            _snapPointOffset = null;
            _movingPoint = null;
            MoveSelection = null;

            _form.Clear();
            _form.Hide();
            Document.UpdateDisplayLists();
        }

        public void SelectionChanged()
        {
            _currentTool.SelectionChanged();
            _form.SelectionChanged();
        }

        /// <summary>
        /// Updates the points list (does not update midpoints)
        /// </summary>
        public void RefreshPoints()
        {
            var selected = Points.Where(x => !x.IsMidPoint && x.IsSelected).Select(x => new { x.Coordinate, x.Solid }).ToList();
            Points.RemoveAll(x => !x.IsMidPoint);
            foreach (var copy in _copies.Keys)
            {
                // Add the vertex points
                // Group by location per solid, duplicate coordinates are "attached" and moved at the same time
                foreach (var group in copy.Faces.SelectMany(x => x.Vertices).GroupBy(x => x.Location))
                {
                    Points.Add(new VMPoint
                    {
                        Solid = copy, // ten four, solid copy
                        Coordinate = group.First().Location,
                        Vertices = group.ToList(),
                        IsSelected = selected.Any(x => x.Solid == copy && x.Coordinate == group.First().Location)
                    });
                }
            }
        }

        /// <summary>
        /// Updates the positions of all midpoints.
        /// </summary>
        public void RefreshMidpoints(bool recreate = true)
        {
            if (recreate) Points.RemoveAll(x => x.IsMidPoint);
            foreach (var copy in _copies.Keys)
            {
                foreach (var group in copy.Faces.SelectMany(x => x.GetLines()).GroupBy(x => new { x.Start, x.End }))
                {
                    var s = group.Key.Start;
                    var e = group.Key.End;
                    var coord = (s + e) / 2;
                    var mpStart = Points.First(x => !x.IsMidPoint && x.Coordinate == s);
                    var mpEnd = Points.First(x => !x.IsMidPoint && x.Coordinate == e);
                    if (recreate)
                    {
                        Points.Add(new VMPoint
                                        {
                                            Solid = copy,
                                            Coordinate = coord,
                                            IsMidPoint = true,
                                            MidpointStart = mpStart,
                                            MidpointEnd = mpEnd
                                        });
                    }
                    else
                    {
                        foreach (var point in Points.Where(x => x.IsMidPoint && x.MidpointStart.Coordinate == s && x.MidpointEnd.Coordinate == e))
                        {
                            point.Coordinate = coord;
                        }
                    }
                }
            }
        }

        public override void MouseDown(ViewportBase vp, ViewportEvent e)
        {
            if (!(vp is Viewport2D))
            {
                base.MouseDown(vp, e);
                return;
            }

            if (_currentTool == null) return;

            // If the current tool handles the event, we're done
            _currentTool.MouseDown(vp, e);
            if (e.Handled) return;

            var viewport = (Viewport2D)vp;

            // Otherwise we try a selection
            // Find the clicked vertices
            var vtxs = _currentTool.GetVerticesAtPoint(e.X, viewport.Height - e.Y, viewport);

            if (!vtxs.Any())
            {
                // Nothing clicked
                if (!KeyboardState.Ctrl)
                {
                    // Deselect all the points if not ctrl-ing
                    Points.ForEach(x => x.IsSelected = false);
                }
                base.MouseDown(vp, e);
                return;
            }

            // Use the topmost vertex as the control point
            var vtx = vtxs.First();

            // Shift selects only the topmost point
            if (KeyboardState.Shift)
            {
                vtxs.Clear();
                vtxs.Add(vtx);
            }

            // Vertex found, cancel the box if needed
            BoxDrawnCancel(vp);

            // Mouse down on a point
            if (!vtx.IsSelected && !KeyboardState.Ctrl)
            {
                // If we aren't clicking on a selected point and ctrl is not down, deselect the others
                Points.ForEach(x => x.IsSelected = false);
                // If this point is already selected, don't deselect others. This is so we can move multiple points easily.
            }
            vtxs.ForEach(x => x.IsSelected = true);
            SelectionChanged();

            _currentTool.DragStart(vtxs);
            MoveSelection = vtxs;
            _snapPointOffset = SnapIfNeeded(viewport.Expand(viewport.ScreenToWorld(e.X, viewport.Height - e.Y))) - viewport.ZeroUnusedCoordinate(vtx.Coordinate);
            _movingPoint = vtx;
        }

        public override void MouseUp(ViewportBase viewport, ViewportEvent e)
        {
            base.MouseUp(viewport, e);

            if (!(viewport is Viewport2D)) return;
            if (_currentTool == null) return;

            _currentTool.MouseUp(viewport, e);
            if (!e.Handled)
            {
                if (MoveSelection != null && !KeyboardState.Ctrl)
                {
                    // If we were clicking on a point, and the mouse hasn't moved yet,
                    // and ctrl is not down, deselect the other points.
                    Points.ForEach(x => x.IsSelected = false);
                    MoveSelection.ForEach(x => x.IsSelected = true);
                    SelectionChanged();

                    _currentTool.MouseClick(viewport, e);
                }
                else
                {
                    _currentTool.DragEnd();
                }
            }

            RefreshMidpoints();
            _snapPointOffset = null;
            _movingPoint = null;
            MoveSelection = null;
        }

        public override void MouseMove(ViewportBase vp, ViewportEvent e)
        {
            base.MouseMove(vp, e);

            if (!(vp is Viewport2D)) return;
            if (_currentTool == null) return;

            _currentTool.MouseMove(vp, e);
            if (e.Handled) return;

            var viewport = (Viewport2D)vp;

            if (_movingPoint == null)
            {
                // Not moving a point, just test for the cursor.
                var vtxs = _currentTool.GetVerticesAtPoint(e.X, viewport.Height - e.Y, viewport);
                if (vtxs.Any()) viewport.Cursor = Cursors.Cross;
                else if (viewport.Cursor == Cursors.Cross) viewport.Cursor = Cursors.Default;
            }
            else
            {
                // Moving a point, get the delta moved
                var point = SnapIfNeeded(viewport.Expand(viewport.ScreenToWorld(e.X, viewport.Height - e.Y)));
                if (!KeyboardState.Alt && KeyboardState.Shift)
                {
                    // If shift is down, retain the offset the point was at before (relative to the grid)
                    point += _snapPointOffset;
                }
                var moveDistance = point - viewport.ZeroUnusedCoordinate(_movingPoint.Coordinate);
                _currentTool.DragMove(moveDistance);
                RefreshMidpoints(false);
                MoveSelection = null;
            }
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage)
        {
            switch (hotkeyMessage)
            {
                case HotkeysMediator.HistoryUndo:
                case HotkeysMediator.HistoryRedo:
                    MessageBox.Show("Please exit the VM tool to undo any changes.");
                    return HotkeyInterceptResult.Abort;
                case HotkeysMediator.OperationsPaste:
                case HotkeysMediator.OperationsPasteSpecial:
                    return HotkeyInterceptResult.SwitchToSelectTool;
            }
            return HotkeyInterceptResult.Continue;
        }

        public override void BoxDrawnConfirm(ViewportBase viewport)
        {
            Box box;
            if (GetSelectionBox(out box))
            {
                foreach (var point in Points.Where(x => box.CoordinateIsInside(x.Coordinate)))
                {
                    // Select all the points in the box
                    point.IsSelected = true;
                }
                SelectionChanged();
            }

            base.BoxDrawnConfirm(viewport);
        }

        protected override void Render2D(Viewport2D vp)
        {
            base.Render2D(vp);

            if (_currentTool != null) _currentTool.Render2D(vp);

            // Render out the solid previews
            GL.Color3(Color.Pink);
            Matrix.Push();
            var matrix = vp.GetModelViewMatrix();
            GL.MultMatrix(ref matrix);
            DataStructures.Rendering.Rendering.DrawWireframe(_copies.Keys.SelectMany(x => x.Faces), true);
            Matrix.Pop();

            // Draw in order by the unused coordinate (the up axis for this viewport)
            var ordered = (from point in Points
                           let unused = vp.GetUnusedCoordinate(point.Coordinate)
                           orderby unused.X + unused.Y + unused.Z
                           select point).ToList();
            // Render out the point handles
            var z = (double) vp.Zoom;
            GL.Begin(BeginMode.Quads);
            foreach (var point in ordered)
            {
                var c = vp.Flatten(point.Coordinate);
                GL.Color3(Color.Black);
                GLX.Square(new Vector2d(c.DX, c.DY), 4, z, true);
                GL.Color3(point.GetColour());
                GLX.Square(new Vector2d(c.DX, c.DY), 3, z, true);
            }
            GL.End();
        }

        protected override void Render3D(Viewport3D vp)
        {
            base.Render3D(vp);

            if (_currentTool != null) _currentTool.Render3D(vp);

            TextureHelper.DisableTexturing();

            // Get us into 2D rendering
            Matrix.Set(MatrixMode.Projection);
            Matrix.Identity();
            Graphics.Helpers.Viewport.Orthographic(0, 0, vp.Width, vp.Height);
            Matrix.Set(MatrixMode.Modelview);
            Matrix.Identity();

            var half = new Coordinate(vp.Width, vp.Height, 0) / 2;
            // Render out the point handles
            GL.Begin(BeginMode.Quads);
            foreach (var point in Points)
            {
                var c = vp.WorldToScreen(point.Coordinate);
                if (c == null || c.Z > 1) continue;
                c -= half;

                GL.Color3(Color.Black);
                GL.Vertex2(c.DX - 4, c.DY - 4);
                GL.Vertex2(c.DX - 4, c.DY + 4);
                GL.Vertex2(c.DX + 4, c.DY + 4);
                GL.Vertex2(c.DX + 4, c.DY - 4);

                GL.Color3(point.GetColour());
                GL.Vertex2(c.DX - 3, c.DY - 3);
                GL.Vertex2(c.DX - 3, c.DY + 3);
                GL.Vertex2(c.DX + 3, c.DY + 3);
                GL.Vertex2(c.DX + 3, c.DY - 3);
            }
            GL.End();

            // Get back into 3D rendering
            Matrix.Set(MatrixMode.Projection);
            Matrix.Identity();
            Graphics.Helpers.Viewport.Perspective(0, 0, vp.Width, vp.Height, Sledge.Settings.View.CameraFOV);
            Matrix.Set(MatrixMode.Modelview);
            Matrix.Identity();
            vp.Camera.Position();

            TextureHelper.EnableTexturing();

            // Render out the solid previews
            GL.Color3(Color.White);
            var faces = _copies.Keys.SelectMany(x => x.Faces).ToList();
            DataStructures.Rendering.Rendering.DrawFilled(faces, Color.Empty);
            DataStructures.Rendering.Rendering.DrawFilled(faces, Color.FromArgb(64, Color.Green));
            GL.Color3(Color.Pink);
            DataStructures.Rendering.Rendering.DrawWireframe(faces, true);
        }

        public override void KeyDown(ViewportBase viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.KeyDown(viewport, e);
            if (e.Handled) return;
            base.KeyDown(viewport, e);
        }

        public override void Render(ViewportBase viewport)
        {
            if (_currentTool != null) _currentTool.Render(viewport);
            base.Render(viewport);
        }

        public override void MouseEnter(ViewportBase viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.MouseEnter(viewport, e);
            if (e.Handled) return;
            base.MouseEnter(viewport, e);
        }

        public override void MouseLeave(ViewportBase viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.MouseLeave(viewport, e);
            if (e.Handled) return;
            base.MouseLeave(viewport, e);
        }

        public override void MouseWheel(ViewportBase viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.MouseWheel(viewport, e);
            if (e.Handled) return;
            base.MouseWheel(viewport, e);
        }

        public override void KeyPress(ViewportBase viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.KeyPress(viewport, e);
            if (e.Handled) return;
            base.KeyPress(viewport, e);
        }

        public override void KeyUp(ViewportBase viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.KeyUp(viewport, e);
            if (e.Handled) return;
            base.KeyUp(viewport, e);
        }

        public override void UpdateFrame(ViewportBase viewport)
        {
            if (_currentTool != null) _currentTool.UpdateFrame(viewport);
            base.UpdateFrame(viewport);
        }

        public override void PreRender(ViewportBase viewport)
        {
            if (_currentTool != null) _currentTool.PreRender(viewport);
            base.PreRender(viewport);
        }
    }
}
