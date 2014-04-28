using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Editor.Tools;

namespace Sledge.Editor.Actions.MapObjects.Selection
{
    public class ChangeFaceSelection : IAction
    {
        public bool SkipInStack { get { return Sledge.Settings.Select.SkipSelectionInUndoStack; } }
        public bool ModifiesState { get { return false; } }

        private List<Face> _selected;
        private List<Face> _deselected;

        public ChangeFaceSelection(IEnumerable<Face> selected, IEnumerable<Face> deselected)
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
            document.Selection.Select(_deselected);
            document.Selection.Deselect(_selected);

            Mediator.Publish(EditorMediator.DocumentTreeSelectedFacesChanged, _selected.Union(_deselected));
            Mediator.Publish(EditorMediator.SelectionChanged);
        }

        public void Perform(Document document)
        {
            document.Selection.Deselect(_deselected);
            document.Selection.Select(_selected);

            Mediator.Publish(EditorMediator.DocumentTreeSelectedFacesChanged, _selected.Union(_deselected));
            Mediator.Publish(EditorMediator.SelectionChanged);
        }
    }
}