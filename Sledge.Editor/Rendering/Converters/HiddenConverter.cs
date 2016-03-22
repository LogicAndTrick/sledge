using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Rendering.Converters
{
    public class HiddenConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority { get { return MapObjectSceneConverterPriority.OverrideLowest; } }

        public bool ShouldStopProcessing(SceneMapObject smo, MapObject obj)
        {
            return true;
        }

        public bool Supports(MapObject obj)
        {
            return obj.IsCodeHidden || obj.IsVisgroupHidden;
        }

        public bool Convert(SceneMapObject smo, Document document, MapObject obj)
        {
            return false;
        }

        public bool Update(SceneMapObject smo, Document document, MapObject obj)
        {
            return false;
        }
    }
}