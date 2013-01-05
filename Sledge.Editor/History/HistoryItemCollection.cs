using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.History
{
    /// <summary>
    /// An item collection is simply multiple history items combined into one transaction.
    /// </summary>
    public class HistoryItemCollection : IHistoryItem
    {
        public string Name { get; private set; }
        private readonly List<IHistoryItem> _items;

        public HistoryItemCollection(string name, IEnumerable<IHistoryItem> items)
        {
            Name = name;
            _items = new List<IHistoryItem>(items);
        }

        public void Undo(Document document)
        {
            for (var i = _items.Count - 1; i >= 0; i--)
            {
                _items[i].Undo(document);
            }
        }

        public void Redo(Document document)
        {
            for (var i = 0; i < _items.Count; i++)
            {
                _items[i].Redo(document);
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
