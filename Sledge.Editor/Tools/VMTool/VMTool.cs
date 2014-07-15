using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Actions.MapObjects.Selection;
using Sledge.Editor.Properties;
using Sledge.Editor.Rendering.Immediate;
using Sledge.Graphics;
using Sledge.Graphics.Helpers;
using Sledge.Settings;
using Sledge.UI;
using Matrix = Sledge.Graphics.Helpers.Matrix;
using Select = Sledge.Settings.Select;
using View = Sledge.Settings.View;

namespace Sledge.Editor.Tools.VMTool
{
    public class VMTool : BaseBoxTool
    {
        private enum ShowPoints
        {
            All,
            Vertices,
            Midpoints
        }

        //private readonly VMForm _form;
        private readonly VMSidebarPanel _controlPanel;
        private readonly VMErrorsSidebarPanel _errorPanel;
        private readonly List<VMSubTool> _tools;
        private VMSubTool _currentTool;

        /// <summary>
        /// Key = copy, Value = original
        /// </summary>
        private Dictionary<Solid, Solid> _copies;

        public List<VMPoint> Points { get; private set; }
        public List<VMPoint> MoveSelection { get; private set; }

        private VMPoint _movingPoint;
        private Coordinate _snapPointOffset;
        private bool _dirty;

        private ShowPoints _showPoints;

        public VMTool()
        {
            _controlPanel = new VMSidebarPanel();
            _controlPanel.ToolSelected += VMToolSelected;
            _controlPanel.DeselectAll += DeselectAll;
            _controlPanel.Reset += Reset;

            _errorPanel = new VMErrorsSidebarPanel();
            _errorPanel.SelectError += SelectError;
            _errorPanel.FixError += FixError;
            _errorPanel.FixAllErrors += FixAllErrors;

            _tools = new List<VMSubTool>();

            AddTool(new StandardTool(this));
            AddTool(new ScaleTool(this));
            AddTool(new EditFaceTool(this));
            _currentTool = _tools.FirstOrDefault();

            _showPoints = ShowPoints.All;
        }

        private void SelectError(object sender, VMError error)
        {
            if (error != null)
            {
                VMToolSelected(null, _tools.First(x => x is StandardTool));
                Points.ForEach(x => x.IsSelected = x.Vertices != null && error.Vertices.Any(y => x.Vertices.Contains(y)));
                foreach (var f in _copies.SelectMany(x => x.Key.Faces))
                {
                    f.IsSelected = error.Faces.Contains(f);
                }
            }
            else
            {
                foreach (var f in _copies.SelectMany(x => x.Key.Faces))
                {
                    f.IsSelected = false;
                }
            }
        }

        private void FixError(object sender, object error)
        {

        }

        private void FixAllErrors(object sender)
        {

        }

        private void Reset(object sender)
        {
            _dirty = false;
            Commit(_copies.Values.ToList());
            _copies.Clear();
            SelectionChanged();
        }

        private void DeselectAll(object sender)
        {
            Points.ForEach(x => x.IsSelected = false);
            VertexSelectionChanged();
        }

        private void VMToolSelected(object sender, VMSubTool tool)
        {
            if (_currentTool == tool) return;
            _controlPanel.SetSelectedTool(tool);
            if (_currentTool != null) _currentTool.ToolDeselected(false);
            _currentTool = tool;
            if (_currentTool != null) _currentTool.ToolSelected(false);

            Mediator.Publish(EditorMediator.ContextualHelpChanged);
        }

        private void AddTool(VMSubTool tool)
        {
            _controlPanel.AddTool(tool);
            _tools.Add(tool);
        }

        private void VMStandardMode()
        {
            VMToolSelected(this, _tools.First(x => x is StandardTool));
        }

        private void VMScalingMode()
        {
            VMToolSelected(this, _tools.First(x => x is ScaleTool));
        }

        private void VMFaceEditMode()
        {
            VMToolSelected(this, _tools.First(x => x is EditFaceTool));
        }

        public override void DocumentChanged()
        {
            _controlPanel.Document = Document;
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

        public override IEnumerable<KeyValuePair<string, Control>> GetSidebarControls()
        {
            yield return new KeyValuePair<string, Control>(GetName(), _controlPanel);
            yield return new KeyValuePair<string, Control>("VM Errors", _errorPanel);
        }

        public override string GetContextualHelp()
        {
            if (_currentTool != null) return _currentTool.GetContextualHelp();
            return "Select a VM mode for more information";
        }

        protected override Color BoxColour
        {
            get { return Color.Orange; }
        }

        protected override Color FillColour
        {
            get { return Color.FromArgb(View.SelectionBoxBackgroundOpacity, Color.DodgerBlue); }
        }

        public void SetDirty(bool points, bool midpoints)
        {
            UpdateEditedFaces();
            if (points) RefreshPoints();
            if (midpoints) RefreshMidpoints();
            _errorPanel.SetErrorList(GetErrors());
            _dirty = true;
        }

        public IEnumerable<VMError> GetErrors()
        {
            foreach (var kv in _copies)
            {
                var s = kv.Key;
                foreach (var g in s.GetCoplanarFaces().GroupBy(x => x.Plane))
                {
                    yield return new VMError("Coplanar faces", s, g);
                }
                foreach (var f in s.GetBackwardsFaces(0.5m))
                {
                    yield return new VMError("Backwards face", s, new[] { f });
                }
                foreach (var f in s.Faces)
                {
                    var np = f.GetNonPlanarVertices(0.5m).ToList();
                    var found = false;
                    if (np.Any())
                    {
                        yield return new VMError("Nonplanar vertex", s, new[] { f }, np);
                        found = true;
                    }
                    foreach (var g in f.Vertices.GroupBy(x => x.Location).Where(x => x.Count() > 1))
                    {
                        yield return new VMError("Overlapping vertices", s, new[] { f }, g);
                        found = true;
                    }
                    if (!f.IsConvex() && !found)
                    {
                        yield return new VMError("Concave face", s, new[] { f });
                    }
                }
            }
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

        public List<VMPoint> GetVerticesAtPoint(int x, int y, Viewport3D viewport)
        {
            var l = viewport.Camera.Location;
            var pos = new Coordinate((decimal) l.X, (decimal) l.Y, (decimal) l.Z);
            var p = new Coordinate(x, y, 0);
            const int d = 5;
            return (from point in Points
                    let c = viewport.WorldToScreen(point.Coordinate)
                    where c != null && c.Z <= 1
                    where p.X >= c.X - d && p.X <= c.X + d && p.Y >= c.Y - d && p.Y <= c.Y + d
                    orderby (pos - point.Coordinate).LengthSquared()
                    select point).ToList();
        }

        public IEnumerable<Solid> GetCopies()
        {
            return _copies.Keys;
        }

        public IEnumerable<Solid> GetOriginals()
        {
            return _copies.Values;
        }

        public List<VMPoint> GetSelectedPoints()
        {
            var list = new List<VMPoint>();
            foreach (var point in Points.Where(point => point.IsSelected))
            {
                if (point.IsMidPoint && _showPoints != ShowPoints.Vertices)
                {
                    if (!list.Contains(point.MidpointStart)) list.Add(point.MidpointStart);
                    if (!list.Contains(point.MidpointEnd)) list.Add(point.MidpointEnd);
                }
                else if (!point.IsMidPoint && _showPoints != ShowPoints.Midpoints)
                {
                    if (!list.Contains(point)) list.Add(point);
                }
            }
            return list;
        }

        private void Commit(IList<Solid> solids)
        {
            if (!solids.Any()) return;

            // Unhide the solids
            foreach (var solid in solids)
            {
                solid.IsCodeHidden = false;
            }
            var kvs = _copies.Where(x => solids.Contains(x.Value)).ToList();
            foreach (var kv in kvs)
            {
                _copies.Remove(kv.Key);
                foreach (var f in kv.Key.Faces) f.IsSelected = false;
                foreach (var f in kv.Value.Faces) f.IsSelected = false;
            }
            if (_dirty)
            {
                // Commit the changes
                var edit = new ReplaceObjects(kvs.Select(x => x.Value), kvs.Select(x => x.Key));
                Document.PerformAction("Vertex Manipulation", edit);
            }
        }

        private void SelectionChanged()
        {
            var selectedSolids = Document.Selection.GetSelectedObjects().OfType<Solid>().ToList();
            var commit = _copies.Values.Where(x => !selectedSolids.Contains(x)).ToList();
            Commit(commit);
            if (!_copies.Any()) _dirty = false;
            foreach (var solid in selectedSolids.Where(x => !_copies.ContainsValue(x)))
            {
                var copy = (Solid)solid.Clone();
                copy.IsSelected = false;
                foreach (var f in copy.Faces) f.IsSelected = false;
                _copies.Add(copy, solid);

                // Set all the original solids to hidden
                // (do this after we clone it so the clones aren't hidden too)
                solid.IsCodeHidden = true;
            }
            RefreshPoints();
            RefreshMidpoints();
        }

        public override void ToolSelected(bool preventHistory)
        {
            Editor.Instance.Focus();

            // Init the points and copy caches
            _copies = new Dictionary<Solid, Solid>();
            Points = new List<VMPoint>();

            SelectionChanged();

            _snapPointOffset = null;
            _movingPoint = null;
            MoveSelection = null;

            if (_currentTool != null) _currentTool.ToolSelected(preventHistory);

            Mediator.Subscribe(EditorMediator.SelectionChanged, this);
            Mediator.Subscribe(HotkeysMediator.VMStandardMode, this);
            Mediator.Subscribe(HotkeysMediator.VMScalingMode, this);
            Mediator.Subscribe(HotkeysMediator.VMFaceEditMode, this);
        }

        public override void ToolDeselected(bool preventHistory)
        {
            Mediator.UnsubscribeAll(this);

            if (_currentTool != null) _currentTool.ToolDeselected(preventHistory);

            // Commit the changes
            Commit(_copies.Values.ToList());

            _copies = null;
            Points = null;
            _snapPointOffset = null;
            _movingPoint = null;
            MoveSelection = null;
        }

        private void VertexSelectionChanged()
        {
            _currentTool.SelectionChanged();
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
                foreach (var group in copy.Faces.SelectMany(x => x.Vertices).GroupBy(x => x.Location.Round(2)))
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
        private void RefreshMidpoints(bool recreate = true)
        {
            var selected = Points.Where(x => x.IsMidPoint && x.IsSelected).Select(x => new { Start = x.MidpointStart.Coordinate, End = x.MidpointEnd.Coordinate, x.Solid }).ToList();
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
                    var existingPoints = Points.Where(x => x.IsMidPointFor(mpStart, mpEnd)).ToList();
                    if (recreate && !existingPoints.Any())
                    {
                        Points.Add(new VMPoint
                                        {
                                            Solid = copy,
                                            Coordinate = coord,
                                            IsMidPoint = true,
                                            MidpointStart = mpStart,
                                            MidpointEnd = mpEnd,
                                            IsSelected = selected.Any(x => x.Solid == copy && x.Start == mpStart.Coordinate && x.End == mpEnd.Coordinate)
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

        private void UpdateMidpoints()
        {
            foreach (var mp in Points.Where(x => x.IsMidPoint))
            {
                mp.Coordinate = (mp.MidpointStart.Coordinate + mp.MidpointEnd.Coordinate) / 2;
            }
        }

        public void UpdateEditedFaces()
        {
            foreach (var face in GetCopies().SelectMany(x => x.Faces))
            {
                if (face.Vertices.Count >= 3) face.Plane = new Plane(face.Vertices[0].Location, face.Vertices[1].Location, face.Vertices[2].Location);
                face.CalculateTextureCoordinates(true);
                face.UpdateBoundingBox();
            }
        }

        private Coordinate GetIntersectionPoint(MapObject obj, Line line)
        {
            if (obj == null) return null;

            var solid = obj as Solid;
            if (solid == null) return obj.GetIntersectionPoint(line);

            return solid.Faces.Where(x => x.Opacity > 0 && !x.IsHidden)
                .Select(x => x.GetIntersectionPoint(line))
                .Where(x => x != null)
                .OrderBy(x => (x - line.Start).VectorMagnitude())
                .FirstOrDefault();
        }

        private void MouseDown(Viewport3D vp, ViewportEvent e)
        {
            if (!_currentTool.NoSelection())
            {
                var vtxs = _currentTool.GetVerticesAtPoint(e.X, vp.Height - e.Y, vp);

                if (vtxs.Any())
                {
                    // Use the topmost vertex as the control point
                    var vtx = vtxs.First();

                    // Mouse down on a point
                    if (vtx.IsSelected && KeyboardState.Ctrl && _currentTool.ShouldDeselect(vtxs))
                    {
                        // If the vertex is selected and ctrl is down, deselect the vertices
                        vtxs.ForEach(x => x.IsSelected = false);
                    }
                    else
                    {
                        if (!vtx.IsSelected && !KeyboardState.Ctrl && _currentTool.ShouldDeselect(vtxs))
                        {
                            // If we aren't clicking on a selected point and ctrl is not down, deselect the others
                            Points.ForEach(x => x.IsSelected = false);
                            // If this point is already selected, don't deselect others. This is the same behaviour as 2D selection.
                        }
                        vtxs.ForEach(x => x.IsSelected = true);
                    }
                    VertexSelectionChanged();

                    // Don't do other click operations
                    return;
                }

                // Nothing clicked
                if (!KeyboardState.Ctrl)
                {
                    // Deselect all the points if not ctrl-ing
                    Points.ForEach(x => x.IsSelected = false);
                }
            }
            if (!_currentTool.No3DSelection())
            {
                // Do selection
                var ray = vp.CastRayFromScreen(e.X, e.Y);
                var hits = Document.Map.WorldSpawn.GetAllNodesIntersectingWith(ray, true);
                var solid = hits
                    .OfType<Solid>()
                    .Select(x => new { Item = x, Intersection = GetIntersectionPoint(x, ray) })
                    .Where(x => x.Intersection != null)
                    .OrderBy(x => (x.Intersection - ray.Start).VectorMagnitude())
                    .Select(x => x.Item)
                    .FirstOrDefault();

                if (solid != null)
                {
                    if (solid.IsSelected && KeyboardState.Ctrl)
                    {
                        // deselect solid
                        var select = new MapObject[0];
                        var deselect = new[] {solid};
                        Document.PerformAction("Deselect VM solid", new ChangeSelection(select, deselect));
                    }
                    else if (!solid.IsSelected)
                    {
                        // select solid
                        var select = new[] {solid};
                        var deselect = !KeyboardState.Ctrl ? Document.Selection.GetSelectedObjects() : new MapObject[0];
                        Document.PerformAction("Select VM solid", new ChangeSelection(select, deselect));
                    }

                    // Don't do other click operations
                    return;
                }
            }

            base.MouseDown(vp, e);
        }

        private bool _clickSelectionDone = false;
        public override void MouseDown(ViewportBase vp, ViewportEvent e)
        {
            _clickSelectionDone = false;
            if (_currentTool != null)
            {
                // If the current tool handles the event, we're done
                _currentTool.MouseDown(vp, e);
                if (e.Handled) return;
            }
            if (!(vp is Viewport2D))
            {
                MouseDown((Viewport3D)vp, e);
                return;
            }

            if (_currentTool == null) return;

            if (_currentTool.NoSelection()) return;

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

                // Try to select in 2D

                // Create a box to represent the click, with a tolerance level
                var unused = viewport.GetUnusedCoordinate(new Coordinate(100000, 100000, 100000));
                var tolerance = 4 / viewport.Zoom; // Selection tolerance of four pixels
                var used = viewport.Expand(new Coordinate(tolerance, tolerance, 0));
                var add = used + unused;
                var click = viewport.Expand(viewport.ScreenToWorld(e.X, viewport.Height - e.Y));
                var box = new Box(click - add, click + add);

                var centerHandles = Select.DrawCenterHandles;
                var centerOnly = Select.ClickSelectByCenterHandlesOnly;
                // Get the first element that intersects with the box
                var solid = Document.Map.WorldSpawn.GetAllNodesIntersecting2DLineTest(box, centerHandles, centerOnly).OfType<Solid>().FirstOrDefault();

                if (solid != null)
                {
                    // select solid
                    var select = new[] { solid };
                    var deselect = !KeyboardState.Ctrl ? Document.Selection.GetSelectedObjects() : new MapObject[0];
                    Document.PerformAction("Select VM solid", new ChangeSelection(select, deselect));

                    // Don't do other click operations
                    return;
                }

                base.MouseDown(vp, e);
                return;
            }

            var vtx = vtxs.First();

            // When clicking, only select vertices in a single solid
            vtxs = vtxs.Where(x => x.Solid == vtx.Solid).ToList();

            // If any vertices are selected, don't change the selection yet
            if (!vtxs.Any(x => x.IsSelected))
            {
                _clickSelectionDone = true;
                DoSelection(vtxs, viewport);
            }

            // Only move selected vertices
            vtxs = vtxs.Where(x => x.IsSelected).ToList();
            if (!vtxs.Any()) return;

            // Use the fist vertex as the control point
            _currentTool.DragStart(vtxs);
            MoveSelection = vtxs;
            _snapPointOffset = SnapIfNeeded(viewport.Expand(viewport.ScreenToWorld(e.X, viewport.Height - e.Y))) - viewport.ZeroUnusedCoordinate(vtx.Coordinate);
            _movingPoint = vtx;
        }

        public override void MouseClick(ViewportBase viewport, ViewportEvent e)
        {
            var vp = viewport as Viewport2D;
            if (vp == null || _clickSelectionDone) return;

            var vtxs = _currentTool.GetVerticesAtPoint(e.X, viewport.Height - e.Y, vp);
            DoSelection(vtxs, vp);
        }

        private void DoSelection(List<VMPoint> vertices, Viewport2D vp)
        {
            if (!vertices.Any()) return;

            var vtx = vertices.First();
            // Shift selects only the topmost point
            if (KeyboardState.Shift)
            {
                vertices.Clear();
                vertices.Add(vtx);
            }

            // Vertex found, cancel the box if needed
            BoxDrawnCancel(vp);

            // Mouse down on a point
            if (vtx.IsSelected && KeyboardState.Ctrl && _currentTool.ShouldDeselect(vertices))
            {
                // If the vertex is selected and ctrl is down, deselect the vertices
                vertices.ForEach(x => x.IsSelected = false);
            }
            else
            {
                if (!vtx.IsSelected && !KeyboardState.Ctrl && _currentTool.ShouldDeselect(vertices))
                {
                    // If we aren't clicking on a selected point and ctrl is not down, deselect the others
                    Points.ForEach(x => x.IsSelected = false);
                    // If this point is already selected, don't deselect others. This is so we can move multiple points easily.
                }
                vertices.ForEach(x => x.IsSelected = true);
            }
            VertexSelectionChanged();
        }

        public override void MouseDoubleClick(ViewportBase viewport, ViewportEvent e)
        {
            // Not used
        }

        public override void MouseUp(ViewportBase viewport, ViewportEvent e)
        {
            base.MouseUp(viewport, e);

            if (_currentTool == null) return;
            _currentTool.MouseUp(viewport, e);

            if (!(viewport is Viewport2D)) return;
            if (_currentTool.NoSelection()) return;

            if (!e.Handled)
            {
                if (MoveSelection != null && !KeyboardState.Ctrl)
                {
                    // If we were clicking on a point, and the mouse hasn't moved yet,
                    // and ctrl is not down, deselect the other points.
                    Points.ForEach(x => x.IsSelected = false);
                    MoveSelection.ForEach(x => x.IsSelected = true);
                    VertexSelectionChanged();

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

        protected override void LeftMouseUpDrawing(Viewport2D viewport, ViewportEvent e)
        {
            base.LeftMouseUpDrawing(viewport, e);
            if (Select.AutoSelectBox)
            {
                BoxDrawnConfirm(viewport);
            }
        }

        public override void MouseMove(ViewportBase vp, ViewportEvent e)
        {
            base.MouseMove(vp, e);

            if (_currentTool == null) return;

            _currentTool.MouseMove(vp, e);
            if (e.Handled) return;

            if (!(vp is Viewport2D)) return;
            if (_currentTool.NoSelection()) return;

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
                //RefreshMidpoints(false);
                UpdateMidpoints();
                MoveSelection = null;
            }
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
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
                case HotkeysMediator.SwitchTool:
                    if (parameters is HotkeyTool && (HotkeyTool)parameters == GetHotkeyToolType())
                    {
                        CycleShowPoints();
                        return HotkeyInterceptResult.Abort;
                    }
                    break;
            }
            return HotkeyInterceptResult.Continue;
        }

        private void CycleShowPoints()
        {
            var side = (int)_showPoints;
            side = (side + 1) % (Enum.GetValues(typeof(ShowPoints)).Length);
            _showPoints = (ShowPoints)side;
        }

        public override void BoxDrawnConfirm(ViewportBase viewport)
        {
            Box box;
            if (GetSelectionBox(out box))
            {
                foreach (var point in Points.Where(x => !x.IsMidPoint && box.CoordinateIsInside(x.Coordinate)))
                {
                    // Select all the points in the box
                    point.IsSelected = true;
                }
                VertexSelectionChanged();
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
            MapObjectRenderer.DrawWireframe(_copies.Keys.SelectMany(x => x.Faces), true, false);
            Matrix.Pop();

            // Draw in order by the unused coordinate (the up axis for this viewport)
            var ordered = (from point in Points
                           where (point.IsMidPoint && _showPoints != ShowPoints.Vertices) || (!point.IsMidPoint && _showPoints != ShowPoints.Midpoints)
                           let unused = vp.GetUnusedCoordinate(point.Coordinate)
                           orderby point.IsSelected, unused.X + unused.Y + unused.Z
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

            TextureHelper.Unbind();

            if (_currentTool == null || _currentTool.DrawVertices())
            {
                // Get us into 2D rendering
                Matrix.Set(MatrixMode.Projection);
                Matrix.Identity();
                Graphics.Helpers.Viewport.Orthographic(0, 0, vp.Width, vp.Height);
                Matrix.Set(MatrixMode.Modelview);
                Matrix.Identity();

                var half = new Coordinate(vp.Width, vp.Height, 0) / 2;
                // Render out the point handles
                GL.Begin(PrimitiveType.Quads);
                foreach (var point in Points)
                {
                    if (point.IsMidPoint && _showPoints == ShowPoints.Vertices) continue;
                    if (!point.IsMidPoint && _showPoints == ShowPoints.Midpoints) continue;

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
                Graphics.Helpers.Viewport.Perspective(0, 0, vp.Width, vp.Height, View.CameraFOV);
                Matrix.Set(MatrixMode.Modelview);
                Matrix.Identity();
                vp.Camera.Position();
            }

            var type = vp.Type;
            bool shaded = type == Viewport3D.ViewType.Shaded || type == Viewport3D.ViewType.Textured,
                 textured = type == Viewport3D.ViewType.Textured,
                 wireframe = type == Viewport3D.ViewType.Wireframe;

            // Render out the solid previews
            GL.Color3(Color.White);
            var faces = _copies.Keys.SelectMany(x => x.Faces).ToList();

            if (!wireframe)
            {
                if (shaded) MapObjectRenderer.EnableLighting();
                GL.Enable(EnableCap.Texture2D);
                MapObjectRenderer.DrawFilled(faces.Where(x => !x.IsSelected), Color.FromArgb(255, 64, 192, 64), textured);
                MapObjectRenderer.DrawFilled(faces.Where(x => x.IsSelected), Color.FromArgb(255, 255, 128, 128), textured);
                GL.Disable(EnableCap.Texture2D);
                MapObjectRenderer.DisableLighting();

                GL.Color3(Color.Pink);
                MapObjectRenderer.DrawWireframe(faces, true, false);
            }
            else
            {
                GL.Color4(Color.FromArgb(255, 64, 192, 64));
                MapObjectRenderer.DrawWireframe(faces.Where(x => !x.IsSelected), true, false);
                GL.Color4(Color.FromArgb(255, 255, 128, 128));
                MapObjectRenderer.DrawWireframe(faces.Where(x => x.IsSelected), true, false);
            }
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

        public override void UpdateFrame(ViewportBase viewport, FrameInfo frame)
        {
            if (_currentTool != null) _currentTool.UpdateFrame(viewport, frame);
            base.UpdateFrame(viewport, frame);
        }

        public override void PreRender(ViewportBase viewport)
        {
            if (_currentTool != null) _currentTool.PreRender(viewport);
            base.PreRender(viewport);
        }
    }
}
