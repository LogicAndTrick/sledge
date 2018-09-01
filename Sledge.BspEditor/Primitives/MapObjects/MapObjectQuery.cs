using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sledge.BspEditor.Primitives.MapObjects
{
    /// <summary>
    /// Jquery type interface for map objects.
    /// Stack modification operators will return a new instance, so make sure you chain things!
    /// </summary>
    public class MapObjectQuery : IEnumerable<IMapObject>
    {
        private readonly MapObjectQuery _parent;
        private readonly List<IMapObject> _context;

        private MapObjectQuery(MapObjectQuery parent, params IMapObject[] context) : this(parent, context.ToList())
        {

        }

        private MapObjectQuery(MapObjectQuery parent, IEnumerable<IMapObject> context)
        {
            _parent = parent;
            _context = context.ToList();
        }

        public MapObjectQuery(params IMapObject[] items) : this(items.ToList())
        {

        }

        public MapObjectQuery(IEnumerable<IMapObject> items)
        {
            _context = items.ToList();
        }

        public bool Any()
        {
            return _context.Any();
        }

        public int Count => _context.Count;
        public IMapObject this[int index] => _context[index];

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IMapObject> GetEnumerator()
        {
            return _context.GetEnumerator();
        }

        /// <summary>
        /// Pop the traversal stack
        /// </summary>
        /// <returns>Parent (or this if we're at the top)</returns>
        public MapObjectQuery Back()
        {
            return _parent ?? this;
        }

        /// <summary>
        /// Push the root node onto the traversal stack
        /// </summary>
        /// <returns></returns>
        public MapObjectQuery Root()
        {
            return new MapObjectQuery(this, _context[0].GetRoot());
        }

        /// <summary>
        /// Push the child nodes onto the traversal stack, with optional filter.
        /// </summary>
        /// <returns></returns>
        public MapObjectQuery Children(Func<IMapObject, bool> filter = null)
        {
            var ch = _context.SelectMany(x => x.Hierarchy);
            if (filter != null) ch = ch.Where(filter);
            return new MapObjectQuery(this, ch);
        }

        /// <summary>
        /// Push the descendant nodes onto the traversal stack, with optional filter.
        /// </summary>
        /// <returns></returns>
        public MapObjectQuery Descendants(Func<IMapObject, bool> filter = null)
        {
            var ch = _context.SelectMany(x => x.FindAll());
            if (filter != null) ch = ch.Where(filter);
            return new MapObjectQuery(this, ch);
        }

        /// <summary>
        /// Push a filtered list onto the traversal stack
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public MapObjectQuery Filter(Func<IMapObject, bool> filter)
        {
            return new MapObjectQuery(this, _context.Where(filter));
        }

        /// <summary>
        /// Push a single object onto the traversal stack by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MapObjectQuery Find(long id)
        {
            return Filter(x => x.ID == id);
        }

        /// <summary>
        /// Perform an operation on the current context
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public MapObjectQuery Each(Action<IMapObject> action)
        {
            foreach (var o in _context)
            {
                action(o);
            }
            return this;
        }
    }
}
