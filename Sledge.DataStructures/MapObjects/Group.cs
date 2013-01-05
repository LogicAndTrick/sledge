using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.MapObjects
{
    public class Group : MapObject
    {
        public Group(long id) : base(id)
        {
        }

        public override MapObject Clone(IDGenerator generator)
        {
            var group = new Group(generator.GetNextObjectID());
            CloneBase(group, generator);
            return group;
        }

        public override void Unclone(MapObject o, IDGenerator generator)
        {
            UncloneBase(o, generator);
        }

        public override void UpdateBoundingBox(bool cascadeToParent = true)
        {
            BoundingBox = Children.All(x => x.BoundingBox == null)
                              ? null
                              : new Box(Children.Where(x => x.BoundingBox != null).Select(x => x.BoundingBox));
            base.UpdateBoundingBox(cascadeToParent);
        }
    }
}
