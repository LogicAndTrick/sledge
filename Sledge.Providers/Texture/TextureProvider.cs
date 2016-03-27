using System.Collections.Generic;
using System.Linq;

namespace Sledge.Providers.Texture
{
    public abstract class TextureProvider
    {
        private static readonly List<TextureProvider> RegisteredProviders;
        private static readonly List<TextureCollection> Collections;
        private static readonly List<TexturePackage> Packages;

        static TextureProvider()
        {
            RegisteredProviders = new List<TextureProvider>();
            Collections = new List<TextureCollection>();
            Packages = new List<TexturePackage>();
        }

        private static string _cachePath;

        public static void SetCachePath(string path)
        {
            _cachePath = path;
            foreach (var p in RegisteredProviders) p.CachePath = _cachePath;
        }

        #region Registration

        public static void Register(TextureProvider provider)
        {
            provider.CachePath = _cachePath;
            RegisteredProviders.Add(provider);
        }

        public static void Deregister(TextureProvider provider)
        {
            RegisteredProviders.Remove(provider);
        }

        #endregion

        protected string CachePath { get; private set; }
        public abstract IEnumerable<TexturePackage> CreatePackages(IEnumerable<string> sourceRoots, IEnumerable<string> additionalPackages, IEnumerable<string> blacklist, IEnumerable<string> whitelist);
        public abstract void DeletePackages(IEnumerable<TexturePackage> packages);
        public abstract ITextureStreamSource GetStreamSource(int maxWidth, int maxHeight, IEnumerable<TexturePackage> packages);

        public static TextureCollection CreateCollection(IEnumerable<string> sourceRoots, IEnumerable<string> additionalPackages, IEnumerable<string> blacklist, IEnumerable<string> whitelist)
        {
            var list = sourceRoots.ToList();
            var additional = additionalPackages == null ? new List<string>() : additionalPackages.ToList();
            var wl = whitelist == null ? new List<string>() : whitelist.ToList();
            var bl = blacklist == null ? new List<string>() : blacklist.ToList();
            var pkgs = new List<TexturePackage>();
            foreach (var provider in RegisteredProviders)
            {
                pkgs.AddRange(provider.CreatePackages(list, additional, bl, wl));
            }
            var tc = new TextureCollection(pkgs);
            Packages.AddRange(pkgs);
            Collections.Add(tc);
            return tc;
        }

        public static void DeleteCollection(TextureCollection collection)
        {
            Collections.RemoveAll(x => x == collection);
            var remove = Packages.Where(package => !Collections.Any(x => x.Packages.Contains(package))).ToList();
            foreach (var package in remove)
            {
                Packages.Remove(package);
                package.Provider.DeletePackages(new[] {package});
                package.Dispose();
            }
        }
    }
}
