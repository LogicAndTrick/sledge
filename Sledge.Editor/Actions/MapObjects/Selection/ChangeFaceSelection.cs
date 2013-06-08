using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Selection
{
    public class ChangeFaceSelection : IAction
    {
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
        }

        public void Perform(Document document)
        {
            document.Selection.Deselect(_deselected);
            document.Selection.Select(_selected);
        }
    }
}