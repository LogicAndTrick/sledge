using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Modification
{
    public class Attach : IOperation
    {
        private long _parentId;
        private List<IMapObject> _objectsToAttach;
        private List<long> _attachedIds;

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

            _attachedIds = _objectsToAttach.Select(x => x.ID).ToList();
            foreach (var o in _objectsToAttach)
            {
                ch.Add(o);
                o.Hierarchy.Parent = document.Map.Root.FindByID(_parentId);
            }
            _objectsToAttach = null;

            return ch;
        }

        public async Task<Change> Reverse(MapDocument document)
        {
            var ch = new Change(document);
            
            _objectsToAttach = _attachedIds.Select(x => document.Map.Root.FindByID(x)).Where(x => x != null).ToList();
            foreach (var o in _objectsToAttach)
            {
                ch.Remove(o);
                o.Hierarchy.Parent = null;
            }
            _attachedIds = null;

            return ch;
        }
    }
}