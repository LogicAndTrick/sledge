using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Selection
{
    public class ChangeSelection : IAction
    {
        public bool SkipInStack { get { return Sledge.Settings.Select.SkipSelectionInUndoStack; } } 
        public bool ModifiesState { get { return false; } }

        private List<long> _selected;
        private List<long> _deselected;

        public ChangeSelection(IEnumerable<MapObject> selected, IEnumerable<MapObject> deselected)
        {
            _selected = selected.Select(x => x.ID).ToList();
            _deselected = deselected.Select(x => x.ID).ToList();
        }

        public ChangeSelection(IEnumerable<long> selected, IEnumerable<long> deselected)
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
            var sel = _selected.Select(x => document.Map.WorldSpawn.FindByID(x)).Where(x => x != null).ToList();
            var desel = _deselected.Select(x => document.Map.WorldSpawn.FindByID(x)).Where(x => x != null && x.BoundingBox != null).ToList();

            document.Selection.Select(desel);
            document.Selection.Deselect(sel);

            Mediator.Publish(EditorMediator.DocumentTreeSelectedObjectsChanged, sel.Union(desel));
            Mediator.Publish(EditorMediator.SelectionChanged);
        }

        public void Perform(Document document)
        {
            var desel = _deselected.Select(x => document.Map.WorldSpawn.FindByID(x)).Where(x => x != null).ToList();
            var sel = _selected.Select(x => document.Map.WorldSpawn.FindByID(x)).Where(x => x != null && x.BoundingBox != null).ToList();

            document.Selection.Deselect(desel);
            document.Selection.Select(sel);

            Mediator.Publish(EditorMediator.DocumentTreeSelectedObjectsChanged, sel.Union(desel));
            Mediator.Publish(EditorMediator.SelectionChanged);
        }
    }
}