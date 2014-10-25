using System.Collections.Generic;
using Sledge.Gui.Structures;

namespace Sledge.Gui.Interfaces
{
    public interface IContainer : IControl
    {
        int NumChildren { get; }
        IEnumerable<IControl> Children { get; }
        void Insert(int index, IControl child);
        void Insert(int index, IControl child, ContainerMetadata metadata);
        Padding Margin { get; set; }
    }
}