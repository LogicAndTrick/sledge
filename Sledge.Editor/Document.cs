using System.Linq;
using OpenTK;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.UI;
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

            Editor.Instance.SelectTool(ToolManager.Tools[0]);

            try
            {
                GameData = GameDataProvider.GetGameDataFromFiles(game.Fgds.Select(f => f.Path));
            }
            catch(ProviderException)
            {
                // TODO: Error logging
                GameData = new GameData();
            }

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
    }
}
