using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapObjects
{
    /// <summary>
    /// The root node of a map object tree
    /// </summary>
    public class Root : BaseMapObject, IPrimitive
    {
        public Root(long id) : base(id)
        {
        }

        protected override Box GetBoundingBox()
        {
            return Hierarchy.NumChildren > 0 ? new Box(Hierarchy.Select(x => x.BoundingBox)) : Box.Empty;
        }

        public override Coordinate Intersect(Line line)
        {
            // Selecting the root would be awkward
            return null;
        }

        public override IMapObject Clone()
        {
            var root = new Root(ID);
            CloneBase(root);
            return root;
        }

        public override void Unclone(IMapObject obj)
        {
            if (!(obj is Root)) throw new ArgumentException("Cannot unclone into a different type.", nameof(obj));
            UncloneBase((BaseMapObject) obj);
        }

        public override IEnumerable<IPrimitive> ToPrimitives()
        {
            yield return this;
        }
    }
}