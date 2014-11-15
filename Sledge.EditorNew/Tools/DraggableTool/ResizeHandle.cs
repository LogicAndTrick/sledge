using System;
using Sledge.Gui.Components;

namespace Sledge.EditorNew.Tools.DraggableTool
{
    public enum ResizeHandle
    {
        TopLeft, Top, TopRight,
        Left, Center, Right,
        BottomLeft, Bottom, BottomRight
    }

    public static class ResizeHandleExtensions
    {
        public static CursorType GetCursorType(this ResizeHandle handle)
        {
            switch (handle)
            {
                case ResizeHandle.TopLeft:
                    return CursorType.SizeTopLeft;
                case ResizeHandle.Top:
                    return CursorType.SizeTop;
                case ResizeHandle.TopRight:
                    return CursorType.SizeTopRight;
                case ResizeHandle.Left:
                    return CursorType.SizeLeft;
                case ResizeHandle.Center:
                    return CursorType.SizeAll;
                case ResizeHandle.Right:
                    return CursorType.SizeRight;
                case ResizeHandle.BottomLeft:
                    return CursorType.SizeBottomLeft;
                case ResizeHandle.Bottom:
                    return CursorType.SizeBottom;
                case ResizeHandle.BottomRight:
                    return CursorType.SizeBottomRight;
                default:
                    throw new ArgumentOutOfRangeException("handle");
            }
        }
    }
}