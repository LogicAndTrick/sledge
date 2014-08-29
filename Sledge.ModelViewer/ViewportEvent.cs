using System;
using Gdk;
using OpenTK.Input;
using Point = System.Drawing.Point;

namespace Sledge.UI
{
    public class ViewportEvent : EventArgs
    {
        public ViewportBase Sender { get; set; }

        public bool Handled { get; set; }

        // Key
        public bool Control { get; set; }
        public bool Shift { get; set; }
        public bool Alt { get; set; }
        public OpenTK.Input.Key KeyCode { get; set; }
        public int KeyValue { get; set; }
        public char KeyChar { get; set; }

        // Mouse
        public OpenTK.Input.MouseButton Button { get; set; }
        public int Clicks { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Delta { get; set; }
        public Point Location { get; set; }

        public ViewportEvent(ViewportBase sender, EventArgs e)
        {
            Sender = sender;
        }

        public ViewportEvent(ViewportBase sender)
        {
            Sender = sender;
        }

        public ViewportEvent(ViewportBase sender, double x, double y)
        {
            Sender = sender;
            X = (int)x;
            Y = (int)y;
            Location = new Point(X, Y);
        }

        public ViewportEvent(ViewportBase sender, double x, double y, ScrollDirection direction)
            : this(sender, x, y)
        {
            Delta = direction == ScrollDirection.Up ? -1 : 1;
        }

        public ViewportEvent(ViewportBase sender, double x, double y, uint mouseButton)
            : this(sender, x, y)
        {
            switch (mouseButton)
            {
                case 1:
                    Button = MouseButton.Left;
                    break;
                case 2:
                    Button = MouseButton.Middle;
                    break;
                case 3:
                    Button = MouseButton.Right;
                    break;
                case 4:
                    Button = MouseButton.Button1;
                    break;
                case 5:
                    Button = MouseButton.Button2;
                    break;

            }
        }

        public ViewportEvent(ViewportBase sender, char key)
        {
            Sender = sender;
            KeyChar = key;
        }

        public ViewportEvent(ViewportBase sender, Gdk.Key key)
        {
            Sender = sender;
            //KeyCode = key;
        }
    }
}