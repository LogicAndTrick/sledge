using Sledge.Gui.Attributes;

namespace Sledge.Gui.Interfaces.Containers
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