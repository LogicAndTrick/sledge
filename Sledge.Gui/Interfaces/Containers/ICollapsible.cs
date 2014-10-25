using Sledge.Gui.Attributes;

namespace Sledge.Gui.Interfaces.Containers
{
    [ControlInterface]
    public interface ICollapsible : ICell
    {
        string Identifier { get; set; }
        string Text { get; set; }
        bool IsCollapsed { get; set; }
        bool CanCollapse { get; set; }
        bool HideHeading { get; set; }
    }
}