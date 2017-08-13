using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Components;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Components;
using Sledge.BspEditor.Editing.Properties;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Editing.Commands
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Edit", "", "Clipboard", "H")]
    [CommandID("BspEditor:Edit:PasteSpecial")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_PasteSpecial))]
    [DefaultHotkey("Ctrl+Shift+V")]
    public class PasteSpecial : BaseCommand
    {
        [Import] private Lazy<ClipboardManager> _clipboard;
        [Import] private Lazy<ITranslationStringProvider> _translator;

        public override string Name { get; set; } = "Paste Special...";
        public override string Details { get; set; } = "Paste multiple copies";

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            if (_clipboard.Value.CanPaste())
            {
                var content = _clipboard.Value.GetPastedContent(document).ToList();
                if (!content.Any()) return;

                using (var psd = new PasteSpecialDialog(new Box(content.Select(x => x.BoundingBox))))
                {
                    _translator.Value.Translate(psd);
                    if (psd.ShowDialog() == DialogResult.OK)
                    {
                        // todo
                        //var op = new Attach(document.Map.Root.ID, content);
                        //await MapDocumentOperation.Perform(document, op);
                    }
                }
            }
        }
    }
}