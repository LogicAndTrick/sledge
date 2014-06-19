using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sledge.DataStructures.MapObjects
{
    [Serializable]
    public class World : MapObject
    {
        public EntityData EntityData { get; set; }
        public List<Path> Paths { get; private set; }

        public World(long id) : base(id)
        {
            Paths = new List<Path>();
            EntityData = new EntityData {Name = "worldspawn"};
        }

        protected World(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            EntityData = (EntityData) info.GetValue("EntityData", typeof (EntityData));
            Paths = ((Path[])info.GetValue("Paths", typeof(Path[]))).ToList();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("EntityData", EntityData);
            info.AddValue("Paths", Paths.ToArray());
        }

        public override MapObject Copy(IDGenerator generator)
        {
            var e = new World(generator.GetNextObjectID())
            {
                EntityData = EntityData.Clone(),
            };
            e.Paths.AddRange(Paths.Select(x => x.Clone()));
            CopyBase(e, generator);
            return e;
        }

        public override void Paste(MapObject o, IDGenerator generator)
        {
            PasteBase(o, generator);
            var e = o as World;
            if (e == null) return;
            EntityData = e.EntityData.Clone();
            Paths.Clear();
            Paths.AddRange(e.Paths.Select(x => x.Clone()));
        }

        public override MapObject Clone()
        {
            var e = new World(ID)
            {
                EntityData = EntityData.Clone(),
            };
            e.Paths.AddRange(Paths.Select(x => x.Clone()));
            CopyBase(e, null, true);
            return e;
        }

        public override void Unclone(MapObject o)
        {
            PasteBase(o, null, true);
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
