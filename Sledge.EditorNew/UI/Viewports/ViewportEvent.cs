using System;
using OpenTK.Input;
using Sledge.DataStructures.Geometric;
using Sledge.Gui.Events;
using Sledge.Gui.Interfaces.Controls;

namespace Sledge.EditorNew.UI.Viewports
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

        // Click and drag
        public bool Dragging { get; set; }
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int LastX { get; set; }
        public int LastY { get; set; }

        // 2D Camera
        public Coordinate CameraPosition { get; set; }
        public decimal CameraZoom { get; set; }

        public ViewportEvent(IViewport sender, EventArgs e)
        {
            Sender = sender;
        }

        public ViewportEvent(IViewport sender)
        {
            Sender = sender;
        }

        public ViewportEvent(IViewport sender, IMouseEvent e)
        {
            Sender = sender;
            X = e.X;
            Y = e.Y;
            Location = new Coordinate(X, Y, 0);
            Delta = e.Delta;
            Button = e.Button;
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