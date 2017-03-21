using System.Threading.Tasks;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Editor.Rendering.Converters
{
    public class DefaultEntityConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority { get { return MapObjectSceneConverterPriority.DefaultLowest; } }
        public bool ShouldStopProcessing(SceneMapObject smo, MapObject obj)
        {
            return false;
        }

        public bool Supports(MapObject obj)
        {
            return obj is Entity;
        }

        public async Task<bool> Convert(SceneMapObject smo, Document document, MapObject obj)
        {
            var flags = CameraFlags.All;
            if (smo.MetaData.ContainsKey("ContentsReplaced")) flags = CameraFlags.Orthographic;

            var entity = (Entity) obj;
            foreach (var face in entity.GetBoxFaces())
            {
                var f = await DefaultSolidConverter.ConvertFace(face, document);
                f.RenderFlags = RenderFlags.Polygon | RenderFlags.Wireframe;
                f.CameraFlags = flags;
                smo.SceneObjects.Add(face, f);
            }
            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, Document document, MapObject obj)
        {
            return false;
        }
    }
}