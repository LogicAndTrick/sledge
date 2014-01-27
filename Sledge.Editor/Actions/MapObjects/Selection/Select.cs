using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Selection
{
    public class Select : IAction
    {
        private List<MapObject> _objects;

        public Select(IEnumerable<MapObject> objects)
        {
            _objects = objects.ToList();
        }

        public Select(params MapObject[] objects)
        {
            _objects = objects.ToList();
        }

        public void Dispose()
        {
            _objects = null;
        }

        public void Reverse(Document document)
        {
            document.Selection.Deselect(_objects);

            Mediator.Publish(EditorMediator.DocumentTreeObjectsChanged, _objects);
            Mediator.Publish(EditorMediator.SelectionChanged);
        }

        public void Perform(Document document)
        {
            document.Selection.Select(_objects.Where(x => x.BoundingBox != null));

            Mediator.Publish(EditorMediator.DocumentTreeObjectsChanged, _objects);
            Mediator.Publish(EditorMediator.SelectionChanged);
        }
    }
}