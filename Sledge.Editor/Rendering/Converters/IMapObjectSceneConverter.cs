using System.Threading.Tasks;
using MapObject = Sledge.DataStructures.MapObjects.MapObject;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Rendering.Converters
{
    public interface IMapObjectSceneConverter
    {
        /// <summary>
        /// The priority of this converter.
        /// </summary>
        MapObjectSceneConverterPriority Priority { get; }

        /// <summary>
        /// Checks if further converters should be abandoned after this converter runs.
        /// </summary>
        /// <param name="smo">The current SceneMapObject</param>
        /// <param name="obj">The MapObject that's being converted</param>
        /// <returns>True to stop processing all further converters</returns>
        bool ShouldStopProcessing(SceneMapObject smo, MapObject obj);

        /// <summary>
        /// Check if the object is supported by this converter.
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>True if this converter supports the object</returns>
        bool Supports(MapObject obj);

        /// <summary>
        /// Convert the MapObject and put the objects in the SceneMapObject.
        /// Returns false if the MapObject is considered invalid and should be ignored.
        /// </summary>
        /// <param name="smo">The SceneMapObject to add scene objects to</param>
        /// <param name="document">The current document</param>
        /// <param name="obj">The object to convert</param>
        /// <returns>False if the object is invalid</returns>
        Task<bool> Convert(SceneMapObject smo, Document document, MapObject obj);

        /// <summary>
        /// Update an existing SceneMapObject with the new properties of this MapObject.
        /// Returns false if the update operation isn't possible.
        /// </summary>
        /// <param name="smo">The SceneMapObject to update scene objects in</param>
        /// <param name="document">The current document</param>
        /// <param name="obj">The object to update</param>
        /// <returns>False if the object could not be updated</returns>
        Task<bool> Update(SceneMapObject smo, Document document, MapObject obj);
    }
}