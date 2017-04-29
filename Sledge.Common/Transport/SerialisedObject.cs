using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sledge.Common.Transport
{
    [Serializable]
    public class SerialisedObject : ISerializable
    {
        public string Name { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public List<SerialisedObject> Children { get; set; }

        public SerialisedObject(string name)
        {
            Name = name;
            Properties = new Dictionary<string, string>();
            Children = new List<SerialisedObject>();
        }

        protected SerialisedObject(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            Properties = (Dictionary<string, string>) info.GetValue("Properties", typeof(Dictionary<string, string>));
            Children = (List<SerialisedObject>) info.GetValue("Children", typeof(List<SerialisedObject>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Properties", Properties);
            info.AddValue("Children", Children);
        }
    }
}
