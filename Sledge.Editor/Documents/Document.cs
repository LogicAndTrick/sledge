using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenTK;
using Sledge.Common;
using Sledge.Common.Mediator;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;
using Sledge.Editor.Editing;
using Sledge.Editor.Environment;
using Sledge.Editor.Extensions;
using Sledge.Editor.History;
using Sledge.Editor.Rendering;
using Sledge.Editor.Rendering.Helpers;
using Sledge.Editor.Settings;
using Sledge.Editor.Tools;
using Sledge.Editor.UI;
using Sledge.FileSystem;
using Sledge.Graphics.Helpers;
using Sledge.Providers;
using Sledge.Providers.GameData;
using Sledge.Providers.Map;
using Sledge.Providers.Texture;
using Sledge.Settings;
using Sledge.Settings.Models;
using Sledge.UI;
using Path = System.IO.Path;

namespace Sledge.Editor.Documents
{
    public class Document
    {
        public string MapFile { get; set; }
        public string MapFileName { get; set; }
        public Map Map { get; set; }

        public Game Game { get; set; }
        public GameEnvironment Environment { get; private set; }
        public GameData GameData { get; set; }
        public Palette Palette { get; set; }

        public Pointfile Pointfile { get; set; }

        public RenderManager Renderer { get; private set; }

        public SelectionManager Selection { get; private set; }
        public HistoryManager History { get; private set; }
        public HelperManager HelperManager { get; set; }
        public TextureCollection TextureCollection { get; set; }

        private readonly DocumentSubscriptions _subscriptions;
        private readonly DocumentMemory _memory;

        private Document()
        {
            Map = new Map();
            Selection = new SelectionManager(this);
            History = new HistoryManager(this);
            HelperManager = new HelperManager(this);
            TextureCollection = new TextureCollection(new List<TexturePackage>());
        }

        public Document(string mapFile, Map map, Game game)
        {
            MapFile = mapFile;
            Map = map;
            Game = game;
            Environment = new GameEnvironment(game);
            MapFileName = mapFile == null
                              ? DocumentManager.GetUntitledDocumentName()
                              : Path.GetFileName(mapFile);
            SelectListTransform = Matrix4.Identity;

            _subscriptions = new DocumentSubscriptions(this);

            _memory = new DocumentMemory();

            var cam = Map.GetActiveCamera();
            if (cam != null) _memory.SetCamera(cam.EyePosition, cam.LookPosition);

            Selection = new SelectionManager(this);
            History = new HistoryManager(this);
            if (Map.GridSpacing <= 0)
            {
                Map.GridSpacing = Grid.DefaultSize;
            }

            try
            {
                GameData =  GameDataProvider.GetGameDataFromFiles(game.Fgds.Select(f => f.Path));
            }
            catch(ProviderException)
            {
                // TODO: Error logging
                GameData = new GameData();
            }

            if (game.OverrideMapSize)
            {
                GameData.MapSizeLow = game.OverrideMapSizeLow;
                GameData.MapSizeHigh = game.OverrideMapSizeHigh;
            }

            // Set up Quake 1/Hexen 2 palette
            var palpath = Environment.Root.TraversePath("gfx/palette.lmp");
            var paldata = new byte[768];
            if (palpath != null)
            {
                try
                {
                    using (var br = new BinaryReader(palpath.Open()))
                    {
                        paldata = br.ReadBytes(768);
                    }
                }
                catch (Exception)
                {
                }
            }
            Palette = new Palette(paldata);
            
            TextureCollection = TextureProvider.CreateCollection(Environment.GetGameDirectories(), Game.AdditionalPackages, Game.GetTextureBlacklist(), Game.GetTextureWhitelist(), Palette);
            /* .Union(GameData.MaterialExclusions) */ // todo material exclusions

            var texList = Map.GetAllTextures();
            var items = TextureCollection.GetItems(texList);
            TextureProvider.LoadTextureItems(items);

            Map.PostLoadProcess(GameData, GetTexture, SettingsManager.GetSpecialTextureOpacity);
            Map.UpdateDecals(this);
            Map.UpdateModels(this);
            Map.UpdateSprites(this);

            HelperManager = new HelperManager(this);
            Renderer = new RenderManager(this);

            if (MapFile != null) Mediator.Publish(EditorMediator.FileOpened, MapFile);

            // Autosaving
            if (Game.Autosave)
            {
                var at = Math.Max(1, Game.AutosaveTime);
                Scheduler.Schedule(this, Autosave, TimeSpan.FromMinutes(at));
            }
        }

        public void SetMemory<T>(string name, T obj)
        {
            _memory.Set(name, obj);
        }

        public T GetMemory<T>(string name, T def = default (T))
        {
            return _memory.Get(name, def);
        }

        public void SetActive()
        {
            if (!Sledge.Settings.View.KeepSelectedTool) ToolManager.Activate(_memory.SelectedTool);
            if (!Sledge.Settings.View.KeepCameraPositions) _memory.RestoreViewports(ViewportManager.Viewports);

            ViewportManager.AddContext3D(new WidgetLinesRenderable());
            Renderer.Register(ViewportManager.Viewports);
            ViewportManager.AddContextAll(new ToolRenderable());
            ViewportManager.AddContextAll(new HelperRenderable(this));

            _subscriptions.Subscribe();

            RenderAll();
        }

        public void SetInactive()
        {
            if (!Sledge.Settings.View.KeepSelectedTool && ToolManager.ActiveTool != null) _memory.SelectedTool = ToolManager.ActiveTool.GetType();
            if (!Sledge.Settings.View.KeepCameraPositions) _memory.RememberViewports(ViewportManager.Viewports);

            ViewportManager.ClearContexts();
            HelperManager.ClearCache();

            _subscriptions.Unsubscribe();
        }

        public void Close()
        {
            Scheduler.Clear(this);
            TextureProvider.DeleteCollection(TextureCollection);
            Renderer.Dispose();
        }

        public bool SaveFile(string path = null, bool forceOverride = false, bool switchPath = true)
        {
            path = forceOverride ? path : path ?? MapFile;
            if (path == null)
            {
                using (var sfd = new SaveFileDialog())
                {
                    var filter = String.Join("|", FileTypeRegistration.GetSupportedExtensions()
                        .Where(x => x.IsPrimaryFormat).Select(x => x.Description + " (*" + x.Extension + ")|*" + x.Extension));
                    var all = FileTypeRegistration.GetSupportedExtensions().Where(x => x.IsPrimaryFormat).Select(x => "*" + x.Extension).ToArray();
                    sfd.Filter = "All supported formats (" + String.Join(", ", all) + ")|" + String.Join(";", all) + "|" + filter;
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        path = sfd.FileName;
                    }
                }
            }
            if (path == null) return false;

            // Save the 3D camera position
            var cam = ViewportManager.Viewports.OfType<Viewport3D>().Select(x => x.Camera).FirstOrDefault();
            if (cam != null)
            {
                if (Map.ActiveCamera == null)
                {
                    Map.ActiveCamera = !Map.Cameras.Any() ? new Camera{LookPosition = Coordinate.UnitX * Map.GridSpacing * 1.5m} : Map.Cameras.First();
                    if (!Map.Cameras.Contains(Map.ActiveCamera)) Map.Cameras.Add(Map.ActiveCamera);
                }
                var dist = (Map.ActiveCamera.LookPosition - Map.ActiveCamera.EyePosition).VectorMagnitude();
                var loc = cam.Location;
                var look = cam.LookAt - cam.Location;
                look.Normalize();
                look = loc + look * (float) dist;
                Map.ActiveCamera.EyePosition = new Coordinate((decimal)loc.X, (decimal)loc.Y, (decimal)loc.Z);
                Map.ActiveCamera.LookPosition = new Coordinate((decimal)look.X, (decimal)look.Y, (decimal)look.Z);
            }
            Map.WorldSpawn.EntityData.SetPropertyValue("wad", string.Join(";", GetUsedTexturePackages().Select(x => x.PackageRoot).Where(x => x.EndsWith(".wad"))));
            MapProvider.SaveMapToFile(path, Map);
            if (switchPath)
            {
                MapFile = path;
                MapFileName = Path.GetFileName(MapFile);
                History.TotalActionsSinceLastSave = 0;
                Mediator.Publish(EditorMediator.DocumentSaved, this);
            }
            return true;
        }

        private string GetAutosaveFormatString()
        {
            if (MapFile == null || Path.GetFileNameWithoutExtension(MapFile) == null) return null;
            var we = Path.GetFileNameWithoutExtension(MapFile);
            var ex = Path.GetExtension(MapFile);
            return we + ".auto.{0}" + ex;
        }

        private string GetAutosaveFolder()
        {
            if (Game.UseCustomAutosaveDir && System.IO.Directory.Exists(Game.AutosaveDir)) return Game.AutosaveDir;
            if (MapFile == null || Path.GetDirectoryName(MapFile) == null) return null;
            return Path.GetDirectoryName(MapFile);
        }

        public void Autosave()
        {
            if (!Game.Autosave) return;
            var dir = GetAutosaveFolder();
            var fmt = GetAutosaveFormatString();

            // Only save on change if the game is configured to do so
            if (dir != null && fmt != null && (History.TotalActionsSinceLastAutoSave != 0 || !Game.AutosaveOnlyOnChanged))
            {
                var date = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd-hh-mm-ss");
                var filename = String.Format(fmt, date);
                if (System.IO.File.Exists(filename)) System.IO.File.Delete(filename);

                // Save the file
                MapProvider.SaveMapToFile(Path.Combine(dir, filename), Map);

                // Delete extra autosaves if there is a limit
                if (Game.AutosaveLimit > 0)
                {
                    var asFiles = GetAutosaveFiles(dir);
                    foreach (var file in asFiles.OrderByDescending(x => x.Value).Skip(Game.AutosaveLimit))
                    {
                        if (System.IO.File.Exists(file.Key)) System.IO.File.Delete(file.Key);
                    }
                }

                // Publish event
                Mediator.Publish(EditorMediator.FileAutosaved, this);
                History.TotalActionsSinceLastAutoSave = 0;

                if (Game.AutosaveTriggerFileSave && MapFile != null)
                {
                    SaveFile();
                }
            }

            // Reschedule autosave
            var at = Math.Max(1, Game.AutosaveTime);
            Scheduler.Schedule(this, Autosave, TimeSpan.FromMinutes(at));
        }

        public Dictionary<string, DateTime> GetAutosaveFiles(string dir)
        {
            var ret = new Dictionary<string, DateTime>();
            var fs = GetAutosaveFormatString();
            if (fs == null || dir == null) return ret;
            // Search for matching files
            var files = System.IO.Directory.GetFiles(dir, String.Format(fs, "*"));
            foreach (var file in files)
            {
                // Match the date portion with a regex
                var re = Regex.Escape(fs.Replace("{0}", ":")).Replace(":", "{0}");
                var regex = String.Format(re, "(\\d{4})-(\\d{2})-(\\d{2})-(\\d{2})-(\\d{2})-(\\d{2})");
                var match = Regex.Match(Path.GetFileName(file), regex, RegexOptions.IgnoreCase);
                if (!match.Success) continue;

                // Parse the date and add it if it is valid
                DateTime date;
                var result = DateTime.TryParse(String.Format("{0}-{1}-{2}T{3}:{4}:{5}Z",
                                                             match.Groups[1].Value, match.Groups[2].Value,
                                                             match.Groups[3].Value, match.Groups[4].Value,
                                                             match.Groups[5].Value, match.Groups[6].Value),
                                                             CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,
                                                             out date);
                if (result)
                {
                    ret.Add(file, date);
                }
            }
            return ret;
        }

        public Coordinate Snap(Coordinate c, decimal spacing = 0)
        {
            if (!Map.SnapToGrid) return c;

            var snap = (Select.SnapStyle == SnapStyle.SnapOnAlt && KeyboardState.Alt) ||
                       (Select.SnapStyle == SnapStyle.SnapOffAlt && !KeyboardState.Alt);

            return snap ? c.Snap(spacing == 0 ? Map.GridSpacing : spacing) : c;
        }

        /// <summary>
        /// Performs the action, adds it to the history stack, and optionally updates the display lists
        /// </summary>
        /// <param name="name">The name of the action, for history purposes</param>
        /// <param name="action">The action to perform</param>
        public void PerformAction(string name, IAction action)
        {
            try
            {
                action.Perform(this);
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var frames = st.GetFrames() ?? new StackFrame[0];
                var msg = "Action exception: " + name + " (" + action + ")";
                foreach (var frame in frames)
                {
                    var method = frame.GetMethod();
                    msg += "\r\n    " + method.ReflectedType.FullName + "." + method.Name;
                }
                Logging.Logger.ShowException(new Exception(msg, ex), "Error performing action");
            }

            var history = new HistoryAction(name, action);
            History.AddHistoryItem(history);
        }

        public Matrix4 SelectListTransform { get; private set; }

        public void SetSelectListTransform(Matrix4 matrix)
        {
            SelectListTransform = matrix;
            Renderer.SetSelectionTransform(matrix);
        }

        public void EndSelectionTransform()
        {
            SelectListTransform = Matrix4.Identity;
            Renderer.SetSelectionTransform(Matrix4.Identity);
        }

        public ITexture GetTexture(string name)
        {
            if (!TextureHelper.Exists(name))
            {
                var ti = TextureCollection.GetItem(name);
                if (ti != null)
                {
                    TextureProvider.LoadTextureItem(ti);
                }
            }
            return TextureHelper.Get(name);
        }

        public void RenderAll()
        {
            Map.PartialPostLoadProcess(GameData, GetTexture, SettingsManager.GetSpecialTextureOpacity);

            var decalsUpdated = Map.UpdateDecals(this);
            var modelsUpdated = Map.UpdateModels(this);
            var spritesUpdated = Map.UpdateSprites(this);
            if (decalsUpdated || modelsUpdated || spritesUpdated) Mediator.Publish(EditorMediator.SelectionChanged);

            HelperManager.UpdateCache();
            Renderer.Update();
            ViewportManager.Viewports.ForEach(vp => vp.UpdateNextFrame());
        }

        public void RenderSelection(IEnumerable<MapObject> objects)
        {
            Renderer.UpdateSelection(objects);
            ViewportManager.Viewports.ForEach(vp => vp.UpdateNextFrame());
        }

        public void RenderObjects(IEnumerable<MapObject> objects)
        {
            var objs = objects.ToList();
            Map.PartialPostLoadProcess(GameData, GetTexture, SettingsManager.GetSpecialTextureOpacity);

            var decalsUpdated = Map.UpdateDecals(this, objs);
            var modelsUpdated = Map.UpdateModels(this, objs);
            var spritesUpdated = Map.UpdateSprites(this, objs);
            if (decalsUpdated || modelsUpdated || spritesUpdated) Mediator.Publish(EditorMediator.SelectionChanged);

            HelperManager.UpdateCache();

            // If the models/decals changed, we need to do a full update
            if (modelsUpdated || decalsUpdated || spritesUpdated) Renderer.Update();
            else Renderer.UpdatePartial(objs);

            ViewportManager.Viewports.ForEach(vp => vp.UpdateNextFrame());
        }

        public void RenderFaces(IEnumerable<Face> faces)
        {
            Map.PartialPostLoadProcess(GameData, GetTexture, SettingsManager.GetSpecialTextureOpacity);
            // No need to update decals or models here: they can only be changed via entity properties
            HelperManager.UpdateCache();
            Renderer.UpdatePartial(faces);
            ViewportManager.Viewports.ForEach(vp => vp.UpdateNextFrame());
        }

        public void Make3D(ViewportBase viewport, Viewport3D.ViewType type)
        {
            var vp = ViewportManager.Make3D(viewport, type);
            vp.RenderContext.Add(new WidgetLinesRenderable());
            Renderer.Register(new[] { vp });
            vp.RenderContext.Add(new ToolRenderable());
            vp.RenderContext.Add(new HelperRenderable(this));
            Renderer.UpdateGrid(Map.GridSpacing, Map.Show2DGrid, Map.Show3DGrid, false);
        }

        public void Make2D(ViewportBase viewport, Viewport2D.ViewDirection direction)
        {
            var vp = ViewportManager.Make2D(viewport, direction);
            Renderer.Register(new[] { vp });
            vp.RenderContext.Add(new ToolRenderable());
            vp.RenderContext.Add(new HelperRenderable(this));
            Renderer.UpdateGrid(Map.GridSpacing, Map.Show2DGrid, Map.Show3DGrid, false);
        }

        public IEnumerable<string> GetUsedTextures()
        {
            return Map.WorldSpawn.Find(x => x is Solid).OfType<Solid>().SelectMany(x => x.Faces).Select(x => x.Texture.Name).Distinct();
        }

        public IEnumerable<TexturePackage> GetUsedTexturePackages()
        {
            var used = GetUsedTextures().ToList();
            return TextureCollection.Packages.Where(x => used.Any(x.HasTexture));
        }
    }
}
