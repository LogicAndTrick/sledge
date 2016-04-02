using System.Collections.Generic;
using Sledge.Editor.Documents;

namespace Sledge.Editor.History
{
    public class HistoryStack
    {
        private readonly List<IHistoryItem> _items;
        public string Name { get; set; }
        private int _maximumSize;
        private int _currentIndex;

        public HistoryStack(string name, int maximumSize)
        {
            _items = new List<IHistoryItem>();
            Name = name;
            _maximumSize = maximumSize;
            _currentIndex = -1;
        }

        public void Clear()
        {
            _items.Clear();
            _currentIndex = -1;
        }

        public void Add(IHistoryItem item)
        {
            // Delete the redo stack if required
            if (_currentIndex < _items.Count - 1)
            {
                _items.GetRange(_currentIndex + 1, _items.Count - _currentIndex - 1).ForEach(x => x.Dispose());
                _items.RemoveRange(_currentIndex + 1, _items.Count - _currentIndex - 1);
            }
            // Remove extra entries if required
            while (_items.Count > _maximumSize - 1)
            {
                _items[0].Dispose();
                _items.RemoveAt(0);
                _currentIndex--;
            }
            // Add the new entry
            _items.Add(item);
            _currentIndex = _items.Count - 1;
        }

        public void Undo(Document document)
        {
            if (!CanUndo()) return;
            _items[_currentIndex].Undo(document);
            _currentIndex--;
        }

        public void Redo(Document document)
        {
            if (!CanRedo()) return;
            _items[_currentIndex + 1].Redo(document);
            _currentIndex++;
        }

        public string GetUndoString()
        {
            if (!CanUndo()) return "Can't undo";
            return "Undo " + _items[_currentIndex].Name;
        }

        public string GetRedoString()
        {
            if (!CanRedo()) return "Can't redo";
            return "Redo " + _items[_currentIndex + 1].Name;
        }

        public bool CanUndo()
        {
            return _currentIndex >= 0;
        }

        public IHistoryItem NextUndo()
        {
            return _items[_currentIndex];
        }

        public bool CanRedo()
        {
            return _currentIndex + 1 <= _items.Count - 1;
        }

        public IHistoryItem NextRedo()
        {
            return _items[_currentIndex + 1];
        }

        public IEnumerable<IHistoryItem> GetHistoryItems()
        {
            return new List<IHistoryItem>(_items.GetRange(0, _currentIndex + 1));
        }
    }
}