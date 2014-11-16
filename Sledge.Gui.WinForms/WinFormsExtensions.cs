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
            return (o, e) => handler(o, new ControlEvent(control, e.X, e.Y, Convert(e.Button)) { Delta = e.Delta, Clicks = e.Clicks });
        }

        public static SWF.KeyEventHandler ToKeyEventHandler(this KeyboardEventHandler handler, IControl control)
        {
            return (o, e) => handler(o, new ControlEvent(control, WinFormsOpenTkKeyMap.Map(e.KeyData), e.Control, e.Shift, e.Alt));
        }

        private static MouseButton Convert(MouseButtons mouseButton)
        {
            if (mouseButton.HasFlag(MouseButtons.Left)) return MouseButton.Left;
            if (mouseButton.HasFlag(MouseButtons.Right)) return MouseButton.Right;
            if (mouseButton.HasFlag(MouseButtons.Middle)) return MouseButton.Middle;
            if (mouseButton.HasFlag(MouseButtons.XButton1)) return MouseButton.Button1;
            if (mouseButton.HasFlag(MouseButtons.XButton2)) return MouseButton.Button2;
            return MouseButton.LastButton;
        }
    }
}
