using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
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
    [CommandID("File:SaveAs")]
    [DefaultHotkey("Ctrl+Shift+S")]
    [MenuItem("File", "", "File", "J")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_SaveAs))]
    public class SaveFileAs : ICommand
    {
        [ImportMany] private IEnumerable<Lazy<IDocumentLoader>> _loaders;

        public string Name { get; set; } = "Save As...";
        public string Details { get; set; } = "Save As...";

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out IDocument _);
        }

        public async Task Invoke(IContext context, CommandParameters parameters)
        {
            var doc = context.Get<IDocument>("ActiveDocument");
            if (doc != null)
            {
                var loaders = _loaders.Select(x => x.Value).Where(x => x.CanSave(doc)).ToList();

                var filter = loaders.SelectMany(x => x.SupportedFileExtensions).Select(x => x.Description + "|" + String.Join(";", x.Extensions.Select(e => "*" + e))).ToList();

                using (var sfd = new SaveFileDialog { Filter = String.Join("|", filter) })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        var loader = loaders.FirstOrDefault(x => x.CanLoad(doc.FileName));
                        if (loader != null)
                        {
                            await loader.Save(doc, sfd.FileName);
                            doc.FileName = sfd.FileName;
                            await Oy.Publish("Document:Saved", doc);
                        }
                    }
                }
            }
        }
    }
}