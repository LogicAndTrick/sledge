using System.Collections.Generic;
using System.Linq;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Modification
{
    public class Change
    {
        public MapDocument Document { get; }
        private readonly HashSet<IMapObject> _added;
        private readonly HashSet<IMapObject> _updated;
        private readonly HashSet<IMapObject> _removed;

        public IEnumerable<IMapObject> Added => _added;
        public IEnumerable<IMapObject> Updated => _updated;
        public IEnumerable<IMapObject> Removed => _removed;
        public bool DocumentUpdated { get; private set; }

        public bool HasObjectChanges => _added.Count + _updated.Count + _removed.Count > 0;

        public Change(MapDocument document)
        {
            Document = document;
            _added = new HashSet<IMapObject>();
            _updated = new HashSet<IMapObject>();
            _removed = new HashSet<IMapObject>();
            DocumentUpdated = false;
        }

        public Change(MapDocument document, IEnumerable<IMapObject> added, IEnumerable<IMapObject> updated, IEnumerable<IMapObject> removed)
        {
            Document = document;
            _added = new HashSet<IMapObject>(added);
            _updated = new HashSet<IMapObject>(updated);
            _removed = new HashSet<IMapObject>(removed);
            DocumentUpdated = false;
        }

        public Change UpdateDocument()
        {
            DocumentUpdated = true;
            return this;
        }

        public Change Add(IMapObject o)
        {
            _added.Add(o);
            _removed.Remove(o);
            _updated.Remove(o);
            return this;
        }

        public Change AddRange(IEnumerable<IMapObject> objects)
        {
            var all = objects.ToList();
            _added.UnionWith(all);
            _removed.ExceptWith(all);
            _updated.ExceptWith(all);
            return this;
        }

        public Change Update(IMapObject o)
        {
            if (_added.Contains(o)) return this;
            if (_removed.Contains(o)) return this;
            _updated.Add(o);
            return this;
        }

        public Change Remove(IMapObject o)
        {
            _added.Remove(o);
            _updated.Remove(o);
            _removed.Add(o);
            return this;
        }

        public Change RemoveRange(IEnumerable<IMapObject> objects)
        {
            var all = objects.ToList();
            _added.ExceptWith(all);
            _updated.ExceptWith(all);
            _removed.UnionWith(all);
            return this;
        }

        public Change Merge(Change change)
        {
            _added.UnionWith(change._added);
            _removed.ExceptWith(change._added);
            _updated.ExceptWith(change._added);

            _added.ExceptWith(change._removed);
            _updated.ExceptWith(change._removed);
            _removed.UnionWith(change._removed);

            _updated.UnionWith(change._updated.Except(_added).Except(_removed));

            DocumentUpdated |= change.DocumentUpdated;

            return this;
        }
    }
}