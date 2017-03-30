using System;
using System.Collections.Generic;

namespace Sledge.BspEditor.Primitives.MapObjects
{
    /// <summary>
    /// A collection of faces
    /// </summary>
    public class Solid : BaseMapObject, IPrimitive
    {
        public Solid(long id) : base(id)
        {
        }

        public override void DescendantsChanged()
        {
            Hierarchy.Parent?.DescendantsChanged();
        }

        public override IMapObject Clone()
        {
            var solid = new Solid(ID);
            CloneBase(solid);
            return solid;
        }

        public override void Unclone(IMapObject obj)
        {
            if (!(obj is Solid)) throw new ArgumentException("Cannot unclone into a different type.", nameof(obj));
            UncloneBase((BaseMapObject)obj);
        }

        public override IEnumerable<IPrimitive> ToPrimitives()
        {
            yield return this;
        }
    }
}