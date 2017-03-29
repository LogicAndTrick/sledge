using System;
using System.Collections.Generic;

namespace Sledge.BspEditor.Primitives
{
    /// <summary>
    /// The root node of a map object tree
    /// </summary>
    public class Root : BaseMapObject, IPrimitive
    {
        public Root(long id) : base(id)
        {
        }

        public override void DescendantsChanged()
        {
            //
        }

        public override IMapObject Clone()
        {
            throw new NotImplementedException();
        }

        public override void Unclone(IMapObject obj)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<IPrimitive> ToPrimitives()
        {
            yield return this;
        }
    }
}