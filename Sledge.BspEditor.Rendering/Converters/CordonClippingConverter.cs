using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Scene;

namespace Sledge.BspEditor.Rendering.Converters
{
    // TODO: Not sure if it's best to do this as a hidden operation
    //       or as a custom converter like it is now. Using a hidden
    //       operation means that each tool doesn't have to know about
    //       cordon, just the normal IHidden interface.
    [Export(typeof(IMapObjectSceneConverter))]
    public class CordonClippingConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.OverrideMedium;

        private CordonBounds GetCordon(MapDocument doc)
        {
            return doc.Map.Data.GetOne<CordonBounds>() ?? new CordonBounds {Enabled = false};
        }

        public bool ShouldStopProcessing(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            // If cordon is enabled and the cordon doesn't intersect our bbox, then we should stop
            var c = GetCordon(document);
            return c.Enabled && !obj.BoundingBox.IntersectsWith(c.Box);
        }

        public bool Supports(IMapObject obj)
        {
            return !(obj is Root);
        }

        public async Task<bool> Convert(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return false;
        }
    }
}