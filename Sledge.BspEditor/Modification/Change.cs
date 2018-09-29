using System.Collections.Generic;
using System.Linq;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Modification
{
    /// <summary>
    /// Represents a change set as a result of an operation on a document.
    /// </summary>
    public class Change
    {
        /// <summary>
        /// The document modified by the change
        /// </summary>
        public MapDocument Document { get; }

        private readonly HashSet<IMapData> _affectedData;

        private readonly HashSet<IMapObject> _added;
        private readonly HashSet<IMapObject> _updated;
        private readonly HashSet<IMapObject> _removed;

        /// <summary>
        /// The items that were added during the change
        /// </summary>
        public IEnumerable<IMapObject> Added => _added;

        /// <summary>
        /// The items that were updated during the change
        /// </summary>
        public IEnumerable<IMapObject> Updated => _updated;

        /// <summary>
        /// The items that were removed during the change
        /// </summary>
        public IEnumerable<IMapObject> Removed => _removed;

        /// <summary>
        /// The map data objects which were affected during the change
        /// </summary>
        public IEnumerable<IMapData> AffectedData => _affectedData;

        /// <summary>
        /// True if there are object changes in this change
        /// </summary>
        public bool HasObjectChanges => _added.Count + _updated.Count + _removed.Count > 0;

        /// <summary>
        /// True if there are map data changes in this change
        /// </summary>
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