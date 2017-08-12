using System.Collections.Generic;
using System.Threading.Tasks;

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
    }
}
