using System.Drawing;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.DataStructures.MapObjects;
using Line = Sledge.Rendering.Scenes.Renderables.Line;

namespace Sledge.BspEditor.Rendering.Converters
{
    public class CordonConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority { get { return MapObjectSceneConverterPriority.OverrideLow; } }

        public bool ShouldStopProcessing(SceneMapObject smo, IMapObject obj)
        {
            return false;
        }

        public bool Supports(IMapObject obj)
        {
            return obj is World;
        }

        public async Task<bool> Convert(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            if (!document.Map.Cordon) return true;
            foreach (var line in document.Map.CordonBounds.GetBoxLines())
            {
                smo.SceneObjects.Add(new object(), new Line(Color.Red, line.Start.ToVector3(), line.End.ToVector3()));
            }
            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return false;
        }
    }
}