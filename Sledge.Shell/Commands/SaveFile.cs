using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;
using Sledge.Shell.Properties;

namespace Sledge.Shell.Commands
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [CommandID("File:Save")]
    [DefaultHotkey("Ctrl+S")]
    [MenuItem("File", "", "File", "H")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_Save))]
    public class SaveFile : ICommand
    {
        [ImportMany] private IEnumerable<Lazy<IDocumentLoader>> _loaders;

        public string Name { get; set; } = "Save";
        public string Details { get; set; } = "Save";

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out IDocument _);
        }

        public async Task Invoke(IContext context, CommandParameters parameters)
        {
            var doc = context.Get<IDocument>("ActiveDocument");
            if (doc != null)
            {
                var loader = _loaders.Select(x => x.Value).FirstOrDefault(x => x.CanSave(doc) && x.CanLoad(doc.FileName));
                if (loader != null)
                {
                    await Oy.Publish("Document:BeforeSave", doc);
                    await loader.Save(doc, doc.FileName);
                    await Oy.Publish("Document:Saved", doc);
                }
            }
        }
    }
}