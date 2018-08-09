using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.OpenGL.Arrays
{
    public class ChunkingVertexArray : IDisposable
    {
        private readonly OpenGLRenderer _renderer;
        private readonly Scene _scene;
        private readonly int _minItemsPerChunk;
        private readonly int _itemsPerChunk;

        private List<RenderableVertexArray> Partitions { get; set; }
        private RenderableVertexArray Spare { get; set; }
        public List<Element> Elements { get; private set; }

        public ChunkingVertexArray(OpenGLRenderer renderer, Scene scene, int itemsPerChunk = 1000)
        {
            _scene = scene;
            _renderer = renderer;
            _itemsPerChunk = itemsPerChunk;
            _minItemsPerChunk = itemsPerChunk / 50;

            Partitions = new List<RenderableVertexArray>();
            Spare = null;
            Elements = new List<Element>();
        }

        public void ApplyChanges()
        {
            SceneChangeSet changes;
            lock (_scene)
            {
                if (!_scene.HasChanges) return;
                changes = _scene.ClearChanges();
            }

            var addRenderable = changes.Added.OfType<RenderableObject>().ToList();
            var removeRenderable = changes.Removed.OfType<RenderableObject>().ToList();
            var updateRenderable = changes.Updated.OfType<RenderableObject>().ToList();
            var replaceRenderable = changes.Replaced.OfType<RenderableObject>().ToList();

            var added = addRenderable.Union(replaceRenderable).Except(removeRenderable).ToList();
            var removed = removeRenderable.Union(replaceRenderable).ToList();

            // Load textures and models
            foreach (var mat in added.Select(x => x.Material).Where(x => x != null))
            {
                if (!_renderer.Materials.Exists(mat.UniqueIdentifier)) _renderer.Materials.Add(mat);
                if (!_renderer.Textures.Exists(mat.CurrentFrame))
                {
                    _renderer.Textures.Create(mat.CurrentFrame);
                    RequestTexture(mat.CurrentFrame);
                }
            }

            foreach (var model in added.OfType<Model>())
            {
                if (!_renderer.Models.Exists(model.Name))
                {
                    _renderer.Models.Add(model.Name);
                    RequestModel(model.Name);
                }
            }

            // Update the existing partitions
            foreach (var part in Partitions.ToList())
            {
                UpdateItemsInPartition(part, updateRenderable, removed);

                // If the partition is too small, remove it and put the items in a new partition
                if (part.Items.Count < _minItemsPerChunk)
                {
                    part.Dispose();
                    Partitions.Remove(part);
                    added.AddRange(part.Items);
                }
            }

            // Create new partitions
            var numInSpare = Spare?.Items.Count ?? 0;
            var numTotal = added.Count + numInSpare;
            if (numTotal > _itemsPerChunk)
            {
                if (Spare != null)
                {
                    added.AddRange(Spare.Items);
                    Spare.Dispose();
                    Spare = null;
                }

                while (added.Count > _itemsPerChunk)
                {
                    var chunk = added.GetRange(0, _itemsPerChunk);
                    added.RemoveRange(0, _itemsPerChunk);
                    var part = CreatePartition(chunk);
                    Partitions.Add(part);
                }

                if (added.Count > 0) Spare = CreatePartition(added);
            }
            else
            {
                if (Spare == null) Spare = CreatePartition(added);
                else RebuildItemsInPartition(Spare, added, removeRenderable);
            }

            // Update element list
            var addElement = changes.Added.OfType<Element>();
            var removeElement = changes.Removed.OfType<Element>();
            Elements = Elements.Except(removeElement).Union(addElement).ToList();
        }

        public void Rebuild()
        {
            var items = Partitions.SelectMany(x => x.Items).ToList();
            if (Spare != null) items.AddRange(Spare.Items);

            Clear();

            while (items.Count > _itemsPerChunk)
            {
                var chunk = items.GetRange(0, _itemsPerChunk);
                items.RemoveRange(0, _itemsPerChunk);
                var part = CreatePartition(chunk);
                Partitions.Add(part);
            }

            if (items.Count > 0)
            {
                Spare = CreatePartition(items);
            }
        }

        public void Clear()
        {
            foreach (var va in Partitions) va.Dispose();
            Partitions.Clear();

            Spare?.Dispose();
            Spare = null;
        }

        public void Dispose()
        {
            Clear();
        }

        protected void RequestModel(string name)
        {
            _renderer.RequestModel(name);
        }

        protected void RequestTexture(string name)
        {
            _renderer.RequestTexture(name);
        }

        protected RenderableVertexArray CreatePartition(List<RenderableObject> items)
        {
            return new RenderableVertexArray(items);
        }

        protected void RebuildItemsInPartition(RenderableVertexArray partition, List<RenderableObject> addedOrUpdatedItems, List<RenderableObject> deletedItems)
        {
            partition.Update(addedOrUpdatedItems.Union(partition.Items.Except(deletedItems)).ToList());
        }

        protected void UpdateItemsInPartition(RenderableVertexArray partition, List<RenderableObject> addedOrUpdatedItems, List<RenderableObject> deletedItems)
        {
            partition.UpdatePartial(addedOrUpdatedItems);
            partition.DeletePartial(deletedItems);
        }

        public void Render(IRenderer renderer, IViewport viewport)
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

        private IEnumerable<RenderableVertexArray> GetVisiblePartitions(IViewport viewport)
        {
            foreach (var part in Partitions) yield return part;
            if (Spare != null) yield return Spare;
        }
    }
}