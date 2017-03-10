using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Rendering.DataStructures;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.OpenGL.Arrays
{
    public class OctreeVertexArray : OctreeRenderableBase<PartitionedVertexArray>
    {
        private readonly OpenGLRenderer _renderer;

        public OctreeVertexArray(OpenGLRenderer renderer, Scene scene, float worldSize = 32768, int limit = 1000) : base(renderer, scene, worldSize, limit)
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

        protected override PartitionedVertexArray CreatePartition(Box box, List<RenderableObject> items)
        {
            return new PartitionedVertexArray(box, items);
        }

        protected override void RebuildItemsInPartition(PartitionedVertexArray partition, List<RenderableObject> addedOrUpdatedItems, List<RenderableObject> deletedItems)
        {
            partition.Update(addedOrUpdatedItems.Union(partition.Items.Except(deletedItems)).ToList());
        }

        protected override void UpdateItemsInPartition(PartitionedVertexArray partition, List<RenderableObject> addedOrUpdatedItems, List<RenderableObject> deletedItems)
        {
            partition.UpdatePartial(addedOrUpdatedItems);
            partition.DeletePartial(deletedItems);
        }

        public override void Render(IRenderer renderer, IViewport viewport)
        {
            if (renderer != _renderer) throw new Exception("The passed renderer is not the owner of this array.");

            var list = GetVisiblePartitions(viewport).ToList();
            foreach (var array in list)
            {
                array.Render(_renderer, viewport);
            }
            foreach (var array in list)
            {
                array.RenderTransparent(_renderer, viewport);
            }
        }
    }
}