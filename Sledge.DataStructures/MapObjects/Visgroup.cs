using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Runtime.Serialization;

namespace Sledge.DataStructures.MapObjects
{
    [Serializable]
    public class Visgroup : ISerializable
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Visible { get; set; }
        public Color Colour { get; set; }
        public Visgroup Parent { get; set; }
        public List<Visgroup> Children { get; set; }

        public virtual bool IsAutomatic { get { return false; }}

        public Visgroup()
        {
            Children = new List<Visgroup>();
        }

        protected Visgroup(SerializationInfo info, StreamingContext context)
        {
            ID = info.GetInt32("ID");
            Name = info.GetString("Name");
            Visible = info.GetBoolean("Visible");
            Colour = Color.FromArgb(info.GetInt32("Colour"));
            Children = ((Visgroup[]) info.GetValue("Children", typeof (Visgroup[]))).ToList();
            Children.ForEach(x => x.Parent = this);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", ID);
            info.AddValue("Name", Name);
            info.AddValue("Visible", Visible);
            info.AddValue("Colour", Colour.ToArgb());
            info.AddValue("Children", Children.ToArray());
        }

        public virtual Visgroup Clone()
        {
            return new Visgroup
                       {
                           ID = ID,
                           Name = Name,
                           Visible = Visible,
                           Colour = Colour,
                           Children = Children.Select(x => x.Clone()).ToList()
                       };
        }
    }
}
