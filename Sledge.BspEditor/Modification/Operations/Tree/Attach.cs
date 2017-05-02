using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Modification.Operations.Tree
{
    public class Attach : IOperation
    {
        private long _parentId;
        private List<IMapObject> _objectsToAttach;
        private List<long> _attachedIds;
        public bool Trivial => false;

        public Attach(long parentId, params IMapObject[] objectsToAttach)
        {
            _parentId = parentId;
            _objectsToAttach = objectsToAttach.ToList();
        }

        public Attach(long parentId, IEnumerable<IMapObject> objectsToAttach)
        {
            _parentId = parentId;
            _objectsToAttach = objectsToAttach.ToList();
        }

        public async Task<Change> Perform(MapDocument document)
        {
            var ch = new Change(document);

            var par = document.Map.Root.FindByID(_parentId);
            _attachedIds = _objectsToAttach.Select(x => x.ID).ToList();

            foreach (var o in _objectsToAttach)
            {
                // Add parent
                ch.Add(o);

                // Add all descendants
                ch.AddRange(o.FindAll());

                o.Hierarchy.Parent = par;
            }
            _objectsToAttach = null;

            return ch;
        }

        public async Task<Change> Reverse(MapDocument document)
        {
            var ch = new Change(document);

            var par = document.Map.Root.FindByID(_parentId);
            _objectsToAttach = _attachedIds.Select(x => par.FindByID(x)).Where(x => x != null).ToList();

            foreach (var o in _objectsToAttach)
            {
                // Remove parent
                ch.Remove(o);

                // Remove all descendants
                ch.RemoveRange(o.FindAll());

                o.Hierarchy.Parent = null;
            }
            _attachedIds = null;

            return ch;
        }
    }
}