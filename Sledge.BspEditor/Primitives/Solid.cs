using System;
using System.Collections.Generic;

namespace Sledge.BspEditor.Primitives
{
    /// <summary>
    /// A collection of faces
    /// </summary>
    public class Solid : BaseMapObject, IPrimitive
    {
        public Solid(long id) : base(id)
        {
        }

        public override void Unclone(IMapObject obj)
        {
            throw new NotImplementedException();
        }

        public override void DescendantsChanged()
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