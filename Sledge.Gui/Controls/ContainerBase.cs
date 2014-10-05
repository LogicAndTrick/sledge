using System.Collections.Generic;
using Sledge.Gui.Interfaces;

namespace Sledge.Gui.Controls
{
    public class ContainerBase<T> : ControlBase<T>, IContainer where T : IContainer
    {
        public int NumChildren
        {
            get { return Control.NumChildren; }
        }

        public IEnumerable<IControl> Children
        {
            get { return Control.Children; }
        }

        public void Insert(int index, IControl child)
        {
            Control.Insert(index, child);
        }

        public void Insert(int index, IControl child, ContainerMetadata metadata)
        {
            Control.Insert(index, child, metadata);
        }

        public Padding Margin { get; set; }
    }
}