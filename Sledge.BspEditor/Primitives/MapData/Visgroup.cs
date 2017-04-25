using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;

namespace Sledge.BspEditor.Primitives.MapData
{
    [Serializable]
    public class Visgroup : IMapData
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public bool Visible { get; set; }
        public Color Colour { get; set; }
        public long Parent { get; set; }

        public Visgroup()
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

        public virtual IMapData Clone()
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
    }
}