using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Gimme;
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
        public string Name { get; set; } = "Open";
        public string Details { get; set; } = "Open...";

        public bool IsInContext(IContext context)
        {
            return true;
        }

        public async Task Invoke(IContext context, CommandParameters parameters)
        {
            using (var ofd = new OpenFileDialog() { Filter = "All files|*.*"})
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var loader = await Gimme.FetchOne<IDocumentLoader>(ofd.FileName, "");
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
