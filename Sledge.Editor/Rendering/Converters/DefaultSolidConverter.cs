using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sledge.Common;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes;
using Face = Sledge.Rendering.Scenes.Renderables.Face;
using Vertex = Sledge.Rendering.Scenes.Renderables.Vertex;

namespace Sledge.Editor.Rendering.Converters
{
    public class DefaultSolidConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority { get { return MapObjectSceneConverterPriority.DefaultLowest; } }

        public bool ShouldStopProcessing(SceneMapObject smo, MapObject obj)
        {
            return false;
        }

        public bool Supports(MapObject obj)
        {
            return obj is Solid;
        }

        public bool Convert(SceneMapObject smo, Document document, MapObject obj)
        {
            var solid = (Solid)obj;
            foreach (var face in solid.Faces.Where(x => !x.IsHidden))
            {
                var f = ConvertFace(face, document);
                smo.SceneObjects.Add(face, f);
            }
            return true;
        }

        public bool Update(SceneMapObject smo, Document document, MapObject obj)
        {
            var solid = (Solid)obj;
            if (smo.SceneObjects.Count != solid.Faces.Count(x => !x.IsHidden)) return false;
            var values = smo.SceneObjects.Values.ToList();
            var objs = new Dictionary<object, SceneObject>();
            for (var i = 0; i < solid.Faces.Count; i++)
            {
                var face = solid.Faces[i];
                if (!UpdateFace(face, (Face)values[i], document)) return false;
                objs.Add(face, values[i]);
            }
            smo.SceneObjects.Clear();
            foreach (var kv in objs) smo.SceneObjects.Add(kv.Key, kv.Value);
            return true;
        }

        public static Face ConvertFace(DataStructures.MapObjects.Face face, Document document)
        {
            var tex = document.TextureCollection.GetItem(face.Texture.Name);
            var mat = tex == null ? Material.Flat(face.Colour) : Material.Texture(tex.Name, tex.Flags.HasFlag(TextureFlags.Transparent));
            var sel = face.IsSelected || (face.Parent != null && face.Parent.IsSelected);
            return new Face(mat, face.Vertices.Select(x => new Vertex(x.Location.ToVector3(), (float)x.TextureU, (float)x.TextureV)).ToList())
            {
                AccentColor = sel ? Color.Red : face.Colour,
                TintColor = sel ? Color.FromArgb(128, Color.Red) : Color.White,
                IsSelected = sel
            };
        }

        public static bool UpdateFace(DataStructures.MapObjects.Face face, Face sceneFace, Document document)
        {
            var tex = document.TextureCollection.GetItem(face.Texture.Name);
            var mat = tex == null ? Material.Flat(face.Colour) : Material.Texture(tex.Name, tex.Flags.HasFlag(TextureFlags.Transparent));
            var sel = face.IsSelected || (face.Parent != null && face.Parent.IsSelected);

            sceneFace.Material = mat;
            sceneFace.Vertices = face.Vertices.Select(x => new Vertex(x.Location.ToVector3(), (float)x.TextureU, (float)x.TextureV)).ToList();
            sceneFace.AccentColor = sel ? Color.Red : face.Colour;
            sceneFace.TintColor = sel ? Color.FromArgb(128, Color.Red) : Color.White;
            sceneFace.IsSelected = sel;

            return true;
        }
    }
}