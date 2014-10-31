using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtk;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces.Controls;

namespace Sledge.Gui.Gtk.Controls
{
    [ControlImplementation("GTK")]
    public class GtkLabel : GtkTextControl, ILabel
    {
        public GtkLabel() : base(new Label(""))
        {
        }

        protected override string ControlText
        {
            get { return ((Label)Control).Text; }
            set { ((Label)Control).Text = value; }
        }
    }
}
