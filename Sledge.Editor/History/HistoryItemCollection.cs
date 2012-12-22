using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.History
{
    public class HistoryItemCollection : IHistoryItem
    {
        private List<IHistoryItem> _items;

        public HistoryItemCollection()
        {
            _items = new List<IHistoryItem>();
        }

        public void Undo(Map map)
        {
            for (var i = _items.Count - 1; i >= 0; i--)
            {
                _items[i].Undo(map);
            }
        }

        public void Redo(Map map)
        {
            for (var i = 0; i < _items.Count; i++)
            {
                _items[i].Redo(map);
            }
        }

        public void Dispose()
        {
            foreach (var item in _items)
            {
                item.Dispose();
            }
            _items.Clear();
        }
    }
}
