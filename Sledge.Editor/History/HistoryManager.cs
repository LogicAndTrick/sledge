using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.History
{
    public static class HistoryManager
    {
        public static int SizeOfHistory { get; set; }
        private static readonly List<IHistoryItem> Items;
        private static int _currentIndex;

        static HistoryManager()
        {
            SizeOfHistory = 100;
            Items = new List<IHistoryItem>();
            _currentIndex = -1;
        }

        public static void Clear()
        {
            Items.Clear();
            _currentIndex = -1;
        }

        public static void AddHistoryItem(IHistoryItem item)
        {
            // Delete the redo stack if required
            if (_currentIndex < Items.Count - 1)
            {
                Items.GetRange(_currentIndex + 1, Items.Count - _currentIndex).ForEach(x => x.Dispose());
                Items.RemoveRange(_currentIndex + 1, Items.Count - _currentIndex);
            }
            // Remove extra entries if required
            while (Items.Count > SizeOfHistory - 1)
            {
                Items[0].Dispose();
                Items.RemoveAt(0);
                _currentIndex--;
            }
            // Add the new entry
            Items.Add(item);
            _currentIndex = Items.Count - 1;
        }

        public static void Undo(Map map)
        {
            if (_currentIndex < 0) return;
            Items[_currentIndex].Undo(map);
            _currentIndex--;
        }

        public static void Redo(Map map)
        {
            if (_currentIndex + 1 > Items.Count - 1) return;
            Items[_currentIndex + 1].Redo(map);
            _currentIndex++;
        }
    }
}
