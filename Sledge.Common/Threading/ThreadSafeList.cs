using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sledge.Common.Threading
{
    /// <summary>
    /// A simple thread-safe list that operates by locking around the list during operations.
    /// </summary>
    /// <typeparam name="T">List type</typeparam>
    public class ThreadSafeList<T> : IList<T>, IReadOnlyCollection<T>
    {
        private readonly object _lock;
        private readonly List<T> _list;

        /// <inheritdoc cref="ICollection{T}.Count"/> />
        public int Count
        {
            get { lock (_lock) return _list.Count; }
        }

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
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

        public ThreadSafeList(IEnumerable<T> items) : this()
        {
            AddRange(items);
        }

        /// <inheritdoc />
        public bool Contains(T item)
        {
            lock (_lock) return _list.Contains(item);
        }

        /// <inheritdoc />
        public int IndexOf(T item)
        {
            lock (_lock) return _list.IndexOf(item);
        }

        /// <inheritdoc />
        public void Add(T item)
        {
            lock (_lock) _list.Add(item);
        }

        /// <summary>
        /// Add a list of items to the list.
        /// </summary>
        /// <param name="items">The items to add</param>
        public void AddRange(IEnumerable<T> items)
        {
            lock (_lock) _list.AddRange(items);
        }

        /// <inheritdoc />
        public void Insert(int index, T item)
        {
            lock (_lock) _list.Insert(index, item);
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            lock (_lock) return _list.Remove(item);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            lock (_lock) _list.RemoveAt(index);
        }

        /// <summary>
        /// Remove all items that match a predicate from the list.
        /// </summary>
        /// <param name="match">The predicate function</param>
        /// <returns>The number of items that were removed</returns>
        public int RemoveAll(Predicate<T> match)
        {
            lock (_lock) return _list.RemoveAll(match);
        }

        /// <summary>
        /// Remove a list of items from the list.
        /// </summary>
        /// <param name="items">The items to remove</param>
        public void RemoveRange(IEnumerable<T> items)
        {
            var li = items.ToHashSet();
            RemoveAll(li.Contains);
        }

        /// <inheritdoc />
        public void Clear()
        {
            lock (_lock) _list.Clear();
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_lock) _list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Get a continuous range of items from the list
        /// </summary>
        /// <param name="start">The start of the range</param>
        /// <param name="count">The number of items in the range</param>
        /// <returns>The selected range</returns>
        public List<T> GetRange(int start, int count)
        {
            lock (_lock) return _list.GetRange(start, count);
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            lock (_lock) return _list.ToList().GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
