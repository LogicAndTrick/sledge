using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.History
{
    /// <summary>
    /// Objects have been selected or deselected.
    /// </summary>
    public class HistorySelect : IHistoryItem
    {
        public string Name { get; private set; }
        private readonly List<MapObject> _objects;
        private readonly bool _selected;

        /// <summary>
        /// Create a history select item.
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <param name="objects">The objects that were selected/deselected</param>
        /// <param name="selected">True if the objects were selected, false if they were deselected.</param>
        public HistorySelect(string name, IEnumerable<MapObject> objects, bool selected)
        {
            Name = name;
            _objects = new List<MapObject>(objects);
            _selected = selected;
        }

        public void Undo(Map map)
        {
            _objects.ForEach(x => x.IsSelected = !_selected);
        }

        public void Redo(Map map)
        {
            _objects.ForEach(x => x.IsSelected = _selected);
        }

        public void Dispose()
        {
            _objects.Clear();
        }
    }
}