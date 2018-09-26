using System;
using System.Threading.Tasks;

namespace Sledge.Common.Shell.Documents
{
    /// <summary>
    /// A document which is hosted in the main tabbed are of the shell.
    /// </summary>
    public interface IDocument : IDisposable
    {
        /// <summary>
        /// Name of this document, will be shown in the UI tab
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The physical location on disk of this document
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// Mountable control for this document
        /// </summary>
        object Control { get; }

        /// <summary>
        /// True if this document has unsaved changes
        /// </summary>
        bool HasUnsavedChanges { get; }

        /// <summary>
        /// Nicely request if this document can be closed.
        /// This might prompt the user in some way to allow cancellation.
        /// </summary>
        /// <returns>A task with true if the document can be closed, false otherwise.</returns>
        Task<bool> RequestClose();
    }
}