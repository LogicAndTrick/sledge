using System;
using System.Drawing;
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
        private readonly Button _button;

        public WinFormsButton()
            : base(new Button())
        {
            _button = (Button)Control;
        }

        protected override Size DefaultPreferredSize
        {
            get
            {
                return new Size(100, FontSize * 2);
            }
        }

        public Image Image
        {
            get { return _button.Image; }
            set { _button.Image = value; }
        }

        public event EventHandler Clicked
        {
            add { Control.Click += value; }
            remove { Control.Click -= value; }
        }
    }
}