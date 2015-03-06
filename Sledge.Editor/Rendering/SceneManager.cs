using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using Poly2Tri;
using Sledge.Common;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Rendering;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.OpenGL;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Rendering.Scenes.Renderables;

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

        private Dictionary<MapObject, SceneMapObject> _sceneObjects;

        public SceneManager(Document document)
        {
            Document = document;
            _sceneObjects = new Dictionary<MapObject, SceneMapObject>();

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

        public void Update()
        {
            // todo delete...
            Update(Document.Map.WorldSpawn.FindAll());
        }

        public void Update(IEnumerable<MapObject> objects)
        {
            foreach (var obj in objects)
            {
                var smo = _sceneObjects.ContainsKey(obj) ? _sceneObjects[obj] : null;

                if (smo != null && obj.Update(smo, Document)) continue;

                if (smo != null)
                {
                    var rem = _sceneObjects[obj];
                    foreach (var r in rem) Scene.Remove(r);
                }

                smo = obj.Convert(Document);
                if (smo == null) continue;

                foreach (var ro in smo) Scene.Add(ro);
                _sceneObjects[smo.MapObject] = smo;
            }
        }
    }

    public class DefaultTextureProvider : ITextureProvider
    {
        public Document Document { get; private set; }

        public DefaultTextureProvider(Document document)
        {
            Document = document;
        }

        public bool Exists(string name)
        {
            return Document.TextureCollection.GetItem(name) != null;
        }

        public TextureDetails Fetch(string name)
        {
            return Fetch(new[] {name}).FirstOrDefault();
        }

        public IEnumerable<TextureDetails> Fetch(IEnumerable<string> names)
        {
            using (var ss = Document.TextureCollection.GetStreamSource(1024, 1024))
            {
                foreach (var item in names.Select(name => Document.TextureCollection.GetItem(name)).Where(item => item != null))
                {
                    yield return new TextureDetails(item.Name, ss.GetImage(item), item.Width, item.Height, item.Flags);
                }
            }
        }
    }
}
