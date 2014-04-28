using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Selection
{
    public class ChangeSelection : IAction
    {
        public bool SkipInStack { get { return true; } } // todo 
        public bool ModifiesState { get { return false; } }

        private List<MapObject> _selected;
        private List<MapObject> _deselected;

        public ChangeSelection(IEnumerable<MapObject> selected, IEnumerable<MapObject> deselected)
        {
            _selected = selected.ToList();
            _deselected = deselected.ToList();
        }

        public void Dispose()
        {
            _selected = _deselected = null;
        }

        public void Reverse(Document document)
        {
            document.Selection.Select(_deselected.Where(x => x.BoundingBox != null));
            document.Selection.Deselect(_selected);

            Mediator.Publish(EditorMediator.DocumentTreeSelectedObjectsChanged, _selected.Union(_deselected));
            Mediator.Publish(EditorMediator.SelectionChanged);
        }

        public void Perform(Document document)
        {
            document.Selection.Deselect(_deselected);
            document.Selection.Select(_selected.Where(x => x.BoundingBox != null));

            Mediator.Publish(EditorMediator.DocumentTreeSelectedObjectsChanged, _selected.Union(_deselected));
            Mediator.Publish(EditorMediator.SelectionChanged);
        }
    }
}