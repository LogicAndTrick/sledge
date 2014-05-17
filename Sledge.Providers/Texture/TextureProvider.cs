using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.FileSystem;

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
        public abstract IEnumerable<TexturePackage> CreatePackages(IEnumerable<string> sourceRoots);
        public abstract void DeletePackages(IEnumerable<TexturePackage> packages);
        public abstract void LoadTextures(IEnumerable<TextureItem> items);
        public abstract ITextureStreamSource GetStreamSource(int maxWidth, int maxHeight, IEnumerable<TexturePackage> packages);

        public static TextureCollection CreateCollection(IEnumerable<string> sourceRoots)
        {
            var list = sourceRoots.ToList();
            var pkgs = new List<TexturePackage>();
            foreach (var provider in RegisteredProviders)
            {
                pkgs.AddRange(provider.CreatePackages(list));
            }
            /*foreach (var package in packages)
            {
                var existing = Packages.FirstOrDefault(x => String.Equals(x.PackageFile.FullPathName, package.FullPathName, StringComparison.InvariantCultureIgnoreCase));
                if (existing != null)
                {
                    // Package already loaded in another map
                    pkgs.Add(existing);
                }
                else
                {
                    // Load the package
                    foreach (var provider in RegisteredProviders.Where(p => p.IsValidForPackageFile(package)).ToList())
                    {
                        var pkg = provider.CreatePackage(package);
                        Packages.Add(pkg);
                        pkgs.Add(pkg);
                    }
                    //var provider = RegisteredProviders.FirstOrDefault(p => p.IsValidForPackageFile(package));
                    //if (provider == null) throw new ProviderNotFoundException("No texture provider was found for package: " + package);
                    //var pkg = provider.CreatePackage(package);
                    //Packages.Add(pkg);
                    //pkgs.Add(pkg);
                }
            }*/
            var tc = new TextureCollection(pkgs);
            Collections.Add(tc);
            return tc;
        }

        public static void DeleteCollection(TextureCollection collection)
        {
            Collections.RemoveAll(x => x == collection);
            foreach (var package in Packages.ToArray())
            {
                if (!Collections.Any(x => x.Packages.Contains(package)))
                {
                    Packages.Remove(package);
                    package.Dispose();
                }
            }
        }

        public static void LoadTextureItem(TextureItem item)
        {
            item.Package.LoadTextures(new[] {item});
        }

        public static void LoadTextureItems(IEnumerable<TextureItem> items)
        {
            foreach (var g in items.GroupBy(x => x.Package))
            {
                g.Key.LoadTextures(g);
            }
        }
    }
}
