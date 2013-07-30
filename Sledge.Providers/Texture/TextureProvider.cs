using System;
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

        #region Registration

        public static void Register(TextureProvider provider)
        {
            RegisteredProviders.Add(provider);
        }

        public static void Deregister(TextureProvider provider)
        {
            RegisteredProviders.Remove(provider);
        }

        #endregion

        public abstract bool IsValidForPackageFile(string package);
        public abstract TexturePackage CreatePackage(string package);
        public abstract void LoadTexture(TextureItem item);
        public abstract void LoadTextures(IEnumerable<TextureItem> items);
        public abstract ITextureStreamSource GetStreamSource(IEnumerable<TexturePackage> packages);

        public static TextureCollection CreateCollection(IEnumerable<string> packages)
        {
            var pkgs = new List<TexturePackage>();
            foreach (var package in packages)
            {
                var existing = Packages.FirstOrDefault(x => String.Equals(x.PackageFile, package, StringComparison.InvariantCultureIgnoreCase));
                if (existing != null)
                {
                    // Package already loaded in another map
                    pkgs.Add(existing);
                }
                else
                {
                    // Load the package
                    var provider = RegisteredProviders.FirstOrDefault(p => p.IsValidForPackageFile(package));
                    if (provider == null) throw new ProviderNotFoundException("No texture provider was found for package: " + package);
                    var pkg = provider.CreatePackage(package);
                    Packages.Add(pkg);
                    pkgs.Add(pkg);
                }
            }
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
            item.Package.LoadTexture(item);
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
