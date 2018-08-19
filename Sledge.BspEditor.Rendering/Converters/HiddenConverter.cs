using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Scene;

namespace Sledge.BspEditor.Rendering.Converters
{
    /// <summary>
    /// Stops processing for hidden objects to ensure they are actually hidden.
    /// </summary>
    [Export(typeof(IMapObjectSceneConverter))]
    public class HiddenConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.OverrideLowest;

        public bool ShouldStopProcessing(MapDocument document, IMapObject obj)
        {
            return true;
        }

        public bool Supports(IMapObject obj)
        {
            return obj.Data.OfType<IObjectVisibility>().Any(x => x.IsHidden);
        }

        public Task Convert(SceneBuilder builder, MapDocument document, IMapObject obj)
        {
            return Task.FromResult(false);
        }
    }
}