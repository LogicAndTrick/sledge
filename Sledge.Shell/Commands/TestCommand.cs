using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.Common.Commands;

namespace Sledge.Shell.Commands
{
    [Export(typeof(ICommand))]
    public class TestCommand : ICommand
    {
        public string Name => "This is a test command";
        public string Details => "Run a test command";

        public async Task Invoke(CommandParameters parameters)
        {
            MessageBox.Show(parameters.Get("Message", "Test Message"), parameters.Get("Title", "Message"));
        }
    }
}
