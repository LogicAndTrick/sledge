using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.DataStructures.MapObjects
{
    public class EntityData
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
    }
}
