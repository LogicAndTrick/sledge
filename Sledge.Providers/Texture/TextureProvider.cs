using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sledge.FileSystem;
using Sledge.Graphics.Helpers;

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
        public abstract void LoadTextures(IEnumerable<TextureItem> items, ISynchronizeInvoke invokable);
        public abstract ITextureStreamSource GetStreamSource(int maxWidth, int maxHeight, IEnumerable<TexturePackage> packages);

        public static TextureCollection CreateCollection(IEnumerable<string> sourceRoots)
        {
            var list = sourceRoots.ToList();
            var pkgs = new List<TexturePackage>();
            foreach (var provider in RegisteredProviders)
            {
                pkgs.AddRange(provider.CreatePackages(list));
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

        public static void LoadTextureItem(TextureItem item, ISynchronizeInvoke invokable)
        {
            item.Package.Provider.LoadTextures(new[] { item }, invokable);
        }

        public static void LoadTextureItems(IEnumerable<TextureItem> items, ISynchronizeInvoke invokable)
        {
            var list = items.ToList();

            foreach (var g in list.GroupBy(x => x.Package.Provider))
            {
                LoadTextures(g.Key, g, invokable);
            }
        }

        private static void LoadTextures(TextureProvider provider, IEnumerable<TextureItem> items, ISynchronizeInvoke invokable)
        {
            var all = items.Where(x => !TextureHelper.Exists(x.Name.ToLowerInvariant())).ToList();
            if (!all.Any()) return;

            if (TextureHelper.PrecacheImage != null && invokable != null)
            {
                foreach (var ti in all)
                {
                    TextureHelper.Create(ti.Name.ToLowerInvariant(), TextureHelper.PrecacheImage, ti.Width, ti.Height, false);
                }
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                    provider.LoadTextures(all, invokable);
                });
            }
            else
            {
                provider.LoadTextures(all, invokable);
            }
        }
    }
}
