using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Rendering.DataStructures;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.OpenGL.Arrays
{
    public abstract class OctreeRenderableBase<T> : IDisposable where T : IDisposable, IBounded
    {
        private int _changeNum;
        private const int MaxChanges = 2000;

        private readonly IRenderer _renderer;
        private readonly Scene _scene;
        private Octree<RenderableObject> Octree { get; set; }

        private List<T> Partitions { get; set; }
        private T Spare { get; set; }
        public List<Element> Elements { get; private set; }

        protected OctreeRenderableBase(IRenderer renderer, Scene scene, float worldSize = 32768, int limit = 1000)
        {
            _renderer = renderer;
            _scene = scene;
            _changeNum = 0;

            Octree = new Octree<RenderableObject>(worldSize, limit);
            Partitions = new List<T>();
            Spare = default(T);
            Elements = new List<Element>();
            Rebuild();
        }

        protected abstract void RequestModel(string name);
        protected abstract void RequestTexture(string name);
        protected abstract T CreatePartition(Box box, List<RenderableObject> items);
        protected abstract void RebuildItemsInPartition(T partition, List<RenderableObject> addedOrUpdatedItems, List<RenderableObject> deletedItems);
        protected abstract void UpdateItemsInPartition(T partition, List<RenderableObject> addedOrUpdatedItems, List<RenderableObject> deletedItems);
        public abstract void Render(IRenderer renderer, IViewport viewport);

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

            Octree.Remove(removeRenderable.Union(replaceRenderable));
            Octree.Add(added);
            _changeNum += addRenderable.Count + removeRenderable.Count + (replaceRenderable.Count * 2);

            if (addRenderable.Count + removeRenderable.Count + replaceRenderable.Count + updateRenderable.Count > 0)
            {
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
                if (_changeNum > MaxChanges)
                {
                    _changeNum = 0;
                    Rebuild();
                }
                else
                {
                    if (Spare == null) Spare = CreatePartition(null, added);
                    else RebuildItemsInPartition(Spare, added, removeRenderable);

                    foreach (var part in Partitions)
                    {
                        UpdateItemsInPartition(part, updateRenderable, removeRenderable);
                    }
                }
            }

            // Update element list
            var addElement = changes.Added.OfType<Element>();
            var removeElement = changes.Removed.OfType<Element>();
            Elements = Elements.Except(removeElement).Union(addElement).ToList();
        }

        public void Rebuild()
        {
            Clear();

            var partitions = Octree.Partition(MaxChanges);
            foreach (var partition in partitions)
            {
                var box = new Box(partition.Select(x => x.BoundingBox));
                var items = partition.SelectMany(x => x).ToList();
                var part = CreatePartition(box, items);
                Partitions.Add(part);
            }
        }

        protected bool CanBeClipped(Box box, List<Plane> clippingPlanes)
        {
            var center = box.Center;
            if (!clippingPlanes.Any(x => x.OnPlane(center) < 0)) return false;

            var points = box.GetBoxPoints().ToList();
            return clippingPlanes.Any(x => points.All(p => x.OnPlane(p) < 0));
        }

        protected IEnumerable<T> GetVisiblePartitions(IViewport viewport)
        {
            var clip = viewport.Camera.GetClippingPlanes(viewport.Control.Width, viewport.Control.Height).ToList();

            foreach (var partition in Partitions)
            {
                if (CanBeClipped(partition.BoundingBox, clip)) continue;
                yield return partition;
            }
            if (Spare != null)
            {
                yield return Spare;
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
            Spare = default(T);
        }

        public void Dispose()
        {
            Clear();
        }
    }
}