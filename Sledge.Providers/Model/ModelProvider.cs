using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sledge.FileSystem;

namespace Sledge.Providers.Model
{
    public abstract class ModelProvider
    {
        private static readonly List<ModelProvider> RegisteredProviders;

        static ModelProvider()
        {
            RegisteredProviders = new List<ModelProvider>();
        }

        public static void Register(ModelProvider provider)
        {
            RegisteredProviders.Add(provider);
        }

        public static void Deregister(ModelProvider provider)
        {
            RegisteredProviders.Remove(provider);
        }

        public static void DeregisterAll()
        {
            RegisteredProviders.Clear();
        }

        public static DataStructures.Models.Model LoadModel(IFile file)
        {
            if (!file.Exists) throw new ProviderException("The supplied file doesn't exist.");
            var provider = RegisteredProviders.FirstOrDefault(p => p.IsValidForFile(file));
            if (provider != null)
            {
                return provider.LoadFromFile(file);
            }
            throw new ProviderNotFoundException("No map provider was found for this file.");
        }

        protected abstract bool IsValidForFile(IFile file);
        protected abstract DataStructures.Models.Model LoadFromFile(IFile file);
    }
}
