using System;

namespace Sledge.Gui.Events
{
    public class HandledEventArgs : EventArgs
    {
        public bool Handled { get; set; }

        public HandledEventArgs()
        {
            Handled = false;
        }
    }
}