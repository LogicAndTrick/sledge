using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sledge.BspEditor.Primitives.MapObjects
{
    /// <summary>
    /// Represents the hierarchy of a map object in the tree.
    /// </summary>
    public class MapObjectHierarchy : IEnumerable<IMapObject>
    {
        private readonly IMapObject _self;
        private readonly HashSet<long> _descendantIds;
        private readonly Dictionary<long, IMapObject> _children;
        private IMapObject _parent;

        public int NumChildren => _children.Count;
        public int NumDescendants => _descendantIds.Count;
        public bool HasChildren => NumChildren > 0;

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

        private void Add(IMapObject item)
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

        private bool Remove(IMapObject item)
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
