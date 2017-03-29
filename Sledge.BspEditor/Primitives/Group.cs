using System;
using System.Collections.Generic;

namespace Sledge.BspEditor.Primitives
{
    /// <summary>
    /// A collection of objects
    /// </summary>
    public class Group : BaseMapObject, IPrimitive
    {
        public Group(long id) : base(id)
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