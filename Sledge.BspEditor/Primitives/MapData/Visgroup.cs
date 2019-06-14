using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Threading;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Primitives.MapData
{
    [Serializable]
    public class Visgroup : IMapData
    {
        public bool AffectsRendering => false;

        public long ID { get; set; }
        public string Name { get; set; }
        public bool Visible { get; set; } = true;
        public Color Colour { get; set; } = Color.Gray;
        public ISet<IMapObject> Objects { get; set; }

        public Visgroup()
        {
            Objects = new ThreadSafeSet<IMapObject>();
        }

        public Visgroup(SerialisedObject obj)
        {
            Objects = new ThreadSafeSet<IMapObject>();
            ID = obj.Get<long>("ID");
            Name = obj.Get<string>("Name");
            Visible = obj.Get<bool>("Visible");
            Colour = obj.GetColor("Colour");
        }

        [Export(typeof(IMapElementFormatter))]
        public class VisgroupFormatter : StandardMapElementFormatter<Visgroup>
        {
        }

        protected Visgroup(SerializationInfo info, StreamingContext context)
        {
            ID = info.GetInt64("ID");
            Name = info.GetString("Name");
            Visible = info.GetBoolean("Visible");
            Colour = Color.FromArgb(info.GetInt32("Colour"));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", ID);
            info.AddValue("Name", Name);
            info.AddValue("Visible", Visible);
            // Colour??
        }

        public virtual IMapElement Clone()
        {
            return new Visgroup
            {
                ID = ID,
                Name = Name,
                Visible = Visible,
                Colour = Colour,
            };
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return new Visgroup
            {
                ID = numberGenerator.Next("Visgroup"),
                Name = Name,
                Visible = Visible,
                Colour = Colour,
            };
        }

        public SerialisedObject ToSerialisedObject()
        {
            var v = new SerialisedObject("Visgroup");
            v.Set("ID", ID);
            v.Set("Name", Name);
            v.Set("Visible", Visible);
            v.SetColor("Colour", Colour);
            return v;
        }
    }
}