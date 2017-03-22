using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicAndTrick.Gimme.Providers;

namespace Sledge.Editor.Documents
{
    public interface IDocument
    {

    }

    public interface IDocumentLoader
    {
        bool CanLoad(string location, string type);
        Task<IDocument> Load(string location, string type);
    }

    public class MapDocumentLoader : IDocumentLoader
    {
        public bool CanLoad(string location, string type)
        {
            // todo 
            return location.EndsWith(".rmf");
        }

        public async Task<IDocument> Load(string location, string type)
        {
            throw new NotImplementedException();
        }
    }
}
