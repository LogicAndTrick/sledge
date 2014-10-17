using Sledge.Gui.Attributes;

namespace Sledge.Gui.Interfaces
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