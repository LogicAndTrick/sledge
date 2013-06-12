using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.History
{
    public class HistoryManager
    {
        private Documents.Document Document { get; set; }
        public int SizeOfHistory { get; set; }
        private readonly List<IHistoryItem> _items;
        private int _currentIndex;

        public HistoryManager(Documents.Document doc)
        {
            Document = doc;
            SizeOfHistory = 100;
            _items = new List<IHistoryItem>();
            _currentIndex = -1;
        }

        public void Clear()
        {
            _items.Clear();
            _currentIndex = -1;
        }

        public void AddHistoryItem(IHistoryItem item)
        {
            // Delete the redo stack if required
            if (_currentIndex < _items.Count - 1)
            {
                _items.GetRange(_currentIndex + 1, _items.Count - _currentIndex - 1).ForEach(x => x.Dispose());
                _items.RemoveRange(_currentIndex + 1, _items.Count - _currentIndex - 1);
            }
            // Remove extra entries if required
            while (_items.Count > SizeOfHistory - 1)
            {
                _items[0].Dispose();
                _items.RemoveAt(0);
                _currentIndex--;
            }
            // Add the new entry
            _items.Add(item);
            _currentIndex = _items.Count - 1;
        }

        public void Undo()
        {
            if (!CanUndo()) return;
            _items[_currentIndex].Undo(Document);
            Document.UpdateDisplayLists();
            _currentIndex--;
        }

        public void Redo()
        {
            if (!CanRedo()) return;
            _items[_currentIndex + 1].Redo(Document);
            Document.UpdateDisplayLists();
            _currentIndex++;
        }

        public bool CanUndo()
        {
            return _currentIndex >= 0;
        }

        public bool CanRedo()
        {
            return _currentIndex + 1 <= _items.Count - 1;
        }
    }
}
