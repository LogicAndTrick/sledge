using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.DataStructures.MapObjects;

namespace Sledge.BspEditor.Rendering.Converters
{
    public class HiddenConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority { get { return MapObjectSceneConverterPriority.OverrideLowest; } }

        public bool ShouldStopProcessing(SceneMapObject smo, IMapObject obj)
        {
            return true;
        }

        public bool Supports(IMapObject obj)
        {
            return obj.IsCodeHidden || obj.IsVisgroupHidden;
        }

        public async Task<bool> Convert(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return false;
        }

        public async Task<bool> Update(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return false;
        }
    }
}