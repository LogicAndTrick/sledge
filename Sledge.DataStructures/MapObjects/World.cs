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

        public World(long id) : base(id)
        {
            Paths = new List<Path>();
            EntityData = new EntityData {Name = "worldspawn"};
        }

        public override MapObject Clone(IDGenerator generator)
        {
            var e = new World(generator.GetNextObjectID())
            {
                EntityData = EntityData.Clone(),
            };
            e.Paths.AddRange(Paths.Select(x => x.Clone()));
            CloneBase(e, generator);
            return e;
        }

        public override void Unclone(MapObject o, IDGenerator generator)
        {
            UncloneBase(o, generator);
            var e = o as World;
            if (e == null) return;
            EntityData = e.EntityData.Clone();
            Paths.Clear();
            Paths.AddRange(e.Paths.Select(x => x.Clone()));
        }

        public override EntityData GetEntityData()
        {
            return EntityData;
        }
    }
}
