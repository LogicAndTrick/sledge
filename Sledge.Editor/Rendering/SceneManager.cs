using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using Sledge.Common;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Rendering;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Materials;
using Sledge.Rendering.OpenGL;
using Sledge.Rendering.Scenes;
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

        private class SceneMapObject
        {
             
        }

        public Document Document { get; private set; }
        public Scene Scene { get { return Document.Scene; } }

        private Dictionary<Sledge.DataStructures.MapObjects.MapObject, SceneMapObject> _sceneObjects;

        public SceneManager(Document document)
        {
            Document = document;
            _sceneObjects = new Dictionary<Sledge.DataStructures.MapObjects.MapObject, SceneMapObject>();

            AddAxisLines();

            Update();
        }

        private void AddAxisLines()
        {
            Scene.Add(new Line(Color.FromArgb(255, Color.Red), Vector3.Zero, Vector3.UnitX * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });
            Scene.Add(new Line(Color.FromArgb(255, Color.Lime), Vector3.Zero, Vector3.UnitY * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });
            Scene.Add(new Line(Color.FromArgb(255, Color.Blue), Vector3.Zero, Vector3.UnitZ * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });
        }

        public void SetActive()
        {
            Engine.Renderer.SetActiveScene(Scene);
        }

        private void Update()
        {
            using (var ss = Document.TextureCollection.GetStreamSource(512, 512))
            {
                var all = Document.Map.WorldSpawn.FindAll();
                foreach (var solid in all.OfType<Sledge.DataStructures.MapObjects.Solid>())
                {
                    foreach (var face in solid.Faces)
                    {
                        var tex = Document.TextureCollection.GetItem(face.Texture.Name);
                        var mat = tex == null ? Material.Flat(face.Colour) : Material.Texture(tex.Name);
                        if (tex != null && !Engine.Renderer.Textures.Exists(tex.Name))
                        {
                            Engine.Renderer.Textures.Create(tex.Name, ss.GetImage(tex), tex.Width, tex.Height, TextureFlags.None);
                        }
                        Engine.Renderer.Materials.Add(mat);
                        var f = new Face(mat, face.Vertices.Select(x => new Vertex(x.Location.ToVector3(), (float) x.TextureU, (float) x.TextureV)).ToList())
                                {
                                    AccentColor = face.Colour,
                                    TintColor = Color.White
                                };
                        Scene.Add(f);
                    }
                }
            }
        }
    }
}
