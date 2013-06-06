using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.Transformations;

namespace Sledge.DataStructures.MapObjects
{
    public abstract class MapObject
    {
        public long ID { get; set; }
        public string ClassName { get; set; }
        public List<int> Visgroups { get; set; }
        public List<MapObject> Children { get; set; }
        public MapObject Parent { get; set; }
        public Color Colour { get; set; }
        public bool IsSelected { get; set; }
        public bool IsCodeHidden { get; set; }
        public bool IsVisgroupHidden { get; set; }
        public Box BoundingBox { get; set; }

        protected MapObject(long id)
        {
            ID = id;
            Visgroups = new List<int>();
            Children = new List<MapObject>();
        }

        /// <summary>
        /// Creates an exact copy of this object
        /// </summary>
        public abstract MapObject Clone(IDGenerator generator);

        /// <summary>
        /// Copies all the values of the provided object into this one
        /// </summary>
        public abstract void Unclone(MapObject o, IDGenerator generator);

        protected void CloneBase(MapObject o, IDGenerator generator)
        {
            o.ClassName = ClassName;
            o.Visgroups.AddRange(Visgroups);
            o.Parent = Parent;
            o.Colour = Colour;
            o.IsSelected = IsSelected;
            o.BoundingBox = BoundingBox.Clone();
            foreach (var c in Children.Select(x => x.Clone(generator)))
            {
                c.Parent = o;
                o.Children.Add(c);
            }
        }

        protected void UncloneBase(MapObject o, IDGenerator generator)
        {
            Visgroups.Clear();

            ClassName = o.ClassName;
            Visgroups.AddRange(o.Visgroups);
            Parent = o.Parent;
            Colour = o.Colour;
            IsSelected = o.IsSelected;
            BoundingBox = o.BoundingBox.Clone();

            Children.Clear();
            foreach (var c in o.Children.Select(x => x.Clone(generator)))
            {
                c.Parent = this;
                Children.Add(c);
            }
        }

        public void SetParent(MapObject parent)
        {
            if (Parent != null)
            {
                Parent.Children.Remove(this);
                Parent.UpdateBoundingBox();
            }
            Parent = parent;
            if (Parent != null)
            {
                Parent.Children.Add(this);
                UpdateBoundingBox();
            }
        }

        public void RemoveDescendant(MapObject remove)
        {
            if (Children.Contains(remove))
            {
                Children.Remove(remove);
            }
            else
            {
                Children.ForEach(x => x.RemoveDescendant(remove));
            }
        }

        public virtual void UpdateBoundingBox(bool cascadeToParent = true)
        {
            if (cascadeToParent && Parent != null)
            {
                Parent.UpdateBoundingBox();
            }
        }

        public virtual void Transform(IUnitTransformation transform)
        {
            Children.ForEach(c => c.Transform(transform));
            UpdateBoundingBox(false);
        }

        public virtual Coordinate GetIntersectionPoint(Line line)
        {
            return null;
        }

        public virtual EntityData GetEntityData()
        {
            return null;
        }

        /// <summary>
        /// Searches upwards for the last parent that is not null and is not
        /// an instance of <code>Sledge.DataStructures.MapObjects.World</code>.
        /// </summary>
        /// <param name="o">The starting node</param>
        /// <returns>The highest parent that is not a worldspawn instance</returns>
        public static MapObject GetTopmostNonRootParent(MapObject o)
        {
            while (o.Parent != null && !(o.Parent is World))
            {
                o = o.Parent;
            }
            return o;
        }

        /// <summary>
        /// Searches upwards for the root node as an instance of
        /// <code>Sledge.DataStructures.MapObjects.World</code>.
        /// </summary>
        /// <param name="o">The starting node</param>
        /// <returns>The root world node, or null if it doesn't exist.</returns>
        public static World GetRoot(MapObject o)
        {
            while (o != null && !(o is World))
            {
                o = o.Parent;
            }
            return o as World;
        }

        /// <summary>
        /// Get all the nodes starting from this node that intersect with a box.
        /// </summary>
        /// <param name="box">The intersection box</param>
        /// <returns>A list of all the descendants that intersect with the box.</returns>
        public IEnumerable<MapObject> GetAllNodesIntersectingWith(Box box)
        {
            var list = new List<MapObject>();
            if (!(this is World) && !IsCodeHidden && !IsVisgroupHidden)
            {
                if (BoundingBox == null || !BoundingBox.IntersectsWith(box)) return list;
                if (this is Solid || this is Entity) list.Add(this);
            }
            list.AddRange(Children.SelectMany(x => x.GetAllNodesIntersectingWith(box)));
            return list;
        }

        /// <summary>
        /// Get all the nodes starting from this node that intersect with a line.
        /// </summary>
        /// <param name="line">The intersection line</param>
        /// <returns>A list of all the descendants that intersect with the line.</returns>
        public IEnumerable<MapObject> GetAllNodesIntersectingWith(Line line)
        {
            var list = new List<MapObject>();
            if (IsCodeHidden || IsVisgroupHidden) return list;
            if (!(this is World))
            {
                if (BoundingBox == null || !BoundingBox.IntersectsWith(line)) return list;
                if (this is Solid || this is Entity) list.Add(this);
            }
            list.AddRange(Children.SelectMany(x => x.GetAllNodesIntersectingWith(line)));
            return list;
        }

        /// <summary>
        /// Get all the nodes starting from this node that are entirely contained within a box.
        /// </summary>
        /// <param name="box">The containing box</param>
        /// <returns>A list of all the descendants that are contained within the box.</returns>
        public IEnumerable<MapObject> GetAllNodesContainedWithin(Box box)
        {
            var list = new List<MapObject>();
            if (!(this is World) && !IsCodeHidden && !IsVisgroupHidden)
            {
                if (BoundingBox == null || !BoundingBox.ContainedWithin(box)) return list;
                if (this is Solid || this is Entity) list.Add(this);
            }
            list.AddRange(Children.SelectMany(x => x.GetAllNodesContainedWithin(box)));
            return list;
        }

        /// <summary>
        /// Get all the solid nodes starting from this node where the
        /// edges of the solid intersect with the provided box.
        /// </summary>
        /// <param name="box">The intersection box</param>
        /// <returns>A list of all the solid descendants where the edges of the solid intersect with the box.</returns>
        public IEnumerable<MapObject> GetAllNodesIntersecting2DLineTest(Box box)
        {
            var list = new List<MapObject>();
            if (!(this is World) && !IsCodeHidden && !IsVisgroupHidden)
            {
                if (BoundingBox == null || !BoundingBox.IntersectsWith(box)) return list;
                if (this is Solid && ((Solid)this).Faces.Any(f => f.IntersectsWithLine(box)))
                {
                    list.Add(this);
                }
            }
            list.AddRange(Children.SelectMany(x => x.GetAllNodesIntersecting2DLineTest(box)));
            return list;
        }

        /// <summary>
        /// Returns true if this object is in the given visgroup.
        /// </summary>
        /// <param name="visgroup">The visgroup to check</param>
        /// <returns>True if this object is in the visgroup</returns>
        public bool IsInVisgroup(int visgroup)
        {
            return Visgroups.Contains(visgroup);
        }

        /// <summary>
        /// Get all the parents of this node that match a predicate. The first item in the list will be the closest parent.
        /// </summary>
        /// <param name="matcher">The predicate to match</param>
        /// <param name="includeWorld">True to include the World element at the bottom of the list</param>
        /// <returns>The list of parents</returns>
        public IEnumerable<MapObject> FindParents(Predicate<MapObject> matcher, bool includeWorld = false)
        {
            var o = this;
            while (o.Parent != null)
            {
                if (o.Parent is World && !includeWorld) break;
                if (matcher(o.Parent)) yield return o.Parent;
                o = o.Parent;
            }
        }

        /// <summary>
        /// Find the first parent of this object that matches a predicate.
        /// </summary>
        /// <param name="matcher">The predicate to match</param>
        /// <returns>The matching parent if it was found, null otherwise</returns>
        public MapObject FindClosestParent(Predicate<MapObject> matcher)
        {
            return FindParents(matcher).FirstOrDefault();
        }

        /// <summary>
        /// Find the last parent of this object that matches a predicate.
        /// </summary>
        /// <param name="matcher">The predicate to match</param>
        /// <returns>The matching parent if it was found, null otherwise</returns>
        public MapObject FindTopmostParent(Predicate<MapObject> matcher)
        {
            return FindParents(matcher).LastOrDefault();
        }

        /// <summary>
        /// Get the single object in the object tree with the given ID.
        /// </summary>
        /// <param name="id">The ID of the object to locate</param>
        /// <returns>The object with the matching ID or null if it wasn't found</returns>
        public MapObject FindByID(long id)
        {
            return ID == id ? this : Children.Select(x => x.FindByID(id)).FirstOrDefault(x => x != null);
        }

        /// <summary>
        /// Flattens the tree underneath this node.
        /// </summary>
        /// <returns>A list containing all the descendants of this node (including this node)</returns>
        public List<MapObject> FindAll()
        {
            return Find(x => true);
        }

        /// <summary>
        /// Flattens the tree and selects the nodes that match the test.
        /// </summary>
        /// <param name="matcher">The prediacate to match</param>
        /// <param name="forceMatchIfParentMatches">If true and a parent matches the predicate, all children will be added regardless of match status.</param>
        /// <returns>A list of all the descendants that match the test (including this node)</returns>
        public List<MapObject> Find(Predicate<MapObject> matcher, bool forceMatchIfParentMatches = false)
        {
            var list = new List<MapObject>();
            FindRecursive(list, matcher, forceMatchIfParentMatches);
            return list;
        }

        /// <summary>
        /// Recursively collect children matching a predicate.
        /// </summary>
        /// <param name="items">The list to populate</param>
        /// <param name="matcher">The prediacate to match</param>
        /// <param name="forceMatchIfParentMatches">If true and a parent matches the predicate, all children will be added regardless of match status.</param>
        private void FindRecursive(ICollection<MapObject> items, Predicate<MapObject> matcher, bool forceMatchIfParentMatches = false)
        {
            var thisMatch = matcher(this);
            if (thisMatch)
            {
                items.Add(this);
                if (forceMatchIfParentMatches) matcher = x => true;
            }
            Children.ForEach(x => x.FindRecursive(items, matcher, forceMatchIfParentMatches));
        }

        /// <summary>
        /// Recursively modify children matching a predicate.
        /// </summary>
        /// <param name="action">The action to perform on matching children</param>
        /// <param name="matcher">The prediacate to match</param>
        /// <param name="forceMatchIfParentMatches">If true and a parent matches the predicate, all children will be modified regardless of match status.</param>
        public void ForEach(Predicate<MapObject> matcher, Action<MapObject> action, bool forceMatchIfParentMatches = false)
        {
            var thisMatch = matcher(this);
            if (thisMatch)
            {
                action(this);
                if (forceMatchIfParentMatches) matcher = x => true;
            }
            Children.ForEach(x => x.ForEach(matcher, action, forceMatchIfParentMatches));
        }
    }
}
