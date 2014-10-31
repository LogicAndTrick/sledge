using Gtk;
using Sledge.Gui.Attributes;
using Sledge.Gui.Gtk.Controls;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Containers;
using Sledge.Gui.Structures;

namespace Sledge.Gui.Gtk.Containers
{
    [ControlImplementation("GTK")]
    public class GtkHorizontalBox : GtkContainer, IHorizontalBox
    {
        private readonly HBox _hbox;

        public bool Uniform
        {
            get { return _hbox.Homogeneous; }
            set { _hbox.Homogeneous = value; }
        }

        public int ControlPadding
        {
            get { return _hbox.Spacing; }
            set { _hbox.Spacing = value; }
        }

        public GtkHorizontalBox() : base(new HBox(false, 5))
        {
            _hbox = (HBox)Container;
        }

        protected override void AppendChild(int index, GtkControl child)
        {
            base.AppendChild(index, child);
            var meta = Metadata[child];
            var fill = meta.Get<bool>("Fill");
            _hbox.SetChildPacking(child.Control, fill, true, 0, PackType.Start);
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