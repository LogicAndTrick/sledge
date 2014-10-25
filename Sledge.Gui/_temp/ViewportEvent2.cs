using System;
using OpenTK.Input;
using Sledge.Gui.Interfaces;
using Point = System.Drawing.Point;

namespace Sledge.Gui.Events
{
    public delegate void MouseEventHandler(object sender, IMouseEvent e);
    public delegate void KeyboardEventHandler(object sender, IKeyboardEvent e);

    public interface IEvent
    {
        IControl Sender { get; }
        bool Handled { get; set; }
    }

    public interface IMouseEvent : IEvent
    {
        MouseButton Button { get; }
        int Clicks { get; }
        int X { get; }
        int Y { get; }
        int Delta { get; }
        Point Location { get; }
    }

    public interface IKeyboardEvent : IEvent
    {
        bool Control { get; }
        bool Shift { get; }
        bool Alt { get; }
        Key KeyValue { get; }
    }

    public class ControlEvent : EventArgs, IMouseEvent, IKeyboardEvent
    {
        public IControl Sender { get; set; }

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
        public Point Location { get; set; }

        public ControlEvent(IControl sender, EventArgs e)
        {
            Sender = sender;
        }

        public ControlEvent(IControl sender)
        {
            Sender = sender;
        }

        public ControlEvent(IControl sender, double x, double y)
        {
            Sender = sender;
            X = (int)x;
            Y = (int)y;
            Location = new Point(X, Y);
        }

        public ControlEvent(IControl sender, double x, double y, int delta)
            : this(sender, x, y)
        {
            Delta = delta;
        }

        public ControlEvent(IControl sender, double x, double y, MouseButton mouseButton) : this(sender, x, y)
        {
            Button = mouseButton;
        }

        public ControlEvent(IControl sender, Key key, bool control, bool shift, bool alt)
        {
            Sender = sender;
            KeyValue = key;
            Shift = shift;
            Control = control;
            Alt = alt;
        }
    }
}