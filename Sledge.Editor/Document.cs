using System.IO;
using System.Linq;
using OpenTK;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Clipboard;
using Sledge.Editor.Compiling;
using Sledge.Editor.History;
using Sledge.Editor.UI;
using Sledge.Editor.Visgroups;
using Sledge.Graphics.Helpers;
using Sledge.Providers.GameData;
using Sledge.Providers.Map;
using Sledge.UI;
using Sledge.Settings;
using Sledge.Database.Models;
using Sledge.Editor.Rendering;
using Sledge.Providers.Texture;
using Sledge.Editor.Editing;
using Sledge.Providers;
using Path = System.IO.Path;
using Sledge.Editor.Tools;

namespace Sledge.Editor
{
    public static class Document
    {
        public static string CurrentMapFile { get; set; }
        public static Map Map { get; set; }
        public static decimal GridSpacing { get; set; }
        public static GameData GameData { get; set; }
        public static Game Game { get; set; }
        public static bool MapOpen { get; private set; }
        public static bool CaptureAltPresses { get; set; }
        public static bool HideFaceMask { get; set; }

        private static DisplayListGroup[] DisplayLists { get; set; }

        static Document()
        {
            Clear();
        }

        public static void Clear()
        {
            Selection.Clear();
            ViewportManager.ClearContexts();
            VisgroupManager.Clear();
            HistoryManager.Clear();
            ClipboardManager.Clear();
            DisplayListGroup.DeleteLists();
            DisplayLists = new[]
                               {
                                   new DisplayListGroup(),
                                   new DisplayListGroup(Viewport2D.ViewDirection.Top),
                                   new DisplayListGroup(Viewport2D.ViewDirection.Front),
                                   new DisplayListGroup(Viewport2D.ViewDirection.Side)
                               };
            CurrentMapFile = null;
            GridSpacing = Grid.DefaultSize;
            GameData = new GameData();
            Game = null;
            Map = new Map();
            MapOpen = false;
            CaptureAltPresses = false;
            TexturePackage.ClearLoadedPackages();
            TextureHelper.ClearLoadedTextures();
            ToolManager.Activate(null);
            HideFaceMask = false;
        }

        public static void Open(string file, Game game)
        {
            Clear();
            try
            {
                var map = MapProvider.GetMapFromFile(file);
                Load(map, game);
            }
            catch (ProviderException)
            {
                // TODO: error logging
                throw;
            }
            CurrentMapFile = file;
        }

        public static void Save(string file)
        {
            if (!MapOpen) return;
            MapProvider.SaveMapToFile(file, Map);
            CurrentMapFile = file;
        }

        public static void New(Game game)
        {
            Clear();
            Load(new Map(), game);
            CurrentMapFile = null;
        }

        public static void Load(Map map, Game game)
        {
            MapOpen = true;
            Game = game;

            Editor.Instance.SelectTool(ToolManager.Tools[0]);

            try
            {
                GameData =  GameDataProvider.GetGameDataFromFiles(game.Fgds.Select(f => f.Path));
            }
            catch(ProviderException)
            {
                // TODO: Error logging
                GameData = new GameData();
            }

            map.SetMapGameData(GameData);

            ViewportManager.AddGrids();

            foreach (var wad in game.Wads.OrderBy(x => Path.GetFileName(x.Path)))
            {
                TexturePackage.Load(wad.Path);
            }
            var texList = map.GetAllTextures();
            TexturePackage.LoadTextureData(texList);
            map.SetMapTextures(TextureHelper.Get);

            Map = map;

            DisplayListGroup.RegenerateSelectLists();
            DisplayListGroup.RegenerateDisplayLists(false);
            foreach (var dl in DisplayLists)
            {
                dl.Register();
            }

            ViewportManager.Viewports.ForEach(vp => vp.RenderContext.Add(new ToolRenderable()));
            ViewportManager.AddContext3D(new WidgetLinesRenderable());

            VisgroupManager.Update(Map);
        }

        public static void StartSelectionTransform()
        {
            DisplayListGroup.RegenerateSelectLists();
            DisplayListGroup.RegenerateDisplayLists(true);
            foreach (var dl in DisplayLists)
            {
                dl.SetTintSelectListEnabled(false);
            }
            ViewportManager.Viewports.ForEach(vp => vp.UpdateNextFrame());
        }

        public static void SetSelectListTransform(Matrix4d matrix)
        {
            foreach (var dl in DisplayLists)
            {
                dl.SetSelectListTransform(matrix);
            }
        }

        public static void EndSelectionTransform()
        {
            DisplayListGroup.RegenerateSelectLists();
            DisplayListGroup.RegenerateDisplayLists(false);
            foreach (var dl in DisplayLists)
            {
                dl.SetSelectListTransform(Matrix4d.Identity);
                dl.SetTintSelectListEnabled(true);
            }
            ViewportManager.Viewports.ForEach(vp => vp.UpdateNextFrame());
        }

        public static void UpdateDisplayLists(bool exclude = false)
        {
            DisplayListGroup.RegenerateSelectLists();
            DisplayListGroup.RegenerateDisplayLists(exclude);
            ViewportManager.Viewports.ForEach(vp => vp.UpdateNextFrame());
        }

        public static void UpdateSelectLists()
        {
            DisplayListGroup.RegenerateSelectLists();
            ViewportManager.Viewports.ForEach(vp => vp.UpdateNextFrame());
        }

        public static void Compile()
        {
            var currentFile = CurrentMapFile;
            if (!currentFile.EndsWith("map"))
            {
                Map.WorldSpawn.EntityData.Properties.Add(new DataStructures.MapObjects.Property
                                                             {
                                                                 Key = "wad",
                                                                 Value = string.Join(";", Game.Wads.Select(x => x.Path))
                                                             });
                var map = Path.ChangeExtension(CurrentMapFile, "map");
                MapProvider.SaveMapToFile(map, Map);
                currentFile = map;
            }
            var batch = new Batch(Game, currentFile);
            BatchCompiler.Compile(batch);
        }

        public static void Undo()
        {
            HistoryManager.Undo(Map);
        }

        public static void Redo()
        {
            HistoryManager.Redo(Map);
        }
    }
}
