using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.Rendering.Resources;

namespace Sledge.BspEditor.Rendering.Converters
{
    /// <summary>
    /// Converts <see cref="IMapObject"/>s for rendering.
    /// </summary>
    public interface IMapObjectSceneConverter
    {
        /// <summary>
        /// The priority of this converter.
        /// </summary>
        MapObjectSceneConverterPriority Priority { get; }

        /// <summary>
        /// Checks if further converters should be abandoned after this converter runs.
        /// </summary>
        /// <param name="document">The current document</param>
        /// <param name="obj">The MapObject that's being converted</param>
        /// <returns>True to stop processing all further converters</returns>
        bool ShouldStopProcessing(MapDocument document, IMapObject obj);

        /// <summary>
        /// Check if the object is supported by this converter.
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>True if this converter supports the object</returns>
        bool Supports(IMapObject obj);

        /// <summary>
        /// Convert the MapObject and put the objects in the buffer.
        /// </summary>
        /// <param name="builder">The scene builder</param>
        /// <param name="document">The current document</param>
        /// <param name="obj">The object to convert</param>
        /// <param name="resourceCollector">A resource collecter to precache resources</param>
        Task Convert(BufferBuilder builder, MapDocument document, IMapObject obj, ResourceCollector resourceCollector);
    }
}