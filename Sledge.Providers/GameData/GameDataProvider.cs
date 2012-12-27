using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sledge.Providers.GameData
{
    public abstract class GameDataProvider
    {
        private static readonly List<GameDataProvider> RegisteredProviders;

        static GameDataProvider()
        {
            RegisteredProviders = new List<GameDataProvider>();
        }

        public static void Register(GameDataProvider provider)
        {
            RegisteredProviders.Add(provider);
        }

        public static void Deregister(GameDataProvider provider)
        {
            RegisteredProviders.Remove(provider);
        }

        public static DataStructures.GameData.GameData GetGameDataFromFiles(IEnumerable<string> files)
        {
            var gd = new DataStructures.GameData.GameData();
            foreach (var d in files.Select(GetGameDataFromFile))
            {
                gd.MapSizeHigh = d.MapSizeHigh;
                gd.MapSizeLow = d.MapSizeLow;
                gd.Classes.AddRange(d.Classes);
            }
            gd.CreateDependencies();
            return gd;
        }

        public static DataStructures.GameData.GameData GetGameDataFromFile(string fileName)
        {
            var provider = RegisteredProviders.FirstOrDefault(p => p.IsValidForFile(fileName));
            if (provider != null)
            {
                var gd = provider.GetFromFile(fileName);
                gd.CreateDependencies();
                return gd;
            }
            throw new ProviderNotFoundException("No GameData provider was found for this file.");
        }

        public static DataStructures.GameData.GameData GetGameDataFromString(string contents)
        {
            var provider = RegisteredProviders.FirstOrDefault(p => p.IsValidForString(contents));
            if (provider != null)
            {
                var gd = provider.GetFromString(contents);
                gd.CreateDependencies();
                return gd;
            }
            throw new ProviderNotFoundException("No GameData provider was found for this string.");
        }

        public static DataStructures.GameData.GameData GetGameDataFromStream(Stream stream)
        {
            var provider = RegisteredProviders.FirstOrDefault(p => p.IsValidForStream(stream));
            if (provider != null)
            {
                var gd = provider.GetFromStream(stream);
                gd.CreateDependencies();
                return gd;
            }
            throw new ProviderNotFoundException("No GameData provider was found for this stream.");
        }

        protected virtual bool IsValidForFile(string filename)
        {
            Stream strm = new FileStream(filename, FileMode.Open, FileAccess.Read);
            return IsValidForStream(strm);
        }

        protected virtual bool IsValidForString(string contents)
        {
            var length = Encoding.UTF8.GetByteCount(contents);
            Stream strm = new MemoryStream(length);
            strm.Write(Encoding.UTF8.GetBytes(contents), 0, length);
            return IsValidForStream(strm);
        }

        protected abstract bool IsValidForStream(Stream stream);

        protected virtual DataStructures.GameData.GameData GetFromFile(string filename)
        {
            Stream strm = new FileStream(filename, FileMode.Open, FileAccess.Read);
            return GetFromStream(strm);
        }

        protected virtual DataStructures.GameData.GameData GetFromString(string contents)
        {
            var length = Encoding.UTF8.GetByteCount(contents);
            Stream strm = new MemoryStream(length);
            strm.Write(Encoding.UTF8.GetBytes(contents), 0, length);
            return GetFromStream(strm);
        }

        protected abstract DataStructures.GameData.GameData GetFromStream(Stream stream);
    }
}
