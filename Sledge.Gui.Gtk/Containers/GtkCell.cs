using Gtk;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Containers;

namespace Sledge.Gui.Gtk.Containers
{
    [ControlImplementation("GTK")]
    public class GtkCell : GtkContainer, ICell
    {
        public GtkCell() : base(new Table(1, 1, true))
        {
            
        }

        internal GtkCell(Container container) : base(container)
        {
        }

        protected override void CalculateLayout()
        {

        }

        public void Set(IControl child)
        {
            Insert(0, child);
        }
    }
}