using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Extensions;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools.DraggableTool;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.Editor.Tools.VMTool
{
    public class VMPoint : BaseDraggable
    {
        private readonly VMTool _tool;
        private bool _isDragging;

        public int ID { get; set; }
        public VMSolid Solid { get; set; }
        public List<Vertex> Vertices { get; set; }
        public Coordinate Position { get; set; }
        public Coordinate DraggingPosition { get; set; }

        public bool IsHighlighted { get; set; }
        public bool IsSelected { get; set; }

        public bool IsDragging
        {
            get { return _isDragging; }
            set
            {
                _isDragging = value;
                if (IsMidpoint)
                {
                    MidpointStart.IsDragging = value;
                    MidpointEnd.IsDragging = value;
                }
            }
        }

        public bool IsMidpoint { get; set; }
        public VMPoint MidpointStart { get; set; }
        public VMPoint MidpointEnd { get; set; }

        public VMPoint(VMTool tool, VMSolid solid)
        {
            _tool = tool;
            DraggingPosition = Position = Coordinate.Zero;
            Solid = solid;
            Vertices = new List<Vertex>();
        }

        protected Color GetColor()
        {
            // Midpoints are selected = pink, deselected = yellow
            // Vertex points are selected = red, deselected = white
            var c = IsMidpoint
                ? (IsSelected ? Color.DeepPink : Color.Yellow)
                : (IsSelected ? Color.Red : Color.White);
            return IsHighlighted ? c.Lighten() : c;
        }

        public IEnumerable<Face> GetAdjacentFaces()
        {
            return IsMidpoint
                ? MidpointStart.GetAdjacentFaces().Intersect(MidpointEnd.GetAdjacentFaces())
                : Vertices.Select(x => x.Parent).Distinct();
        }

        public void Move(Coordinate delta)
        {
            Position += delta;
            if (IsMidpoint)
            {
                MidpointStart.Move(delta);
                MidpointEnd.Move(delta);
            }
            DraggingPosition = Position;
            Vertices.ForEach(x => x.Location = Position);
        }

        public void DragMove(Coordinate delta)
        {
            DraggingPosition = Position + delta;
            if (IsMidpoint)
            {
                MidpointStart.DragMove(delta);
                MidpointEnd.DragMove(delta);
            }
            Vertices.ForEach(x => x.Location = DraggingPosition);
        }

        private VMPoint[] _selfArray;

        public VMPoint[] GetStandardPointList()
        {
            return _selfArray ?? (_selfArray = IsMidpoint ? new[] {MidpointStart, MidpointEnd} : new[] {this});
        }

        public override void MouseDown(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            _tool.PointMouseDown(viewport, this);
        }

        public override void Click(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            _tool.PointClick(viewport, this);
        }

        public override bool CanDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            const int width = 5;
            var screenPosition = viewport.ProperWorldToScreen(Position);
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

        public override void StartDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            if (!_tool.CanDragPoint(this)) return;
            _tool.StartPointDrag(viewport, e, Position);
            base.StartDrag(viewport, e, position);
        }

        public override void Drag(MapViewport viewport, ViewportEvent e, Coordinate lastPosition, Coordinate position)
        {
            if (!_tool.CanDragPoint(this)) return;
            position = _tool.SnapIfNeeded(viewport.Expand(position));
            _tool.PointDrag(viewport, e, lastPosition, position);
            base.Drag(viewport, e, lastPosition, position);
        }

        public override void EndDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            if (!_tool.CanDragPoint(this)) return;
            position = _tool.SnapIfNeeded(viewport.Expand(position));
            _tool.EndPointDrag(viewport, e, position);
            base.EndDrag(viewport, e, position);
        }

        public override IEnumerable<SceneObject> GetSceneObjects()
        {
            yield break;
        }

        public override IEnumerable<Element> GetViewportElements(MapViewport viewport, PerspectiveCamera camera)
        {
            var pos = IsDragging ? DraggingPosition : Position;
            yield return new HandleElement(PositionType.Anchored, HandleElement.HandleType.SquareTexture, new Position(pos.ToVector3()), 4)
            {
                Color = Color.FromArgb(255, GetColor())
            };
        }

        public override IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            var opac = IsDragging ? 128 : 255;
            yield return new HandleElement(PositionType.World, HandleElement.HandleType.SquareTexture, new Position(Position.ToVector3()), 4)
            {
                Color = Color.FromArgb(opac, GetColor())
            };
            if (IsDragging)
            {
                yield return new HandleElement(PositionType.World, HandleElement.HandleType.SquareTexture, new Position(DraggingPosition.ToVector3()), 4)
                {
                    Color = Color.FromArgb(128, GetColor())
                };
            }
        }
    }
}