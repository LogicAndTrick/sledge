using System.Collections.Generic;
using System.Linq;
using System;
using System.Drawing;

namespace Sledge.Providers.Texture
{
    public abstract class TextureProvider
    {
        public abstract class TextureStreamSource : IDisposable
        {
            protected List<TexturePackage> Packages;

            protected TextureStreamSource(IEnumerable<TexturePackage> packages)
            {
                Packages = new List<TexturePackage>(packages);
            }

            public abstract Bitmap GetImage(TextureItem item);

            public abstract void Dispose();
        }

        private static readonly List<TextureProvider> RegisteredProviders;

        static TextureProvider()
        {
            RegisteredProviders = new List<TextureProvider>();
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

        protected abstract bool IsValidForPackageFile(string package);

        public static void LoadTexturesFromPackages(IEnumerable<TexturePackage> packages, IEnumerable<string> names)
        {
            packages.ToList().ForEach(p => LoadTexturesFromPackage(p, names));
        }

        public static void LoadTexturesFromPackage(TexturePackage package, IEnumerable<string> names)
        {
            var provider = RegisteredProviders.FirstOrDefault(p => p.IsValidForPackageFile(package.PackageFile));
            if (provider != null)
            {
                provider.LoadTextures(package, names);
                return;
            }
        }

        public static void LoadTextureFromPackage(TexturePackage package, string name)
        {
            var provider = RegisteredProviders.FirstOrDefault(p => p.IsValidForPackageFile(package.PackageFile));
            if (provider != null)
            {
                provider.LoadTexture(package, name);
                return;
            }
            throw new ProviderNotFoundException("No texture provider was found for this package.");
        }

        protected abstract void LoadTexture(TexturePackage package, string name);
        protected abstract void LoadTextures(TexturePackage package, IEnumerable<string> names);

        public static IEnumerable<TextureItem> GetAllTextureItemsFromPackage(TexturePackage package)
        {
            var provider = RegisteredProviders.FirstOrDefault(p => p.IsValidForPackageFile(package.PackageFile));
            if (provider != null)
            {
                return provider.GetAllTextureItems(package);
            }
            throw new ProviderNotFoundException("No texture provider was found for this package.");
        }

        /// <summary>
        /// Loads all the texture items for a package. Doesn't get image data, just
        /// the metadata of the texture.
        /// </summary>
        /// <param name="package">The texture package to load textures from</param>
        /// <returns>A list of texture items in the package.</returns>
        protected abstract IEnumerable<TextureItem> GetAllTextureItems(TexturePackage package);

        public static TextureStreamSource GetStreamSourceForPackages(IEnumerable<TexturePackage> packages)
        {
            foreach (var rp in RegisteredProviders)
            {
                var valid = true;
                foreach (var tp in packages)
                {
                    if (!rp.IsValidForPackageFile(tp.PackageFile))
                    {
                        valid = false;
                    }
                }
                if (valid)
                {
                    return rp.GetStreamSource(packages);
                }
            }
            throw new ProviderNotFoundException("No texture provider was found for this package.");
        }

        protected abstract TextureStreamSource GetStreamSource(IEnumerable<TexturePackage> packages);
    }
}
