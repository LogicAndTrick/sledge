using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK.Input;
using Sledge.Gui.Events;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Structures;
using MouseEventHandler = Sledge.Gui.Events.MouseEventHandler;
using Padding = Sledge.Gui.Structures.Padding;
using SWF = System.Windows.Forms;

namespace Sledge.Gui.WinForms
{
    public static class WinFormsExtensions
    {
        public static Rectangle ToRectangle(this System.Drawing.Rectangle rectangle)
        {
            return new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public static Padding ToPadding(this System.Windows.Forms.Padding padding)
        {
            return new Padding(padding.Top, padding.Left, padding.Bottom, padding.Right);
        }

        public static SWF.MouseEventHandler ToMouseEventHandler(this MouseEventHandler handler, IControl control)
        {
            return (o, e) => handler(o, new ControlEvent(control, e.X, e.Y, Convert(e.Button)) {Delta = e.Delta, Clicks = e.Clicks});
        }

        private static MouseButton Convert(MouseButtons mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButtons.None:
                    return MouseButton.LastButton;
                case MouseButtons.Left:
                    return MouseButton.Left;
                case MouseButtons.Right:
                    return MouseButton.Right;
                case MouseButtons.Middle:
                    return MouseButton.Middle;
                case MouseButtons.XButton1:
                    return MouseButton.Button1;
                case MouseButtons.XButton2:
                    return MouseButton.Button2;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
