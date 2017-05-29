using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Properties;
using Sledge.BspEditor.Modification;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.History
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [CommandID("BspEditor:Edit:Redo")]
    [DefaultHotkey("Ctrl+Y")]
    [MenuItem("Edit", "", "History", "D")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_Redo))]
    public class RedoCommand : BaseCommand
    {
        public override string Name { get; set; } = "Redo";
        public override string Details { get; set; } = "Redo the last undone operation";

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            var stack = document.Map.Data.GetOne<HistoryStack>();
            if (stack == null) return;
            if (stack.CanRedo()) await MapDocumentOperation.Perform(document, stack.RedoOperation());
        }
    }
}