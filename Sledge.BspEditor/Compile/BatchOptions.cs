using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Compile
{
    /// <summary>
    /// A list of common batch options to override from the environment.
    /// Any null options will use the environment default.
    /// </summary>
    public class BatchOptions
    {
        /// <summary>
        /// Specify if the user can be interrupted at all during this batch.
        /// If set to false, the user will not be notified of errors or asked for confirmation.
        /// If this is false and 'ask to run game' is true, the game will not run.
        /// Default: Interruption is allowed
        /// </summary>
        public bool? AllowUserInterruption { get; set; }

        /// <summary>
        /// Specify a custom working directory for the batch.
        /// Default: Create a temporary folder
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Specify a custom map file name
        /// Default: Use the document name, or a random name for an unsaved document
        /// </summary>
        public string MapFileName { get; set; }

        /// <summary>
        /// Specify a custom map file extension, including the dot
        /// Default: Use the extension the environment's build tools expect
        /// </summary>
        public string MapFileExtension { get; set; }

        /// <summary>
        /// Specify a custom save function
        /// Default: Apply cordon and then use Internal:ExportDocument
        /// </summary>
        public Func<MapDocument, string, Task> ExportDocument { get; set; }

        /// <summary>
        /// Specify whether to use cordon bounds when exporting the map.
        /// This is ignored if a custom export function is used.
        /// Default: Cordon will be used if it's enabled
        /// </summary>
        public bool? UseCordonBounds { get; set; }

        /// <summary>
        /// Specify custom cordon bounds to use.
        /// Default: Use cordon bounds from the map data
        /// </summary>
        public Box CordonBounds { get; set; }

        /// <summary>
        /// A list of steps to include in the batch.
        /// Note that if this list is not-null but empty, no steps will be included.
        /// Default: All steps will be included
        /// </summary>
        public List<BatchStepType> BatchSteps { get; set; }

        /// <summary>
        /// Whether to run the game or not.
        /// Note that if the game executable is not set up properly, the game won't run.
        /// Default: As configured in the environment.
        /// </summary>
        public bool? RunGame { get; set; }

        /// <summary>
        /// Whether to ask before running the game or not.
        /// Default: As configured in the environment.
        /// </summary>
        public bool? AskRunGame { get; set; }
    }
}