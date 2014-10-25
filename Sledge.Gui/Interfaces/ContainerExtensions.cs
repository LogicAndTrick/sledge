using Sledge.Gui.Interfaces.Containers;
using Sledge.Gui.Structures;

namespace Sledge.Gui.Interfaces
{
    public static class ContainerExtensions
    {
        public static void Add(this IContainer container, IControl child)
        {
            var index = container.NumChildren;
            container.Insert(index, child);
        }
        public static void Add(this IContainer container, IControl child, ContainerMetadata metadata)
        {
            var index = container.NumChildren;
            container.Insert(index, child, metadata);
        }
        public static void Add(this IBox container, IControl child, bool fill)
        {
            var index = container.NumChildren;
            container.Insert(index, child, fill);
        }
    }
}