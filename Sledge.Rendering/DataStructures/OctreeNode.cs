using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenTK;
using Sledge.Rendering.Interfaces;

namespace Sledge.Rendering.DataStructures
{
    public class OctreeNode<T> : ICollection<T> where T : IBounded
    {
        public Octree<T> Root { get; protected set; }
        public OctreeNode<T> Parent { get; private set; }
        
        public Box ClippingBox { get; private set; }
        public Box BoundingBox { get; private set; }
        public int Count { get; private set; }

        private int _limit;
        private HashSet<T> _elements;
        private OctreeNode<T>[] _children;

        public OctreeNode(Octree<T> root, OctreeNode<T> parent, Box clippingBox, int limit)
        {
            Root = root;
            Parent = parent;
            _limit = limit;
            BoundingBox = ClippingBox = clippingBox;
            Count = 0;
            _elements = new HashSet<T>();
            _children = null;
        }

        public List<OctreeNode<T>> GetChildNodes()
        {
            return _children == null ? new List<OctreeNode<T>>() : _children.ToList();
        }

        public void Clear()
        {
            _elements = new HashSet<T>();
            _children = null;
            Count = 0;
        }

        public void Defragment()
        {
            var items = this.ToList();
            Clear();
            Add(items);
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
            var list = new HashSet<T>(elements);
            var switched = false;

            if (_children == null)
            {
                _elements = new HashSet<T>(_elements.Union(list));
                Count = _elements.Count;

                // If we're still under the limit, break out
                if (Count <= _limit)
                {
                    BoundingBox = _elements.Count == 0 ? ClippingBox : new Box(_elements.Select(x => x.BoundingBox));
                    return;
                }

                // We're over the limit, create the child nodes
                var center = _elements.Aggregate(Vector3.Zero, (a, b) => a + b.Origin) / _elements.Count;
                _children = ClippingBox.GetBoxPoints().Select(x => new OctreeNode<T>(Root, this, new Box(x, center), _limit)).ToArray();

                // We need to init the child nodes with all elements, so sub in the elements list
                list = _elements;
                _elements = null;
                switched = true;
            }

            // Add the elements into their particular nodes
            var grouped = list.GroupBy(x => _children.First(y => y.ClippingBox.VectorIsInside(x.Origin))).ToList();
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
            BoundingBox = new Box(_children.Select(x => x.BoundingBox));
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
                _elements = new HashSet<T>(_elements.Except(list));
                Count = _elements.Count;
                BoundingBox = _elements.Count == 0 ? ClippingBox : new Box(_elements.Select(x => x.BoundingBox));
                return Count != startCount;
            }

            // Remove the elements from their nodes
            var grouped = list.GroupBy(x => _children.FirstOrDefault(y => y.Contains(x)));
            if (list.Count < _limit / 2)
            {
                foreach (var g in grouped)
                {
                    if (g.Key != null) g.Key.Remove(g);
                }
            }
            else
            {
                Parallel.ForEach(grouped, g =>
                {
                    if (g.Key != null) g.Key.Remove(g);
                });
            }

            Count = _children.Sum(x => x.Count);

            // If we've dropped under the limit, collapse the child nodes
            if (Count <= _limit)
            {
                _elements = new HashSet<T>(_children.SelectMany(x => x));
                BoundingBox = _elements.Count == 0 ? ClippingBox : new Box(_elements.Select(x => x.BoundingBox));
                _children = null;
            }
            else
            {
                BoundingBox = new Box(_children.Select(x => x.BoundingBox));
            }

            return Count != startCount;
        }

        public IEnumerable<List<OctreeNode<T>>> Partition(int maxPartitionSize = 1000)
        {
            var nodes = GetChildNodes();
            if (Count <= maxPartitionSize || !nodes.Any())
            {
                yield return new List<OctreeNode<T>> { this };
            }
            else
            {
                var count = 0;
                var part = new List<OctreeNode<T>>();
                foreach (var node in nodes)
                {
                    if (node.Count > maxPartitionSize)
                    {
                        foreach (var p in node.Partition(maxPartitionSize)) yield return p;
                    }
                    else if (node.Count + count > maxPartitionSize)
                    {
                        yield return part;
                        part = new List<OctreeNode<T>> {node};
                        count = node.Count;
                    }
                    else
                    {
                        part.Add(node);
                        count += node.Count;
                    }
                }
                if (count > 0)
                {
                    yield return part;
                }
            }
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