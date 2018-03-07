using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.Common.Shell;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Shell.Registers;

namespace Sledge.Shell.Commands
{
    /// <summary>
    /// Internal: Load a document from a path
    /// </summary>
    [Export(typeof(ICommand))]
    [CommandID("Internal:LoadDocument")]
    [Internal]
    public class LoadDocument : ICommand
    {
        [ImportMany] private IEnumerable<Lazy<IDocumentLoader>> _loaders;
        [Import] private DocumentRegister _documentRegister;

        public string Name { get; set; } = "Load";
        public string Details { get; set; } = "Load";

        public bool IsInContext(IContext context)
        {
            return true;
        }

        public async Task Invoke(IContext context, CommandParameters parameters)
        {
            var path = parameters.Get<string>("Path");
            var hint = parameters.Get("LoaderHint", "");
            if (path != null && File.Exists(path))
            {
                // Is the document already open?
                var openDoc = _documentRegister.OpenDocuments.FirstOrDefault(x => string.Equals(x.FileName, path, StringComparison.InvariantCultureIgnoreCase));
                if (openDoc != null)
                {
                    await Oy.Publish("Document:Switch", openDoc);
                    return;
                }

                IDocumentLoader loader = null;
                if (!String.IsNullOrWhiteSpace(hint)) loader = _loaders.Select(x => x.Value).FirstOrDefault(x => x.GetType().Name == hint);
                if (loader == null) loader = _loaders.Select(x => x.Value).FirstOrDefault(x => x.CanLoad(path));
                if (loader != null)
                {
                    var doc = await loader.Load(path);
                    if (doc != null)
                    {
                        await Oy.Publish("Document:Opened", doc);
                    }
                }
            }
        }
    }
}