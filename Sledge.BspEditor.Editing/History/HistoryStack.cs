using System.Collections.Generic;
using System.Runtime.Serialization;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Editing.History
{
    public class HistoryStack : IMapData
    {
        public bool AffectsRendering => false;

        private readonly List<IOperation> _items;
        private readonly int _maximumSize;
        private int _currentIndex;

        public HistoryStack(int maximumSize)
        {
            _items = new List<IOperation>();
            _maximumSize = maximumSize;
            _currentIndex = -1;
        }

        public void Clear()
        {
            _items.Clear();
            _currentIndex = -1;
        }

        public void Add(IOperation item)
        {
            // If we're adding the redo operation, don't change the list, just increment the pointer
            if (item == RedoOperation())
            {
                _currentIndex++;
                return;
            }
            // Delete the redo stack if required
            if (_currentIndex < _items.Count - 1)
            {
                _items.RemoveRange(_currentIndex + 1, _items.Count - _currentIndex - 1);
            }
            // Remove extra entries if required
            while (_items.Count > _maximumSize - 1)
            {
                _items.RemoveAt(0);
                _currentIndex--;
            }
            // Add the new entry
            _items.Add(item);
            _currentIndex = _items.Count - 1;
        }

        public void Remove(IOperation item)
        {
            if (item == UndoOperation())
            {
                _currentIndex--;
            }
        }

        public bool CanUndo()
        {
            return _currentIndex >= 0;
        }

        public IOperation UndoOperation()
        {
            return CanUndo() ? _items[_currentIndex] : null;
        }

        public bool CanRedo()
        {
            return _currentIndex + 1 <= _items.Count - 1;
        }

        public IOperation RedoOperation()
        {
            return CanRedo() ? _items[_currentIndex + 1] : null;
        }

        public IEnumerable<IOperation> GetOperations()
        {
            return new List<IOperation>(_items.GetRange(0, _currentIndex + 1));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // 
        }

        public SerialisedObject ToSerialisedObject()
        {
            return new SerialisedObject("HistoryStack");
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public IMapElement Clone()
        {
            return new HistoryStack(_maximumSize);
        }
    }
}