using Gtk;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces.Containers;

namespace Sledge.Gui.Gtk.Containers
{
    [ControlImplementation("GTK")]
    public class GtkCollapsible : GtkCell, ICollapsible
    {
        private readonly Expander _expander;

        public string Identifier { get; set; }

        public string Text
        {
            get { return _expander.Label; }
            set { _expander.Label = value; }
        }

        public bool IsCollapsed
        {
            get { return !_expander.Expanded; }
            set { _expander.Expanded = !value; }
        }

        public bool CanCollapse
        {
            get { return _expander.Sensitive; }
            set { _expander.Sensitive = value; }
        }

        public bool HideHeading
        {
            get { return _expander.LabelWidget.Visible; }
            set { _expander.LabelWidget.Visible = value; }
        }

        public GtkCollapsible() : base(new Expander(""))
        {
            _expander = (Expander) Container;
        }

        protected override void CalculateLayout()
        {

        }
    }
}