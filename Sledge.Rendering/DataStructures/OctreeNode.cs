using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.DataStructures.Geometric;

namespace Sledge.Rendering.DataStructures
{
    public class OctreeNode<T> : ICollection<T> where T : IOrigin
    {
        public Octree<T> Root { get; protected set; }
        public OctreeNode<T> Parent { get; private set; }
        
        public Box Box { get; private set; }
        public int Count { get; private set; }

        private int _limit;
        private List<T> _elements;
        private OctreeNode<T>[] _children;

        public OctreeNode(Octree<T> root, OctreeNode<T> parent, Box box, int limit)
        {
            Root = root;
            Parent = parent;
            _limit = limit;
            Box = box;
            Count = 0;
            _elements = new List<T>();
            _children = null;
        }

        public List<OctreeNode<T>> GetChildNodes()
        {
            return _children == null ? new List<OctreeNode<T>>() : _children.ToList();
        }

        public void Clear()
        {
            _elements = new List<T>();
            _children = null;
            Count = 0;
        }

        public bool Contains(T item)
        {
            if (_children == null) return _elements.Contains(item);
            else return _children.Any(x => x.Contains(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (_children == null)
            {
                _elements.CopyTo(array, arrayIndex);
            }
            else
            {
                foreach (var child in _children)
                {
                    child.CopyTo(array, arrayIndex);
                    arrayIndex += child.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(T element)
        {
            Add(new[] {element});
        }

        public void Add(IEnumerable<T> elements)
        {
            var list = elements.ToList();
            var switched = false;

            if (_children == null)
            {
                _elements = _elements.Union(list).ToList();
                Count = _elements.Count;

                // If we're still under the limit, break out
                if (Count <= _limit)
                {
                    return;
                }

                // We're over the limit, create the child nodes
                _children = new OctreeNode<T>[8];
                var center = _elements.Aggregate(Coordinate.Zero, (a, b) => a + b.Origin) / _elements.Count;
                _children = Box.GetBoxPoints().Select(x => new OctreeNode<T>(Root, this, new Box(x, center), _limit)).ToArray();

                // We need to init the child nodes with all elements, so sub in the elements list
                list = _elements;
                _elements = null;
                switched = true;
            }

            // Add the elements into their particular nodes
            var grouped = list.GroupBy(x => _children.First(y => y.Box.CoordinateIsInside(x.Origin))).ToList();
            if (switched && grouped.Count == 1)
            {
                // Everything is in the same group! Since we split based on the average origin, that means all the origins are identical.
                // In this case we cannot split anymore.
                _elements = list;
                Count = _elements.Count;
                _children = null;
                return;
            }
            if (list.Count < _limit / 2)
            {
                foreach (var g in grouped)
                {
                    g.Key.Add(g);
                }
            }
            else
            {
                Parallel.ForEach(grouped, g => g.Key.Add(g));
            }

            Count = _children.Sum(x => x.Count);
        }

        public bool Remove(T element)
        {
            return Remove(new[] { element });
        }

        public bool Remove(IEnumerable<T> elements)
        {
            var startCount = Count;
            var list = elements.ToList();
            if (_children == null)
            {
                // We're under the limit, no need to do anything when removing stuff
                _elements = _elements.Except(list).ToList();
                return Count != startCount;
            }

            // Remove the elements from their nodes
            var grouped = list.GroupBy(x => _children.First(y => y.Box.CoordinateIsInside(x.Origin)));
            if (list.Count < _limit / 2)
            {
                foreach (var g in grouped)
                {
                    g.Key.Remove(g);
                }
            }
            else
            {
                Parallel.ForEach(grouped, g => g.Key.Remove(g));
            }

            Count = _children.Sum(x => x.Count);

            // If we've dropped under the limit, collapse the child nodes
            if (Count <= _limit)
            {
                _elements = _children.SelectMany(x => x).ToList();
                _children = null;
            }

            return Count != startCount;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_children == null) return _elements.GetEnumerator();
            else return new OctreeNodeEnumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (_children == null) return _elements.GetEnumerator();
            else return new OctreeNodeEnumerator(this);
        }

        private class OctreeNodeEnumerator : IEnumerator<T>
        {
            private int _index;
            private List<IEnumerator<T>> _enumerators;
            private T _current;

            public OctreeNodeEnumerator(OctreeNode<T> node)
            {
                _index = 0;
                _current = default(T);
                _enumerators = node._children.Select(x => x.GetEnumerator()).ToList();
            }

            public T Current { get { return _current; } }
            object System.Collections.IEnumerator.Current { get { return _current; } }

            public bool MoveNext()
            {
                if (_index >= _enumerators.Count) throw new InvalidOperationException();
                for (; _index < _enumerators.Count; _index++)
                {
                    if (_enumerators[_index].MoveNext())
                    {
                        _current = _enumerators[_index].Current;
                        return true;
                    }
                }
                return false;
            }

            public void Reset()
            {
                _enumerators.ForEach(x => x.Reset());
                _index = 0;
                _current = default(T);
            }

            public void Dispose()
            {
                _enumerators.ForEach(x => x.Dispose());
                _enumerators = null;
                _current = default(T);
            }
        }
    }
}