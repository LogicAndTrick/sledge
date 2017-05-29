using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Components;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Properties;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Commands.Clipboard
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [CommandID("BspEditor:Edit:Copy")]
    [DefaultHotkey("Ctrl+C")]
    [MenuItem("Edit", "", "Clipboard", "D")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_Copy))]
    public class Copy : BaseCommand
    {
        [Import] private Lazy<ClipboardManager> _clipboard;

        public override string Name { get; set; } = "Copy";
        public override string Details { get; set; } = "Copy the current selection";
        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            var sel = document.Selection.GetSelectedParents().ToList();
            if (sel.Any()) _clipboard.Value.Push(sel);
        }
    }
}