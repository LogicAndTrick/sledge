using System.Collections.Generic;
using Sledge.BspEditor.Primitives.MapObjectData;

namespace Sledge.BspEditor.Primitives
{
    /// <summary>
    /// Nice and simple base class for all map objects. Strongly recommended but
    /// not specifically required to use this class as a base.
    /// </summary>
    public abstract class BaseMapObject : IMapObject
    {
        public long ID { get; }
        public MapObjectDataCollection Data { get; }
        public MapObjectHierarchy Hierarchy { get; }

        protected BaseMapObject(long id)
        {
            ID = id;
            Data = new MapObjectDataCollection();
            Hierarchy = new MapObjectHierarchy(this);
        }

        public abstract void DescendantsChanged();

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