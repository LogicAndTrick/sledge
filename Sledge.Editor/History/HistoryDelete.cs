using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.History
{
    /// <summary>
    /// A history delete removes objects from the map.
    /// </summary>
    public class HistoryDelete : IHistoryItem
    {
        public string Name { get; private set; }
        private readonly List<MapObject> _deletedObjects;

        public HistoryDelete(string name, IEnumerable<MapObject> deletedObjects)
        {
            Name = name;
            _deletedObjects = new List<MapObject>(deletedObjects);
        }

        public void Undo(Document document)
        {
            _deletedObjects.ForEach(x => x.Parent.Children.Add(x));
        }

        public void Redo(Document document)
        {
            _deletedObjects.ForEach(x => x.Parent.Children.Remove(x));
        }

        public void Dispose()
        {
            _deletedObjects.Clear();
        }
    }
}