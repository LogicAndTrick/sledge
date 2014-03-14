using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Selection
{
    public class SelectFace : IAction
    {
        private List<Face> _objects;

        public SelectFace(IEnumerable<Face> objects)
        {
            _objects = objects.ToList();
        }

        public SelectFace(params Face[] objects)
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

            Mediator.Publish(EditorMediator.DocumentTreeSelectedFacesChanged, _objects);
            Mediator.Publish(EditorMediator.SelectionChanged);
        }

        public void Perform(Document document)
        {
            document.Selection.Select(_objects);

            Mediator.Publish(EditorMediator.DocumentTreeSelectedFacesChanged, _objects);
            Mediator.Publish(EditorMediator.SelectionChanged);
        }
    }
}