using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Operations
{
    /// <summary>
    /// Perform: Adds the given objects to the map
    /// Reverse: Removes the objects from the map
    /// </summary>
    public class Create : IAction
    {
        private List<long> _ids;
        private List<MapObject> _objects;

        public Create(List<MapObject> objects)
        {
            _objects = objects;
        }

        public void Dispose()
        {
            _ids = null;
            _objects = null;
        }

        public void Reverse(Document document)
        {
            _objects = document.Map.WorldSpawn.Find(x => _ids.Contains(x.ID));
            _objects.ForEach(x => x.SetParent(null));
            _ids = null;
        }

        public void Perform(Document document)
        {
            _ids = _objects.Select(x => x.ID).ToList();
            _objects.ForEach(x => x.SetParent(document.Map.WorldSpawn));
            _objects = null;
        }
    }
}