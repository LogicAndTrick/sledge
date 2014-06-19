using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.MapObjects
{
    [Serializable]
    public class EntityData : ISerializable
    {
        public string Name { get; set; }
        public int Flags { get; set; }
        public List<Property> Properties { get; private set; }
        public List<Output> Outputs { get; private set; }

        public EntityData()
        {
            Properties = new List<Property>();
            Outputs = new List<Output>();
        }

        public EntityData(GameData.GameDataObject gd)
        {
            Properties = new List<Property>();
            Outputs = new List<Output>();
            if (gd == null) return;
            Name = gd.Name;
            foreach (var prop in gd.Properties.Where(x => x.Name != "spawnflags"))
            {
                Properties.Add(new Property {Key = prop.Name, Value = prop.DefaultValue});
            }
        }

        protected EntityData(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            Flags = info.GetInt32("Flags");
            Properties = ((Property[]) info.GetValue("Properties", typeof (Property[]))).ToList();
            Outputs = ((Output[]) info.GetValue("Outputs", typeof (Output[]))).ToList();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Flags", Flags);
            info.AddValue("Properties", Properties.ToArray());
            info.AddValue("Outputs", Outputs.ToArray());
        }

        public EntityData Clone()
        {
            var ed = new EntityData { Name = Name, Flags = Flags };
            ed.Properties.AddRange(Properties.Select(x => x.Clone()));
            ed.Outputs.AddRange(Outputs.Select(x => x.Clone()));
            return ed;
        }

        public string GetPropertyValue(string key)
        {
            var prop = Properties.FirstOrDefault(x => String.Equals(key, x.Key, StringComparison.InvariantCultureIgnoreCase));
            return prop == null ? null : prop.Value;
        }

        public void SetPropertyValue(string key, string value)
        {
            var prop = Properties.FirstOrDefault(x => String.Equals(key, x.Key, StringComparison.InvariantCultureIgnoreCase));
            if (prop == null)
            {
                prop = new Property { Key = key};
                Properties.Add(prop);
            }
            prop.Value = value;
        }

        public Coordinate GetPropertyCoordinate(string key, Coordinate def = null)
        {
            var prop = Properties.FirstOrDefault(x => String.Equals(key, x.Key, StringComparison.InvariantCultureIgnoreCase));
            return prop == null ? def : prop.GetCoordinate(def);
        }
    }
}
