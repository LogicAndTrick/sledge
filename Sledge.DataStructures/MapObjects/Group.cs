using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.MapObjects
{
    public class Group : MapObject
    {
        public override MapObject Clone()
        {
            var group = new Group();
            CloneBase(group);
            return group;
        }

        public override void Unclone(MapObject o)
        {
            UncloneBase(o);
        }

        public override void UpdateBoundingBox(bool cascadeToParent = true)
        {
            BoundingBox = !Children.Any(x => x.BoundingBox != null) ? null
                : new Box(Children.Where(x => x.BoundingBox != null).SelectMany(x => new[] {x.BoundingBox.Start, x.BoundingBox.End}));
            base.UpdateBoundingBox(cascadeToParent);
        }
    }
}
