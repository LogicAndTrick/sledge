using System.Drawing;
using System.Threading.Tasks;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Line = Sledge.Rendering.Scenes.Renderables.Line;

namespace Sledge.Editor.Rendering.Converters
{
    public class CordonConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority { get { return MapObjectSceneConverterPriority.OverrideLow; } }

        public bool ShouldStopProcessing(SceneMapObject smo, MapObject obj)
        {
            return false;
        }

        public bool Supports(MapObject obj)
        {
            return obj is World;
        }

        public async Task<bool> Convert(SceneMapObject smo, Document document, MapObject obj)
        {
            if (!document.Map.Cordon) return true;
            foreach (var line in document.Map.CordonBounds.GetBoxLines())
            {
                smo.SceneObjects.Add(new object(), new Line(Color.Red, line.Start.ToVector3(), line.End.ToVector3()));
            }
            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, Document document, MapObject obj)
        {
            return false;
        }
    }
}