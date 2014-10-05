using Sledge.Gui.Interfaces;

namespace Sledge.Gui.Controls
{
    public abstract class BoxBase<T> : ContainerBase<T>, IBox where T : IBox
    {
        public bool Uniform
        {
            get { return Control.Uniform; }
            set { Control.Uniform = value; }
        }

        public int ControlPadding
        {
            get { return Control.ControlPadding; }
            set { Control.ControlPadding = value; }
        }

        public void Insert(int index, IControl child, bool fill)
        {
            Control.Insert(index, child, fill);
        }
    }
}