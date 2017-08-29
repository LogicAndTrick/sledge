using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading.Tasks;
using LogicAndTrick.Gimme;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Compile;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Providers;
using Sledge.Common.Shell.Commands;
using Sledge.DataStructures.GameData;
using Sledge.FileSystem;
using Sledge.Providers.Texture;

namespace Sledge.BspEditor.Environment.Goldsource
{
    public class GoldsourceEnvironment : IEnvironment
    {
        public string Engine => "Goldsource";
        public string ID { get; set; }
        public string Name { get; set; }

        public string BaseDirectory { get; set; }
        public string GameDirectory { get; set; }
        public string ModDirectory { get; set; }
        public string GameExe { get; set; }
        public bool LoadHdModels { get; set; }

        public List<string> FgdFiles { get; set; }
        public bool IncludeFgdDirectoriesInEnvironment { get; set; }
        public string DefaultPointEntity { get; set; }
        public string DefaultBrushEntity { get; set; }
        public bool OverrideMapSize { get; set; }
        public decimal MapSizeLow { get; set; }
        public decimal MapSizeHigh { get; set; }

        public decimal DefaultTextureScale { get; set; }

        public string ToolsDirectory { get; set; }
        public bool IncludeToolsDirectoryInEnvironment { get; set; }

        public string CsgExe { get; set; }
        public string VisExe { get; set; }
        public string BspExe { get; set; }
        public string RadExe { get; set; }

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
                
                if (IncludeToolsDirectoryInEnvironment && !String.IsNullOrWhiteSpace(ToolsDirectory) && Directory.Exists(ToolsDirectory))
                {
                    yield return ToolsDirectory;
                }

                if (IncludeFgdDirectoriesInEnvironment)
                {
                    foreach (var file in FgdFiles)
                    {
                        if (File.Exists(file)) yield return Path.GetDirectoryName(file);
                    }
                }

                // Editor location to the path, for sprites and the like
                yield return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
        }

        private readonly Lazy<Task<TextureCollection>> _textureCollection;
        private readonly List<IEnvironmentData> _data;
        private readonly Lazy<Task<GameData>> _gameData;

        public GoldsourceEnvironment()
        {
            _textureCollection = new Lazy<Task<TextureCollection>>(MakeTextureCollectionAsync);
            _gameData = new Lazy<Task<GameData>>(MakeGameDataAsync);
            _data = new List<IEnvironmentData>();
            FgdFiles = new List<string>();
            IncludeToolsDirectoryInEnvironment = IncludeToolsDirectoryInEnvironment = true;
        }

        private async Task<TextureCollection> MakeTextureCollectionAsync()
        {
            var packages = await Directories.Select(x => Gimme.Fetch<TexturePackage>(x, null)).Merge().ToList().ToTask();
            return new TextureCollection(packages);
        }

        private async Task<GameData> MakeGameDataAsync()
        {
            var gds = await FgdFiles.Select(x => Gimme.Fetch<GameData>(x, null)).Merge().ToList().ToTask();

            var gd = new GameData();
            foreach (var d in gds)
            {
                gd.MapSizeHigh = d.MapSizeHigh;
                gd.MapSizeLow = d.MapSizeLow;
                gd.Classes.AddRange(d.Classes);
                gd.MaterialExclusions.AddRange(d.MaterialExclusions);
            }
            gd.CreateDependencies();
            gd.RemoveDuplicates();

            return gd;
        }

        public Task<TextureCollection> GetTextureCollection()
        {
            return _textureCollection.Value;
        }

        public Task<GameData> GetGameData()
        {
            return _gameData.Value;
        }

        public async Task UpdateDocumentData(MapDocument document)
        {
            var tc = await GetTextureCollection();
            document.Map.Root.Data.GetOne<EntityData>()?.Set("wad", string.Join(";", GetUsedTexturePackages(document, tc).Select(x => x.Location).Where(x => x.EndsWith(".wad"))));
        }

        private IEnumerable<string> GetUsedTextures(MapDocument document)
        {
            return document.Map.Root.FindAll().SelectMany(x => x.Data.OfType<ITextured>()).Select(x => x.Texture.Name).Distinct();
        }

        private IEnumerable<TexturePackage> GetUsedTexturePackages(MapDocument document, TextureCollection collection)
        {
            var used = GetUsedTextures(document).ToList();
            return collection.Packages.Where(x => used.Any(x.HasTexture));
        }

        public void AddData(IEnvironmentData data)
        {
            if (!_data.Contains(data)) _data.Add(data);
        }

        public IEnumerable<T> GetData<T>() where T : IEnvironmentData
        {
            return _data.OfType<T>();
        }

        public async Task<Batch> CreateBatch(IEnumerable<BatchArgument> arguments)
        {
            var args = arguments.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.First().Arguments);

            var batch = new Batch();

            // Create the working directory
            batch.Steps.Add(new BatchCallback(async (b, d) =>
            {
                var workingDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                if (!Directory.Exists(workingDir)) Directory.CreateDirectory(workingDir);
                batch.Variables["WorkingDirectory"] = workingDir;

                await Oy.Publish("Compile:Debug", $"Working directory is: {workingDir}\r\n");
            }));

            // Save the file to the working directory
            batch.Steps.Add(new BatchCallback(async (b, d) =>
            {
                var fn = d.FileName;
                if (String.IsNullOrWhiteSpace(fn) || fn.IndexOf('.') < 0) fn = Path.GetRandomFileName();
                var mapFile = Path.GetFileNameWithoutExtension(fn) + ".map";
                batch.Variables["MapFileName"] = mapFile;

                var path = Path.Combine(b.Variables["WorkingDirectory"], mapFile);
                b.Variables["MapFile"] = path;

                await Oy.Publish("Command:Run", new CommandMessage("Internal:SaveDocument", new
                {
                    Document = d,
                    Path = path,
                    LoaderHint = nameof(MapBspSourceProvider)
                }));

                await Oy.Publish("Compile:Debug", $"Map file is: {path}\r\n");
            }));

            // Run the compile tools
            if (args.ContainsKey("CSG")) batch.Steps.Add(new BatchProcess(Path.Combine(ToolsDirectory, CsgExe), args["CSG"] + " \"{MapFile}\""));
            if (args.ContainsKey("BSP")) batch.Steps.Add(new BatchProcess(Path.Combine(ToolsDirectory, BspExe), args["BSP"] + " \"{MapFile}\""));
            if (args.ContainsKey("VIS")) batch.Steps.Add(new BatchProcess(Path.Combine(ToolsDirectory, VisExe), args["VIS"] + " \"{MapFile}\""));
            if (args.ContainsKey("RAD")) batch.Steps.Add(new BatchProcess(Path.Combine(ToolsDirectory, RadExe), args["RAD"] + " \"{MapFile}\""));

            // Check for errors
            batch.Steps.Add(new BatchCallback(async (b, d) =>
            {
                var errFile = Path.ChangeExtension(b.Variables["MapFile"], "err");
                if (errFile != null && File.Exists(errFile))
                {
                    var errors = File.ReadAllText(errFile);
                    b.Successful = false;
                    await Oy.Publish("Compile:Error", errors);
                }

                var bspFile = Path.ChangeExtension(b.Variables["MapFile"], "bsp");
                if (bspFile != null && !File.Exists(bspFile))
                {
                    b.Successful = false;
                }
            }));

            // Copy resulting files around
            batch.Steps.Add(new BatchCallback(async (b, d) =>
            {
                var origDir = Path.GetDirectoryName(d.FileName);
                if (origDir != null && Directory.Exists(origDir))
                {
                    var linFile = Path.ChangeExtension(b.Variables["MapFile"], "lin");
                    if (File.Exists(linFile)) File.Copy(linFile, Path.Combine(origDir, Path.GetFileName(linFile)), true);

                    var ptsFile = Path.ChangeExtension(b.Variables["MapFile"], "pts");
                    if (File.Exists(ptsFile)) File.Copy(ptsFile, Path.Combine(origDir, Path.GetFileName(ptsFile)), true);
                }

                var gameMapDir = Path.Combine(BaseDirectory, ModDirectory, "maps");
                if (b.Successful && Directory.Exists(gameMapDir))
                {
                    var bspFile = Path.ChangeExtension(b.Variables["MapFile"], "bsp");
                    if (File.Exists(bspFile)) File.Copy(bspFile, Path.Combine(gameMapDir, Path.GetFileName(bspFile)), true);

                    var resFile = Path.ChangeExtension(b.Variables["MapFile"], "res");
                    if (File.Exists(resFile)) File.Copy(resFile, Path.Combine(gameMapDir, Path.GetFileName(resFile)), true);
                }
            }));

            // Delete temp directory
            batch.Steps.Add(new BatchCallback(async (b, d) =>
            {
                var workingDir = batch.Variables["WorkingDirectory"];
                if (Directory.Exists(workingDir)) Directory.Delete(workingDir, true);
            }));

            return batch;
        }
    }
}