using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Sledge.Graphics;
using Sledge.Graphics.Helpers;

namespace Sledge.Providers.Texture
{
    public class TexturePackage
    {
        private static readonly Dictionary<string, TexturePackage> LoadedPackages = new Dictionary<string, TexturePackage>();
        private static readonly Dictionary<string, TextureItem> LoadedItems = new Dictionary<string, TextureItem>();
        
        public static void ClearLoadedPackages()
        {
            LoadedPackages.Clear();
            LoadedItems.Clear();
            TextureHelper.ClearLoadedTextures();
        }

        public static void LoadTextureData(string name)
        {
            if (TextureHelper.Exists(name)) return;
            var item = LoadedItems[name];
            if (item != null)
            {
                TextureProvider.LoadTextureFromPackage(item.Package, item.Name);
            }
        }

        public static void LoadTextureData(IEnumerable<string> names)
        {
            var texes = names.Where(x => !TextureHelper.Exists(x));
            TextureProvider.LoadTexturesFromPackages(LoadedPackages.Values, texes);
        }

        public static IEnumerable<TexturePackage> GetLoadedPackages()
        {
            return LoadedPackages.Values;
        }

        public static IEnumerable<TextureItem> GetLoadedItems()
        {
            return LoadedItems.Values;
        }

        public static TextureItem GetItem(string name)
        {
            return LoadedItems.ContainsKey(name) ? LoadedItems[name] : null;
        }

        public static void Load(string file)
        {
            if (!File.Exists(file)) return;
            var tp = new TexturePackage(file);
            tp.LoadAllTextureItems();
            LoadedPackages.Add(file, tp);
        }

        public string PackageFile { get; private set; }
        public Dictionary<string, TextureItem> Items { get; private set; }

        private TexturePackage(string packageFile)
        {
            PackageFile = packageFile;
            Items = new Dictionary<string, TextureItem>();
        }

        /// <summary>
        /// Tells this package to retrieve all the texture names from the file.
        /// This will not get the actual texture data, but it will get some metadata
        /// like name, width, height, and so on.
        /// </summary>
        public void LoadAllTextureItems()
        {
            foreach (var ti in TextureProvider.GetAllTextureItemsFromPackage(this))
            {
                if (LoadedItems.ContainsKey(ti.Name)) continue;
                Items.Add(ti.Name, ti);
                LoadedItems.Add(ti.Name, ti);
            }
        }
    }
}
