using Sledge.Gui.Attributes;
using Sledge.Gui.Events;
using Sledge.Gui.Interfaces.Models;
using Sledge.Gui.Structures;

namespace Sledge.Gui.Interfaces.Controls
{
    [ControlInterface]
    public interface ITabStrip : IControl
    {
        event TabEventHandler TabCloseRequested;
        event TabEventHandler TabSelected;

        ITab SelectedTab { get; set; }
        int SelectedIndex { get; set; }

        int NumTabs { get; }
        ItemList<ITab> Tabs { get; }
    }
}