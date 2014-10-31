using Gtk;
using Sledge.Gui.Attributes;
using Sledge.Gui.Gtk.Controls;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Containers;
using Sledge.Gui.Structures;

namespace Sledge.Gui.Gtk.Containers
{
    [ControlImplementation("GTK")]
    public class GtkVerticalBox : GtkContainer, IVerticalBox
    {
        private readonly VBox _vbox;

        public bool Uniform
        {
            get { return _vbox.Homogeneous; }
            set { _vbox.Homogeneous = value; }
        }

        public int ControlPadding
        {
            get { return _vbox.Spacing; }
            set { _vbox.Spacing = value; }
        }

        public GtkVerticalBox()
            : base(new VBox(false, 5))
        {
            _vbox = (VBox)Container;
        }

        protected override void AppendChild(int index, GtkControl child)
        {
            var meta = Metadata[child];
            var fill = meta.Get<bool>("Fill");
            _vbox.SetChildPacking(child.Control, fill, fill, 0, PackType.Start);
            base.AppendChild(index, child);
        }

        protected override void CalculateLayout()
        {

        }

        public void Insert(int index, IControl child, bool fill)
        {
            Insert(index, child, new ContainerMetadata { { "Fill", fill } });
        }
    }
}