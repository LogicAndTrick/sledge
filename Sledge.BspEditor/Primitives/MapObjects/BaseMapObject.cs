using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapObjects
{
    /// <summary>
    /// Nice and simple base class for all map objects. Strongly recommended but
    /// not specifically required to use this class as a base.
    /// </summary>
    public abstract class BaseMapObject : IMapObject
    {
        public long ID { get; }
        public bool IsSelected { get; set; }
        public Box BoundingBox { get; private set; }
        public MapObjectDataCollection Data { get; private set; }
        public MapObjectHierarchy Hierarchy { get; private set; }

        protected BaseMapObject(long id)
        {
            ID = id;
            Data = new MapObjectDataCollection();
            Hierarchy = new MapObjectHierarchy(this);
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", ID);
            info.AddValue("Data", Data);
            info.AddValue("Children", Hierarchy.ToArray());
        }

        public void DescendantsChanged()
        {
            OnDescendantsChanged();
            BoundingBox = GetBoundingBox();
            Hierarchy.Parent?.DescendantsChanged();
        }

        protected virtual void OnDescendantsChanged()
        {
            //
        }

        protected abstract Box GetBoundingBox();

        protected void CloneBase(BaseMapObject copy)
        {
            copy.Data = Data.Clone();
            foreach (var child in Hierarchy)
            {
                var c = child.Clone();
                c.Hierarchy.Parent = copy;
            }
        }

        protected void UncloneBase(BaseMapObject source)
        {
            Data = source.Data.Clone();
            Hierarchy.Clear();
            foreach (var obj in source.Hierarchy)
            {
                var copy = obj.Clone();
                copy.Hierarchy.Parent = this;
            }
        }

        public abstract Coordinate Intersect(Line line);
        public abstract IMapObject Clone();
        public abstract void Unclone(IMapObject obj);
        public abstract IEnumerable<IPrimitive> ToPrimitives();

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IMapObject)obj);
        }

        public bool Equals(IMapObject other)
        {
            return other != null && other.ID == ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}