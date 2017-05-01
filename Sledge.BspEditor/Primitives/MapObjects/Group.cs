using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapObjects
{
    /// <summary>
    /// A collection of objects
    /// </summary>
    public class Group : BaseMapObject, IPrimitive
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

        public override IMapObject Clone()
        {
            var grp = new Group(ID);
            CloneBase(grp);
            return grp;
        }

        public override void Unclone(IMapObject obj)
        {
            if (!(obj is Group)) throw new ArgumentException("Cannot unclone into a different type.", nameof(obj));
            UncloneBase((BaseMapObject)obj);
        }

        public override IEnumerable<IPrimitive> ToPrimitives()
        {
            yield return this;
        }
    }
}