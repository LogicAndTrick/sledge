using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Containers;
using Sledge.Gui.Structures;

namespace Sledge.Gui.Gtk.Containers
{
    [ControlImplementation("GTK")]
    public class GtkResizableTable : GtkTable, IResizableTable
    {
        public int MinimumViewSize { get; set; }
        public ResizableTableConfiguration Configuration { get; set; }

        public GtkResizableTable()
        {
            Configuration = ResizableTableConfiguration.Default();
        }

        public void ResetViews()
        {

        }

        public void FocusOn(IControl ctrl)
        {

        }

        public void FocusOn(int rowIndex, int columnIndex)
        {

        }

        public void Unfocus()
        {

        }

        public bool IsFocusing()
        {
            return false;
        }
    }
}