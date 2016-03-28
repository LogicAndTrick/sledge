using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using Sledge.Rendering.DataStructures.Models;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Materials;
using Sledge.Rendering.OpenGL.Arrays;

namespace Sledge.Rendering.OpenGL
{
    public class ModelStorage : IModelStorage
    {
        private static readonly Model Blank;

        static ModelStorage()
        {
            Blank = new Model(new List<Mesh>
            {
                new Mesh(Material.Flat(Color.White), new List<MeshVertex>
                {
                    new MeshVertex(Vector3.Zero, Vector3.UnitZ, 0, 0, new Dictionary<int, float>()),
                    new MeshVertex(Vector3.Zero, Vector3.UnitZ, 0, 0, new Dictionary<int, float>()),
                    new MeshVertex(Vector3.Zero, Vector3.UnitZ, 0, 0, new Dictionary<int, float>()),
                })
            });
        }

        private readonly Dictionary<string, Model> _models;
        private readonly Dictionary<string, ModelVertexArray> _arrays;

        public ModelStorage()
        {
            _models = new Dictionary<string, Model>();
            _arrays = new Dictionary<string, ModelVertexArray>();
        }

        public void Update(Frame frame)
        {
            foreach (var model in _models.Values)
            {
                model.Update(frame);
            }
        }

        public void Add(string name)
        {
            _models.Add(name, Blank);
        }

        public void Add(string name, Model model)
        {
            if (_arrays.ContainsKey(name)) _arrays.Remove(name);
            _models[name] = model;
        }

        public bool Exists(string name)
        {
            return _models.ContainsKey(name);
        }

        public Model Get(string name)
        {
            return _models.ContainsKey(name) ? _models[name] : null;
        }

        public ModelVertexArray GetArray(string name)
        {
            if (!_models.ContainsKey(name)) return null;
            if (!_arrays.ContainsKey(name)) _arrays.Add(name, new ModelVertexArray(new[] {_models[name]}));
            return _arrays[name];
        }

        public void Remove(string name)
        {
            _models.Remove(name);
            if (_arrays.ContainsKey(name))
            {
                _arrays[name].Dispose();
                _arrays.Remove(name);
            }
        }

        public void Clear()
        {
            _models.Clear();
            _arrays.Clear();
        }
    }
}