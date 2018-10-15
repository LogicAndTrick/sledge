using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Documents;

namespace Sledge.BspEditor.Providers
{
    /// <summary>
    /// A document loader for BSP source files (such as MAP, RMF, VMF, etc)
    /// </summary>
    public interface IBspSourceProvider
    {
        /// <summary>
        /// A list of data types supported by this loader.
        /// If a type isn't supported, it will be rearranged to remove the unsupported structures.
        /// All formats must support Root, Solid, and Entity.
        /// </summary>
        IEnumerable<Type> SupportedDataTypes { get; }

        /// <summary>
        /// A list of file extensions supported by this loader.
        /// </summary>
        IEnumerable<FileExtensionInfo> SupportedFileExtensions { get; }
        
        /// <summary>
        /// Load a map from a stream
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <param name="environment">The environment to load the map into</param>
        /// <returns>Completion task for the map</returns>
        Task<BspFileLoadResult> Load(Stream stream, IEnvironment environment);

        /// <summary>
        /// Save the map to a stream
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <param name="map">The map to write</param>
        /// <returns>Completion task for the save</returns>
        Task Save(Stream stream, Map map);
    }
}