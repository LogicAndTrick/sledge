using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.DataStructures.MapObjects
{
    public class Path
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public PathDirection Direction { get; set; }
        public List<PathNode> Nodes { get; private set; }

        public Path()
        {
            Nodes = new List<PathNode>();
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
