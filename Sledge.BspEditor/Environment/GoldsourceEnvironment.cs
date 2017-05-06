using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading.Tasks;
using LogicAndTrick.Gimme;
using Sledge.BspEditor.Primitives;
using Sledge.DataStructures.GameData;
using Sledge.FileSystem;
using Sledge.Providers.Texture;

namespace Sledge.BspEditor.Environment
{
    public class GoldsourceEnvironment : IEnvironment
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string BaseDirectory { get; set; }
        public string GameDirectory { get; set; }
        public string ModDirectory { get; set; }

        private IFile _root;

        public IFile Root
        {
            get
            {
                if (_root == null)
                {
                    var dirs = Directories.Where(Directory.Exists).ToList();
                    if (dirs.Any()) _root = new RootFile(Name, dirs.Select(x => new NativeFile(x)));
                    else _root = new VirtualFile(null, "");
                }
                return _root;
            }
        }

        public IEnumerable<string> Directories
        {
            get
            {
                // mod_addon (custom content)
                yield return Path.Combine(BaseDirectory, ModDirectory + "_addon");

                //mod_downloads (downloaded content)
                yield return Path.Combine(BaseDirectory, ModDirectory + "_downloads");

                // mod_hd (high definition content)
                yield return Path.Combine(BaseDirectory, ModDirectory + "_hd");

                // mod (base mod content)
                yield return Path.Combine(BaseDirectory, ModDirectory);

                if (!String.Equals(GameDirectory, ModDirectory, StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return Path.Combine(BaseDirectory, GameDirectory + "_addon");
                    yield return Path.Combine(BaseDirectory, GameDirectory + "_downloads");
                    yield return Path.Combine(BaseDirectory, GameDirectory + "_hd");
                    yield return Path.Combine(BaseDirectory, GameDirectory);
                }

                //todo !environment include build tools and fgd directories
                //var b = Build;
                //if (b != null && b.IncludePathInEnvironment)
                //{
                //    yield return b.Path;
                //}

                //if (Game.IncludeFgdDirectoriesInEnvironment)
                //{
                //    foreach (var d in Game.GetFgdDirectories()) yield return d;
                //}

                // Editor location to the path, for sprites and the like
                yield return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
        }

        private readonly Lazy<Task<TextureCollection>> _textureCollection;
        private readonly List<IEnvironmentData> _data;

        public GoldsourceEnvironment()
        {
            ID = "GSE-" + Convert.ToString(UniqueNumberGenerator.Instance.Next("Environment"));
            _textureCollection = new Lazy<Task<TextureCollection>>(MakeTextureCollectionAsync);
            _data = new List<IEnvironmentData>();
        }

        private async Task<TextureCollection> MakeTextureCollectionAsync()
        {
            var packages = await Directories.Select(x => Gimme.Fetch<TexturePackage>(x, null)).Merge().ToList().ToTask();
            return new TextureCollection(packages);
        }

        public Task<TextureCollection> GetTextureCollection()
        {
            return _textureCollection.Value;
        }

        public async Task<GameData> GetGameData()
        {
            // todo ! gamedata
            return new GameData();
        }

        public void AddData(IEnvironmentData data)
        {
            if (!_data.Contains(data)) _data.Add(data);
        }

        public IEnumerable<T> GetData<T>() where T : IEnvironmentData
        {
            return _data.OfType<T>();
        }
    }
}