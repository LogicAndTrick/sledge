using System.Collections.Generic;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Selection
{
    public class Deselect : IAction
    {
        private List<MapObject> _objects;

        public Deselect(List<MapObject> objects)
        {
            _objects = objects;
        }

        public void Dispose()
        {
            _objects = null;
        }

        public void Reverse(Document document)
        {
            document.Selection.Select(_objects);

            Mediator.Publish(EditorMediator.DocumentTreeObjectsChanged, _objects);
            Mediator.Publish(EditorMediator.SelectionChanged);
        }

        public void Perform(Document document)
        {
            document.Selection.Deselect(_objects);

            Mediator.Publish(EditorMediator.DocumentTreeObjectsChanged, _objects);
            Mediator.Publish(EditorMediator.SelectionChanged);
        }
    }
}