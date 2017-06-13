using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Scene;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Renderables;
using Face = Sledge.BspEditor.Primitives.MapObjectData.Face;
using SceneFace = Sledge.Rendering.Scenes.Renderables.Face;

namespace Sledge.BspEditor.Rendering.Converters
{
    [Export(typeof(IMapObjectSceneConverter))]
    public class DefaultSolidConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.DefaultLowest;

        public bool ShouldStopProcessing(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return false;
        }

        public bool Supports(IMapObject obj)
        {
            return obj is Solid;
        }

        private bool ShouldBeVisible(Face face, MapDocument document)
        {
            //if (document.Map.HideNullTextures)
            //{
            //    var opac = SettingsManager.GetSpecialTextureOpacity(face.Texture.Name);
            //    if (opac < 0.1f) return false;
            //}
            //if (document.Map.HideDisplacementSolids && face.Parent.Faces.Any(x => x is Displacement) && !(face is Displacement))
            //{
            //    return false;
            //}
            //return !face.IsHidden;
            return true;
        }

        public async Task<bool> Convert(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            var solid = (Solid) obj;
            foreach (var face in solid.Faces.Where(x => ShouldBeVisible(x, document)).ToList())
            {
                var f = await ConvertFace(solid, face, document);
                smo.SceneObjects.Add(face, f);
            }
            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            var solid = (Solid) obj;
            var faces = solid.Faces.Where(x => ShouldBeVisible(x, document)).ToList();
            var values = smo.SceneObjects.Where(x => x.Key is Face).Select(x => x.Value).ToList();
            if (values.Count != faces.Count) return false;

            var objs = new Dictionary<object, SceneObject>();
            for (var i = 0; i < faces.Count; i++)
            {
                var face = faces[i];
                if (!await UpdateFace(solid, face, (SceneFace) values[i], document)) return false;
                objs.Add(face, values[i]);
            }
            smo.SceneObjects.Clear();
            foreach (var kv in objs) smo.SceneObjects.Add(kv.Key, kv.Value);
            return true;
        }

        private static async Task<Material> GetMaterial(Solid solid, Face face, MapDocument document)
        {
            var c = await document.Environment.GetTextureCollection();
            var tex = await c.GetTextureItem(face.Texture.Name);
            
            //    var op = SettingsManager.GetSpecialTextureOpacity(face.Texture.Name);
            //    if (op < 0.1 && !document.Map.HideNullTextures) op = 1;

            // todo !solid converter texture opacity
            // if (tex == null) return Material.Flat(Color.FromArgb((int)(op * 255), face.Colour));

            if (tex == null) return Material.Flat(solid.Color?.Color ?? Color.Red);
            return Material.Texture($"{document.Environment.ID}::{tex.Name}", tex.Flags.HasFlag(TextureFlags.Transparent));

            //    return op < 1
            //        ? Material.Texture(tex.Name, op)
            //        : Material.Texture(tex.Name, tex.Flags.HasFlag(TextureFlags.Transparent));
        }

        public static async Task<SceneFace> ConvertFace(Solid solid, Face face, MapDocument document)
        {
            var mat = await GetMaterial(solid, face, document);

            var sel = solid.IsSelected;

            var color = solid.Color?.Color ?? Color.Green;

            var c = await document.Environment.GetTextureCollection();
            var tex = await c.GetTextureItem(face.Texture.Name);

            var size = tex?.Size ?? new Size(16, 16);
            var coords = face.GetTextureCoordinates(size.Width, size.Height);

            var sceneFace = new SceneFace(mat, coords.Select(x => new Vertex(x.Item1.ToVector3(), (float) x.Item2, (float) x.Item3)).ToList())
            {
                AccentColor = sel ? Color.Red : color,
                PointColor = sel ? Color.Red : color,
                TintColor = sel ? Color.FromArgb(128, Color.Red) : Color.White,
                IsSelected = sel,
                ForcedRenderFlags = sel ? RenderFlags.Wireframe : RenderFlags.None
            };

            //if (View.Draw2DVertices)
            //{
            //    sceneFace.RenderFlags |= RenderFlags.Point;
            //}

            //if (document.Map.HideFaceMask && face.IsSelected)
            //{
            //    sceneFace.TintColor = Color.White;
            //    sceneFace.AccentColor = Color.Yellow;
            //}

            return sceneFace;
        }

        public static async Task<bool> UpdateFace(Solid solid, Face face, SceneFace sceneFace, MapDocument document)
        {
            var mat = await GetMaterial(solid, face, document);

            var sel = solid.IsSelected;

            var color = solid.Color?.Color ?? Color.Green;

            var c = await document.Environment.GetTextureCollection();
            var tex = await c.GetTextureItem(face.Texture.Name);

            var size = tex?.Size ?? new Size(16, 16);
            var coords = face.GetTextureCoordinates(size.Width, size.Height);

            sceneFace.Material = mat;
            sceneFace.Vertices = coords.Select(x => new Vertex(x.Item1.ToVector3(), (float) x.Item2, (float) x.Item3)).ToList();
            sceneFace.AccentColor = sel ? Color.Red : color;
            sceneFace.PointColor = sel ? Color.Red : color;
            sceneFace.TintColor = sel ? Color.FromArgb(128, Color.Red) : Color.White;
            sceneFace.IsSelected = sel;
            sceneFace.ForcedRenderFlags = sel ? RenderFlags.Wireframe : RenderFlags.None;
            sceneFace.RenderFlags = RenderFlags.Polygon | RenderFlags.Wireframe;

            //if (View.Draw2DVertices)
            //{
            //    sceneFace.RenderFlags |= RenderFlags.Point;
            //}

            //if (document.Map.HideFaceMask && face.IsSelected)
            //{
            //    sceneFace.TintColor = Color.White;
            //    sceneFace.AccentColor = Color.Yellow;
            //}

            return true;
        }
    }
}