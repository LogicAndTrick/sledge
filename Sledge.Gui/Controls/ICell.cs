using Sledge.Gui.Attributes;

namespace Sledge.Gui.Controls
{
    /// <summary>
    /// Cell: contains a single child that fills the entire available space
    /// </summary>
    [ControlInterface]
    public interface ICell : IContainer
    {
        void Set(IControl child);
    }
}