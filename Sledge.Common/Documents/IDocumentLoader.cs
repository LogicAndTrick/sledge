using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.Common.Documents
{
    public interface IDocumentLoader
    {
        bool CanLoad(string location);
        Task<IDocument> CreateBlank();
        Task<IDocument> Load(string location);
    }
}
