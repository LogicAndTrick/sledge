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
        private readonly Dictionary<long, IMapObject> _descendants;
        private readonly Dictionary<long, IMapObject> _children;
        private IMapObject _parent;

        public int NumChildren => _children.Count;
        public int NumDescendants => _descendants.Count;
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
            get => _parent;
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
            _descendants = new Dictionary<long, IMapObject>();
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
            return _descendants.ContainsKey(id);
        }

        public IMapObject GetDescendant(long id)
        {
            return _descendants.ContainsKey(id) ? _descendants[id] : null;
        }

        private void Add(IMapObject item)
        {
            var id = item.ID;
            _children[id] = item;

            var p = _self;
            while (p != null)
            {
                p.Hierarchy._descendants[id] = item;
                foreach (var kv in item.Hierarchy._descendants) p.Hierarchy._descendants[kv.Key] = kv.Value;
                p = p.Hierarchy._parent;
            }
        }

        private bool Remove(IMapObject item)
        {
            if (item == null || !_children.ContainsKey(item.ID)) return false;

            var id = item.ID;
            _children.Remove(id);

            var p = _self;
            while (p != null)
            {
                p.Hierarchy._descendants.Remove(id);
                foreach (var kv in item.Hierarchy._descendants) p.Hierarchy._descendants.Remove(kv.Key);
                p = p.Hierarchy._parent;
            }
            return true;
        }

        public void Clear()
        {
            var set = _descendants.Keys.ToList();
            var p = _parent;
            while (p != null)
            {
                foreach (var v in set) p.Hierarchy._descendants.Remove(v);
                p = p.Hierarchy._parent;
            }
            foreach (var mo in _children.Values)
            {
                mo.Hierarchy._parent = null;
            }

            _descendants.Clear();
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
