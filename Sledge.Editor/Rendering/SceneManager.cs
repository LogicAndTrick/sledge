using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Rendering;
using Sledge.Rendering.OpenGL;
using Sledge.Rendering.Scenes;
using Sledge.Settings;

namespace Sledge.Editor.Rendering
{
    public class SceneManager
    {
        public static Engine Engine { get; private set; }

        static SceneManager()
        {
            var renderer = new OpenGLRenderer();
            Engine = new Engine(renderer);
            UpdateRendererSettings();
        }

        public static void UpdateRendererSettings()
        {
            var settings = Engine.Renderer.Settings;
            settings.DisableTextureTransparency = Sledge.Settings.View.GloballyDisableTransparency;
            settings.DisableTextureFiltering = Sledge.Settings.View.DisableTextureFiltering;
            settings.ForcePowerOfTwoTextureSizes = Sledge.Settings.View.ForcePowerOfTwoTextureResizing;
            settings.PerspectiveBackgroundColour = Sledge.Settings.View.ViewportBackground;
            settings.OrthographicBackgroundColour = Sledge.Settings.Grid.Background;
            settings.PointSize = Sledge.Settings.View.VertexPointSize;
        }

        public Document Document { get; private set; }
        public Scene Scene { get { return Document.Scene; } }
        private readonly ConvertedScene _convertedScene;

        public SceneManager(Document document)
        {
            Document = document;
            _convertedScene = new ConvertedScene(document);

            Engine.Renderer.TextureProviders.Add(new DefaultTextureProvider(document));
            Engine.Renderer.ModelProviders.Add(new DefaultModelProvider(document));

            Update();
        }

        public void SetActive()
        {
            Engine.Renderer.SetActiveScene(Scene);
        }

        private void Process(IList<MapObject> objects)
        {
            // todo: how much can be moved out of the post load process now that some of it should be offloaded to the renderer?
            Document.Map.PartialPostLoadProcess(objects, Document.GameData, Document.GetTextureSize, SettingsManager.GetSpecialTextureOpacity);
        }

        public void Update()
        {
            Process(Document.Map.WorldSpawn.FindAll());
            _convertedScene.UpdateAll();
        }

        public void Create(IList<MapObject> objects)
        {
            Process(objects);
            _convertedScene.Create(objects);
        }

        public void Delete(IEnumerable<MapObject> objects)
        {
            _convertedScene.Delete(objects);
        }

        public void Update(IList<MapObject> objects)
        {
            Process(objects);
            _convertedScene.Update(objects);
        }
    }
}
