using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Rendering.DataStructures;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.OpenGL.Shaders;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.OpenGL.Arrays
{
    public class OctreeVertexArray : IDisposable
    {
        private int _changeNum;
        private const int MaxChanges = 2000;

        private readonly OpenGLRenderer _renderer;
        private readonly Scene _scene;
        public Octree<RenderableObject> Octree { get; private set; }
        public List<PartitionedVertexArray> Partitions { get; private set; }
        public RenderableVertexArray Spare { get; private set; }
        public List<Element> Elements { get; private set; }

        public OctreeVertexArray(OpenGLRenderer renderer, Scene scene, float worldSize = 32768, int limit = 1000)
        {
            _renderer = renderer;
            _scene = scene;
            _changeNum = 0;

            Octree = new Octree<RenderableObject>(worldSize, limit);
            Partitions = new List<PartitionedVertexArray>();
            Spare = null;
            Elements = new List<Element>();
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
                        _renderer.RequestTexture(mat.CurrentFrame);
                    }
                }
                if (_changeNum > MaxChanges)
                {
                    _changeNum = 0;
                    Rebuild();
                }
                else
                {
                    if (Spare == null) Spare = new RenderableVertexArray(added);
                    else Spare.Update(added.Union(Spare.Items.Except(removeRenderable)).ToList());

                    foreach (var part in Partitions)
                    {
                        part.UpdatePartial(updateRenderable);
                        part.DeletePartial(removeRenderable);
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
                var array = new PartitionedVertexArray(box, items);
                Partitions.Add(array);
            }
        }

        // todo clipping
        public void Render(IRenderer renderer, Passthrough shader, ModelShader modelShader, IViewport viewport)
        {
            foreach (var array in Partitions)
            {
                array.Render(renderer, shader, modelShader, viewport);
            }
            if (Spare != null)
            {
                Spare.Render(renderer, shader, modelShader, viewport);
            }
            foreach (var array in Partitions)
            {
                array.RenderTransparent(renderer, shader, viewport);
            }
            if (Spare != null)
            {
                Spare.RenderTransparent(renderer, shader, viewport);
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