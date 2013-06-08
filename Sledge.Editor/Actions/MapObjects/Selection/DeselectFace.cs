using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Selection
{
    public class DeselectFace : IAction
    {
        private List<Face> _objects;

        public DeselectFace(IEnumerable<Face> objects)
        {
            _objects = objects.ToList();
        }

        public DeselectFace(params Face[] objects)
        {
            _objects = objects.ToList();
        }

        public void Dispose()
        {
            _objects = null;
        }

        public void Reverse(Document document)
        {
            document.Selection.Select(_objects);
        }

        public void Perform(Document document)
        {
            document.Selection.Deselect(_objects);
        }
    }
}