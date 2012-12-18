using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.History
{
    public class HistoryCreate : IHistoryItem
    {
        private readonly List<MapObject> _createdObjects;

        public HistoryCreate(IEnumerable<MapObject> createdObjects)
        {
            _createdObjects = new List<MapObject>(createdObjects);
        }

        public void Undo(Map map)
        {
            _createdObjects.ForEach(x => x.Parent.Children.Remove(x));
        }

        public void Redo(Map map)
        {
            _createdObjects.ForEach(x => x.Parent.Children.Add(x));
        }

        public void Dispose()
        {
            _createdObjects.Clear();
        }
    }
}