using Sledge.Gui.Interfaces;

namespace Sledge.Gui.Controls
{
    public class CellBase<T> : ContainerBase<T>, ICell where T : ICell
    {
        public void Set(IControl child)
        {
            Control.Set(child);
        }
    }
}