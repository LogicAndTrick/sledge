using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Gimme;
using LogicAndTrick.Oy;
using Sledge.Common.Commands;
using Sledge.Common.Context;
using Sledge.Common.Documents;
using Sledge.Common.Hotkeys;

namespace Sledge.MinimalEditor.TextEditor
{
    [Export(typeof(ICommand))]
    [CommandID("TestCommand")]
    [DefaultHotkey("Ctrl+O")]
    public class TestCommand : ICommand
    {
        public string Name => "This is a test command";
        public string Details => "Run a test command";

        public bool IsInContext(IContext context)
        {
            return true;
        }

        public async Task Invoke(CommandParameters parameters)
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
