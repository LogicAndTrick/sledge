using System.ComponentModel.Composition;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;

namespace Sledge.Shell.Commands
{
    [Export(typeof(ICommand))]
    [CommandID("File:Save")]
    [DefaultHotkey("Ctrl+S")]
    [MenuItem("File", "", "File", "F")]
    public class SaveFile : ICommand
    {
        public string Name => "Save";
        public string Details => "Save";

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out IDocument _);
        }

        public async Task Invoke(IContext context, CommandParameters parameters)
        {
            var doc = context.Get<IDocument>("ActiveDocument");
            if (doc != null) await Oy.Publish("Document:Save", doc);
        }
    }
}