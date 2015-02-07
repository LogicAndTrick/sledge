using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Interfaces;

namespace Sledge.Rendering.DataStructures
{
    public class Octree<T> : OctreeNode<T> where T : IBounded
    {
        public Octree(decimal worldSize = 32768, int limit = 100)
            : base(null, null, new Box(-Coordinate.One * worldSize, Coordinate.One * worldSize), limit)
        {
            Root = this;
        }

        private class NodeGroup
        {
            public int Count { get; set; }
            public List<OctreeNode<T>> Nodes { get; set; }

            public NodeGroup()
            {
                Count = 0;
                Nodes = new List<OctreeNode<T>>();
            }
        }

        public IEnumerable<List<OctreeNode<T>>> Partition(int maxPartitionSize = 1000)
        {
            var groups = new List<NodeGroup>();
            var queue = new Queue<OctreeNode<T>>();
            queue.Enqueue(this);
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node.Count <= maxPartitionSize)
                {
                    var group = groups.FirstOrDefault(x => x.Count + node.Count <= maxPartitionSize);
                    if (group == null) groups.Add(group = new NodeGroup());
                    group.Count += node.Count;
                    group.Nodes.Add(node);
                }
                else
                {
                    var children = node.GetChildNodes();
                    if (children.Count == 0) groups.Add(new NodeGroup {Count = node.Count, Nodes = new List<OctreeNode<T>> {node}});
                    else children.ForEach(queue.Enqueue);
                }
            }
            return groups.Select(x => x.Nodes);
        }
    }
}
