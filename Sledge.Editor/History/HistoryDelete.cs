using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.History
{
    public class HistoryDelete : IHistoryItem
    {
        private readonly List<MapObject> _deletedObjects;

        public HistoryDelete(IEnumerable<MapObject> deletedObjects)
        {
            _deletedObjects = new List<MapObject>(deletedObjects);
        }

        public void Undo(Map map)
        {
            _deletedObjects.ForEach(x => x.Parent.Children.Add(x));
        }

        public void Redo(Map map)
        {
            _deletedObjects.ForEach(x => x.Parent.Children.Remove(x));
        }

        public void Dispose()
        {
            _deletedObjects.Clear();
        }
    }
}