using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Sledge.Common;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Renderables;
using Sledge.Settings;
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

        private bool ShouldBeVisible(DataStructures.MapObjects.Face face, Document document)
        {
            if (document.Map.HideNullTextures)
            {
                var opac = SettingsManager.GetSpecialTextureOpacity(face.Texture.Name);
                if (opac < 0.1f) return false;
            }
            if (document.Map.HideDisplacementSolids && face.Parent.Faces.Any(x => x is Displacement) && !(face is Displacement))
            {
                return false;
            }
            return !face.IsHidden;
        }

        public async Task<bool> Convert(SceneMapObject smo, Document document, MapObject obj)
        {
            var solid = (Solid)obj;
            foreach (var face in solid.Faces.Where(x => ShouldBeVisible(x, document)))
            {
                var f = await ConvertFace(face, document);
                smo.SceneObjects.Add(face, f);
            }
            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, Document document, MapObject obj)
        {
            var solid = (Solid)obj;
            var faces = solid.Faces.Where(x => ShouldBeVisible(x, document)).ToList();
            var values = smo.SceneObjects.Where(x => x.Key is Face).Select(x => x.Value).ToList();
            if (values.Count != faces.Count) return false;

            var objs = new Dictionary<object, SceneObject>();
            for (var i = 0; i < faces.Count; i++)
            {
                var face = faces[i];
                if (!await UpdateFace(face, (Face)values[i], document)) return false;
                objs.Add(face, values[i]);
            }
            smo.SceneObjects.Clear();
            foreach (var kv in objs) smo.SceneObjects.Add(kv.Key, kv.Value);
            return true;
        }

        private static async Task<Material> GetMaterial(DataStructures.MapObjects.Face face, Document document)
        {
            var tex = await document.TextureCollection.GetTextureItem(face.Texture.Name);
            var op = SettingsManager.GetSpecialTextureOpacity(face.Texture.Name);
            if (op < 0.1 && !document.Map.HideNullTextures) op = 1;

            if (tex == null) return Material.Flat(Color.FromArgb((int)(op * 255), face.Colour));

            return op < 1
                ? Material.Texture(tex.Name, op)
                : Material.Texture(tex.Name, tex.Flags.HasFlag(TextureFlags.Transparent));
        }

        public static async Task<Face> ConvertFace(DataStructures.MapObjects.Face face, Document document)
        {
            var mat = await GetMaterial(face, document);

            var sel = face.IsSelected || (face.Parent != null && face.Parent.IsSelected);

            var size = document.GetTextureSize(face.Texture.Name);
            var coords = face.GetTextureCoordinates(size.Width, size.Height);
            var sceneFace =  new Face(mat, coords.Select(x => new Vertex(x.Item1.Location.ToVector3(), (float)x.Item2, (float)x.Item3)).ToList())
            {
                AccentColor = sel ? Color.Red : face.Colour,
                PointColor = sel ? Color.Red : (View.OverrideVertexColour ? View.VertexOverrideColour : face.Colour),
                TintColor = sel ? Color.FromArgb(128, Color.Red) : Color.White,
                IsSelected = sel,
                ForcedRenderFlags = sel ? RenderFlags.Wireframe : RenderFlags.None
            };

            if (View.Draw2DVertices)
            {
                sceneFace.RenderFlags |= RenderFlags.Point;
            }

            if (document.Map.HideFaceMask && face.IsSelected)
            {
                sceneFace.TintColor = Color.White;
                sceneFace.AccentColor = Color.Yellow;
            }

            return sceneFace;
        }

        public static async Task<bool> UpdateFace(DataStructures.MapObjects.Face face, Face sceneFace, Document document)
        {
            var mat = await GetMaterial(face, document);

            var sel = face.IsSelected || (face.Parent != null && face.Parent.IsSelected);

            var size = document.GetTextureSize(face.Texture.Name);
            var coords = face.GetTextureCoordinates(size.Width, size.Height);

            sceneFace.Material = mat;
            sceneFace.Vertices = coords.Select(x => new Vertex(x.Item1.Location.ToVector3(), (float)x.Item2, (float)x.Item3)).ToList();
            sceneFace.AccentColor = sel ? Color.Red : face.Colour;
            sceneFace.PointColor = sel ? Color.Red : (View.OverrideVertexColour ? View.VertexOverrideColour : face.Colour);
            sceneFace.TintColor = sel ? Color.FromArgb(128, Color.Red) : Color.White;
            sceneFace.IsSelected = sel;
            sceneFace.ForcedRenderFlags = sel ? RenderFlags.Wireframe : RenderFlags.None;
            sceneFace.RenderFlags = RenderFlags.Polygon | RenderFlags.Wireframe;

            if (View.Draw2DVertices)
            {
                sceneFace.RenderFlags |= RenderFlags.Point;
            }

            if (document.Map.HideFaceMask && face.IsSelected)
            {
                sceneFace.TintColor = Color.White;
                sceneFace.AccentColor = Color.Yellow;
            }

            return true;
        }
    }
}