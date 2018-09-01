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
    [CommandID("BspEditor:Edit:Undo")]
    [DefaultHotkey("Ctrl+Z")]
    [MenuItem("Edit", "", "History", "B")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_Undo))]
    public class UndoCommand : BaseCommand
    {
        public override string Name { get; set; } = "Undo";
        public override string Details { get; set; } = "Undo the last operation";

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            var stack = document.Map.Data.GetOne<HistoryStack>();
            if (stack == null) return;
            if (stack.CanUndo()) await MapDocumentOperation.Reverse(document, stack.UndoOperation());
        }
    }
}