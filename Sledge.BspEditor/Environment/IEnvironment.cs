using System.Collections.Generic;
using System.Threading.Tasks;
using Sledge.BspEditor.Compile;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.DataStructures.GameData;
using Sledge.FileSystem;

namespace Sledge.BspEditor.Environment
{
    /// <summary>
    /// Represents an environment for a map document.
    /// </summary>
    public interface IEnvironment
    {
        /// <summary>
        /// The name of the engine
        /// </summary>
        string Engine { get; }

        /// <summary>
        /// The unique ID for this environment
        /// </summary>
        string ID { get; }

        /// <summary>
        /// The name of this environment
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The virtual file system root for the environment
        /// </summary>
        IFile Root { get; }

        /// <summary>
        /// The on-disk directories of this environment.
        /// These directories do not have to physically exist.
        /// </summary>
        IEnumerable<string> Directories { get; }

        /// <summary>
        /// Get the texture collection for this environment
        /// </summary>
        /// <returns>Async task that will return when the collection has been created</returns>
        Task<TextureCollection> GetTextureCollection();

        /// <summary>
        /// Gets the game data for this environment
        /// </summary>
        /// <returns>Game data task</returns>
        Task<GameData> GetGameData();

        /// <summary>
        /// Set up any extra map data that this environment requires for a document.
        /// </summary>
        /// <param name="document">The document</param>
        /// <returns>Initialise task</returns>
        Task UpdateDocumentData(MapDocument document);

        /// <summary>
        /// Environment extension point for custom data
        /// </summary>
        /// <param name="data">The data to add</param>
        void AddData(IEnvironmentData data);

        /// <summary>
        /// Get extension data from the environment
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <returns>The list of stored data</returns>
        IEnumerable<T> GetData<T>() where T : IEnvironmentData;

        /// <summary>
        /// Create a batch from the selected arguments
        /// </summary>
        /// <param name="arguments">The list of batch arguments the user has selected</param>
        /// <param name="options">The options to use for the batch</param>
        /// <returns>A batch to process this map</returns>
        Task<Batch> CreateBatch(IEnumerable<BatchArgument> arguments, BatchOptions options);

        /// <summary>
        /// Get a list of automatic visgroups provided by this environment
        /// </summary>
        /// <returns>A list of automatic visgroups</returns>
        IEnumerable<AutomaticVisgroup> GetAutomaticVisgroups();
        
        /// <summary>
        /// The name of the default brush entity for this environment
        /// </summary>
        string DefaultBrushEntity { get; }

        /// <summary>
        /// The name of the default point entity for this environment
        /// </summary>
        string DefaultPointEntity { get; }

        /// <summary>
        /// The name of the default texture scale for this environment
        /// </summary>
        decimal DefaultTextureScale { get; }
    }
}
