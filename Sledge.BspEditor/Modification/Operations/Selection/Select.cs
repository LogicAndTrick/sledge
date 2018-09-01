using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Modification.Operations.Selection
{
    public class Select : IOperation
    {
        private readonly List<long> _idsToSelect;
        public bool Trivial => false;

        public Select(params IMapObject[] objectsToSelect)
        {
            _idsToSelect = objectsToSelect.Where(x => !x.IsSelected).Select(x => x.ID).ToList();
        }

        public Select(IEnumerable<IMapObject> objectsToSelect)
        {
            _idsToSelect = objectsToSelect.Where(x => !x.IsSelected).Select(x => x.ID).ToList();
        }

        public async Task<Change> Perform(MapDocument document)
        {
            var ch = new Change(document);

            foreach (var id in _idsToSelect)
            {
                var o = document.Map.Root.FindByID(id);
                if (o != null)
                {
                    o.IsSelected = true;
                    ch.Update(o);
                }
            }

            return ch;
        }

        public async Task<Change> Reverse(MapDocument document)
        {
            var ch = new Change(document);

            foreach (var id in _idsToSelect)
            {
                var o = document.Map.Root.FindByID(id);
                if (o != null)
                {
                    o.IsSelected = false;
                    ch.Update(o);
                }
            }

            return ch;
        }
    }
}