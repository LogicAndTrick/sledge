using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapObjects
{
    /// <summary>
    /// The root node of a map object tree
    /// </summary>
    public class Root : BaseMapObject
    {
        public override bool IsSelected
        {
            get => false;
            set {}
        }

        public Root(long id) : base(id)
        {
        }

        public Root(SerialisedObject obj) : base(obj)
        {
        }

        [Export(typeof(IMapElementFormatter))]
        public class RootFormatter : StandardMapElementFormatter<Root> { }

        protected override Box GetBoundingBox()
        {
            return Hierarchy.NumChildren > 0 ? new Box(Hierarchy.Select(x => x.BoundingBox)) : Box.Empty;
        }
        
        public override IEnumerable<Polygon> GetPolygons()
        {
            // The root never contains geometry
            yield break;
        }

        protected override string SerialisedName => "Root";

        public override IEnumerable<IMapObject> Decompose(IEnumerable<Type> allowedTypes)
        {
            yield return this;
        }
    }
}