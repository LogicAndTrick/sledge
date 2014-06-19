using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.MapObjects
{
    [Serializable]
    public class PathNode : ISerializable
    {
        public Coordinate Position { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public List<Property> Properties { get; private set; }
        public Path Parent { get; set; }

        public PathNode()
        {
            Properties = new List<Property>();
        }

        protected PathNode(SerializationInfo info, StreamingContext context)
        {
            Position = (Coordinate) info.GetValue("Position", typeof (Coordinate));
            ID = info.GetInt32("ID");
            Name = info.GetString("Name");
            Properties = ((Property[]) info.GetValue("Properties", typeof (Property[]))).ToList();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Position", Position);
            info.AddValue("ID", ID);
            info.AddValue("Name", Name);
            info.AddValue("Properties", Properties.ToArray());
        }

        public PathNode Clone()
        {
            var node = new PathNode
                           {
                               Position = Position.Clone(),
                               ID = ID,
                               Name = Name,
                               Parent = Parent
                           };
            node.Properties.AddRange(Properties.Select(x => x.Clone()));
            return node;
        }
    }
}
