using System;
using System.Collections.Generic;
using System.Linq;

namespace Sledge.BspEditor.Primitives.MapObjects
{
    public static class MapObjectExtensions
    {
        public static MapObjectQuery Query(this Map map)
        {
            return new MapObjectQuery(map.Root);
        }

        public static MapObjectQuery Query(this IMapObject obj)
        {
            return new MapObjectQuery(obj);
        }

        public static MapObjectQuery Query(this IEnumerable<IMapObject> objs)
        {
            return new MapObjectQuery(objs);
        }

        /// <summary>
        /// Get the root of this node.
        /// </summary>
        /// <param name="obj">This object</param>
        /// <returns>The root node of this object's tree</returns>
        public static IMapObject GetRoot(this IMapObject obj)
        {
            var p = obj;
            while (p.Hierarchy.Parent != null) p = p.Hierarchy.Parent;
            return p;
        }

        /// <summary>
        /// Get all the parents of this node that match a predicate. The first item in the list will be the closest parent.
        /// </summary>
        /// <param name="obj">This object</param>
        /// <param name="matcher">The predicate to match</param>
        /// <param name="includeRoot">True to include the World element at the bottom of the list</param>
        /// <returns>The list of parents</returns>
        public static IEnumerable<IMapObject> FindParents(this IMapObject obj, Predicate<IMapObject> matcher, bool includeRoot = false)
        {
            while (obj.Hierarchy.Parent != null)
            {
                if (obj.Hierarchy.Parent is Root && !includeRoot) break;
                if (matcher(obj.Hierarchy.Parent)) yield return obj.Hierarchy.Parent;
                obj = obj.Hierarchy.Parent;
            }
        }

        /// <summary>
        /// Find the first parent of this object that matches a predicate.
        /// </summary>
        /// <param name="obj">This object</param>
        /// <param name="matcher">The predicate to match</param>
        /// <returns>The matching parent if it was found, null otherwise</returns>
        public static IMapObject FindClosestParent(this IMapObject obj, Predicate<IMapObject> matcher)
        {
            return obj.FindParents(matcher).FirstOrDefault();
        }

        /// <summary>
        /// Find the last parent of this object that matches a predicate.
        /// </summary>
        /// <param name="obj">This object</param>
        /// <param name="matcher">The predicate to match</param>
        /// <returns>The matching parent if it was found, null otherwise</returns>
        public static IMapObject FindTopmostParent(this IMapObject obj, Predicate<IMapObject> matcher)
        {
            return obj.FindParents(matcher).LastOrDefault();
        }

        /// <summary>
        /// Get the single object in the object tree with the given ID.
        /// </summary>
        /// <param name="id">The ID of the object to locate</param>
        /// <returns>The object with the matching ID or null if it wasn't found</returns>
        public static IMapObject FindByID(this IMapObject o, long id)
        {
            if (o.ID == id) return o;
            return o.Hierarchy.GetDescendant(id);
        }

        /// <summary>
        /// Flattens the tree underneath this node.
        /// </summary>
        /// <returns>A list containing all the descendants of this node (including this node)</returns>
        public static List<IMapObject> FindAll(this IMapObject obj)
        {
            return obj.Find(x => true);
        }

        /// <summary>
        /// Flattens the tree and selects the nodes that match the test.
        /// </summary>
        /// <param name="obj">This object</param>
        /// <param name="matcher">The prediacate to match</param>
        /// <param name="forceMatchIfParentMatches">If true and a parent matches the predicate, all children will be added regardless of match status.</param>
        /// <returns>A list of all the descendants that match the test (including this node)</returns>
        public static List<IMapObject> Find(this IMapObject obj, Predicate<IMapObject> matcher, bool forceMatchIfParentMatches = false)
        {
            var list = new List<IMapObject>();
            obj.FindRecursive(list, matcher, forceMatchIfParentMatches);
            return list;
        }

        public static List<IMapObject> Collect(this IMapObject obj, Predicate<IMapObject> traverseNode, Predicate<IMapObject> includeNode)
        {
            var list = new List<IMapObject>();

            // Check if we're traversing this node at all
            var t = traverseNode(obj);
            if (!t) return list;

            // Check if we should include this node
            if (includeNode(obj)) list.Add(obj);

            // Traverse and include child nodes
            foreach (var ch in obj.Hierarchy)
            {
                list.AddRange(ch.Collect(traverseNode, includeNode));
            }

            return list;
        }

        /// <summary>
        /// Recursively collect children matching a predicate.
        /// </summary>
        /// <param name="obj">This object</param>
        /// <param name="items">The list to populate</param>
        /// <param name="matcher">The prediacate to match</param>
        /// <param name="forceMatchIfParentMatches">If true and a parent matches the predicate, all children will be added regardless of match status.</param>
        private static void FindRecursive(this IMapObject obj, ICollection<IMapObject> items, Predicate<IMapObject> matcher, bool forceMatchIfParentMatches = false)
        {
            var thisMatch = matcher(obj);
            if (thisMatch)
            {
                items.Add(obj);
                if (forceMatchIfParentMatches) matcher = x => true;
            }
            foreach (var mo in obj.Hierarchy)
            {
                mo.FindRecursive(items, matcher, forceMatchIfParentMatches);
            }
        }

        /// <summary>
        /// Recursively modify children matching a predicate.
        /// </summary>
        /// <param name="action">The action to perform on matching children</param>
        /// <param name="obj">This object</param>
        /// <param name="matcher">The prediacate to match</param>
        /// <param name="forceMatchIfParentMatches">If true and a parent matches the predicate, all children will be modified regardless of match status.</param>
        public static void ForEach(this IMapObject obj, Predicate<IMapObject> matcher, Action<IMapObject> action, bool forceMatchIfParentMatches = false)
        {
            var thisMatch = matcher(obj);
            if (thisMatch)
            {
                action(obj);
                if (forceMatchIfParentMatches) matcher = x => true;
            }
            foreach (var mo in obj.Hierarchy)
            {
                mo.ForEach(matcher, action, forceMatchIfParentMatches);
            }
        }
    }
}
