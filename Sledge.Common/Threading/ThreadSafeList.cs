using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sledge.Common.Threading
{
    public class ThreadSafeList<T> : IList<T>
    {
        private readonly object _lock;
        private readonly List<T> _list;

        public int Count
        {
            get { lock (_lock) return _list.Count; }
        }

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get { lock (_lock) return _list[index]; }
            set { lock (_lock) _list[index] = value; }
        }

        public ThreadSafeList()
        {
            _list = new List<T>();
            _lock = ((ICollection) _list).SyncRoot;
        }

        public bool Contains(T item)
        {
            lock (_lock) return _list.Contains(item);
        }

        public int IndexOf(T item)
        {
            lock (_lock) return _list.IndexOf(item);
        }

        public void Add(T item)
        {
            lock (_lock) _list.Add(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            lock (_lock) _list.AddRange(items);
        }

        public void Insert(int index, T item)
        {
            lock (_lock) _list.Insert(index, item);
        }

        public bool Remove(T item)
        {
            lock (_lock) return _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            lock (_lock) _list.RemoveAt(index);
        }

        public int RemoveAll(Predicate<T> match)
        {
            lock (_lock) return _list.RemoveAll(match);
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            var li = items.ToHashSet();
            RemoveAll(li.Contains);
        }

        public void Clear()
        {
            lock (_lock) _list.Clear();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_lock) _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_lock) return _list.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
