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

        public Change(MapDocument document)
        {
            Document = document;
            _added = new HashSet<IMapObject>();
            _updated = new HashSet<IMapObject>();
            _removed = new HashSet<IMapObject>();
        }

        public Change(MapDocument document, IEnumerable<IMapObject> added, IEnumerable<IMapObject> updated, IEnumerable<IMapObject> removed)
        {
            Document = document;
            _added = new HashSet<IMapObject>(added);
            _updated = new HashSet<IMapObject>(updated);
            _removed = new HashSet<IMapObject>(removed);
        }

        public Change Add(IMapObject o)
        {
            _added.Add(o);
            _removed.Remove(o);
            _updated.Remove(o);
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

        public Change Merge(Change change)
        {
            _added.UnionWith(change._added);
            _removed.ExceptWith(change._added);
            _updated.ExceptWith(change._added);

            _added.ExceptWith(change._removed);
            _updated.ExceptWith(change._removed);
            _removed.UnionWith(change._removed);

            _updated.UnionWith(change._updated.Except(_added).Except(_removed));
            return this;
        }
    }
}