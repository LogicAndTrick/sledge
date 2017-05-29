using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Sledge.BspEditor.Components;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Properties;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Commands.Clipboard
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [CommandID("BspEditor:Edit:Paste")]
    [DefaultHotkey("Ctrl+V")]
    [MenuItem("Edit", "", "Clipboard", "F")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_Paste))]
    public class Paste : BaseCommand
    {
        [Import] private Lazy<ClipboardManager> _clipboard;

        public override string Name { get; set; } = "Paste";
        public override string Details { get; set; } = "Paste the current clipboard contents";

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            if (_clipboard.Value.CanPaste())
            {
                var content = _clipboard.Value.GetPastedContent(document);
                var op = new Attach(document.Map.Root.ID, content);
                await MapDocumentOperation.Perform(document, op);
            }
        }
    }
}