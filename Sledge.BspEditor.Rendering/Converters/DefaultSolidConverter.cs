using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Scene;
using Sledge.Rendering.Materials;
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

        private bool ShouldBeVisible(Face face, MapDocument document, DisplayFlags displayFlags)
        {
            //if (document.Map.HideDisplacementSolids && face.Parent.Faces.Any(x => x is Displacement) && !(face is Displacement))
            //{
            //    return false;
            //}
            //return !face.IsHidden;
            return true;
        }

        public async Task<bool> Convert(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            var df = document.Map.Data.GetOne<DisplayFlags>() ?? new DisplayFlags();
            var solid = (Solid) obj;
            foreach (var face in solid.Faces.Where(x => ShouldBeVisible(x, document, df)).ToList())
            {
                var f = await ConvertFace(solid, face, document);
                smo.SceneObjects.Add(face, f);
            }

            return true;
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

            return sceneFace;
        }

        private static async Task<Material> GetMaterial(Solid solid, Face face, MapDocument document)
        {
            var c = await document.Environment.GetTextureCollection();
            var tex = await c.GetTextureItem(face.Texture.Name);

            float op;
            if (c.IsNullTexture(face.Texture.Name) && document.Map.Data.GetOne<DisplayFlags>()?.HideNullTextures == true) op = 0;
            else op = c.GetOpacity(face.Texture.Name);

            if (tex == null) return Material.Flat(Color.FromArgb((int)op * 255, solid.Color?.Color ?? Color.Red));

            var texName = $"{document.Environment.ID}::{tex.Name}";
            return op < 1
                ? Material.Texture(texName, op)
                : Material.Texture(texName, tex.Flags.HasFlag(TextureFlags.Transparent));
        }

        public async Task<bool> PropertiesChanged(SceneObjectsChangedEventArgs args, SceneMapObject smo, MapDocument document, IMapObject obj, HashSet<string> propertyNames)
        {
            var solid = (Solid) obj;
            var df = document.Map.Data.GetOne<DisplayFlags>() ?? new DisplayFlags();

            if (propertyNames.Contains("IsSelected"))
            {
                var sel = solid.IsSelected;
                foreach (var sceneFace in smo.SceneObjects.Where(x => x.Key is Face).Select(x => x.Value).OfType<SceneFace>())
                {
                    var color = solid.Color?.Color ?? Color.Green;
                    sceneFace.AccentColor = sel ? Color.Red : color;
                    sceneFace.PointColor = sel ? Color.Red : color;
                    sceneFace.TintColor = sel ? Color.FromArgb(128, Color.Red) : Color.White;
                    sceneFace.IsSelected = sel;
                    sceneFace.ForcedRenderFlags = sel ? RenderFlags.Wireframe : RenderFlags.None;
                }
            }

            if (propertyNames.Contains("Data.Face"))
            {
                var currentFaces = smo.SceneObjects.Where(x => x.Key is Face).ToDictionary(x => (Face) x.Key, x => (SceneFace) x.Value);
                foreach (var face in solid.Faces.Where(x => ShouldBeVisible(x, document, df)).ToList())
                {
                    if (currentFaces.ContainsKey(face))
                    {
                        await UpdateFace(solid, face, currentFaces[face], document);
                        currentFaces.Remove(face);
                    }
                    else
                    {
                        var f = await ConvertFace(solid, face, document);
                        smo.SceneObjects.Add(face, f);
                        args.Add(f);
                    }
                }

                foreach (var kv in currentFaces)
                {
                    smo.Remove(kv.Value);
                    args.Remove(kv.Value);
                }
            }

            return true;
        }

        private static async Task<bool> UpdateFace(Solid solid, Face face, SceneFace sceneFace, MapDocument document)
        {
            var mat = await GetMaterial(solid, face, document);

            var c = await document.Environment.GetTextureCollection();
            var tex = await c.GetTextureItem(face.Texture.Name);

            var size = tex?.Size ?? new Size(16, 16);
            var coords = face.GetTextureCoordinates(size.Width, size.Height);

            sceneFace.Material = mat;
            sceneFace.Vertices = coords.Select(x => new Vertex(x.Item1.ToVector3(), (float) x.Item2, (float) x.Item3)).ToList();
            
            return true;
        }
    }
}