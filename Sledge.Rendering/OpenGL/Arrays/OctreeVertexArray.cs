using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Rendering.DataStructures;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.OpenGL.Shaders;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.OpenGL.Arrays
{
    public class OctreeVertexArray : IDisposable
    {
        private int _changeNum;
        private const int MaxChanges = 2000;

        private readonly Scene _scene;
        public Octree<RenderableObject> Octree { get; private set; }
        public List<PartitionedVertexArray> Partitions { get; private set; }
        public RenderableVertexArray Spare { get; private set; }

        public OctreeVertexArray(Scene scene, float worldSize = 32768, int limit = 100)
        {
            _scene = scene;
            _changeNum = 0;

            Octree = new Octree<RenderableObject>(worldSize, limit);
            Partitions = new List<PartitionedVertexArray>();
            Spare = null;
            Rebuild();
        }

        public void ApplyChanges()
        {

            if (!_scene.HasChanges) return;

            SceneChangeSet changes;
            lock (_scene)
            {
                changes = _scene.CurrentChangeSet;
                _scene.ClearChanges();
            }

            var addRenderable = changes.Added.OfType<RenderableObject>().ToList();
            var removeRenderable = changes.Removed.OfType<RenderableObject>().ToList();
            var updateRenderable = changes.Updated.OfType<RenderableObject>().ToList();
            var replaceRenderable = changes.Replaced.OfType<RenderableObject>().ToList();

            Octree.Remove(removeRenderable.Union(replaceRenderable));
            Octree.Add(addRenderable.Union(replaceRenderable));
            _changeNum += addRenderable.Count + removeRenderable.Count + (replaceRenderable.Count * 2);

            if (_changeNum > MaxChanges)
            {
                _changeNum = 0;
                Rebuild();
            }
            else
            {
                var added = addRenderable.Union(replaceRenderable).Except(removeRenderable).ToList();
                if (Spare == null) Spare = new RenderableVertexArray(added); 
                else Spare.Update(added.Union(Spare.Items.Except(removeRenderable)).ToList());

                foreach (var part in Partitions)
                {
                    part.UpdatePartial(updateRenderable);
                    part.DeletePartial(removeRenderable);
                }
            }
        }

        public void Rebuild()
        {
            Clear();

            var partitions = Octree.Partition2(MaxChanges);
            foreach (var partition in partitions)
            {
                var box = new Box(partition.Select(x => x.BoundingBox));
                var items = partition.SelectMany(x => x).ToList();
                var array = new PartitionedVertexArray(box, items);
                Partitions.Add(array);
            }
        }

        // todo clipping
        public void Render(IRenderer renderer, Passthrough shader, IViewport viewport)
        {
            foreach (var array in Partitions)
            {
                array.Render(renderer, shader, viewport);
            }
            if (Spare != null)
            {
                Spare.Render(renderer, shader, viewport);
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