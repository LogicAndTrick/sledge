using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.DataStructures.Geometric;

namespace Sledge.Rendering.DataStructures
{
    public class Octree<T> : OctreeNode<T> where T : IOrigin
    {
        public Octree(decimal worldSize = 32768, int limit = 100)
            : base(null, null, new Box(-Coordinate.One * worldSize, Coordinate.One * worldSize), limit)
        {
            Root = this;
        }

        private class NodeGroup
        {
            public int Count { get; set; }
            public List<T> Items { get; set; }

            public NodeGroup()
            {
                Count = 0;
                Items = new List<T>();
            }
        }

        public IEnumerable<List<T>> GetNodeGroups(int maxGroupSize = 1000)
        {
            var groups = new List<NodeGroup>();
            var queue = new Queue<OctreeNode<T>>();
            queue.Enqueue(this);
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node.Count <= maxGroupSize)
                {
                    var group = groups.FirstOrDefault(x => x.Count + node.Count <= maxGroupSize);
                    if (group == null) groups.Add(group = new NodeGroup());
                    group.Count += node.Count;
                    group.Items.AddRange(node);
                }
                else
                {
                    var children = node.GetChildNodes();
                    if (children.Count == 0) groups.Add(new NodeGroup {Count = node.Count, Items = node.ToList()});
                    else children.ForEach(x => queue.Enqueue(x));
                }
            }
            return groups.Select(x => x.Items);
        }
    }
}
