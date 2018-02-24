using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Rendering;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.Vertex.Controls;
using Sledge.BspEditor.Tools.Vertex.Selection;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Shell.Input;

namespace Sledge.BspEditor.Tools.Vertex.Tools
{
    [Export(typeof(VertexSubtool))]
    public class VertexScaleTool : VertexSubtool, IDraggableState
    {
        public override string OrderHint => "D";
        public override string GetName() => "Point scaling";
        public override Control Control => _control;
        
        private readonly BoxDraggableState _boxState;
        private readonly Dictionary<VertexSolid, VertexList> _vertices;
        private readonly ScaleControl _control;
        private readonly ScaleOrigin _origin;
        private Dictionary<VertexPoint, Coordinate> _originals;
        
        public VertexScaleTool()
        {
            _vertices = new Dictionary<VertexSolid, VertexList>();
            States.Add(this);

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
            
            var sc = new ScaleControl();
            sc.ValueChanged += ValueChanged;
            sc.ValueReset += ValueReset;
            sc.ResetOrigin += ResetOrigin;
            _control = sc;
            
            _origin = new ScaleOrigin(this);
        }

        protected override IEnumerable<Subscription> Subscribe()
        {
            yield return Oy.Subscribe<object>("VertexTool:DeselectAll", _ => DeselectAll());
        }

        private void ValueChanged(object sender, decimal value)
        {
            MovePoints(value);
        }

        private void ValueReset(object sender, decimal value)
        {
            _originals = GetVisiblePoints().ToDictionary(x => x, x => x.Position);
        }

        private void ResetOrigin(object sender)
        {
            ResetOrigin();
        }

        private void ResetOrigin()
        {
            var points = GetVisiblePoints().Where(x => x.IsSelected).Select(x => x.Position).ToList();
            if (!points.Any()) points = GetVisiblePoints().Select(x => x.Position).ToList();
            if (!points.Any()) _origin.Position = Coordinate.Zero;
            else _origin.Position = points.Aggregate(Coordinate.Zero, (a, b) => a + b) / points.Count;
            Invalidate();
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
            _control.ResetValue();
            if (GetVisiblePoints().Any(x => x.IsSelected)) ResetOrigin();
        }

        public IEnumerable<IDraggable> GetDraggables()
        {
            return GetVisiblePoints().OrderBy(x => x.IsSelected ? 1 : 0).Union(new IDraggable[] { _origin });
        }

        private void MovePoints(decimal value)
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
        }

        private void UpdateSolids(List<VertexSolid> solids)
        {
            if (!solids.Any()) return;

            foreach (var solid in solids)
            {
                solid.IsDirty = true;
                solid.Copy.DescendantsChanged();
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
        }

        private IEnumerable<VertexPoint> GetVisiblePoints()
        {
            return _vertices.SelectMany(x => x.Value.Points);
        }

        #region Point selection

        protected override void MouseDown(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            if (KeyboardState.IsKeyDown(Keys.Space)) return;

            var toggle = KeyboardState.Ctrl;

            var l = camera.EyeLocation;
            var pos = new Coordinate((decimal)l.X, (decimal)l.Y, (decimal)l.Z);
            var p = new Coordinate(e.X, viewport.Height - e.Y, 0);
            const int d = 5;
            var clicked = (from point in GetVisiblePoints()
                let c = viewport.Viewport.Camera.WorldToScreen(point.Position.ToVector3(), viewport.Width, viewport.Height).ToCoordinate()
                where c.Z <= 1
                where p.X >= c.X - d && p.X <= c.X + d && p.Y >= c.Y - d && p.Y <= c.Y + d
                orderby (pos - point.Position).LengthSquared()
                select point).ToList();

            Select(clicked, toggle);
            if (clicked.Any()) e.Handled = true;
        }

        public bool SelectPointsInBox(Box box, bool toggle)
        {
            var inBox = GetVisiblePoints().Where(x => box.CoordinateIsInside(x.Position)).ToList();
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
                foreach (var group in verts.GroupBy(x => x.Location.Round(2)))
                {
                    Points.Add(new VertexPoint(Tool, Solid)
                    {
                        ID = verts.IndexOf(group.First()) + 1,
                        Position = group.First().Location.Round(2),
                        Vertices = group.Select(x => x.Location).ToList(),
                        Faces = group.Select(x => x.Face).ToList(),
                        IsSelected = selected.Any(x => x.Position == group.First().Location.Round(2))
                    });
                }
            }
        }

        private class VertexPoint : BaseDraggable
        {
            public VertexScaleTool Tool { get; set; }
            private bool _isDragging;

            public int ID { get; set; }
            public VertexSolid Solid { get; set; }
            public List<Coordinate> Vertices { get; set; }
            public List<Face> Faces { get; set; }
            public Coordinate Position { get; set; }
            public Coordinate DraggingPosition { get; set; }

            public bool IsHighlighted { get; set; }
            public bool IsSelected { get; set; }

            public VertexPoint(VertexScaleTool tool, VertexSolid solid)
            {
                Tool = tool;
                DraggingPosition = Position = Coordinate.Zero;
                Solid = solid;
                Vertices = new List<Coordinate>();
                Faces = new List<Face>();
            }

            private Color GetColor()
            {
                // Midpoints are selected = pink, deselected = yellow
                // Vertex points are selected = red, deselected = white
                var c = (IsSelected ? Color.Red : Color.White);
                return IsHighlighted ? c.Lighten() : c;
            }
            
            public void Move(Coordinate delta)
            {
                Position += delta;
                DraggingPosition = Position;
                Vertices.ForEach(x => x.Set(Position));
                Faces.ForEach(x => x.Vertices.UpdatePlane());
                Solid.IsDirty = true;
            }

            private VertexPoint[] _selfArray;

            public VertexPoint[] GetStandardPointList()
            {
                return _selfArray ?? (_selfArray = new[] {this});
            }

            public override void MouseDown(MapViewport viewport, ViewportEvent e, Coordinate position)
            {
                e.Handled = true;
            }

            public override void Click(MapViewport viewport, ViewportEvent e, Coordinate position)
            {
                // 
            }

            public override bool CanDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
            {
                return false;
            }

            public override void Highlight(MapViewport viewport)
            {
                IsHighlighted = true;
            }

            public override void Unhighlight(MapViewport viewport)
            {
                IsHighlighted = false;
            }

            public override IEnumerable<SceneObject> GetSceneObjects()
            {
                yield break;
            }

            public override IEnumerable<Element> GetViewportElements(MapViewport viewport, PerspectiveCamera camera)
            {
                var pos = Position;
                yield return new HandleElement(PositionType.Anchored, HandleElement.HandleType.SquareTexture, new Position(pos.ToVector3()), 4)
                {
                    Color = Color.FromArgb(255, GetColor())
                };
            }

            public override IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
            {
                var opac = 255;
                yield return new HandleElement(PositionType.World, HandleElement.HandleType.SquareTexture, new Position(Position.ToVector3()), 4)
                {
                    Color = Color.FromArgb(opac, GetColor())
                };
            }
        }
        
        private class ScaleOrigin : DraggableCoordinate
        {
            private readonly VertexScaleTool _vmScaleTool;

            public ScaleOrigin(VertexScaleTool vmScaleTool)
            {
                _vmScaleTool = vmScaleTool;
                Width = 10;
            }

            public override void Drag(MapViewport viewport, ViewportEvent e, Coordinate lastPosition, Coordinate position)
            {
                Position = _vmScaleTool.SnapIfNeeded(viewport.Expand(position) + viewport.GetUnusedCoordinate(Position));
                OnDragMoved();
            }

            public override IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
            {
                yield return new HandleElement(PositionType.World, HandleElement.HandleType.Circle, new Position(Position.ToVector3()), 8)
                {
                    Color = Color.Transparent,
                    LineColor = Color.AliceBlue,
                    ZIndex = 10
                };
                yield return new HandleElement(PositionType.World, HandleElement.HandleType.Circle, new Position(Position.ToVector3()), 4)
                {
                    Color = Color.Transparent,
                    LineColor = Color.AliceBlue,
                    ZIndex = 10
                };
            }
        }

        #region Draggable non-implementation

        public event EventHandler DragStarted;
        public event EventHandler DragMoved;
        public event EventHandler DragEnded;
        public void MouseDown(MapViewport viewport, ViewportEvent e, Coordinate position) {}
        public void MouseUp(MapViewport viewport, ViewportEvent e, Coordinate position) {}
        public void Click(MapViewport viewport, ViewportEvent e, Coordinate position) {}
        public bool CanDrag(MapViewport viewport, ViewportEvent e, Coordinate position) => false;
        public void Highlight(MapViewport viewport) {}
        public void Unhighlight(MapViewport viewport) {}
        public void StartDrag(MapViewport viewport, ViewportEvent e, Coordinate position) {}
        public void Drag(MapViewport viewport, ViewportEvent e, Coordinate lastPosition, Coordinate position) {}
        public void EndDrag(MapViewport viewport, ViewportEvent e, Coordinate position) {}
        IEnumerable<SceneObject> IDraggable.GetSceneObjects() => new List<SceneObject>();
        IEnumerable<Element> IDraggable.GetViewportElements(MapViewport viewport, PerspectiveCamera camera) => new List<Element>();
        IEnumerable<Element> IDraggable.GetViewportElements(MapViewport viewport, OrthographicCamera camera) => new List<Element>();

        #endregion
    }
}