using System;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Controls;

namespace Sledge.Gui.WinForms.Controls
{
    [ControlImplementation("WinForms")]
    public class WinFormsButton : WinFormsControl, IButton
    {
        private Button _button;

        public string Text
        {
            get { return _button.Text; }
            set { _button.Text = value; }
        }

        public bool Enabled
        {
            get { return _button.Enabled; }
            set { _button.Enabled = value; }
        }

        public event EventHandler Clicked
        {
            add { _button.Click += value; }
            remove { _button.Click -= value; }
        }

        public WinFormsButton() : base(new Button())
        {
            _button = (Button) Control;
        }

        public override Size PreferredSize
        {
            get
            {
                return new Size(100, 25);
            }
        }
    }
}