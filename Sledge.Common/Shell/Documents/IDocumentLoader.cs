using System.Collections.Generic;
using System.Threading.Tasks;
using Sledge.Common.Transport;

namespace Sledge.Common.Shell.Documents
{
    public interface IDocumentLoader
    {
        string FileTypeDescription { get; }
        IEnumerable<FileExtensionInfo> SupportedFileExtensions { get; }

        bool CanLoad(string location);
        Task<IDocument> CreateBlank();
        Task<IDocument> Load(string location);

        bool CanSave(IDocument document);
        Task Save(IDocument document, string location);

        /// <summary>
        /// Convert a document into a pointer that can be loaded later
        /// </summary>
        /// <param name="document">The document</param>
        /// <returns>A minimal pointer that contains any metadata needed to load the document</returns>
        SerialisedObject GetDocumentPointer(IDocument document);

        /// <summary>
        /// Load the document from a pointer
        /// </summary>
        /// <param name="documentPointer"></param>
        /// <returns></returns>
        Task<IDocument> Load(SerialisedObject documentPointer);
    }
}
