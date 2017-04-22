using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;

namespace Sledge.Shell.Commands
{
    [Export(typeof(ICommand))]
    [CommandID("File:Exit")]
    [DefaultHotkey("Alt+F4")]
    [MenuItem("File", "", "Exit", "M")]
    public class Exit : ICommand
    {
        [Import] private Forms.Shell _shell;

        public string Name => "Exit";
        public string Details => "Exit";

        public bool IsInContext(IContext context)
        {
            return true;
        }

        public async Task Invoke(IContext context, CommandParameters parameters)
        {
            _shell.Close();
        }
    }
}