using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Primitives.MapData
{
    [Serializable]
    public class Visgroup : IMapData
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public bool Visible { get; set; } = true;
        public Color Colour { get; set; }
        public long Parent { get; set; }
        public HashSet<IMapObject> Objects { get; set; }

        public Visgroup()
        {
            Objects = new HashSet<IMapObject>();
        }

        public Visgroup(SerialisedObject obj)
        {
            Objects = new HashSet<IMapObject>();
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
            Parent = info.GetInt32("Parent");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", ID);
            info.AddValue("Name", Name);
            info.AddValue("Visible", Visible);
            info.AddValue("Parent", Parent);
        }

        public virtual IMapElement Clone()
        {
            return new Visgroup
            {
                ID = ID,
                Name = Name,
                Visible = Visible,
                Colour = Colour,
                Parent = Parent
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
                Parent = Parent
            };
        }

        public SerialisedObject ToSerialisedObject()
        {
            var v = new SerialisedObject("visgroup");
            v.Set("ID", ID);
            v.Set("Name", Name);
            v.Set("Visible", Visible);
            v.SetColor("Colour", Colour);
            v.Set("ParentID", Parent);
            return v;
        }
    }
}