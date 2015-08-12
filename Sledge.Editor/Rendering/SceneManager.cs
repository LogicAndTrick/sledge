using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Rendering;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.OpenGL;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Rendering.Scenes.Renderables;
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
        }

        public Document Document { get; private set; }
        public Scene Scene { get { return Document.Scene; } }
        private readonly ConvertedScene _convertedScene;

        public SceneManager(Document document)
        {
            Document = document;
            _convertedScene = new ConvertedScene(document);

            Engine.Renderer.TextureProviders.Add(new DefaultTextureProvider(document));

            AddAxisLines();

            Update();
        }

        private void AddAxisLines()
        {
            Scene.Add(new Line(Color.FromArgb(255, Color.Red), Vector3.Zero, Vector3.UnitX * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });
            Scene.Add(new Line(Color.FromArgb(255, Color.Lime), Vector3.Zero, Vector3.UnitY * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });
            Scene.Add(new Line(Color.FromArgb(255, Color.Blue), Vector3.Zero, Vector3.UnitZ * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });

            Scene.Add(new TextElement(PositionType.Screen, new Vector3(0, 0, 0), "Viewport", Color.White) { AnchorX = 0, AnchorY = 0, BackgroundColor = Color.FromArgb(128, Color.Red) });
        }

        public void SetActive()
        {
            Engine.Renderer.SetActiveScene(Scene);
        }

        private void Process(IList<MapObject> objects)
        {
            // todo: how much can be moved out of the post load process now that some of it should be offloaded to the renderer?
            Document.Map.PartialPostLoadProcess(Document.GameData, Document.GetTexture, SettingsManager.GetSpecialTextureOpacity);
        }

        public void Update()
        {
            Process(new MapObject[] { Document.Map.WorldSpawn });
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
