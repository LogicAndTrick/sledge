using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.Vertex.Controls;
using Sledge.BspEditor.Tools.Vertex.Selection;
using Sledge.Common;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;
using Sledge.Shell.Input;

namespace Sledge.BspEditor.Tools.Vertex.Tools
{
    [AutoTranslate]
    [Export(typeof(VertexSubtool))]
    public class VertexPointTool : VertexSubtool
    {
        [Import] private VertexPointControl _control;

        public enum VisiblePoints
        {
            All,
            Vertices,
            Midpoints
        }

        public override string OrderHint => "A";
        public override string GetName() => "Point manipulation";
        public override Control Control => _control;

        private readonly BoxDraggableState _boxState;
        private readonly Dictionary<VertexSolid, VertexList> _vertices;

        private VisiblePoints _showPoints;

        public VertexPointTool()
        {
            _vertices = new Dictionary<VertexSolid, VertexList>();
            
            States.Add(new WrapperDraggableState(GetDraggables));

            _boxState = new BoxDraggableState(this);
            _boxState.BoxColour = Color.Orange;
            _boxState.FillColour = Color.FromArgb(64, Color.DodgerBlue);
            _boxState.DragStarted += (sender, args) =>
            {
                if (!KeyboardState.Ctrl)
                {
                    DeselectAll();
                }
            };
            
            States.Add(_boxState);

            _showPoints = VisiblePoints.All;
        }

        protected override IEnumerable<Subscription> Subscribe()
        {
            yield return Oy.Subscribe<VertexTool>("Tool:Activated", t => CycleShowPoints());
            yield return Oy.Subscribe<string>("VertexPointTool:SetVisiblePoints", v => SetVisiblePoints(v));
            yield return Oy.Subscribe<object>("VertexPointTool:Split", _ => Split());
            yield return Oy.Subscribe<object>("VertexPointTool:Merge", _ => Merge());
            yield return Oy.Subscribe<object>("VertexTool:DeselectAll", _ => DeselectAll());
        }

        private void SetVisiblePoints(string visiblePoints)
        {
            if (Enum.TryParse(visiblePoints, true, out VisiblePoints p) && p != _showPoints)
            {
                _showPoints = p;
                Invalidate();
            }

            _control.SetVisiblePoints(_showPoints);
        }

        private void CycleShowPoints()
        {
            var side = (int) _showPoints;
            side = (side + 1) % (Enum.GetValues(typeof(VisiblePoints)).Length);
            _showPoints = (VisiblePoints) side;
            Invalidate();

            _control.SetVisiblePoints(_showPoints);
        }

        #region Box confirm / cancel

        public override void KeyDown(MapViewport viewport, ViewportEvent e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Confirm();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Cancel();
                e.Handled = true;
            }
            base.KeyDown(viewport, e);
        }

        private void Confirm()
        {
            if (_boxState.State.Action != BoxAction.Drawn) return;
            var bbox = _boxState.State.GetSelectionBox();
            if (bbox != null && !bbox.IsEmpty())
            {
                SelectPointsInBox(bbox, KeyboardState.Ctrl);
                _boxState.RememberedDimensions = bbox;
            }
            _boxState.State.Action = BoxAction.Idle;

            Invalidate();
        }

        private void Cancel()
        {
            if (_boxState.State.Action != BoxAction.Idle)
            {
                _boxState.RememberedDimensions = new Box(_boxState.State.Start, _boxState.State.End);
                _boxState.State.Action = BoxAction.Idle;
            }
            else
            {
                DeselectAll();
            }

            Invalidate();
        }

        #endregion

        public override async Task SelectionChanged()
        {
            await UpdateVertices();
        }

        public IEnumerable<IDraggable> GetDraggables()
        {
            return GetVisiblePoints().OrderBy(x => x.IsSelected ? 1 : 0);
        }

        public override async Task ToolSelected()
        {
            await UpdateVertices();
            await base.ToolSelected();

            _control.SetVisiblePoints(_showPoints);
        }

        public override async Task ToolDeselected()
        {
            _vertices.Clear();
            await base.ToolDeselected();
        }

        public override void Update()
        {
            foreach (var v in _vertices.Values)
            {
                v.Update();
            }
        }

        private void UpdateSolids(List<VertexSolid> solids)
        {
            if (!solids.Any()) return;

            foreach (var solid in solids)
            {
                solid.IsDirty = true;
            }

            foreach (var vl in _vertices.Values) vl.Update();
            Invalidate();
        }

        private async Task UpdateVertices()
        {
            var existing = new Dictionary<VertexSolid, VertexList>(_vertices);
            _vertices.Clear();
            foreach (var solid in Selection)
            {
                if (existing.ContainsKey(solid))
                {
                    existing[solid].Update();
                    _vertices.Add(solid, existing[solid]);
                }
                else
                {
                    _vertices.Add(solid, new VertexList(this, solid));
                }
            }
        }

        #region Merging vertices

        private bool AutomaticallyMerge()
        {
            return _control.AutomaticallyMerge;
        }

        private void Merge()
        {
            var res = CheckMergedVertices().ToList();
            if (res.Any())
            {
                UpdateSolids(res);
            }
        }

        private bool CanMerge()
        {
            foreach (var solid in Selection)
            {
                foreach (var face in solid.Copy.Faces)
                {
                    for (var i = 0; i < face.Vertices.Count; i++)
                    {
                        var j = (i + 1) % face.Vertices.Count;
                        var v1 = face.Vertices[i];
                        var v2 = face.Vertices[j];

                        if (!v1.Position.EquivalentTo(v2.Position, 0.01f)) continue;
                        return true;
                    }
                }
            }
            return false;
        }

        private bool ConfirmMerge()
        {
            return MessageBox.Show("Merge vertices?", "Overlapping vertices detected", MessageBoxButtons.YesNo) == DialogResult.Yes;
        }

        private IEnumerable<VertexSolid> CheckMergedVertices()
        {
            var res = new HashSet<VertexSolid>();

            var mergedVertices = 0;
            var removedFaces = 0;
            foreach (var solid in Selection)
            {
                foreach (var face in solid.Copy.Faces)
                {
                    // Remove adjacent duplicates
                    for (var i = 0; i < face.Vertices.Count; i++)
                    {
                        // Loop through to the start to cater for when the first & last vertices are equal
                        var j = (i + 1) % face.Vertices.Count;
                        var v1 = face.Vertices[i];
                        var v2 = face.Vertices[j];

                        if (!v1.Position.EquivalentTo(v2.Position, 0.01f)) continue;

                        // Two adjacent vertices are equivalent, remove the latter...
                        face.Vertices.RemoveAt(j);
                        mergedVertices++;

                        // Check i again with its new neighbour
                        i--;
                    }
                }
                
                // Remove empty faces from the solid

                var invalidFaces = solid.Copy.Faces.Where(x => x.Vertices.Count < 3).ToList();
                if (invalidFaces.Any())
                {
                    foreach (var f in invalidFaces) solid.Copy.Faces.Remove(f);
                    removedFaces += invalidFaces.Count;
                    res.Add(solid);
                }
            }

            _control.ShowMergeResult(mergedVertices, removedFaces);
            return res;
        }

        #endregion

        #region Splitting faces

        private void UpdateSplitEnabled()
        {
            _control.SplitEnabled = CanSplit();
        }

        private bool CanSplit()
        {
            return GetSplitFace(out _) != null;
        }

        private MutableFace GetSplitFace(out VertexSolid solid)
        {
            solid = null;
            var selected = GetVisiblePoints().Where(x => x.IsSelected).ToList();

            // Must have two points selected
            if (selected.Count != 2) return null;

            solid = selected[0].Solid;

            // Selection must share a face
            var commonFace = selected[0].GetAdjacentFaces().Intersect(selected[1].GetAdjacentFaces()).ToList();
            if (commonFace.Count != 1) return null;

            var face = commonFace[0];
            var s = selected[0].Position;
            var e = selected[1].Position;
            var edges = face.GetEdges();
            
            // The points cannot be adjacent
            return edges.Any(x => (x.Start == s && x.End == e) || (x.Start == e && x.End == s))
                       ? null
                       : face;
        }

        private void Split()
        {
            throw new NotImplementedException();
            //var face = GetSplitFace(out var vertexSolid);
            //if (face == null) return;
            //
            //var solid = vertexSolid.Copy;
            //
            //var sel = GetVisiblePoints().Where(x => x.IsSelected).ToList();
            //var p1 = sel[0];
            //var p2 = sel[1];
            //
            //if (p1.IsMidpoint) AddAdjacentPoint(face, p1, solid);
            //if (p2.IsMidpoint) AddAdjacentPoint(face, p2, solid);
            //
            //var polygon = new Polygon(face.Vertices.Select(x => x));
            //var clip = new Plane(p1.Position, p2.Position, p1.Position + face.Plane.Normal * 10);
            //Polygon back, front;
            //polygon.Split(clip, out back, out front);
            //if (back == null || front == null) return;
            //
            //solid.Data.Remove(face);
            //
            //CreateFace(back, solid, face);
            //CreateFace(front, solid, face);
            //
            //UpdateSolids(new List<VertexSolid> { vertexSolid });
        }

        //private void SplitFace()
        //{
        //    if (CanSplit())
        //    {
        //        Split();
        //    }
        //}
        //
        //private void CreateFace(Polygon polygon, Solid parent, Face original)
        //{
        //    var verts = polygon.Vertices;
        //    var f = new Face(Document.Map.NumberGenerator.Next("Face"))
        //    {
        //        Plane = new Plane(verts[0], verts[1], verts[2]),
        //        Texture = original.Texture.Clone()
        //    };
        //    f.Vertices.AddRange(verts.Select(x => x.Clone()));
        //    parent.Data.Add(f);
        //}

        //private void AddAdjacentPoint(Face face, VertexPoint point, Solid solid)
        //{
        //    var s = point.MidpointStart.Position;
        //    var e = point.MidpointEnd.Position;
        //
        //    foreach (var f in solid.Faces.Where(x => x != face))
        //    {
        //        foreach (var edge in f.GetEdges())
        //        {
        //            if (edge.Start == s && edge.End == e)
        //            {
        //                var idx = f.Vertices.IndexOf(e);
        //                f.Vertices.Insert(idx, point.Position.Clone());
        //                return;
        //            }
        //            if (edge.Start == e && edge.End == s)
        //            {
        //                var idx = f.Vertices.IndexOf(s);
        //                f.Vertices.Insert(idx, point.Position.Clone());
        //                return;
        //            }
        //        }
        //    }
        //}

        #endregion

        #region Get point lists

        private IEnumerable<VertexPoint> GetVisiblePoints()
        {
            var points = _vertices.SelectMany(x => x.Value.Points);
            switch (_showPoints)
            {
                case VisiblePoints.All:
                    return points;
                case VisiblePoints.Vertices:
                    return points.Where(x => !x.IsMidpoint);
                case VisiblePoints.Midpoints:
                    return points.Where(x => x.IsMidpoint);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private List<VertexPoint> GetPoints(OrthographicCamera camera, Vector3 position, bool allowMixed, bool topmostOnly, bool oneSolidOnly)
        {
            var p = camera.Flatten(position);
            var d = 5 / camera.Zoom; // Tolerance value = 5 pixels

            // Order by the unused Vector3 in the view (which is the up axis) descending to get the "closest" point
            var points = (from pp in GetVisiblePoints()
                let c = camera.Flatten(pp.Position)
                where p.X >= c.X - d && p.X <= c.X + d && p.Y >= c.Y - d && p.Y <= c.Y + d
                let unused = camera.GetUnusedCoordinate(pp.Position)
                orderby unused.X + unused.Y + unused.Z descending
                select pp).ToList();

            if (!allowMixed && points.Any(x => !x.IsMidpoint)) points.RemoveAll(x => x.IsMidpoint);
            if (points.Count <= 0) return points;

            var first = points[0];
            if (topmostOnly) points = new List<VertexPoint> { first };
            if (oneSolidOnly) points.RemoveAll(x => x.Solid != first.Solid);

            return points;
        }

        #endregion

        #region Point selection

        protected override void MouseDown(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            if (KeyboardState.IsKeyDown(Keys.Space)) return;

            var toggle = KeyboardState.Ctrl;

            var l = camera.EyeLocation;
            var pos = new Vector3((float)l.X, (float)l.Y, (float)l.Z);
            var p = new Vector3(e.X, e.Y, 0);
            const int d = 5;
            var clicked = (from point in GetVisiblePoints()
                let c = viewport.Viewport.Camera.WorldToScreen(point.Position)
                where c.Z <= 1
                where p.X >= c.X - d && p.X <= c.X + d && p.Y >= c.Y - d && p.Y <= c.Y + d
                orderby (pos - point.Position).LengthSquared()
                select point).ToList();

            Select(clicked, toggle);
            if (clicked.Any()) e.Handled = true;
        }

        public bool SelectPointsInBox(Box box, bool toggle)
        {
            var inBox = GetVisiblePoints().Where(x => box.Vector3IsInside(x.Position)).ToList();
            Select(inBox, toggle);
            return inBox.Any();
        }

        private void Select(List<VertexPoint> points, bool toggle)
        {
            if (!points.Any()) return;
            if (!toggle) _vertices.SelectMany(x => x.Value.Points).ToList().ForEach(x => x.IsSelected = false);
            var first = points[0];
            var val = !toggle || !first.IsSelected;
            points.ForEach(x => x.IsSelected = val);

            Invalidate();
        }

        private void DeselectAll()
        {
            foreach (var point in _vertices.SelectMany(x => x.Value.Points))
            {
                point.IsSelected = false;
            }

            Invalidate();
        }

        #endregion

        #region Point clicking and dragging

        private bool _selectOnClick;

        private void PointMouseDown(OrthographicCamera camera, VertexPoint point)
        {
            if (_boxState.State.Action != BoxAction.Idle)
            {
                _boxState.RememberedDimensions = new Box(_boxState.State.Start, _boxState.State.End);
                _boxState.State.Action = BoxAction.Idle;
            }

            var vtxs = GetPoints(camera, point.Position, false, KeyboardState.Shift, true);
            if (!vtxs.Any()) return;

            _selectOnClick = true;
            if (!vtxs.Any(x => x.IsSelected))
            {
                Select(vtxs, KeyboardState.Ctrl);
                _selectOnClick = false;
            }
        }

        private void PointClick(OrthographicCamera camera, VertexPoint point)
        {
            if (!_selectOnClick) return;
            _selectOnClick = false;

            var vtxs = GetPoints(camera, point.Position, false, KeyboardState.Shift, true);
            if (!vtxs.Any()) return;
            Select(vtxs, KeyboardState.Ctrl);
        }

        private Vector3 _pointDragStart;
        private Vector3 _pointDragGridOffset;

        private void StartPointDrag(OrthographicCamera camera, ViewportEvent viewportEvent, Vector3 startLocation)
        {
            foreach (var p in GetVisiblePoints().Where(x => x.IsSelected))
            {
                p.IsDragging = true;
            }
            _pointDragStart = camera.ZeroUnusedCoordinate(startLocation);
            _pointDragGridOffset = SnapIfNeeded(camera.ZeroUnusedCoordinate(startLocation)) - camera.ZeroUnusedCoordinate(startLocation);

            Invalidate();
        }

        private void PointDrag(OrthographicCamera camera, ViewportEvent viewportEvent, Vector3 lastPosition, Vector3 position)
        {
            var delta = camera.ZeroUnusedCoordinate(position) - _pointDragStart;
            if (KeyboardState.Shift && !KeyboardState.Alt) delta -= _pointDragGridOffset;

            var selected = GetVisiblePoints().Where(x => x.IsSelected).Distinct().SelectMany(x => x.GetStandardPointList()).ToList();
            selected.ForEach(x => x.DragMove(delta));
            

            foreach (var midpoint in selected.Select(x => x.Solid).Distinct().SelectMany(x => _vertices[x].Points.Where(p => p.IsMidpoint)))
            {
                var p1 = midpoint.MidpointStart.IsDragging ? midpoint.MidpointStart.DraggingPosition : midpoint.MidpointStart.Position;
                var p2 = midpoint.MidpointEnd.IsDragging ? midpoint.MidpointEnd.DraggingPosition : midpoint.MidpointEnd.Position;
                midpoint.DraggingPosition = midpoint.Position = (p1 + p2) / 2;
            }

            Invalidate();
        }

        private void EndPointDrag(OrthographicCamera camera, ViewportEvent viewportEvent, Vector3 endLocation)
        {
            var delta = camera.ZeroUnusedCoordinate(endLocation) - _pointDragStart;
            if (KeyboardState.Shift && !KeyboardState.Alt) delta -= _pointDragGridOffset;

            var selected = GetVisiblePoints().Where(x => x.IsSelected).ToList();
            selected.ForEach(x => x.IsDragging = false);

            // Update positions
            var pts = selected.SelectMany(x => x.GetStandardPointList()).Distinct().ToList();
            foreach (var point in pts) point.Move(delta);

            // Merge points if required
            if (AutomaticallyMerge()) CheckMergedVertices();
            else if (CanMerge() && ConfirmMerge()) CheckMergedVertices();

            UpdateSolids(pts.Select(x => x.Solid).Distinct().ToList());
        }

        #endregion

        private class VertexList
        {
            public VertexPointTool Tool { get; set; }
            public VertexSolid Solid { get; set; }
            public List<VertexPoint> Points { get; set; }
            
            public VertexList(VertexPointTool tool, VertexSolid solid)
            {
                Tool = tool;
                Solid = solid;
                Points = new List<VertexPoint>();
                Update();
            }

            public void Update()
            {
                var selected = Points.Where(x => x.IsSelected).ToList();
                Points.Clear();

                var copy = Solid.Copy;

                var verts = copy.Faces.SelectMany(x => x.Vertices.Select(v => new { Location = v, Face = x })).ToList();

                // Add vertex points
                foreach (var group in verts.GroupBy(x => x.Location.Position.Round(2)))
                {
                    Points.Add(new VertexPoint(Tool, Solid)
                    {
                        ID = verts.IndexOf(group.First()) + 1,
                        Position = group.First().Location.Position.Round(2),
                        Vertices = group.Select(x => x.Location).ToList(),
                        Faces = group.Select(x => x.Face).ToList(),
                        IsSelected = selected.Any(x => !x.IsMidpoint && x.Position == group.First().Location.Position.Round(2))
                    });
                }

                // Add midpoints
                foreach (var group in copy.Faces.SelectMany(x => x.GetEdges()).GroupBy(x => new { Start = x.Start.Round(2), End = x.End.Round(2) }))
                {
                    var s = group.Key.Start;
                    var e = group.Key.End;
                    var coord = (s + e) / 2;
                    var mpStart = Points.First(x => !x.IsMidpoint && x.Position == s);
                    var mpEnd = Points.First(x => !x.IsMidpoint && x.Position == e);
                    Points.Add(new VertexPoint(Tool, Solid)
                    {
                        Position = coord,
                        IsMidpoint = true,
                        MidpointStart = mpStart,
                        MidpointEnd = mpEnd,
                        IsSelected = selected.Any(x => x.IsMidpoint && x.MidpointStart.Position == mpStart.Position && x.MidpointEnd.Position == mpEnd.Position)
                    });
                }
            }
        }

        private class VertexPoint : BaseDraggable
        {
            public VertexPointTool Tool { get; set; }
            private bool _isDragging;

            public int ID { get; set; }
            public VertexSolid Solid { get; set; }
            public List<MutableVertex> Vertices { get; set; }
            public List<MutableFace> Faces { get; set; }
            public Vector3 Position { get; set; }
            public Vector3 DraggingPosition { get; set; }

            public bool IsHighlighted { get; set; }
            public bool IsSelected { get; set; }

            public bool IsDragging
            {
                get => _isDragging;
                set
                {
                    _isDragging = value;
                    if (!IsMidpoint) return;

                    MidpointStart.IsDragging = value;
                    MidpointEnd.IsDragging = value;
                }
            }

            public bool IsMidpoint { get; set; }
            public VertexPoint MidpointStart { get; set; }
            public VertexPoint MidpointEnd { get; set; }

            public VertexPoint(VertexPointTool tool, VertexSolid solid)
            {
                Tool = tool;
                DraggingPosition = Position = Vector3.Zero;
                Solid = solid;
                Vertices = new List<MutableVertex>();
                Faces = new List<MutableFace>();
            }

            private Color GetColor()
            {
                // Midpoints are selected = pink, deselected = yellow
                // Vertex points are selected = red, deselected = white
                var c = IsMidpoint
                    ? (IsSelected ? Color.DeepPink : Color.Yellow)
                    : (IsSelected ? Color.Red : Color.White);
                return IsHighlighted ? c.Lighten() : c;
            }

            public IEnumerable<MutableFace> GetAdjacentFaces()
            {
                return IsMidpoint
                    ? MidpointStart.GetAdjacentFaces().Intersect(MidpointEnd.GetAdjacentFaces())
                    : Faces;
            }

            public void Move(Vector3 delta)
            {
                Position += delta;
                if (IsMidpoint)
                {
                    MidpointStart.Move(delta);
                    MidpointEnd.Move(delta);
                }

                DraggingPosition = Position;
                Vertices.ForEach(x => x.Set(Position));
                Solid.IsDirty = true;
            }

            public void DragMove(Vector3 delta)
            {
                DraggingPosition = Position + delta;
                if (IsMidpoint)
                {
                    MidpointStart.DragMove(delta);
                    MidpointEnd.DragMove(delta);
                }

                Vertices.ForEach(x => x.Set(DraggingPosition));
            }

            private VertexPoint[] _selfArray;

            public VertexPoint[] GetStandardPointList()
            {
                return _selfArray ?? (_selfArray = IsMidpoint ? new[] {MidpointStart, MidpointEnd} : new[] {this});
            }

            public override void MouseDown(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
            {
                e.Handled = true;
                Tool.PointMouseDown(camera, this);
            }

            public override void Click(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
            {
                Tool.PointClick(camera, this);
            }

            public override bool CanDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
            {
                const int width = 5;
                var screenPosition = camera.WorldToScreen(Position);
                var diff = (e.Location - screenPosition).Absolute();
                return diff.X < width && diff.Y < width;
            }

            protected virtual void SetMoveCursor(MapViewport viewport)
            {
                viewport.Control.Cursor = Cursors.SizeAll;
            }

            public override void Highlight(MapViewport viewport)
            {
                IsHighlighted = true;
                SetMoveCursor(viewport);
            }

            public override void Unhighlight(MapViewport viewport)
            {
                IsHighlighted = false;
                viewport.Control.Cursor = Cursors.Default;
            }

            public override void StartDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
            {
                Tool.StartPointDrag(camera, e, Position);
                base.StartDrag(viewport, camera, e, position);
            }

            public override void Drag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 lastPosition, Vector3 position)
            {
                position = Tool.SnapIfNeeded(camera.Expand(position));
                Tool.PointDrag(camera, e, lastPosition, position);
                base.Drag(viewport, camera, e, lastPosition, position);
            }

            public override void EndDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
            {
                position = Tool.SnapIfNeeded(camera.Expand(position));
                Tool.EndPointDrag(camera, e, position);
                base.EndDrag(viewport, camera, e, position);
            }

            public override void Render(BufferBuilder builder)
            {
                // 
            }

            public override void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, Graphics graphics)
            {
                const int size = 8;

                var opac = IsDragging ? 128 : 255;
                var spos = camera.WorldToScreen(Position);
                var rect = new Rectangle((int)spos.X - size / 2, (int)spos.Y - size / 2, size, size);

                var color = Color.FromArgb(opac, GetColor());
                using (var brush = new SolidBrush(color))
                {
                    graphics.FillRectangle(brush, rect);
                    graphics.DrawRectangle(Pens.Black, rect);
                }

                if (IsDragging)
                {
                    spos = camera.WorldToScreen(DraggingPosition);
                    rect = new Rectangle((int)spos.X - size / 2, (int)spos.Y - size / 2, size, size);
                    color = Color.FromArgb(128, GetColor());
                    using (var brush = new SolidBrush(color))
                    {
                        graphics.FillRectangle(brush, rect);
                        graphics.DrawRectangle(Pens.Black, rect);
                    }
                }
            }

            public override void Render(IViewport viewport, PerspectiveCamera camera, Graphics graphics)
            {
                const int size = 8;
                var p = IsDragging ? DraggingPosition : Position;

                var spos = camera.WorldToScreen(p);
                var rect = new Rectangle((int)spos.X - size / 2, (int)spos.Y - size / 2, size, size);

                var color = Color.FromArgb(255, GetColor());
                using (var brush = new SolidBrush(color))
                {
                    graphics.FillRectangle(brush, rect);
                    graphics.DrawRectangle(Pens.Black, rect);
                }
            }
        }
    }
}