using System;
using Sledge.DataStructures.Geometric;
using Sledge.EditorNew.UI.Viewports;

namespace Sledge.EditorNew.Tools.DraggableTool
{
    public class BoxState
    {
        public IMapViewport Viewport { get; set; }
        public BoxAction Action { get; set; }
        public Coordinate OrigStart { get; set; }
        public Coordinate OrigEnd { get; set; }
        public Coordinate Start { get; set; }
        public Coordinate End { get; set; }

        public void FixBounds()
        {
            var box = new Box(Start, End);
            Start = box.Start;
            End = box.End;
        }

        public void Resize(ResizeHandle handle, IViewport2D viewport, int dx, int dy)
        {
            var x = dx * viewport.Zoom;
            var y = dy * viewport.Zoom;
            switch (handle)
            {
                case ResizeHandle.TopLeft:
                    End += viewport.Expand(new Coordinate(0, y, 0));
                    Start += viewport.Expand(new Coordinate(x, 0, 0));
                    break;
                case ResizeHandle.Top:
                    End += viewport.Expand(new Coordinate(0, y, 0));
                    break;
                case ResizeHandle.TopRight:
                    End += viewport.Expand(new Coordinate(x, y, 0));
                    break;
                case ResizeHandle.Left:
                    Start += viewport.Expand(new Coordinate(x, 0, 0));
                    break;
                case ResizeHandle.Center:
                    var offset = viewport.Expand(new Coordinate(x, y, 0));
                    Start += offset;
                    End += offset;
                    break;
                case ResizeHandle.Right:
                    End += viewport.Expand(new Coordinate(x, 0, 0));
                    break;
                case ResizeHandle.BottomLeft:
                    Start += viewport.Expand(new Coordinate(x, y, 0));
                    break;
                case ResizeHandle.Bottom:
                    Start += viewport.Expand(new Coordinate(0, y, 0));
                    break;
                case ResizeHandle.BottomRight:
                    Start += viewport.Expand(new Coordinate(0, y, 0));
                    End += viewport.Expand(new Coordinate(x, 0, 0));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("handle");
            }
        }
    }
}