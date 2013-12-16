using System;

namespace Sledge.Providers.Model
{
    public class ModelReference : IDisposable
    {
        public string Path { get; private set; }
        public DataStructures.Models.Model Model { get; private set; }

        public ModelReference(string path, DataStructures.Models.Model model)
        {
            Path = path;
            Model = model;
        }

        public void Dispose()
        {
            ModelProvider.DeleteModelReference(this);
        }
    }
}