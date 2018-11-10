using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.Vertex.Controls;
using Sledge.BspEditor.Tools.Vertex.Selection;
using Sledge.Common;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;
using Sledge.Shell.Input;

namespace Sledge.BspEditor.Tools.Vertex.Tools
{
    [AutoTranslate]
    [Export(typeof(VertexSubtool))]
    public class VertexScaleTool : VertexSubtool
    {
        [Import] private VertexScaleControl _control;

        public override string OrderHint => "D";
        public override string GetName() => "Point scaling";
        public override Control Control => _control;
        
        private readonly BoxDraggableState _boxState;
        private readonly Dictionary<VertexSolid, VertexList> _vertices;
        private readonly ScaleOrigin _origin;
        private Dictionary<VertexPoint, Vector3> _originals;
        
        public VertexScaleTool()
        {
            _vertices = new Dictionary<VertexSolid, VertexList>();

            States.Add(new WrapperDraggableState(GetDraggables));

            _boxState = new BoxDraggableState(this)
            {
                RenderBoxText = false,
                BoxColour = Color.Orange,
                FillColour = Color.FromArgb(64, Color.DodgerBlue)
            };
            _boxState.DragStarted += (sender, args) =>
            {
                if (!KeyboardState.Ctrl)
                {
                    DeselectAll();
                }
            };
            
            States.Add(_boxState);
            
            _origin = new ScaleOrigin(this);
        }

        protected override IEnumerable<Subscription> Subscribe()
        {
            yield return Oy.Subscribe<object>("VertexTool:DeselectAll", _ => DeselectAll());
            yield return Oy.Subscribe<decimal>("VertexScaleTool:ValueChanged", v => ValueChanged(v));
            yield return Oy.Subscribe<decimal>("VertexScaleTool:ValueReset", v => ValueReset(v));
            yield return Oy.Subscribe("VertexScaleTool:ResetOrigin", () => ResetOrigin());
        }

        private void ValueChanged(decimal value)
        {
            MovePoints((float) value);
        }

        private void ValueReset(decimal value)
        {
            _originals = GetVisiblePoints().ToDictionary(x => x, x => x.Position);
        }

        private void ResetOrigin()
        {
            var points = GetVisiblePoints().Where(x => x.IsSelected).Select(x => x.Position).ToList();
            if (!points.Any()) points = GetVisiblePoints().Select(x => x.Position).ToList();
            if (!points.Any()) _origin.Position = Vector3.Zero;
            else _origin.Position = points.Aggregate(Vector3.Zero, (a, b) => a + b) / points.Count;
            Invalidate();
        }

        #region Box confirm / cancel

        private void HandleKeyDown(ViewportEvent e)
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
        }

        protected override void KeyDown(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            HandleKeyDown(e);
            base.KeyDown(document, viewport, camera, e);
        }

        protected override void KeyDown(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            HandleKeyDown(e);
            base.KeyDown(document, viewport, camera, e);
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
            _control.ResetValue();
            if (GetVisiblePoints().Any(x => x.IsSelected)) ResetOrigin();
        }

        public IEnumerable<IDraggable> GetDraggables()
        {
            return GetVisiblePoints().OrderBy(x => x.IsSelected ? 1 : 0).Union(new IDraggable[] { _origin }).ToList();
        }

        private void MovePoints(float value)
        {
            var o = _origin.Position;
            var solids = new List<VertexSolid>();

            // Move each selected point by the computed offset from the origin
            foreach (var p in GetVisiblePoints().Where(x => x.IsSelected).SelectMany(x => x.GetStandardPointList()).Distinct())
            {
                if (!solids.Contains(p.Solid)) solids.Add(p.Solid);
                var orig = _originals[p];
                var diff = orig - o;
                var move = o + diff * value / 100;
                p.Move(move - p.Position);
            }
            UpdateSolids(solids);
        }

        public override async Task ToolSelected()
        {
            await UpdateVertices();
            await base.ToolSelected();
            _control.ResetValue();
            ResetOrigin();
        }

        public override async Task ToolDeselected()
        {
            _vertices.Clear();
            await base.ToolDeselected();
            _originals = null;
        }

        public override void Update()
        {
            foreach (var v in _vertices.Values)
            {
                v.Update();
            }
            _originals = GetVisiblePoints().ToDictionary(x => x, x => x.Position);
        }

        private void UpdateSolids(List<VertexSolid> solids)
        {
            if (!solids.Any()) return;

            foreach (var solid in solids)
            {
                solid.IsDirty = true;
            }

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
            _originals = GetVisiblePoints().ToDictionary(x => x, x => x.Position);
        }

        private IEnumerable<VertexPoint> GetVisiblePoints()
        {
            return _vertices.SelectMany(x => x.Value.Points);
        }

        #region Point selection

        protected override void MouseDown(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
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
        
        private class VertexList
        {
            public VertexScaleTool Tool { get; set; }
            public VertexSolid Solid { get; set; }
            public List<VertexPoint> Points { get; set; }
            
            public VertexList(VertexScaleTool tool, VertexSolid solid)
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
                        IsSelected = selected.Any(x => x.Position == group.First().Location.Position.Round(2))
                    });
                }
            }
        }

        private class VertexPoint : BaseDraggable
        {
            public VertexScaleTool Tool { get; set; }

            public int ID { get; set; }
            public VertexSolid Solid { get; set; }
            public List<MutableVertex> Vertices { get; set; }
            public List<MutableFace> Faces { get; set; }
            public Vector3 Position { get; set; }
            public Vector3 DraggingPosition { get; set; }

            public override Vector3 Origin => Position;
            
            public override Vector3 ZIndex
            {
                get
                {
                    var pos = Position;
                    if (IsSelected) pos += Vector3.One * 1000000;
                    return pos;
                }
            }

            public bool IsHighlighted { get; set; }
            public bool IsSelected { get; set; }

            public VertexPoint(VertexScaleTool tool, VertexSolid solid)
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
                var c = (IsSelected ? Color.Red : Color.White);
                return IsHighlighted ? c.Lighten() : c;
            }
            
            public void Move(Vector3 delta)
            {
                Position += delta;
                DraggingPosition = Position;
                Vertices.ForEach(x => x.Set(Position));
                Solid.IsDirty = true;
            }

            private VertexPoint[] _selfArray;

            public VertexPoint[] GetStandardPointList()
            {
                return _selfArray ?? (_selfArray = new[] {this});
            }

            public override void MouseDown(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
            {
                e.Handled = true;
            }

            public override void Click(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
            {
                // 
            }

            public override bool CanDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
            {
                return false;
            }

            public override void Highlight(MapDocument document, MapViewport viewport)
            {
                IsHighlighted = true;
            }

            public override void Unhighlight(MapDocument document, MapViewport viewport)
            {
                IsHighlighted = false;
            }

            public override void Render(MapDocument document, BufferBuilder builder)
            {
                // 
            }

            public override void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im)
            {
                const int size = 4;
                
                var spos = camera.WorldToScreen(Position);

                var color = Color.FromArgb(255, GetColor());
                im.AddRectOutlineOpaque(new Vector2(spos.X - size, spos.Y - size), new Vector2(spos.X + size, spos.Y + size), Color.Black, color);
            }

            public override void Render(IViewport viewport, PerspectiveCamera camera, I2DRenderer im)
            {
                const int size = 4;

                var spos = camera.WorldToScreen(Position);

                var color = Color.FromArgb(255, GetColor());
                im.AddRectOutlineOpaque(new Vector2(spos.X - size, spos.Y - size), new Vector2(spos.X + size, spos.Y + size), Color.Black, color);
            }
        }
        
        private class ScaleOrigin : DraggableVector3
        {
            private readonly VertexScaleTool _vmScaleTool;

            public ScaleOrigin(VertexScaleTool vmScaleTool)
            {
                _vmScaleTool = vmScaleTool;
                Width = 10;
            }

            public override void Drag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 lastPosition, Vector3 position)
            {
                Position = _vmScaleTool.SnapIfNeeded(camera.Expand(position) + camera.GetUnusedCoordinate(Position));
                OnDragMoved();
            }

            public override void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im)
            {
                var spos = camera.WorldToScreen(Position);

                const int inner = 4;
                const int outer = 8;
                
                var col = Highlighted ? Color.DarkOrange : Color.LightBlue;
                im.AddCircle(spos.ToVector2(), inner, Color.AliceBlue);
                im.AddCircle(spos.ToVector2(), outer, col);
            }
        }
    }
}