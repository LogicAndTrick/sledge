using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using Sledge.Common.Mediator;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Database.Models;
using Sledge.Editor.Actions;
using Sledge.Editor.Editing;
using Sledge.Editor.History;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools;
using Sledge.Editor.UI;
using Sledge.Editor.Visgroups;
using Sledge.Graphics;
using Sledge.Graphics.Helpers;
using Sledge.Providers;
using Sledge.Providers.GameData;
using Sledge.Providers.Texture;
using Sledge.Settings;
using Sledge.UI;
using Path = System.IO.Path;

namespace Sledge.Editor.Documents
{
    public class Document
    {
        public string MapFile { get; set; }
        public Map Map { get; set; }

        public Game Game { get; set; }
        public GameData GameData { get; set; }

        public Pointfile Pointfile { get; set; }

        public RenderManager Renderer { get; private set; }

        public SelectionManager Selection { get; private set; }
        public HistoryManager History { get; private set; }

        private readonly DocumentSubscriptions _subscriptions;

        private Document()
        {
            Selection = new SelectionManager(this);
            History = new HistoryManager(this);
        }

        public Document(string mapFile, Map map, Game game)
        {
            MapFile = mapFile;
            Map = map;
            Game = game;

            _subscriptions = new DocumentSubscriptions(this);

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

            foreach (var wad in game.Wads.OrderBy(x => Path.GetFileName(x.Path)))
            {
                TexturePackage.Load(wad.Path);
            }
            var texList = Map.GetAllTextures();
            TexturePackage.LoadTextureData(texList);

            Map.PostLoadProcess(GameData, TextureHelper.Get);

            Renderer = new RenderManager(this);

            if (MapFile != null) Mediator.Publish(EditorMediator.FileOpened, MapFile);
            Mediator.Publish(EditorMediator.DocumentOpened, this);
        }

        public void SetActive()
        {
            Editor.Instance.SelectTool(ToolManager.Tools[0]); // todo keep this? cache?

            //ViewportManager.Viewports.OfType<Viewport2D>().ToList().ForEach(x => x.RenderContext.Add(new GridRenderable(this, x)));

            MapDisplayLists.RegenerateSelectLists(Selection);
            MapDisplayLists.RegenerateDisplayLists(Map.WorldSpawn.Children, false);

            Renderer.Register(ViewportManager.Viewports);

            ViewportManager.Viewports.ForEach(vp => vp.RenderContext.Add(new ToolRenderable()));
            ViewportManager.AddContext3D(new WidgetLinesRenderable());

            _subscriptions.Subscribe();

            Mediator.Publish(EditorMediator.DocumentActivated, this);
        }

        public void SetInactive()
        {
            // todo save state (camera locations, selected tool)
            ViewportManager.ClearContexts();
            MapDisplayLists.DeleteLists();

            _subscriptions.Unsubscribe();
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
            action.Perform(this);
            var history = new HistoryAction(name, action);
            History.AddHistoryItem(history);
        }

        public void StartSelectionTransform()
        {
            // todo selection transform shader
            //foreach (var dl in DisplayLists)
            //{
            //    dl.SetTintSelectListEnabled(false);
            //}
            //UpdateDisplayLists(true);
        }

        public void SetSelectListTransform(Matrix4 matrix)
        {
            Renderer.Shader.Bind();
            Renderer.Shader.Set("selectionTransform", matrix);
            Renderer.Shader.Unbind();
        }

        public void EndSelectionTransform()
        {
            Renderer.Shader.Bind();
            Renderer.Shader.Set("selectionTransform", Matrix4.Identity);
            Renderer.Shader.Unbind();
        }

        public void UpdateDisplayLists()
        {
            Map.PartialPostLoadProcess(GameData, TextureHelper.Get);
            Renderer.Update();
            ViewportManager.Viewports.ForEach(vp => vp.UpdateNextFrame());
        }

        public void UpdateDisplayLists(IEnumerable<MapObject> objects)
        {
            Map.PartialPostLoadProcess(GameData, TextureHelper.Get);
            Renderer.UpdatePartial(objects);
            ViewportManager.Viewports.ForEach(vp => vp.UpdateNextFrame());
        }

        public void UpdateDisplayLists(IEnumerable<Face> faces)
        {
            Map.PartialPostLoadProcess(GameData, TextureHelper.Get);
            Renderer.UpdatePartial(faces);
            ViewportManager.Viewports.ForEach(vp => vp.UpdateNextFrame());
        }
    }
}
