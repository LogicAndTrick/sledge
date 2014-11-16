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

        public Coordinate Location
        {
            get { return new Coordinate(X, Y, 0); }
        }

        // Mouse movement
        public int LastX { get; set; }
        public int LastY { get; set; }

        public int DeltaX
        {
            get { return X - LastX; }
        }

        public int DeltaY
        {
            get { return Y - LastY; }
        }

        // Click and drag
        public bool Dragging { get; set; }
        public int StartX { get; set; }
        public int StartY { get; set; }

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
            Delta = e.Delta;
            Button = e.Button;
        }

        public ViewportEvent(IViewport sender, IKeyboardEvent e)
        {
            Sender = sender;
            KeyValue = e.KeyValue;
            Shift = e.Shift;
            Control = e.Control;
            Alt = e.Alt;
        }
    }
}