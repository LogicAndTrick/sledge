using System.Collections.Generic;
using Sledge.Rendering.DataStructures.Models;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.OpenGL.Arrays;

namespace Sledge.Rendering.OpenGL
{
    public class ModelStorage : IModelStorage
    {
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

        public void Add(string name, Model model)
        {
            _models.Add(name, model);
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