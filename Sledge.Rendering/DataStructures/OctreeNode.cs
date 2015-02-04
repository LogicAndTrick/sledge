using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.DataStructures.Geometric;

namespace Sledge.Rendering.DataStructures
{
    public class OctreeNode : IEnumerable<IPosition>
    {
        public Octree Root { get; protected set; }
        public OctreeNode Parent { get; private set; }
        
        public Box Box { get; private set; }
        public int Count { get; private set; }

        private int _limit;
        private List<IPosition> _elements;
        private OctreeNode[] _children;

        public OctreeNode(Octree root, OctreeNode parent, Box box, int limit)
        {
            Root = root;
            Parent = parent;
            _limit = limit;
            Box = box;
            Count = 0;
            _elements = new List<IPosition>();
            _children = null;
        }

        public void Add(IEnumerable<IPosition> elements)
        {
            var list = elements.ToList();
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
                _children = new OctreeNode[8];
                var center = Box.Center;
                _children = Box.GetBoxPoints().Select(x => new OctreeNode(Root, this, new Box(x, center), _limit)).ToArray();

                // We need to init the child nodes with all elements, so sub in the elements list
                list = _elements;
                _elements = null;
            }

            // Add the elements into their particular nodes
            var grouped = list.GroupBy(x => _children.First(y => y.Box.CoordinateIsInside(x.Origin)));
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

        public void Remove(IEnumerable<IPosition> elements)
        {
            var list = elements.ToList();
            if (_children == null)
            {
                // We're under the limit, no need to do anything when removing stuff
                _elements = _elements.Except(list).ToList();
                return;
            }

            // Remove the elements from their nodes
            var grouped = list.GroupBy(x => _children.First(y => y.Box.CoordinateIsInside(x.Origin)));
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

            // If we've dropped under the limit, collapse the child nodes
            if (Count <= _limit)
            {
                _elements = _children.SelectMany(x => x).ToList();
                _children = null;
            }
        }

        public IEnumerator<IPosition> GetEnumerator()
        {
            if (_children == null) return _elements.GetEnumerator();
            else return new OctreeNodeEnumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (_children == null) return _elements.GetEnumerator();
            else return new OctreeNodeEnumerator(this);
        }

        private class OctreeNodeEnumerator : IEnumerator<IPosition>
        {
            private int _index;
            private List<IEnumerator<IPosition>> _enumerators;
            private IPosition _current;

            public OctreeNodeEnumerator(OctreeNode node)
            {
                _index = 0;
                _current = null;
                _enumerators = node._children.Select(x => x.GetEnumerator()).ToList();
            }

            public IPosition Current { get { return _current; } }
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
                _current = null;
            }

            public void Dispose()
            {
                _enumerators.ForEach(x => x.Dispose());
                _enumerators = null;
                _current = null;
            }
        }
    }
}