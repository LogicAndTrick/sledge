using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sledge.UI
{
    public class ViewportEvent : EventArgs
    {
        public ViewportBase Sender { get; set; }

        public bool Handled { get; set; }

        // Key
        public Keys Modifiers { get; set; }
        public bool Control { get; set; }
        public bool Shift { get; set; }
        public bool Alt { get; set; }
        public Keys KeyCode { get; set; }
        public int KeyValue { get; set; }
        public char KeyChar { get; set; }

        // Mouse
        public MouseButtons Button { get; set; }
        public int Clicks { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Delta { get; set; }
        public Point Location { get; set; }

        public ViewportEvent(ViewportBase sender, KeyEventArgs e)
        {
            Sender = sender;
            Modifiers = e.Modifiers;
            Control = e.Control;
            Shift = e.Shift;
            Alt = e.Alt;
            KeyCode = e.KeyCode;
            KeyValue = e.KeyValue;
        }

        public ViewportEvent(ViewportBase sender, KeyPressEventArgs e)
        {
            Sender = sender;
            KeyChar = e.KeyChar;
        }
        
        public ViewportEvent(ViewportBase sender, MouseEventArgs e)
        {
            Sender = sender;
            Button = e.Button;
            Clicks = e.Clicks;
            X = e.X;
            Y = e.Y;
            Delta = e.Delta;
            Location = e.Location;
        }

        public ViewportEvent(ViewportBase sender, EventArgs e)
        {
            Sender = sender;
        }
    }
}