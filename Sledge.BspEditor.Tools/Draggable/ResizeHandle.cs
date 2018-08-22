using System;
using System.Windows.Forms;

namespace Sledge.BspEditor.Tools.Draggable
{
    public enum ResizeHandle
    {
        TopLeft, Top, TopRight,
        Left, Center, Right,
        BottomLeft, Bottom, BottomRight
    }

    public static class ResizeHandleExtensions
    {
        public static Cursor GetCursorType(this ResizeHandle handle)
        {
            switch (handle)
            {
                case ResizeHandle.TopLeft:
                    return Cursors.SizeNWSE;
                case ResizeHandle.Top:
                    return Cursors.SizeNS;
                case ResizeHandle.TopRight:
                    return Cursors.SizeNESW;
                case ResizeHandle.Left:
                    return Cursors.SizeWE;
                case ResizeHandle.Center:
                    return Cursors.SizeAll;
                case ResizeHandle.Right:
                    return Cursors.SizeWE;
                case ResizeHandle.BottomLeft:
                    return Cursors.SizeNESW;
                case ResizeHandle.Bottom:
                    return Cursors.SizeNS;
                case ResizeHandle.BottomRight:
                    return Cursors.SizeNWSE;
                default:
                    throw new ArgumentOutOfRangeException("handle");
            }
        }
    }
}