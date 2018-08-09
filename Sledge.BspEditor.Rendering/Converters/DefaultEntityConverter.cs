using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Scene;
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

        public async Task<bool> Convert(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            var flags = CameraFlags.All;
            if (smo.MetaData.ContainsKey("ContentsReplaced")) flags = CameraFlags.Orthographic;

            var entity = (Entity) obj;
            foreach (var face in entity.BoundingBox.GetBoxFaces())
            {
                var sel = entity.IsSelected;
                var color = entity.Color?.Color ?? Color.Green;
                var mat = Material.Flat(color);

                var f = new Face(mat, face.Select(x => new Vertex(x.ToVector3(), 0, 0)).ToList())
                {
                    AccentColor = sel ? Color.Red : color,
                    PointColor = sel ? Color.Red : color,
                    TintColor = sel ? Color.FromArgb(128, Color.Red) : Color.White,
                    IsSelected = sel,
                    ForcedRenderFlags = sel ? RenderFlags.Wireframe : RenderFlags.None,
                    RenderFlags = RenderFlags.Polygon | RenderFlags.Wireframe,
                    CameraFlags = flags
                };

                smo.SceneObjects.Add(face, f);
            }

            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return false;
        }
    }
}