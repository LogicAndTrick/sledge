using System;
using Sledge.DataStructures.Geometric;
using Sledge.EditorNew.UI.Viewports;

namespace Sledge.EditorNew.Tools.DraggableTool
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

        public IMapViewport Viewport { get; set; }
        public BoxAction Action { get; set; }
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

        public void Move(IViewport2D viewport, Coordinate delta)
        {
            delta = viewport.Expand(delta);
            Start += delta;
            End += delta;
            OnChanged();
        }

        public void Resize(ResizeHandle handle, IViewport2D viewport, Coordinate position)
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
    }
}