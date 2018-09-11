using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sledge.Common.Threading
{
    /// <summary>
    /// A simple thread-safe set that operates by locking around the set during operations.
    /// </summary>
    /// <typeparam name="T">Set type</typeparam>
    public class ThreadSafeSet<T> : ISet<T>
    {
        private readonly object _lock;
        private readonly HashSet<T> _set;

        /// <inheritdoc />
        public int Count
        {
            get { lock(_lock) return _set.Count; }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        void ICollection<T>.Add(T item)
        {
            lock (_lock) _set.Add(item);
        }

        /// <inheritdoc />
        public bool Add(T item)
        {
            lock (_lock) return _set.Add(item);
        }

        /// <inheritdoc />
        public void UnionWith(IEnumerable<T> other)
        {
            lock (_lock) _set.UnionWith(other);
        }

        /// <inheritdoc />
        public void IntersectWith(IEnumerable<T> other)
        {
            lock (_lock) _set.IntersectWith(other);
        }

        /// <inheritdoc />
        public void ExceptWith(IEnumerable<T> other)
        {
            lock (_lock) _set.ExceptWith(other);
        }

        /// <inheritdoc />
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            lock (_lock) _set.SymmetricExceptWith(other);
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            lock (_lock) return _set.Remove(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            lock (_lock) _set.Clear();
        }

        /// <inheritdoc />
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            lock (_lock) return _set.IsSubsetOf(other);
        }

        /// <inheritdoc />
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            lock (_lock) return _set.IsSupersetOf(other);
        }

        /// <inheritdoc />
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            lock (_lock) return _set.IsProperSupersetOf(other);
        }

        /// <inheritdoc />
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            lock (_lock) return _set.IsProperSubsetOf(other);
        }

        /// <inheritdoc />
        public bool Overlaps(IEnumerable<T> other)
        {
            lock (_lock) return _set.Overlaps(other);
        }

        /// <inheritdoc />
        public bool SetEquals(IEnumerable<T> other)
        {
            lock (_lock) return _set.SetEquals(other);
        }

        /// <inheritdoc />
        public bool Contains(T item)
        {
            lock (_lock) return _set.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_lock) _set.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            lock (_lock) return _set.ToHashSet().GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}