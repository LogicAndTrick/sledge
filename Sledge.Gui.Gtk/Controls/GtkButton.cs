using System;
using Gtk;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces.Controls;

namespace Sledge.Gui.Gtk.Controls
{
    [ControlImplementation("GTK")]
    public class GtkButton : GtkTextControl, IButton
    {
        private Button _button;
        
        public GtkButton() : base(new Button())
        {
            _button = (Button) Control;
        }

        protected override string ControlText
        {
            get { return _button.Label; }
            set { _button.Label = value; }
        }

        public event EventHandler Clicked
        {
            add { _button.Activated += value; }
            remove { _button.Activated -= value; }
        }
    }
}