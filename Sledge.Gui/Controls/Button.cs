using System;
using System.Drawing;
using Sledge.Gui.Interfaces.Controls;

namespace Sledge.Gui.Controls
{
    public class Button : TextControlBase<IButton>, IButton
    {
        public Image Image
        {
            get { return Control.Image; }
            set { Control.Image = value; }
        }

        public event EventHandler Clicked
        {
            add { Control.Clicked += value; }
            remove { Control.Clicked -= value; }
        }
    }
}