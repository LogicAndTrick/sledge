using System;
using Sledge.Gui.Interfaces.Controls;

namespace Sledge.Gui.Controls
{
    public class Button : TextControlBase<IButton>, IButton
    {
        public event EventHandler Clicked
        {
            add { Control.Clicked += value; }
            remove { Control.Clicked -= value; }
        }
    }
}