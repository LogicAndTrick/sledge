using System.Collections.Generic;
using System.Linq;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Modification
{
    public class Change
    {
        public MapDocument Document { get; }
        private readonly HashSet<IMapData> _affectedData;

        private readonly HashSet<IMapObject> _added;
        private readonly HashSet<IMapObject> _updated;
        private readonly HashSet<IMapObject> _removed;

        public IEnumerable<IMapObject> Added => _added;
        public IEnumerable<IMapObject> Updated => _updated;
        public IEnumerable<IMapObject> Removed => _removed;

        public IEnumerable<IMapData> AffectedData => _affectedData;

        public bool HasObjectChanges => _added.Count + _updated.Count + _removed.Count > 0;
        public bool HasDataChanges => _affectedData.Count > 0;

        public Change(MapDocument document)
        {
            Document = document;

            _added = new HashSet<IMapObject>();
            _updated = new HashSet<IMapObject>();
            _removed = new HashSet<IMapObject>();

            _affectedData = new HashSet<IMapData>();
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

        public Change UpdateRange(IEnumerable<IMapObject> objects)
        {
            var all = objects.ToList();
            _updated.UnionWith(all);
            _added.ExceptWith(all);
            _removed.ExceptWith(all);
            return this;
        }

        public Change Update(IMapData data)
        {
            _affectedData.Add(data);
            return this;
        }

        public Change Update(IEnumerable<IMapData> datas)
        {
            _affectedData.UnionWith(datas);
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

            _affectedData.UnionWith(change._affectedData);

            return this;
        }
    }
}