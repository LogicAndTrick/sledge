using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sledge.Common.Shell.Documents
{
    public interface IDocumentLoader
    {
        IEnumerable<FileExtensionInfo> SupportedFileExtensions { get; }
        bool CanLoad(string location);
        Task<IDocument> CreateBlank();
        Task<IDocument> Load(string location);
    }
}
