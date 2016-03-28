using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.FileSystem;
using Sledge.Providers.Model;

namespace Sledge.Editor.Documents
{
    public class ModelCollection : IDisposable
    {
        private readonly Dictionary<string, ModelReference> _loadedModels;

        public ModelCollection()
        {
            _loadedModels = new Dictionary<string, ModelReference>();
        }

        public bool CanLoad(IFile model)
        {
            return ModelProvider.CanLoad(model);
        }

        public bool IsModelLoaded(string name)
        {
            return _loadedModels.ContainsKey(name);
        }

        public ModelReference GetLoadedModel(string name)
        {
            return _loadedModels.ContainsKey(name) ? _loadedModels[name] : null;
        }

        public ModelReference GetModel(IFile model)
        {
            if (_loadedModels.ContainsKey(model.FullPathName))
            {
                return _loadedModels[model.FullPathName];
            }

            if (!CanLoad(model))
            {
                return null;
            }
            try
            {
                _loadedModels[model.FullPathName] = ModelProvider.CreateModelReference(model);
            }
            catch
            {
                _loadedModels[model.FullPathName] = null;
            }
            return _loadedModels[model.FullPathName];
        }

        public void Dispose()
        {
            foreach (var reference in _loadedModels.Values)
            {
                ModelProvider.DeleteModelReference(reference);
            }
        }
    }
}
