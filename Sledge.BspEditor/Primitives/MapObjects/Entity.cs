using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapObjects
{
    public class Entity : BaseMapObject, IPrimitive
    {
        public EntityData EntityData => Data.GetOne<EntityData>();
        public ObjectColor Color => Data.GetOne<ObjectColor>();
        public Coordinate Origin { get; set; }

        public Entity(long id) : base(id)
        {
        }

        public Entity(SerialisedObject obj) : base(obj)
        {
            Origin = obj.Get<Coordinate>("Origin");
        }

        [Export(typeof(IMapElementFormatter))]
        public class EntityFormatter : StandardMapElementFormatter<Entity> { }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Origin", Origin);
        }

        protected override Box GetBoundingBox()
        {
            return Hierarchy.NumChildren > 0
                ? new Box(Hierarchy.Select(x => x.BoundingBox))
                : new Box(Origin - Coordinate.One * 16, Origin + Coordinate.One * 16);
        }

        public override IEnumerable<Polygon> GetPolygons()
        {
            // Entities with children don't contain any geometry directly
            if (Hierarchy.HasChildren) return new Polygon[0];

            // Otherwise we use the bounding box faces
            return BoundingBox.GetBoxFaces().Select(x => new Polygon(x));
        }

        protected override string SerialisedName => "Entity";

        protected override void AddCustomSerialisedData(SerialisedObject obj)
        {
            obj.Set("Origin", Origin);
            base.AddCustomSerialisedData(obj);
        }

        public override IMapObject Clone()
        {
            var ent = new Entity(ID);
            CloneBase(ent);
            ent.Origin = Origin.Clone();
            return ent;
        }

        public override void Unclone(IMapObject obj)
        {
            if (!(obj is Entity)) throw new ArgumentException("Cannot unclone into a different type.", nameof(obj));
            UncloneBase((BaseMapObject)obj);
        }

        public override IEnumerable<IPrimitive> ToPrimitives()
        {
            yield return this;
        }
    }
}