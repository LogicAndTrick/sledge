using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Scene;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.BspEditor.Rendering.Converters
{
    [Export(typeof(IMapObjectSceneConverter))]
    public class DefaultEntityConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.DefaultLowest;

        public bool ShouldStopProcessing(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return false;
        }

        public bool Supports(IMapObject obj)
        {
            return obj is Entity && !obj.Hierarchy.HasChildren;
        }

        public Task<bool> Convert(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            var flags = CameraFlags.All;
            if (smo.MetaData.ContainsKey("ContentsReplaced")) flags = CameraFlags.Orthographic;

            var entity = (Entity) obj;
            var sel = entity.IsSelected;
            var color = entity.Color?.Color ?? Color.Green;
            var mat = Material.Flat(color);

            foreach (var face in entity.BoundingBox.GetBoxFaces())
            {
                var f = ConvertFace(flags, sel, color, mat, face);
                smo.SceneObjects.Add(face, f);
            }

            return Task.FromResult(true);
        }

        private static Face ConvertFace(CameraFlags flags, bool sel, Color color, Material mat, IEnumerable<Coordinate> face)
        {
            return new Face(mat, face.Select(x => new Vertex(x.ToVector3(), 0, 0)).ToList())
            {
                AccentColor = sel ? Color.Red : color,
                PointColor = sel ? Color.Red : color,
                TintColor = sel ? Color.FromArgb(128, Color.Red) : Color.White,
                IsSelected = sel,
                ForcedRenderFlags = sel ? RenderFlags.Wireframe : RenderFlags.None,
                RenderFlags = RenderFlags.Polygon | RenderFlags.Wireframe,
                CameraFlags = flags
            };
        }

        public Task<bool> PropertiesChanged(SceneObjectsChangedEventArgs args, SceneMapObject smo, MapDocument document, IMapObject obj, HashSet<string> propertyNames)
        {
            var entity = (Entity) obj;
            
            if (propertyNames.Contains("Data.Origin") || propertyNames.Contains("Data.EntityData"))
            {
                var flags = CameraFlags.All;
                if (smo.MetaData.ContainsKey("ContentsReplaced")) flags = CameraFlags.Orthographic;

                var sel = entity.IsSelected;
                var color = entity.Color?.Color ?? Color.Green;
                var mat = Material.Flat(color);

                foreach (var so in smo.SceneObjects.Values.OfType<Face>().ToList())
                {
                    smo.Remove(so);
                    args.Remove(so);
                }

                foreach (var face in entity.BoundingBox.GetBoxFaces())
                {
                    var f = ConvertFace(flags, sel, color, mat, face);
                    smo.SceneObjects.Add(face, f);
                    args.Add(f);
                }
            }
            else if (propertyNames.Contains("IsSelected"))
            {
                var sel = entity.IsSelected;
                foreach (var sceneFace in smo.SceneObjects.Values.OfType<Face>())
                {
                    var color = entity.Color?.Color ?? Color.Green;
                    sceneFace.AccentColor = sel ? Color.Red : color;
                    sceneFace.PointColor = sel ? Color.Red : color;
                    sceneFace.TintColor = sel ? Color.FromArgb(128, Color.Red) : Color.White;
                    sceneFace.IsSelected = sel;
                    sceneFace.ForcedRenderFlags = sel ? RenderFlags.Wireframe : RenderFlags.None;
                }
            }

            return Task.FromResult(true);
        }
    }
}