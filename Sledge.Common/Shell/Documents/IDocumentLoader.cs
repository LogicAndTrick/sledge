using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sledge.Common.Shell.Documents
{
    /// <summary>
    /// An interface for classes that load documents from disk.
    /// </summary>
    public interface IDocumentLoader
    {
        /// <summary>
        /// A description of the document's file type
        /// </summary>
        string FileTypeDescription { get; }

        /// <summary>
        /// All file extensions supported by this document loader
        /// </summary>
        IEnumerable<FileExtensionInfo> SupportedFileExtensions { get; }

        /// <summary>
        /// Determine if a path can be loaded by this class. The file may not physically exist.
        /// </summary>
        /// <param name="location">The path to a file. The file may not exist on disk.</param>
        /// <returns>True if the path could be loaded by this class</returns>
        bool CanLoad(string location);

        /// <summary>
        /// Create a blank document
        /// </summary>
        /// <returns></returns>
        Task<IDocument> CreateBlank();

        /// <summary>
        /// Load a document from the given path
        /// </summary>
        /// <param name="location">The location to the file</param>
        /// <returns>A completion task that will return a loaded document</returns>
        Task<IDocument> Load(string location);

        /// <summary>
        /// Determine if a document can be saved by this class.
        /// </summary>
        /// <param name="document">The document to be saved.</param>
        /// <returns>True if the document could be saved by this class</returns>
        bool CanSave(IDocument document);

        /// <summary>
        /// Save a document to the given path
        /// </summary>
        /// <param name="document">The document to save</param>
        /// <param name="location">The location to save to</param>
        /// <returns>A completion task</returns>
        Task Save(IDocument document, string location);

        /// <summary>
        /// Convert a document into a pointer that can be loaded later
        /// </summary>
        /// <param name="document">The document</param>
        /// <returns>A minimal pointer that contains any metadata needed to load the document</returns>
        DocumentPointer GetDocumentPointer(IDocument document);

        /// <summary>
        /// Load the document from a pointer
        /// </summary>
        /// <param name="documentPointer"></param>
        /// <returns></returns>
        Task<IDocument> Load(DocumentPointer documentPointer);
    }
}
