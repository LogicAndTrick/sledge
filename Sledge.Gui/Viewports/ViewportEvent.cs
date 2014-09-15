using System;
using OpenTK.Input;
using Sledge.DataStructures.Geometric;

namespace Sledge.Gui.Viewports
{
    public class ViewportEvent : EventArgs
    {
        public IViewport Sender { get; set; }

        public bool Handled { get; set; }

        // Key
        public bool Control { get; set; }
        public bool Shift { get; set; }
        public bool Alt { get; set; }
        public Key KeyValue { get; set; }

        // Mouse
        public MouseButton Button { get; set; }
        public int Clicks { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Delta { get; set; }
        public Coordinate Location { get; set; }

        public ViewportEvent(IViewport sender, EventArgs e)
        {
            Sender = sender;
        }

        public ViewportEvent(IViewport sender)
        {
            Sender = sender;
        }

        public ViewportEvent(IViewport sender, double x, double y)
        {
            Sender = sender;
            X = (int)x;
            Y = (int)y;
            Location = new Coordinate(X, Y, 0);
        }

        public ViewportEvent(IViewport sender, double x, double y, int delta)
            : this(sender, x, y)
        {
            Delta = delta;
        }

        public ViewportEvent(IViewport sender, double x, double y, uint mouseButton)
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

        public ViewportEvent(IViewport sender, Key key, bool control, bool shift, bool alt)
        {
            Sender = sender;
            KeyValue = key;
            Shift = shift;
            Control = control;
            Alt = alt;
        }
    }
}