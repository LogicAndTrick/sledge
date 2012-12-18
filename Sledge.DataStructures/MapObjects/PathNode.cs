using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.MapObjects
{
    public class PathNode
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
