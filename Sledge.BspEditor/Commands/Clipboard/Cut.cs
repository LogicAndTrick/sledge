using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Components;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;

namespace Sledge.BspEditor.Commands.Clipboard
{
    [Export(typeof(ICommand))]
    [CommandID("BspEditor:Edit:Cut")]
    [DefaultHotkey("Ctrl+X")]
    [MenuItem("Edit", "", "Clipboard", "B")]
    public class Cut : BaseCommand
    {
        [Import] private Lazy<ClipboardManager> _clipboard;

        public override string Name => "Cut";
        public override string Details => "Copy the current selection and remove it";
        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            var sel = document.Selection.ToList();
            if (sel.Any())
            {
                _clipboard.Value.Push(sel);
                var t = new Transaction(sel.GroupBy(x => x.Hierarchy.Parent.ID).Select(x => new Detatch(x.Key, x)));
                await MapDocumentOperation.Perform(document, t);
            }
        }
    }
}