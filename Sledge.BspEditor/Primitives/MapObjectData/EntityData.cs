using System.Collections.Generic;

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

        public void Set(string key, string value)
        {
            Properties[key] = value;
        }

        public void Unset(string key)
        {
            if (Properties.ContainsKey(key)) Properties.Remove(key);
        }
    }
}