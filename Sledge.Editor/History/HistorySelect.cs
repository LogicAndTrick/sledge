using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.History
{
    public class HistorySelect : IHistoryItem
    {
        private readonly List<MapObject> _selectedObjects;

        public HistorySelect(IEnumerable<MapObject> selectedObjects)
        {
            _selectedObjects = new List<MapObject>(selectedObjects);
        }

        public void Undo(Map map)
        {
            _selectedObjects.ForEach(x => x.IsSelected = !x.IsSelected);
        }

        public void Redo(Map map)
        {
            _selectedObjects.ForEach(x => x.IsSelected = !x.IsSelected);
        }

        public void Dispose()
        {
            _selectedObjects.Clear();
        }
    }
}