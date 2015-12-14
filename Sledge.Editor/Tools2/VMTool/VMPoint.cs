using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Extensions;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools2.DraggableTool;
using Sledge.Editor.UI;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.Editor.Tools2.VMTool
{
    public class VMPoint : BaseDraggable
    {
        private readonly VMPointsDraggableState _state;
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

        public VMPoint(VMPointsDraggableState state, VMSolid solid)
        {
            _state = state;
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
            _state.PointMouseDown(viewport, this);
        }

        public override void Click(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            _state.PointClick(viewport, this);
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
            _state.StartPointDrag(viewport, e, Position);
            base.StartDrag(viewport, e, position);
        }

        public override void Drag(MapViewport viewport, ViewportEvent e, Coordinate lastPosition, Coordinate position)
        {
            _state.PointDrag(viewport, e, lastPosition, position);
            base.Drag(viewport, e, lastPosition, position);
        }

        public override void EndDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            _state.EndPointDrag(viewport, e, position);
            base.EndDrag(viewport, e, position);
        }

        public override IEnumerable<SceneObject> GetSceneObjects()
        {
            yield break;
        }

        public override IEnumerable<Element> GetViewportElements(MapViewport viewport, PerspectiveCamera camera)
        {
            yield break;
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