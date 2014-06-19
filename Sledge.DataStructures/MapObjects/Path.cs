using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sledge.DataStructures.MapObjects
{
    [Serializable]
    public class Path : ISerializable
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public PathDirection Direction { get; set; }
        public List<PathNode> Nodes { get; private set; }

        public Path()
        {
            Nodes = new List<PathNode>();
        }

        protected Path(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            Type = info.GetString("Type");
            Direction = (PathDirection) info.GetValue("Direction", typeof (PathDirection));
            Nodes = ((PathNode[]) info.GetValue("Nodes", typeof (PathNode[]))).ToList();
            Nodes.ForEach(x => x.Parent = this);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Type", Type);
            info.AddValue("Direction", Direction);
            info.AddValue("Nodes", Nodes.ToArray());
        }

        public Path Clone()
        {
            var p = new Path
                        {
                            Name = Name,
                            Type = Type,
                            Direction = Direction
                        };
            foreach (var n in Nodes.Select(node => node.Clone()))
            {
                n.Parent = p;
                p.Nodes.Add(n);
            }
            return p;
        }
    }
}
