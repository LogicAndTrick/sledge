using System;

namespace Sledge.Gui
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