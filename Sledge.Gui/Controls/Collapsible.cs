using Sledge.Gui.Interfaces;

namespace Sledge.Gui.Controls
{
    public class Collapsible : CellBase<ICollapsible>, ICollapsible
    {
        public string Identifier
        {
            get { return Control.Identifier; }
            set { Control.Identifier = value; }
        }

        public string Text
        {
            get { return Control.Text; }
            set { Control.Text = value; }
        }

        public bool IsCollapsed
        {
            get { return Control.IsCollapsed; }
            set { Control.IsCollapsed = value; }
        }

        public bool CanCollapse
        {
            get { return Control.CanCollapse; }
            set { Control.CanCollapse = value; }
        }

        public bool HideHeading
        {
            get { return Control.HideHeading; }
            set { Control.HideHeading = value; }
        }
    }
}