using System;
using System.Collections.Generic;

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

        public override void DescendantsChanged()
        {
            Hierarchy.Parent?.DescendantsChanged();
        }

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