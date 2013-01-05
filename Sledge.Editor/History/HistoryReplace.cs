using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.History
{
    /// <summary>
    /// A history replace deletes some objects from the map and replaces them with some new ones at the same time.
    /// </summary>
    public class HistoryReplace : IHistoryItem
    {
        public string Name { get; private set; }
        private readonly List<MapObject> _createdObjects;
        private readonly List<MapObject> _deletedObjects;

        public HistoryReplace(string name, IEnumerable<MapObject> deletedObjects, IEnumerable<MapObject> createdObjects)
        {
            Name = name;
            _createdObjects = new List<MapObject>(createdObjects);
            _deletedObjects = new List<MapObject>(deletedObjects);
        }

        public void Undo(Document document)
        {
            _createdObjects.ForEach(x => x.Parent.Children.Remove(x));
            _deletedObjects.ForEach(x => x.Parent.Children.Add(x));
        }

        public void Redo(Document document)
        {
            _deletedObjects.ForEach(x => x.Parent.Children.Remove(x));
            _createdObjects.ForEach(x => x.Parent.Children.Add(x));
        }

        public void Dispose()
        {
            _createdObjects.Clear();
            _deletedObjects.Clear();
        }
    }
}