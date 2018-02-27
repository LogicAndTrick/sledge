using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Providers.Processors
{
    [Export(typeof(IBspSourceProcessor))]
    public class RemoveInvalidObjects : IBspSourceProcessor
    {
        public string OrderHint => "D";

        public Task AfterLoad(MapDocument document)
        {
            // empty groups need to be removed
            var emptyGroups = new Queue<IMapObject>(document.Map.Root.Find(x => x is Group && !x.Hierarchy.HasChildren));
            while (emptyGroups.Any())
            {
                var g = emptyGroups.Dequeue();
                var par = g.Hierarchy.Parent;
                g.Hierarchy.Parent = null;

                // If the parent was a group and the parent is now empty, make sure it gets deleted recursively
                if (par is Group && !par.Hierarchy.HasChildren) emptyGroups.Enqueue(par);
            }
            return Task.FromResult(0);
        }

        public Task BeforeSave(MapDocument document)
        {
            return Task.FromResult(0);
        }
    }
}