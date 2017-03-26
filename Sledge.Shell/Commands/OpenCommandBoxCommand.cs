using System.ComponentModel.Composition;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.Common.Commands;
using Sledge.Common.Context;
using Sledge.Common.Hotkeys;

namespace Sledge.Shell.Commands
{
    /// <summary>
    /// Opens the command box
    /// </summary>
    [Export(typeof(ICommand))]
    [DefaultHotkey("Ctrl+T")]
    public class OpenCommandBoxCommand : ICommand
    {
        public string Name => "Open the command box";
        public string Details => "";

        public bool IsInContext(IContext context)
        {
            return true;
        }

        public async Task Invoke(CommandParameters parameters)
        {
            await Oy.Publish<string>("Shell:OpenCommandBox", "");
        }
    }
}
