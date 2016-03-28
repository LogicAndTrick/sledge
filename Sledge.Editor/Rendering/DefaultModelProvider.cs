using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.Editor.Documents;
using Sledge.FileSystem;
using Sledge.Providers.Model;
using Sledge.Rendering.DataStructures.Models;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Materials;

namespace Sledge.Editor.Rendering
{
    public class DefaultModelProvider : IModelProvider
    {
        public Document Document { get; private set; }

        public DefaultModelProvider(Document document)
        {
            Document = document;
        }

        private IFile GetFile(string name)
        {
            return Document.Environment.Root.TraversePath(name);
        }

        public bool Exists(string name)
        {
            var file = GetFile(name);
            return Document.ModelCollection.IsModelLoaded(name) || (file != null && Document.ModelCollection.CanLoad(file));
        }

        public ModelDetails Fetch(string name)
        {
            return Fetch(new[] {name}).FirstOrDefault();
        }

        public IEnumerable<ModelDetails> Fetch(IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                var model = Document.ModelCollection.GetModel(GetFile(name));
                if (model != null)
                {
                    yield return ConvertModel(model);
                }
            }
        }

        private readonly ConcurrentQueue<ModelDetails> _modelQueue = new ConcurrentQueue<ModelDetails>();

        public void Request(IEnumerable<string> names)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var name in names)
                {
                    var file = GetFile(name);
                    if (file != null)
                    {
                        var model = Document.ModelCollection.GetModel(file);
                        if (model != null)
                        {
                            _modelQueue.Enqueue(ConvertModel(model));
                        }
                    }
                }
            });
        }

        public IEnumerable<ModelDetails> PopRequestedModels(int count)
        {
            for (var i = 0; i < count; i++)
            {
                ModelDetails md;
                if (_modelQueue.TryDequeue(out md)) yield return md;
                else break;
            }
        }

        private ModelDetails ConvertModel(ModelReference model)
        {
            const string textureFormat = "Model::{0}::{1}";

            var meshes = model.Model.GetActiveMeshes().Select(x =>
            {
                var verts = x.Vertices.Select(v =>
                {
                    var weight = v.BoneWeightings.ToDictionary(w => w.Bone.BoneIndex, w => w.Weight);
                    return new MeshVertex(v.Location, v.Normal, v.TextureU, v.TextureV, weight);
                });
                var mat = Material.Texture(String.Format(textureFormat, model.Path, x.SkinRef), false);
                return new Mesh(mat, verts.ToList());
            });

            var ai = 0;
            var anim = new Animation(15, model.Model.Animations[ai].Frames.Select((z, i) => new AnimationFrame(model.Model.GetTransforms(ai, i))).ToList());
            
            var modelObj = new Model(meshes.ToList()) {Animation = anim};
            var textures = model.Model.Textures.Select(x => new TextureDetails(String.Format(textureFormat, model.Path, x.Index), x.Image, x.Width, x.Height, TextureFlags.None)).ToList();

            return new ModelDetails(model.Path, modelObj, textures);
        }
    }
}