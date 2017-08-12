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
    [CommandID("File:Open")]
    [DefaultHotkey("Ctrl+O")]
    [MenuItem("File", "", "File", "D")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_Open))]
    public class OpenFile : ICommand
    {
        [ImportMany] private IEnumerable<Lazy<IDocumentLoader>> _loaders;

        public string Name { get; set; } = "Open";
        public string Details { get; set; } = "Open...";

        public bool IsInContext(IContext context)
        {
            return true;
        }

        public async Task Invoke(IContext context, CommandParameters parameters)
        {
            var filter = _loaders.Select(x => x.Value).Select(x => x.FileTypeDescription + "|" + String.Join(";", x.SupportedFileExtensions.SelectMany(e => e.Extensions).Select(e => "*" + e))).ToList();
            filter.Add("All files|*.*");
            using (var ofd = new OpenFileDialog { Filter = String.Join("|", filter)})
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var loader = _loaders.Select(x => x.Value).FirstOrDefault(x => x.CanLoad(ofd.FileName));
                    if (loader != null)
                    {
                        var doc = await loader.Load(ofd.FileName);
                        if (doc != null)
                        {
                            await Oy.Publish("Document:Opened", doc);
                        }
                    }
                }
            }
        }
    }
}
