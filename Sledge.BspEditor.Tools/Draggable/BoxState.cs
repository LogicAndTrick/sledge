using System;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Tools.Draggable
{
    public class BoxState
    {
        private Coordinate _start;
        private Coordinate _end;
        public event EventHandler Changed;

        protected virtual void OnChanged()
        {
            var handler = Changed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public MapViewport Viewport { get; set; }
        private BoxAction _action;
        public BoxAction Action
        {
            get { return _action; }
            set
            {
                _action = value;
                OnChanged();
            }
        }

        public Coordinate OrigStart { get; set; }
        public Coordinate OrigEnd { get; set; }

        public Coordinate Start
        {
            get { return _start; }
            set
            {
                _start = value;
                OnChanged();
            }
        }

        public Coordinate End
        {
            get { return _end; }
            set
            {
                _end = value;
                OnChanged();
            }
        }

        public void FixBounds()
        {
            var box = new Box(Start, End);
            OrigStart = Start = box.Start;
            OrigEnd = End = box.End;
            OnChanged();
        }

        public void Move(MapViewport viewport, Coordinate delta)
        {
            delta = viewport.Expand(delta);
            Start += delta;
            End += delta;
            OnChanged();
        }

        public void Resize(ResizeHandle handle, MapViewport viewport, Coordinate position)
        {
            var fs = viewport.Flatten(Start);
            var fe = viewport.Flatten(End);
            var us = viewport.GetUnusedCoordinate(Start);
            var ue = viewport.GetUnusedCoordinate(End);
            switch (handle)
            {
                case ResizeHandle.TopLeft:
                    End = viewport.Expand(fe.X, position.Y) + ue;
                    Start = viewport.Expand(position.X, fs.Y) + us;
                    break;
                case ResizeHandle.Top:
                    End = viewport.Expand(fe.X, position.Y) + ue;
                    break;
                case ResizeHandle.TopRight:
                    End = viewport.Expand(position.X, position.Y) + ue;
                    break;
                case ResizeHandle.Left:
                    Start = viewport.Expand(position.X, fs.Y) + us;
                    break;
                case ResizeHandle.Center:
                    // 
                    break;
                case ResizeHandle.Right:
                    End = viewport.Expand(position.X, fe.Y) + ue;
                    break;
                case ResizeHandle.BottomLeft:
                    Start = viewport.Expand(position.X, position.Y) + us;
                    break;
                case ResizeHandle.Bottom:
                    Start = viewport.Expand(fs.X, position.Y) + us;
                    break;
                case ResizeHandle.BottomRight:
                    Start = viewport.Expand(fs.X, position.Y) + us;
                    End = viewport.Expand(position.X, fe.Y) + ue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("handle");
            }
            OnChanged();
        }

        public Box GetSelectionBox()
        {
            // Don't actually use Decimal.Min/MaxValue because it just causes headaches
            const decimal minValue = -10000000000m;
            const decimal maxValue = +10000000000m;

            var state = this;

            // If one of the dimensions has a depth value of 0, extend it out into infinite space
            // If two or more dimensions have depth 0, do nothing.
            
            var sameX = state.Start.X == state.End.X;
            var sameY = state.Start.Y == state.End.Y;
            var sameZ = state.Start.Z == state.End.Z;
            var start = state.Start.Clone();
            var end = state.End.Clone();

            if (sameX)
            {
                if (sameY || sameZ) return null;
                start.X = minValue;
                end.X = maxValue;
            }

            if (sameY)
            {
                if (sameZ) return null;
                start.Y = minValue;
                end.Y = maxValue;
            }

            if (sameZ)
            {
                start.Z = minValue;
                end.Z = maxValue;
            }

            return new Box(start, end);
        }
    }
}