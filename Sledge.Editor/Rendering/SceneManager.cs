using System.Collections.Generic;
using System.Linq;
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
        }

        public void SetActive()
        {
            Engine.Renderer.SetActiveScene(Scene);

            // Clear out any temporary objects
            foreach (var key in _temporaryObjects.Keys.ToList())
            {
                ClearTemporaryObjects(key);
            }
        }

        private void Process(IList<MapObject> objects)
        {
            // todo: how much can be moved out of the post load process now that some of it should be offloaded to the renderer?
            Document.Map.PartialPostLoadProcess(objects, Document.GameData, SettingsManager.GetSpecialTextureOpacity);
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

        private readonly Dictionary<object, List<SceneObject>> _temporaryObjects = new Dictionary<object, List<SceneObject>>();

        public void AddTemporaryObject(object owner, SceneObject sceneObject)
        {
            if (!_temporaryObjects.ContainsKey(owner)) _temporaryObjects[owner] = new List<SceneObject>();
            _temporaryObjects[owner].Add(sceneObject);
            Document.Scene.Add(sceneObject);
        }

        public void ClearTemporaryObjects(object owner)
        {
            if (_temporaryObjects.ContainsKey(owner))
            {
                foreach (var sceneObject in _temporaryObjects[owner])
                {
                    Document.Scene.Remove(sceneObject);
                }
                _temporaryObjects[owner].Clear();
            }
            _temporaryObjects.Remove(owner);
        }
    }
}
