using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sledge.Common.Threading
{
    public class ThreadSafeSet<T> : ISet<T>
    {
        private readonly object _lock;
        private readonly HashSet<T> _set;

        public int Count
        {
            get { lock(_lock) return _set.Count; }
        }

        public bool IsReadOnly => false;

        public ThreadSafeSet()
        {
            _set = new HashSet<T>();
            _lock = new object();
        }

        public ThreadSafeSet(IEnumerable<T> items) : this()
        {
            UnionWith(items);
        }

        void ICollection<T>.Add(T item)
        {
            lock (_lock) _set.Add(item);
        }

        bool ISet<T>.Add(T item)
        {
            lock (_lock) return _set.Add(item);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            lock (_lock) _set.UnionWith(other);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            lock (_lock) _set.IntersectWith(other);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            lock (_lock) _set.ExceptWith(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            lock (_lock) _set.SymmetricExceptWith(other);
        }

        public bool Remove(T item)
        {
            lock (_lock) return _set.Remove(item);
        }

        public void Clear()
        {
            lock (_lock) _set.Clear();
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            lock (_lock) return _set.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            lock (_lock) return _set.IsSupersetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            lock (_lock) return _set.IsProperSupersetOf(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            lock (_lock) return _set.IsProperSubsetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            lock (_lock) return _set.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            lock (_lock) return _set.SetEquals(other);
        }

        public bool Contains(T item)
        {
            lock (_lock) return _set.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_lock) _set.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_lock) return _set.ToHashSet().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}