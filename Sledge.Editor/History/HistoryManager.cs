using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.History
{
    public class HistoryManager
    {
        private Documents.Document Document { get; set; }
        public int SizeOfHistory { get; set; }
        private readonly List<IHistoryItem> _items;
        private int _currentIndex;

        public long TotalActionsSinceLastSave { get; set; }

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
            TotalActionsSinceLastSave++;
            Mediator.Publish(EditorMediator.HistoryChanged);
        }

        public void Undo()
        {
            if (!CanUndo()) return;
            _items[_currentIndex].Undo(Document);
            _currentIndex--;
            TotalActionsSinceLastSave--;
            Mediator.Publish(EditorMediator.HistoryChanged);
        }

        public void Redo()
        {
            if (!CanRedo()) return;
            _items[_currentIndex + 1].Redo(Document);
            _currentIndex++;
            TotalActionsSinceLastSave++;
            Mediator.Publish(EditorMediator.HistoryChanged);
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

        public bool CanRedo()
        {
            return _currentIndex + 1 <= _items.Count - 1;
        }
    }
}
