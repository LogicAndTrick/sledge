using Sledge.Gui.Attributes;

namespace Sledge.Gui.Controls
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