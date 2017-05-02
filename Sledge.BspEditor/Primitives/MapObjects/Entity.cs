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

        public Coordinate Origin
        {
            get => Data.GetOne<Origin>()?.Location ?? Coordinate.Zero;
            set => Data.Replace(new Origin(value));
        }

        public Entity(long id) : base(id)
        {
        }

        public Entity(SerialisedObject obj) : base(obj)
        {
        }

        [Export(typeof(IMapElementFormatter))]
        public class EntityFormatter : StandardMapElementFormatter<Entity> { }

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

        public override IEnumerable<IPrimitive> ToPrimitives()
        {
            yield return this;
        }
    }
}