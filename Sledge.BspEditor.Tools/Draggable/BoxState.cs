using System;
using System.Numerics;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;

namespace Sledge.BspEditor.Tools.Draggable
{
    public class BoxState
    {
        private Vector3 _start;
        private Vector3 _end;
        public event EventHandler Changed;

        protected virtual void OnChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public MapViewport Viewport { get; set; }
        private BoxAction _action;
        public BoxAction Action
        {
            get => _action;
            set
            {
                _action = value;
                OnChanged();
            }
        }

        public Vector3 OrigStart { get; set; }
        public Vector3 OrigEnd { get; set; }

        public Vector3 Start
        {
            get => _start;
            set
            {
                _start = value;
                OnChanged();
            }
        }

        public Vector3 End
        {
            get => _end;
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

        public void Move(OrthographicCamera camera, Vector3 delta)
        {
            delta = camera.Expand(delta);
            Start += delta;
            End += delta;
            OnChanged();
        }

        public void Resize(ResizeHandle handle, MapViewport viewport, OrthographicCamera camera, Vector3 position)
        {
            var fs = camera.Flatten(Start);
            var fe = camera.Flatten(End);
            var us = camera.GetUnusedCoordinate(Start);
            var ue = camera.GetUnusedCoordinate(End);
            switch (handle)
            {
                case ResizeHandle.TopLeft:
                    End = camera.Expand(fe.X, position.Y) + ue;
                    Start = camera.Expand(position.X, fs.Y) + us;
                    break;
                case ResizeHandle.Top:
                    End = camera.Expand(fe.X, position.Y) + ue;
                    break;
                case ResizeHandle.TopRight:
                    End = camera.Expand(position.X, position.Y) + ue;
                    break;
                case ResizeHandle.Left:
                    Start = camera.Expand(position.X, fs.Y) + us;
                    break;
                case ResizeHandle.Center:
                    // 
                    break;
                case ResizeHandle.Right:
                    End = camera.Expand(position.X, fe.Y) + ue;
                    break;
                case ResizeHandle.BottomLeft:
                    Start = camera.Expand(position.X, position.Y) + us;
                    break;
                case ResizeHandle.Bottom:
                    Start = camera.Expand(fs.X, position.Y) + us;
                    break;
                case ResizeHandle.BottomRight:
                    Start = camera.Expand(fs.X, position.Y) + us;
                    End = camera.Expand(position.X, fe.Y) + ue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("handle");
            }
            OnChanged();
        }

        public Box GetSelectionBox()
        {
            // Don't actually use Single.Min/MaxValue because it just causes headaches
            const float minValue = -10000000000f;
            const float maxValue = +10000000000f;

            var state = this;

            // If one of the dimensions has a depth value of 0, extend it out into infinite space
            // If two or more dimensions have depth 0, do nothing.
            
            var sameX = Math.Abs(state.Start.X - state.End.X) < 0.001f;
            var sameY = Math.Abs(state.Start.Y - state.End.Y) < 0.001f;
            var sameZ = Math.Abs(state.Start.Z - state.End.Z) < 0.001f;
            var start = state.Start;
            var end = state.End;

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