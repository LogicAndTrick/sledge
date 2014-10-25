using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces.Models;

namespace Sledge.Gui.Interfaces.Controls
{
    [ControlInterface]
    public interface ITreeView : IControl
    {
        ITreeModel Model { get; set; }
        bool ShowCheckboxes { get; set; }
    }
}