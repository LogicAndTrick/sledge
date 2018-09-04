using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Numerics;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapObjects
{
    public class Entity : BaseMapObject
    {
        public EntityData EntityData => Data.GetOne<EntityData>();
        public ObjectColor Color => Data.GetOne<ObjectColor>();

        public Vector3 Origin
        {
            get => Data.GetOne<Origin>()?.Location ?? Vector3.Zero;
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
                : MakeBoundingBox();
        }

        private Box MakeBoundingBox()
        {
            foreach (var p in Data.Get<IBoundingBoxProvider>())
            {
                var box = p.GetBoundingBox(this);
                if (box != null) return box;
            }

            var root = this.GetRoot();
            if (root != null)
            {
                foreach (var p in root.Data.Get<IBoundingBoxProvider>())
                {
                    var box = p.GetBoundingBox(this);
                    if (box != null) return box;
                }
            }

            return new Box(Origin - Vector3.One * 8, Origin + Vector3.One * 8);
        }

        public override IEnumerable<Polygon> GetPolygons()
        {
            // Entities with children don't contain any geometry directly
            if (Hierarchy.HasChildren) return new Polygon[0];

            // Otherwise we use the bounding box faces
            return BoundingBox.GetBoxFaces().Select(x => new Polygon(x));
        }

        protected override string SerialisedName => "Entity";

        public override IEnumerable<IMapObject> Decompose(IEnumerable<Type> allowedTypes)
        {
            yield return this;
        }
    }
}