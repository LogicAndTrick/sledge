using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Rendering.DataStructures;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.OpenGL.Arrays;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.OpenGL.Lists
{
    public class OctreeDisplayList : OctreeRenderableBase<PartitionedDisplayList>
    {
        private readonly DisplayListRenderer _renderer;

        public OctreeDisplayList(DisplayListRenderer renderer, Scene scene, float worldSize = 32768, int limit = 1000) : base(renderer, scene, worldSize, limit)
        {
            _renderer = renderer;
        }

        protected override void RequestModel(string name)
        {
            _renderer.RequestModel(name);
        }

        protected override void RequestTexture(string name)
        {
            _renderer.RequestTexture(name);
        }

        protected override PartitionedDisplayList CreatePartition(Box box, List<RenderableObject> items)
        {
            return new PartitionedDisplayList(_renderer, box, items);
        }

        protected override void RebuildItemsInPartition(PartitionedDisplayList partition, List<RenderableObject> addedOrUpdatedItems, List<RenderableObject> deletedItems)
        {
            partition.Update(_renderer, addedOrUpdatedItems.Union(partition.Items.Except(deletedItems)).ToList());
        }

        protected override void UpdateItemsInPartition(PartitionedDisplayList partition, List<RenderableObject> addedOrUpdatedItems, List<RenderableObject> deletedItems)
        {
            RebuildItemsInPartition(partition, addedOrUpdatedItems, deletedItems);
        }

        public override void Render(IRenderer renderer, IViewport viewport)
        {
            if (renderer != _renderer) throw new Exception("The passed renderer is not the owner of this array.");

            var partitions = GetVisiblePartitions(viewport).ToList();
            foreach (var list in partitions)
            {
                list.Render(_renderer, viewport);
            }
            foreach (var list in partitions)
            {
                list.RenderTransparent(_renderer, viewport);
            }
        }
    }
}