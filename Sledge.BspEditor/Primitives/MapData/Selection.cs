using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Threading;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapData
{
    public class Selection : IMapData, IEnumerable<IMapObject>
    {
        public bool AffectsRendering => false;

        private readonly ISet<IMapObject> _selectedObjects;

        public Selection()
        {
            _selectedObjects = new ThreadSafeSet<IMapObject>();
        }

        public Selection(SerialisedObject obj)
        {
            _selectedObjects = new ThreadSafeSet<IMapObject>();
        }

        [Export(typeof(IMapElementFormatter))]
        public class SelectionFormatter : StandardMapElementFormatter<Selection> { }

        public bool IsEmpty => _selectedObjects.Count == 0;
        public int Count => _selectedObjects.Count;

        /// <summary>
        /// Update the selection based on a change.
        /// </summary>
        /// <param name="change"></param>
        /// <returns>True if the selection has changed after updating.</returns>
        public bool Update(Change change)
        {
            var changed = false;

            // Each of these operations is only adding or removing items,
            // so checking the count to detect changes is fine.

            var c = _selectedObjects.Count;
            _selectedObjects.UnionWith(change.Added.Where(x => x.IsSelected));
            _selectedObjects.UnionWith(change.Updated.Where(x => x.IsSelected));
            changed |= c != _selectedObjects.Count;

            c = _selectedObjects.Count;
            _selectedObjects.ExceptWith(change.Added.Where(x => !x.IsSelected));
            _selectedObjects.ExceptWith(change.Updated.Where(x => !x.IsSelected));
            _selectedObjects.ExceptWith(change.Removed);
            changed |= c != _selectedObjects.Count;

            return changed;
        }

        /// <summary>
        /// Update the selection to match the object state in the document
        /// </summary>
        /// <param name="document">The documnet</param>
        /// <returns>True if the selection has changed after updating</returns>
        public bool Update(MapDocument document)
        {
            var changed = false;

            var selected = document.Map.Root.Find(x => x.IsSelected).ToList();

            if (_selectedObjects.Count != selected.Count) changed = true;
            _selectedObjects.ExceptWith(selected);
            if (_selectedObjects.Count != 0) changed = true;
            _selectedObjects.Clear();
            _selectedObjects.UnionWith(selected);


            return changed;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Nothing.
        }

        public IMapElement Clone()
        {
            var c = new Selection();
            c._selectedObjects.UnionWith(_selectedObjects);
            return c;
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject("Selection");
            so.Set("SelectedObjects", String.Join(",", _selectedObjects.Select(x => Convert.ToString(x.ID, CultureInfo.InvariantCulture))));
            return so;
        }

        public IEnumerator<IMapObject> GetEnumerator()
        {
            return _selectedObjects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Box GetSelectionBoundingBox()
        {
            return IsEmpty ? Box.Empty : new Box(_selectedObjects.Select(x => x.BoundingBox).Where(x => x != null).DefaultIfEmpty(Box.Empty));
        }

        public IEnumerable<IMapObject> GetSelectedParents()
        {
            var sel = _selectedObjects.ToList();
            sel.SelectMany(x => x.Hierarchy).ToList().ForEach(x => sel.Remove(x));
            return sel.Where(x => x.Hierarchy.Parent != null);
        }
    }
}
