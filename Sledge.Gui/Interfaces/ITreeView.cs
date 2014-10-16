using Sledge.Gui.Attributes;

namespace Sledge.Gui.Interfaces
{
    [ControlInterface]
    public interface ITreeView : IControl
    {
        ITreeModel Model { get; set; }
        bool ShowCheckboxes { get; set; }
    }
}