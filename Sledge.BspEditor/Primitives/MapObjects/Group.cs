using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapObjects
{
    /// <summary>
    /// A collection of objects
    /// </summary>
    public class Group : BaseMapObject
    {
        public Group(long id) : base(id)
        {
        }

        public Group(SerialisedObject obj) : base(obj)
        {
        }

        [Export(typeof(IMapElementFormatter))]
        public class GroupFormatter : StandardMapElementFormatter<Group> { }

        protected override Box GetBoundingBox()
        {
            return Hierarchy.NumChildren > 0 ? new Box(Hierarchy.Select(x => x.BoundingBox)) : Box.Empty;
        }
        
        public override IEnumerable<Polygon> GetPolygons()
        {
            // Groups are virtual and never contain geometry
            yield break;
        }

        protected override string SerialisedName => "Group";

        public override IEnumerable<IMapObject> Decompose(IEnumerable<Type> allowedTypes)
        {
            yield return this;
        }
    }
}