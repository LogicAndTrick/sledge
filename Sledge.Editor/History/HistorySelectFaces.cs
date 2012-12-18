using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.History
{
    public class HistorySelectFaces : IHistoryItem
    {
        private readonly List<Face> _selectedFaces;

        public HistorySelectFaces(IEnumerable<Face> selectedFaces)
        {
            _selectedFaces = new List<Face>(selectedFaces);
        }

        public void Undo(Map map)
        {
            _selectedFaces.ForEach(x => x.IsSelected = !x.IsSelected);
        }

        public void Redo(Map map)
        {
            _selectedFaces.ForEach(x => x.IsSelected = !x.IsSelected);
        }

        public void Dispose()
        {
            _selectedFaces.Clear();
        }
    }
}