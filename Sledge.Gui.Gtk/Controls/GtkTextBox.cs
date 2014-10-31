using Gtk;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces.Controls;

namespace Sledge.Gui.Gtk.Controls
{
    [ControlImplementation("GTK")]
    public class GtkTextBox : GtkTextControl, ITextBox
    {
        private Entry _entry;

        public GtkTextBox()
            : base(new Entry(""))
        {
            _entry = (Entry)Control;
        }

        protected override string ControlText
        {
            get { return _entry.Text; }
            set { _entry.Text = value; }
        }
    }
}