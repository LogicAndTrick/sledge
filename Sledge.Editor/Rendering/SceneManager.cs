using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using Poly2Tri;
using Sledge.Common;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Rendering;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Materials;
using Sledge.Rendering.OpenGL;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Rendering.Scenes.Renderables;
using Face = Sledge.Rendering.Scenes.Renderables.Face;
using Vertex = Sledge.Rendering.Scenes.Renderables.Vertex;

namespace Sledge.Editor.Rendering
{
    public class SceneMapObject : IEnumerable<SceneObject>
    {
        public MapObject MapObject { get; set; }
        public Dictionary<object, SceneObject> SceneObjects { get; private set; }

        public SceneMapObject(MapObject mapObject)
        {
            MapObject = mapObject;
            SceneObjects = new Dictionary<object, SceneObject>();
        }

        public IEnumerator<SceneObject> GetEnumerator()
        {
            return SceneObjects.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

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

    public static class MapObjectConverter
    {
        public static SceneMapObject Convert(this MapObject obj, Document document)
        {
            if (obj is Solid) return Convert((Solid)obj, document);
            return null;
        }

        public static bool Update(this MapObject obj, SceneMapObject smo, Document document)
        {
            if (obj is Solid) return Update((Solid)obj, smo, document);
            return false;
        }

        public static SceneMapObject Convert(this Solid solid, Document document)
        {
            var smo = new SceneMapObject(solid);
            foreach (var face in solid.Faces)
            {
                var f = Convert(face, document);
                smo.SceneObjects.Add(face, f);
            }
            return smo;
        }

        public static bool Update(this Solid solid, SceneMapObject smo, Document document)
        {
            if (smo.SceneObjects.Count != solid.Faces.Count) return false;
            var values = smo.SceneObjects.Values.ToList();
            var objs = new Dictionary<object, SceneObject>();
            for (int i = 0; i < solid.Faces.Count; i++)
            {
                var face = solid.Faces[i];
                if (!Update(face, (Face) values[i], document)) return false;
                objs.Add(face, values[i]);
            }
            smo.SceneObjects.Clear();
            foreach (var kv in objs) smo.SceneObjects.Add(kv.Key, kv.Value);
            return true;
        }

        public static Face Convert(this DataStructures.MapObjects.Face face, Document document)
        {
            var tex = document.TextureCollection.GetItem(face.Texture.Name);
            var mat = tex == null ? Material.Flat(face.Colour) : Material.Texture(tex.Name);
            var sel = face.IsSelected || (face.Parent != null && face.Parent.IsSelected);
            return new Face(mat, face.Vertices.Select(x => new Vertex(x.Location.ToVector3(), (float)x.TextureU, (float)x.TextureV)).ToList())
            {
                AccentColor = sel ? Color.Red : face.Colour,
                TintColor = sel ? Color.FromArgb(128, Color.Red) : Color.White,
                IsSelected = sel
            };
        }

        public static bool Update(this DataStructures.MapObjects.Face face, Face sceneFace, Document document)
        {
            var tex = document.TextureCollection.GetItem(face.Texture.Name);
            var mat = tex == null ? Material.Flat(face.Colour) : Material.Texture(tex.Name);
            var sel = face.IsSelected || (face.Parent != null && face.Parent.IsSelected);

            sceneFace.Material = mat;
            sceneFace.Vertices = face.Vertices.Select(x => new Vertex(x.Location.ToVector3(), (float) x.TextureU, (float) x.TextureV)).ToList();
            sceneFace.AccentColor = sel ? Color.Red : face.Colour;
            sceneFace.TintColor = sel ? Color.FromArgb(128, Color.Red) : Color.White;
            sceneFace.IsSelected = sel;

            return true;
        }
    }
}
