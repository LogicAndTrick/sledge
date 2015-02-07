using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.DataStructures;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.OpenGL.Arrays
{
    public class OctreeVertexArray : IDisposable
    {
        public Octree<RenderableObject> Octree { get; private set; }
        public List<PartitionedVertexArray> Partitions { get; private set; }
        public RenderableVertexArray Spare { get; private set; }

        public OctreeVertexArray(Octree<RenderableObject> octree)
        {
            Octree = octree;
            Partitions = new List<PartitionedVertexArray>();
            Spare = null;
            Rebuild();
        }

        public void Rebuild()
        {
            Clear();

            var partitions = Octree.Partition2(10000);
            foreach (var partition in partitions)
            {
                var box = new Box(partition.Select(x => x.BoundingBox));
                var items = partition.SelectMany(x => x).ToList();
                var array = new PartitionedVertexArray(box, items);
                Partitions.Add(array);
            }
        }

        // todo clipping
        public void RenderTextured(IRenderer renderer)
        {
            foreach (var array in Partitions)
            {
                array.RenderTextured(renderer);
            }
            if (Spare != null)
            {
                Spare.RenderTextured(renderer);
            }
        }

        public void Clear()
        {
            foreach (var va in Partitions)
            {
                va.Dispose();
            }
            Partitions.Clear();

            if (Spare != null)
            {
                Spare.Dispose();
            }
            Spare = null;
        }

        public void Dispose()
        {
            Clear();
        }
    }
}