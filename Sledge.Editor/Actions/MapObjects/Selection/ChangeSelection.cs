using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Selection
{
    public class ChangeSelection : IAction
    {
        private List<MapObject> _selected;
        private List<MapObject> _deselected;

        public ChangeSelection(List<MapObject> selected, List<MapObject> deselected)
        {
            _selected = selected;
            _deselected = deselected;
        }

        public void Dispose()
        {
            _selected = _deselected = null;
        }

        public void Reverse(Document document)
        {
            document.Selection.Deselect(_selected);
            document.Selection.Select(_deselected);
        }

        public void Perform(Document document)
        {
            document.Selection.Select(_selected);
            document.Selection.Deselect(_deselected);
        }
    }
}