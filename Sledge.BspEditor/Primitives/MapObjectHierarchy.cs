using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sledge.BspEditor.Primitives
{
    /// <summary>
    /// Represents the hierarchy of a map object in the tree.
    /// </summary>
    public class MapObjectHierarchy : ICollection<IMapObject>
    {
        private readonly IMapObject _self;
        private readonly HashSet<long> _descendantIds;
        private readonly Dictionary<long, IMapObject> _children;
        private IMapObject _parent;

        public bool IsReadOnly => false;

        public int Count => _children.Count;
        public int NumDescendants => _descendantIds.Count;

        /// <summary>
        /// This object
        /// </summary>
        public IMapObject Self => _self;

        /// <summary>
        /// The parent of this object
        /// </summary>
        public IMapObject Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != null)
                {
                    if (_parent.Hierarchy.HasChild(_self.ID) && Parent.Hierarchy.GetChild(_self.ID) == _self) Parent.Hierarchy.Remove(_self);
                    _parent.DescendantsChanged();
                }
                _parent = value;
                if (_parent != null)
                {
                    _parent.Hierarchy.Add(_self);
                    _self.DescendantsChanged();
                }
            }
        }

        public MapObjectHierarchy(IMapObject obj)
        {
            _self = obj;
            _descendantIds = new HashSet<long>();
            _children = new Dictionary<long, IMapObject>();
        }

        public bool HasChild(long id)
        {
            return _children.ContainsKey(id);
        }

        public IMapObject GetChild(long id)
        {
            return _children.ContainsKey(id) ? _children[id] : null;
        }

        public bool HasDescendant(long id)
        {
            return _descendantIds.Contains(id);
        }

        public IMapObject GetDescendant(long id)
        {
            if (!HasDescendant(id)) return null;
            if (HasChild(id)) return GetChild(id);
            var child = _children.FirstOrDefault(x => x.Value.Hierarchy.HasDescendant(id));
            return child.Value?.Hierarchy.GetDescendant(id);
        }

        public bool Contains(IMapObject item)
        {
            return item != null && HasChild(item.ID);
        }

        public void CopyTo(IMapObject[] array, int arrayIndex)
        {
            var list = _children.Values.ToList();
            list.CopyTo(array, arrayIndex);
        }

        public void Add(IMapObject item)
        {
            _children[item.ID] = item;
            var set = new HashSet<long>(item.Hierarchy._descendantIds) { item.ID };

            var p = _self;
            while (p != null)
            {
                p.Hierarchy._descendantIds.UnionWith(set);
                p = p.Hierarchy._parent;
            }
        }

        public bool Remove(IMapObject item)
        {
            if (item == null || !_children.ContainsKey(item.ID)) return false;
            _children.Remove(item.ID);
            var set = new HashSet<long>(item.Hierarchy._descendantIds) { item.ID };

            var p = _self;
            while (p != null)
            {
                p.Hierarchy._descendantIds.ExceptWith(set);
                p = p.Hierarchy._parent;
            }
            return true;
        }

        public void Clear()
        {
            var set = _descendantIds;
            var p = _parent;
            while (p != null)
            {
                p.Hierarchy._descendantIds.ExceptWith(set);
                p = p.Hierarchy._parent;
            }
            foreach (var mo in _children.Values)
            {
                mo.Hierarchy._parent = null;
            }
            _descendantIds.Clear();
            _children.Clear();
        }

        public IEnumerator<IMapObject> GetEnumerator()
        {
            return _children.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
