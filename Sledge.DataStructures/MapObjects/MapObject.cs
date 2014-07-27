using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Drawing;
using System.Runtime.Serialization;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.Meta;
using Sledge.DataStructures.Transformations;

namespace Sledge.DataStructures.MapObjects
{
    [Serializable]
    public abstract class MapObject : ISerializable
    {
        public long ID { get; set; }
        public string ClassName { get; set; }
        public List<int> Visgroups { get; set; }
        public List<int> AutoVisgroups { get; set; }
        protected Dictionary<long, MapObject> Children { get; set; }
        public MapObject Parent { get; private set; }
        public virtual Color Colour { get; set; }
        public bool IsSelected { get; set; }
        public bool IsCodeHidden { get; set; }
        public bool IsRenderHidden2D { get; set; }
        public bool IsRenderHidden3D { get; set; }
        public bool IsVisgroupHidden { get; set; }
        public Box BoundingBox { get; set; }
        public MetaData MetaData { get; private set; }

        protected MapObject(long id)
        {
            ID = id;
            Visgroups = new List<int>();
            AutoVisgroups = new List<int>();
            Children = new Dictionary<long, MapObject>();
            MetaData = new MetaData();
        }

        protected MapObject(SerializationInfo info, StreamingContext context)
        {
            ID = info.GetInt64("ID");
            ClassName = info.GetString("ClassName");
            Visgroups = info.GetString("Visgroups").Split(',').Select(x => int.Parse(x, CultureInfo.InvariantCulture)).ToList();
            AutoVisgroups = info.GetString("AutoVisgroups").Split(',').Select(x => int.Parse(x, CultureInfo.InvariantCulture)).ToList();
            Colour = Color.FromArgb(info.GetInt32("Colour"));

            var children = (MapObject[]) info.GetValue("Children", typeof (MapObject[]));
            foreach (var child in children)
            {
                child.SetParent(this);
            }
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", ID);
            info.AddValue("ClassName", ClassName);
            info.AddValue("Visgroups", String.Join(",", Visgroups.Select(x => x.ToString(CultureInfo.InvariantCulture))));
            info.AddValue("AutoVisgroups", String.Join(",", AutoVisgroups.Select(x => x.ToString(CultureInfo.InvariantCulture))));
            info.AddValue("Colour", Colour.ToArgb());
            info.AddValue("Children", GetChildren().ToArray());
        }

        public IEnumerable<MapObject> GetChildren()
        {
            return Children.Values.ToList();
        }

        public int ChildCount
        {
            get { return Children.Count; }
        }

        public bool HasChildren
        {
            get { return Children.Count > 0; }
        }

        /// <summary>
        /// Creates an exact copy of this object with a new ID.
        /// </summary>
        public abstract MapObject Copy(IDGenerator generator);

        /// <summary>
        /// Copies all the values of the provided object into this one, copying children and faces.
        /// </summary>
        public abstract void Paste(MapObject o, IDGenerator generator);

        /// <summary>
        /// Creates an exact clone of this object with the same ID.
        /// </summary>
        public abstract MapObject Clone();

        /// <summary>
        /// Copies all the values of the provided object into this one, cloning children and faces.
        /// </summary>
        public abstract void Unclone(MapObject o);

        protected void CopyBase(MapObject o, IDGenerator generator, bool performClone = false)
        {
            if (performClone && o.ID != ID)
            {
                var parent = o.Parent;
                var setPar = o.Parent != null && o.Parent.Children.ContainsKey(o.ID) && o.Parent.Children[o.ID] == o;
                if (setPar) o.SetParent(null);
                o.ID = ID;
                if (setPar) o.SetParent(parent);
            }
            o.ClassName = ClassName;
            o.Visgroups.AddRange(Visgroups);
            o.AutoVisgroups.AddRange(AutoVisgroups);
            o.Parent = Parent;
            o.Colour = Colour;
            o.IsSelected = IsSelected;
            o.IsCodeHidden = IsCodeHidden;
            o.IsRenderHidden2D = IsRenderHidden2D;
            o.IsRenderHidden3D = IsRenderHidden3D;
            o.IsVisgroupHidden = IsVisgroupHidden;
            o.BoundingBox = BoundingBox.Clone();
            o.MetaData = MetaData.Clone();
            var children = GetChildren().Select(x => performClone ? x.Clone() : x.Copy(generator));
            foreach (var c in children)
            {
                c.SetParent(o);
            }
        }

        protected void PasteBase(MapObject o, IDGenerator generator, bool performUnclone = false)
        {
            Visgroups.Clear();
            AutoVisgroups.Clear();
            Children.Clear();

            if (performUnclone && o.ID != ID)
            {
                var parent = Parent;
                var setPar = Parent != null && Parent.Children.ContainsKey(ID) && Parent.Children[ID] == this;
                if (setPar) SetParent(null);
                ID = o.ID;
                if (setPar) SetParent(parent);
            }
            ClassName = o.ClassName;
            Visgroups.AddRange(o.Visgroups);
            AutoVisgroups.AddRange(o.AutoVisgroups);
            Parent = o.Parent;
            Colour = o.Colour;
            IsSelected = o.IsSelected;
            IsCodeHidden = o.IsCodeHidden;
            IsRenderHidden2D = o.IsRenderHidden2D;
            IsRenderHidden3D = o.IsRenderHidden3D;
            IsVisgroupHidden = o.IsVisgroupHidden;
            BoundingBox = o.BoundingBox.Clone();
            MetaData = o.MetaData.Clone();

            var children = o.GetChildren().Select(x => performUnclone ? x.Clone() : x.Copy(generator));
            foreach (var c in children)
            {
                c.SetParent(this);
            }
        }

        public void SetParent(MapObject parent, bool updateBoundingBox = true)
        {
            if (Parent != null)
            {
                if (Parent.Children.ContainsKey(ID) && Parent.Children[ID] == this) Parent.Children.Remove(ID);
                if (updateBoundingBox) Parent.UpdateBoundingBox();
            }
            Parent = parent;
            if (Parent != null)
            {
                Parent.Children[ID] = this;
                if (updateBoundingBox) UpdateBoundingBox();
            }
        }

        public bool RemoveDescendant(MapObject remove)
        {
            if (remove == null) return false;
            if (Children.Remove(remove.ID)) return true;
            foreach (var child in GetChildren())
            {
                if (child.RemoveDescendant(remove)) return true;
            }
            return false;
        }

        public virtual void UpdateBoundingBox(bool cascadeToParent = true)
        {
            if (cascadeToParent && Parent != null)
            {
                Parent.UpdateBoundingBox();
            }
        }

        public virtual void Transform(IUnitTransformation transform, TransformFlags flags)
        {
            foreach (var mo in GetChildren())
            {
                mo.Transform(transform, flags);
            }
            UpdateBoundingBox();
        }

        public virtual Coordinate GetIntersectionPoint(Line line)
        {
            return null;
        }

        public virtual EntityData GetEntityData()
        {
            return null;
        }

        public virtual Box GetIntersectionBoundingBox()
        {
            return BoundingBox;
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
        /// <param name="allowCodeHidden">Set to true to include nodes that have been hidden by code</param>
        /// <param name="allowVisgroupHidden">Set to true to include nodes that have been hidden by the user</param>
        /// <returns>A list of all the descendants that intersect with the box.</returns>
        public IEnumerable<MapObject> GetAllNodesIntersectingWith(Box box, bool allowCodeHidden = false, bool allowVisgroupHidden = false)
        {
            var list = new List<MapObject>();
            if ((!allowCodeHidden && IsCodeHidden) || (!allowVisgroupHidden && IsVisgroupHidden)) return list;
            if (!(this is World))
            {
                if (BoundingBox == null || !BoundingBox.IntersectsWith(box)) return list;
                if (this is Solid || this is Entity) list.Add(this);
            }
            list.AddRange(GetChildren().SelectMany(x => x.GetAllNodesIntersectingWith(box, allowCodeHidden, allowVisgroupHidden)));
            return list;
        }

        /// <summary>
        /// Get all the nodes starting from this node with the centers contained within a box.
        /// </summary>
        /// <param name="box">The containing box</param>
        /// <param name="allowCodeHidden">Set to true to include nodes that have been hidden by code</param>
        /// <param name="allowVisgroupHidden">Set to true to include nodes that have been hidden by the user</param>
        /// <returns>A list of all the descendants that have centers inside the box.</returns>
        public IEnumerable<MapObject> GetAllNodesWithCentersContainedWithin(Box box, bool allowCodeHidden = false, bool allowVisgroupHidden = false)
        {
            var list = new List<MapObject>();
            if ((!allowCodeHidden && IsCodeHidden) || (!allowVisgroupHidden && IsVisgroupHidden)) return list;
            if (!(this is World))
            {
                if (BoundingBox == null || !box.CoordinateIsInside(BoundingBox.Center)) return list;
                if ((this is Solid || this is Entity) && !Children.Any()) list.Add(this);
            }
            list.AddRange(GetChildren().SelectMany(x => x.GetAllNodesWithCentersContainedWithin(box, allowCodeHidden, allowVisgroupHidden)));
            return list;
        }

        /// <summary>
        /// Get all the nodes starting from this node that intersect with a line.
        /// </summary>
        /// <param name="line">The intersection line</param>
        /// <param name="allowCodeHidden">Set to true to include nodes that have been hidden by code</param>
        /// <param name="allowVisgroupHidden">Set to true to include nodes that have been hidden by the user</param>
        /// <returns>A list of all the descendants that intersect with the line.</returns>
        public IEnumerable<MapObject> GetAllNodesIntersectingWith(Line line, bool allowCodeHidden = false, bool allowVisgroupHidden = false)
        {
            var list = new List<MapObject>();
            if ((!allowCodeHidden && IsCodeHidden) || (!allowVisgroupHidden && IsVisgroupHidden)) return list;
            if (!(this is World))
            {
                var bbox = GetIntersectionBoundingBox();
                if (bbox == null || !bbox.IntersectsWith(line)) return list;
                if (this is Solid || this is Entity) list.Add(this);
            }
            list.AddRange(GetChildren().SelectMany(x => x.GetAllNodesIntersectingWith(line, allowCodeHidden, allowVisgroupHidden)));
            return list;
        }

        /// <summary>
        /// Get all the nodes starting from this node that are entirely contained within a box.
        /// </summary>
        /// <param name="box">The containing box</param>
        /// <param name="allowCodeHidden">Set to true to include nodes that have been hidden by code</param>
        /// <param name="allowVisgroupHidden">Set to true to include nodes that have been hidden by the user</param>
        /// <returns>A list of all the descendants that are contained within the box.</returns>
        public IEnumerable<MapObject> GetAllNodesContainedWithin(Box box, bool allowCodeHidden = false, bool allowVisgroupHidden = false)
        {
            var list = new List<MapObject>();
            if (!(this is World) && (allowCodeHidden || !IsCodeHidden) && (allowVisgroupHidden || !IsVisgroupHidden))
            {
                if (BoundingBox == null || !BoundingBox.ContainedWithin(box)) return list;
                if (this is Solid || this is Entity) list.Add(this);
            }
            list.AddRange(GetChildren().SelectMany(x => x.GetAllNodesContainedWithin(box, allowCodeHidden, allowVisgroupHidden)));
            return list;
        }

        /// <summary>
        /// Get all the solid nodes starting from this node where the
        /// edges of the solid intersect with the provided box.
        /// </summary>
        /// <param name="box">The intersection box</param>
        /// <param name="includeOrigin">Set to true to test against the object origins as well</param>
        /// <param name="forceOrigin">Set to true to only test against the object origins and ignore other tests</param>
        /// <param name="allowCodeHidden">Set to true to include nodes that have been hidden by code</param>
        /// <param name="allowVisgroupHidden">Set to true to include nodes that have been hidden by the user</param>
        /// <returns>A list of all the solid descendants where the edges of the solid intersect with the box.</returns>
        public IEnumerable<MapObject> GetAllNodesIntersecting2DLineTest(Box box, bool includeOrigin = false, bool forceOrigin = false, bool allowCodeHidden = false, bool allowVisgroupHidden = false)
        {
            var list = new List<MapObject>();
            if (!(this is World) && (allowCodeHidden || !IsCodeHidden) && (allowVisgroupHidden || !IsVisgroupHidden))
            {
                if (BoundingBox == null || !BoundingBox.IntersectsWith(box)) return list;
                // Solids: Match face edges against box
                if (!forceOrigin && this is Solid && ((Solid)this).Faces.Any(f => f.IntersectsWithLine(box)))
                {
                    list.Add(this);
                }
                // Point entities: Match bounding box edges against box
                else if (!forceOrigin && this is Entity && !Children.Any() && BoundingBox.GetBoxLines().Any(box.IntersectsWith))
                {
                    list.Add(this);
                }
                // Origins: Match bounding box center against box (for solids and point entities)
                else if ((includeOrigin || forceOrigin) && !Children.Any() && (this is Solid || this is Entity) && box.CoordinateIsInside(BoundingBox.Center))
                {
                    list.Add(this);
                }
            }
            list.AddRange(GetChildren().SelectMany(x => x.GetAllNodesIntersecting2DLineTest(box, includeOrigin, forceOrigin, allowCodeHidden, allowVisgroupHidden)));
            return list;
        }

        /// <summary>
        /// Get the visgroups for this object.
        /// </summary>
        /// <param name="inherit">True to include inherited visgroups</param>
        /// <returns>The list of visgroups</returns>
        public IEnumerable<int> GetVisgroups(bool inherit)
        {
            if (!inherit || Parent == null) return Visgroups;
            return Visgroups.Union(Parent.GetVisgroups(true));
        }

        /// <summary>
        /// Returns true if this object is in the given visgroup.
        /// </summary>
        /// <param name="visgroup">The visgroup to check</param>
        /// <param name="inherit">True to consider inherited visgroups</param>
        /// <returns>True if this object is in the visgroup</returns>
        public bool IsInVisgroup(int visgroup, bool inherit)
        {
            return GetVisgroups(inherit).Contains(visgroup);
        }

        /// <summary>
        /// Returns true if this object is in the given auto visgroup.
        /// </summary>
        /// <param name="visgroup">The auto visgroup to check</param>
        /// <returns>True if this object is in the auto visgroup</returns>
        public bool IsInAutoVisgroup(int visgroup)
        {
            return AutoVisgroups.Contains(visgroup);
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
            if (ID == id) return this;
            if (Children.ContainsKey(id)) return Children[id];
            foreach (var mo in GetChildren())
            {
                var by = mo.FindByID(id);
                if (by != null) return by;
            }
            return null;
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
            foreach (var mo in GetChildren())
            {
                mo.FindRecursive(items, matcher, forceMatchIfParentMatches);
            }
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
            foreach (var mo in GetChildren())
            {
                mo.ForEach(matcher, action, forceMatchIfParentMatches);
            }
        }
    }
}
