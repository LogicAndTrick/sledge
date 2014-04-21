using System.Collections.Generic;
using System.Linq;
using System.IO;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Providers.Map
{
    public abstract class MapProvider
    {
        private static readonly List<MapProvider> RegisteredProviders;

        static MapProvider()
        {
            RegisteredProviders = new List<MapProvider>();
        }

        public static void Register(MapProvider provider)
        {
            RegisteredProviders.Add(provider);
        }

        public static void Deregister(MapProvider provider)
        {
            RegisteredProviders.Remove(provider);
        }

        public static void DeregisterAll()
        {
            RegisteredProviders.Clear();
        }

        public static DataStructures.MapObjects.Map GetMapFromFile(string fileName)
        {
            if (!File.Exists(fileName)) throw new ProviderException("The supplied file doesn't exist.");
            var provider = RegisteredProviders.FirstOrDefault(p => p.IsValidForFileName(fileName));
            if (provider != null)
            {
                return provider.GetFromFile(fileName);
            }
            throw new ProviderNotFoundException("No map provider was found for this file.");
        }

        public static void SaveMapToFile(string filename, DataStructures.MapObjects.Map map)
        {
            var provider = RegisteredProviders.FirstOrDefault(p => p.IsValidForFileName(filename));
            if (provider != null)
            {
                provider.SaveToFile(filename, map);
                return;
            }
            throw new ProviderNotFoundException("No map provider was found for this file format.");
        }

        public static IEnumerable<MapFeature> GetFormatFeatures(string filename)
        {
            var provider = RegisteredProviders.FirstOrDefault(p => p.IsValidForFileName(filename));
            if (provider != null)
            {
                return provider.GetFormatFeatures();
            }
            throw new ProviderNotFoundException("No map provider was found for this file format.");
        }

        protected virtual DataStructures.MapObjects.Map GetFromFile(string filename)
        {
            using (var strm = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                return GetFromStream(strm);
            }
        }

        protected virtual void SaveToFile(string filename, DataStructures.MapObjects.Map map)
        {
            using (var strm = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                SaveToStream(strm, map);
            }
        }

        protected abstract bool IsValidForFileName(string filename);
        protected abstract DataStructures.MapObjects.Map GetFromStream(Stream stream);
        protected abstract void SaveToStream(Stream stream, DataStructures.MapObjects.Map map);
        protected abstract IEnumerable<MapFeature> GetFormatFeatures();
    }
}
