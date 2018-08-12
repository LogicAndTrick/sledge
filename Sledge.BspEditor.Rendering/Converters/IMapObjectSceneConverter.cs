using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Scene;
using Sledge.Rendering.Renderables;

namespace Sledge.BspEditor.Rendering.Converters
{
    /// <summary>
    /// Converts <see cref="IMapObject"/>s into <see cref="IRenderable"/>s for rendering.
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
        /// <param name="smo">The current SceneMapObject</param>
        /// <param name="document">The current document</param>
        /// <param name="obj">The MapObject that's being converted</param>
        /// <returns>True to stop processing all further converters</returns>
        bool ShouldStopProcessing(SceneMapObject smo, MapDocument document, IMapObject obj);

        /// <summary>
        /// Check if the object is supported by this converter.
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>True if this converter supports the object</returns>
        bool Supports(IMapObject obj);

        /// <summary>
        /// Convert the MapObject and put the objects in the SceneMapObject.
        /// </summary>
        /// <param name="smo">The SceneMapObject to add scene objects to</param>
        /// <param name="document">The current document</param>
        /// <param name="obj">The object to convert</param>
        Task Convert(SceneMapObject smo, MapDocument document, IMapObject obj);

        /// <summary>
        /// Update an existing SceneMapObject with the new properties of this MapObject.
        /// Returns false if the update couldn't be done in-place and the renderable list has changed.
        /// This method is responsible for disposing of any resources that it replaces as part of this operation.
        /// </summary>
        /// <param name="smo">The SceneMapObject to update scene objects in</param>
        /// <param name="document">The current document</param>
        /// <param name="obj">The object to update</param>
        /// <returns>False if the renderable list has changed</returns>
        Task<bool> Update(SceneMapObject smo, MapDocument document, IMapObject obj);
    }
}