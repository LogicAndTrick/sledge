using System;
using System.Collections.Generic;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Materials;

namespace Sledge.Rendering.OpenGL
{
    public class MaterialStorage : IMaterialStorage
    {
        private readonly IRenderer _renderer;
        private readonly Dictionary<string, Material> _materials;
        private readonly Dictionary<string, Material> _updatableMaterials;

        public MaterialStorage(IRenderer renderer)
        {
            _renderer = renderer;
            _materials = new Dictionary<string, Material>();
            _updatableMaterials = new Dictionary<string, Material>();
        }

        public void Initialise()
        {
            foreach (var im in Internal.InternalMaterials.GetInternalMaterials())
            {
                Add(im);
            }
        }

        public void Update(Frame frame)
        {
            foreach (var mat in _updatableMaterials.Values)
            {
                mat.Update(frame);
            }
        }

        public void Add(Material material)
        {
            if (_materials.ContainsKey(material.UniqueIdentifier)) return;
            _materials.Add(material.UniqueIdentifier, material);
            if (material.IsUpdatable()) _updatableMaterials.Add(material.UniqueIdentifier, material);
        }

        public void Bind(string uniqueIdentifier)
        {
            if (!Exists(uniqueIdentifier)) throw new Exception("Material " + uniqueIdentifier + " doesn't exist");
            var mat = Get(uniqueIdentifier);
            _renderer.Textures.Bind(0, mat.CurrentFrame);
        }

        public bool Exists(string uniqueIdentifier)
        {
            return _materials.ContainsKey(uniqueIdentifier);
        }

        public Material Get(string uniqueIdentifier)
        {
            return _materials.ContainsKey(uniqueIdentifier) ? _materials[uniqueIdentifier] : null;
        }

        public void Remove(string uniqueIdentifier)
        {
            _materials.Remove(uniqueIdentifier);
            _updatableMaterials.Remove(uniqueIdentifier);
        }

        public void Clear()
        {
            _materials.Clear();
            _updatableMaterials.Clear();
        }
    }
}