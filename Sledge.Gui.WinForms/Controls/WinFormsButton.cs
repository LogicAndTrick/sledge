using System;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Controls;
using Size = Sledge.Gui.Structures.Size;

namespace Sledge.Gui.WinForms.Controls
{
    [ControlImplementation("WinForms")]
    public class WinFormsButton : WinFormsControl, IButton
    {
        public WinFormsButton()
            : base(new Button())
        {

        }

        protected override Size DefaultPreferredSize
        {
            get
            {
                return new Size(100, FontSize * 2);
            }
        }

        public event EventHandler Clicked
        {
            add { Control.Click += value; }
            remove { Control.Click -= value; }
        }
    }
}