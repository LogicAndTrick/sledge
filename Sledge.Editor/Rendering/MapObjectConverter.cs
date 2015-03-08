using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes;
using Face = Sledge.Rendering.Scenes.Renderables.Face;
using Vertex = Sledge.Rendering.Scenes.Renderables.Vertex;

namespace Sledge.Editor.Rendering
{
    public static class MapObjectConverter
    {
        public static SceneMapObject Convert(this MapObject obj, Document document)
        {
            if (obj is Solid) return Convert((Solid)obj, document);
            if (obj is Entity) return Convert((Entity) obj, document);
            return null;
        }

        public static bool Update(this MapObject obj, SceneMapObject smo, Document document)
        {
            if (obj is Solid) return Update((Solid)obj, smo, document);
            if (obj is Entity) return Update((Entity)obj, smo, document);
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

        public static SceneMapObject Convert(this Entity entity, Document document)
        {
            var smo = new SceneMapObject(entity);
            if (entity.EntityData.Name == "infodecal")
            {
                // 
            }
            else if (false) // should have model
            {
                
            }
            else if (false) // should have sprite
            {

            }
            else
            {
                
            }
            foreach (var face in entity.GetBoxFaces())
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
                if (!Update(face, (Face)values[i], document)) return false;
                objs.Add(face, values[i]);
            }
            smo.SceneObjects.Clear();
            foreach (var kv in objs) smo.SceneObjects.Add(kv.Key, kv.Value);
            return true;
        }

        public static bool Update(this Entity entity, SceneMapObject smo, Document document)
        {
            return false;
            // if (smo.SceneObjects.Count != entity.Faces.Count) return false;
            // var values = smo.SceneObjects.Values.ToList();
            // var objs = new Dictionary<object, SceneObject>();
            // for (int i = 0; i < entity.Faces.Count; i++)
            // {
            //     var face = entity.Faces[i];
            //     if (!Update(face, (Face)values[i], document)) return false;
            //     objs.Add(face, values[i]);
            // }
            // smo.SceneObjects.Clear();
            // foreach (var kv in objs) smo.SceneObjects.Add(kv.Key, kv.Value);
            // return true;
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