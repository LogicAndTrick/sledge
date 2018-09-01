using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    [Serializable]
    public class Path : IMapObjectData
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public PathDirection Direction { get; set; }
        public List<PathNode> Nodes { get; private set; }

        public Path()
        {
            Nodes = new List<PathNode>();
        }

        public Path(SerialisedObject obj)
        {
            Name = obj.Get("Name", "");
            Type = obj.Get("Type", "");
            Direction = obj.Get("Direction", PathDirection.OneWay);

            var children = obj.Children.Where(x => x.Name == "Node");
            Nodes = children.Select(x => new PathNode(x)).ToList();
        }

        [Export(typeof(IMapElementFormatter))]
        public class EntityFormatter : StandardMapElementFormatter<Path> { }

        protected Path(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            Type = info.GetString("Type");
            Direction = (PathDirection) info.GetValue("Direction", typeof(PathDirection));
            Nodes = (List<PathNode>) info.GetValue("Nodes", typeof(List<PathNode>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Type", Type);
            info.AddValue("Direction", Direction);
            info.AddValue("Nodes", Nodes);
        }

        public IMapElement Clone()
        {
            return new Path
            {
                Name = Name,
                Type = Type,
                Direction = Direction,
                Nodes = Nodes.Select(x => x.Clone()).ToList()
            };
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject("Path");
            so.Set("Name", Name);
            so.Set("Type", Type);
            so.Set("Direction", Direction);
            so.Children.AddRange(Nodes.Select(x => x.ToSerialisedObject()));
            return so;
        }

        public enum PathDirection
        {
            OneWay,
            Circular,
            PingPong
        }

        [Serializable]
        public class PathNode : ISerializable
        {
            public Vector3 Position { get; set; }
            public int ID { get; set; }
            public string Name { get; set; }
            public Dictionary<string, string> Properties { get; private set; }

            public PathNode()
            {
                Properties = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            }

            public PathNode(SerialisedObject obj)
            {
                ID = obj.Get("ID", 0);
                Name = obj.Get("Name", "");
                Position = obj.Get<Vector3>("Position");
                
                Properties = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                foreach (var prop in obj.Properties)
                {
                    if (prop.Key == "ID" || prop.Key == "Name" || prop.Key == "Position") continue;
                    Properties[prop.Key] = prop.Value;
                }
            }

            protected PathNode(SerializationInfo info, StreamingContext context)
            {
                ID = info.GetInt32("ID");
                Name = info.GetString("Name");
                Position = (Vector3) info.GetValue("Position", typeof(Vector3));
                Properties = (Dictionary<string, string>) info.GetValue("Properties", typeof(Dictionary<string, string>));
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("ID", ID);
                info.AddValue("Name", Name);
                info.AddValue("Position", Position);
                info.AddValue("Properties", Properties);
            }

            public SerialisedObject ToSerialisedObject()
            {
                var so = new SerialisedObject("PathNode");

                foreach (var kv in Properties) so.Set(kv.Key, kv.Value);
                
                so.Set("ID", ID);
                so.Set("Name", Name);
                so.Set("Position", Position);

                return so;
            }

            public PathNode Clone()
            {
                var node = new PathNode
                {
                    Position = Position,
                    ID = ID,
                    Name = Name
                };
                foreach (var kv in Properties) node.Properties[kv.Key] = kv.Value;
                return node;
            }
        }
    }
}