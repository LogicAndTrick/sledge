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

        public override MapObject Copy(IDGenerator generator)
        {
            var group = new Group(generator.GetNextObjectID());
            CopyBase(group, generator);
            return group;
        }

        public override void Paste(MapObject o, IDGenerator generator)
        {
            PasteBase(o, generator);
        }

        public override MapObject Clone()
        {
            var group = new Group(ID);
            CopyBase(group, null, true);
            return group;
        }

        public override void Unclone(MapObject o)
        {
            PasteBase(o, null, true);
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
