using System;
using System.Collections.Generic;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives
{
    public class Entity : BaseMapObject, IPrimitive
    {
        public EntityData EntityData => Data.GetOne<EntityData>();
        public Coordinate Origin { get; set; }

        public Entity(long id) : base(id)
        {
        }

        public override void DescendantsChanged()
        {
            Hierarchy.Parent?.DescendantsChanged();
        }

        public override void Unclone(IMapObject obj)
        {
            throw new NotImplementedException();
        }

        public override IMapObject Clone()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<IPrimitive> ToPrimitives()
        {
            yield return this;
        }
    }
}