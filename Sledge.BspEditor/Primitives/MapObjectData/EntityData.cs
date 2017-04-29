using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    public class EntityData : IMapObjectData
    {
        public string Name { get; set; }
        public int Flags { get; set; }
        public Dictionary<string, string> Properties { get; set; }

        public EntityData()
        {
            Properties = new Dictionary<string, string>();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Properties", Properties);
        }

        public void Set(string key, string value)
        {
            Properties[key] = value;
        }

        public void Unset(string key)
        {
            if (Properties.ContainsKey(key)) Properties.Remove(key);
        }

        public IMapObjectData Clone()
        {
            var ed = new EntityData();
            ed.Name = Name;
            ed.Flags = Flags;
            ed.Properties = new Dictionary<string, string>(Properties);
            return ed;
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject("EntityData");
            foreach (var p in Properties)
            {
                so.Set(p.Key, p.Value);
            }
            so.Set("Name", Name);
            so.Set("Flags", Flags);
            return so;
        }
    }
}