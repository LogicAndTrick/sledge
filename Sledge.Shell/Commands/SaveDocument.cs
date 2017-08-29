using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.Common.Shell;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;

namespace Sledge.Shell.Commands
{
    /// <summary>
    /// Internal: Save a document to a path
    /// </summary>
    [Export(typeof(ICommand))]
    [CommandID("Internal:SaveDocument")]
    [Internal]
    public class SaveDocument : ICommand
    {
        [ImportMany] private IEnumerable<Lazy<IDocumentLoader>> _loaders;

        public string Name { get; set; } = "Save";
        public string Details { get; set; } = "Save";

        public bool IsInContext(IContext context)
        {
            return true;
        }

        public async Task Invoke(IContext context, CommandParameters parameters)
        {
            var doc = parameters.Get<IDocument>("Document");
            var path = parameters.Get<string>("Path");
            var hint = parameters.Get("LoaderHint", "");
            if (doc != null && path != null)
            {
                IDocumentLoader loader = null;
                if (!String.IsNullOrWhiteSpace(hint)) loader = _loaders.Select(x => x.Value).FirstOrDefault(x => x.GetType().Name == hint);
                if (loader == null) loader = _loaders.Select(x => x.Value).FirstOrDefault(x => x.CanSave(doc) && x.CanLoad(path));
                if (loader != null)
                {
                    await Oy.Publish("Document:BeforeSave", doc);
                    await loader.Save(doc, path);
                }
            }
        }
    }
}