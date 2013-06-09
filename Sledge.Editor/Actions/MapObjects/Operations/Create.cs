using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Operations
{
    /// <summary>
    /// Perform: Adds the given objects to the map, selecting them if required.
    /// Reverse: Removes the objects from the map, deselecting if required.
    /// </summary>
    public class Create : IAction
    {
        private List<long> _ids;
        private List<MapObject> _objects;

        public Create(List<MapObject> objects)
        {
            _objects = objects;
        }

        public Create(params MapObject[] objects)
        {
            _objects = objects.ToList();
        }

        public void Dispose()
        {
            _ids = null;
            _objects = null;
        }

        public void Reverse(Document document)
        {
            _objects = document.Map.WorldSpawn.Find(x => _ids.Contains(x.ID));
            if (_objects.Any(x => x.IsSelected))
            {
                document.Selection.Deselect(_objects.Where(x => x.IsSelected));
            }
            _objects.ForEach(x => x.SetParent(null));
            _ids = null;

            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
        }

        public void Perform(Document document)
        {
            _ids = _objects.Select(x => x.ID).ToList();
            _objects.ForEach(x => x.SetParent(document.Map.WorldSpawn));
            if (_objects.Any(x => x.IsSelected))
            {
                document.Selection.Select(_objects.Where(x => x.IsSelected));
            }
            _objects = null;

            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
        }
    }
}