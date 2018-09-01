using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Modification.Operations.Tree
{
    public class Detatch : IOperation
    {
        private long _parentId;
        private List<long> _idsToDetatch;
        private List<IMapObject> _detatchedObjects;
        public bool Trivial => false;

        public Detatch(long parentId, params IMapObject[] objectsToDetatch)
        {
            _parentId = parentId;
            _idsToDetatch = objectsToDetatch.Select(x => x.ID).ToList();
        }

        public Detatch(long parentId, IEnumerable<IMapObject> objectsToDetatch)
        {
            _parentId = parentId;
            _idsToDetatch = objectsToDetatch.Select(x => x.ID).ToList();
        }

        public async Task<Change> Perform(MapDocument document)
        {
            var ch = new Change(document);

            var par = document.Map.Root.FindByID(_parentId);
            _detatchedObjects = _idsToDetatch.Select(x => par.FindByID(x)).Where(x => x != null).ToList();

            foreach (var o in _detatchedObjects)
            {
                // Remove parent
                ch.Remove(o);

                // Remove all descendants
                ch.RemoveRange(o.FindAll());

                o.Hierarchy.Parent = null;
            }
            _idsToDetatch = null;

            return ch;
        }

        public async Task<Change> Reverse(MapDocument document)
        {
            var ch = new Change(document);

            var par = document.Map.Root.FindByID(_parentId);
            _idsToDetatch = _detatchedObjects.Select(x => x.ID).ToList();

            foreach (var o in _detatchedObjects)
            {
                // Add parent
                ch.Add(o);

                // Add all descendants
                ch.AddRange(o.FindAll());
                
                o.Hierarchy.Parent = par;
            }
            _detatchedObjects = null;

            return ch;
        }
    }
}