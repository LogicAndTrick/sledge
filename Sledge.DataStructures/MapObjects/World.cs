using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.DataStructures.MapObjects
{
    public class World : MapObject
    {
        public EntityData EntityData { get; set; }
        public List<Path> Paths { get; private set; }

        public World()
        {
            Paths = new List<Path>();
            EntityData = new EntityData();
        }

        public override MapObject Clone()
        {
            var e = new World
            {
                EntityData = EntityData.Clone(),
            };
            e.Paths.AddRange(Paths.Select(x => x.Clone()));
            CloneBase(e);
            return e;
        }

        public override void Unclone(MapObject o)
        {
            UncloneBase(o);
            var e = o as World;
            if (e == null) return;
            EntityData = e.EntityData.Clone();
            Paths.Clear();
            Paths.AddRange(e.Paths.Select(x => x.Clone()));
        }
    }
}
