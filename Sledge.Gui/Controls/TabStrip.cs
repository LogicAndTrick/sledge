using Sledge.Gui.Events;
using Sledge.Gui.Interfaces.Controls;
using Sledge.Gui.Interfaces.Models;
using Sledge.Gui.Structures;

namespace Sledge.Gui.Controls
{
    public class TabStrip : ControlBase<ITabStrip>, ITabStrip
    {
        public event TabEventHandler TabCloseRequested
        {
            add { Control.TabCloseRequested += value; }
            remove { Control.TabCloseRequested -= value; }
        }

        public event TabEventHandler TabSelected
        {
            add { Control.TabSelected += value; }
            remove { Control.TabSelected -= value; }
        }

        public ITab SelectedTab
        {
            get { return Control.SelectedTab; }
            set { Control.SelectedTab = value; }
        }

        public int SelectedIndex
        {
            get { return Control.SelectedIndex; }
            set { Control.SelectedIndex = value; }
        }

        public int NumTabs
        {
            get { return Control.NumTabs; }
        }

        public ItemList<ITab> Tabs
        {
            get { return Control.Tabs; }
        }
    }
}