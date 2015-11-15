using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sledge.Common;
using Sledge.FileSystem;
using Sledge.Graphics.Helpers;

namespace Sledge.Providers.Model
{
    public abstract class ModelProvider
    {
        private static readonly List<ModelProvider> RegisteredProviders;
        private static readonly List<ModelReference> References;
        private static readonly Dictionary<string, DataStructures.Models.Model> Models;

        static ModelProvider()
        {
            RegisteredProviders = new List<ModelProvider>();
            References = new List<ModelReference>();
            Models = new Dictionary<string, DataStructures.Models.Model>();
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

        public static ModelReference CreateModelReference(IFile file, DataStructures.GameData.Palette pal)
        {
            var model = LoadModel(file, pal);
            var reference = new ModelReference(file.FullPathName, model);
            References.Add(reference);
            return reference;
        }

        public static void DeleteModelReference(ModelReference reference)
        {
            References.Remove(reference);
            if (References.All(x => x.Model != reference.Model))
            {
                UnloadModel(reference.Model);
            }
        }

        public static bool CanLoad(IFile file)
        {
            return RegisteredProviders.Any(p => p.IsValidForFile(file));
        }

        private static DataStructures.Models.Model LoadModel(IFile file, DataStructures.GameData.Palette pal)
        {
            var path = file.FullPathName;
            if (Models.ContainsKey(path)) return Models[path];

            if (!file.Exists) throw new ProviderException("The supplied file doesn't exist.");
            var provider = RegisteredProviders.FirstOrDefault(p => p.IsValidForFile(file));
            if (provider != null)
            {
                var model = provider.LoadFromFile(file, pal);
                model.PreprocessModel();
                for (var i = 0; i < model.Textures.Count; i++)
                {
                    var t = model.Textures[i];
                    t.TextureObject = TextureHelper.Create(String.Format("ModelProvider: {0}/{1}/{2}", path, t.Name, i), t.Image, t.Image.Width, t.Image.Height, TextureFlags.None);
                }
                Models[path] = model;
                return model;
            }
            throw new ProviderNotFoundException("No model provider was found for this file.");
        }

        private static void UnloadModel(DataStructures.Models.Model model)
        {
            model.Dispose();
            foreach (var kv in Models.Where(x => x.Value == model)) Models.Remove(kv.Key);
        }

        protected abstract bool IsValidForFile(IFile file);
        protected abstract DataStructures.Models.Model LoadFromFile(IFile file, DataStructures.GameData.Palette pal);
    }
}
