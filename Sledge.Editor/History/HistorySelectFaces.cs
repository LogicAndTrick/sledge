using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.History
{
    public class HistorySelectFaces : IHistoryItem
    {
        public string Name { get; private set; }
        private readonly List<Face> _faces;
        private readonly bool _selected;

        /// <summary>
        /// Create a history face select item.
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <param name="faces">The faces that were selected/deselected</param>
        /// <param name="selected">True if the faces were selected, false if they were deselected.</param>
        public HistorySelectFaces(string name, IEnumerable<Face> faces, bool selected)
        {
            Name = name;
            _faces = new List<Face>(faces);
            _selected = selected;
        }

        public void Undo(Map map)
        {
            _faces.ForEach(x => x.IsSelected = !_selected);
        }

        public void Redo(Map map)
        {
            _faces.ForEach(x => x.IsSelected = _selected);
        }

        public void Dispose()
        {
            _faces.Clear();
        }
    }
}