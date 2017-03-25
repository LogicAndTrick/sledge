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
using Sledge.Common.Components;
using Sledge.Common.Documents;

namespace Sledge.MinimalEditor.TextEditor
{
    [Export(typeof(ICommand))]
    [CommandID("TestCommand")]
    public class TestCommand : ICommand
    {
        public string Name => "This is a test command";
        public string Details => "Run a test command";

        public bool IsInContext()
        {
            return true;
        }

        public async Task Invoke(CommandParameters parameters)
        {
            using (var ofd = new OpenFileDialog() { Filter = "Text files|*.txt"})
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

    [Export(typeof(ISidebarComponent))]
    public class SampleSidebarComponent : ISidebarComponent
    {
        private Control _control;
        public string Title => "Example";
        public object Control => _control;

        public SampleSidebarComponent()
        {
            _control = new Panel();
            _control.Controls.Add(new TextBox());
        }

        public bool IsInContext()
        {
            return true;
        }
    }
}
