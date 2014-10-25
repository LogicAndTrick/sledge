using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Containers;

namespace Sledge.Gui.Containers
{
    public class CellBase<T> : ContainerBase<T>, ICell where T : ICell
    {
        public void Set(IControl child)
        {
            Control.Set(child);
        }
    }
}