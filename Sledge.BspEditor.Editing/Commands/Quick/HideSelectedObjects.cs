using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Properties;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Data;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Commands.Quick
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("View", "", "Quick", "B")]
    [CommandID("BspEditor:View:QuickHideSelected")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_HideSelected))]
    [DefaultHotkey("H")]
    public class HideSelectedObjects : BaseCommand
    {
        public override string Name { get; set; } = "Quick hide selected";
        public override string Details { get; set; } = "Quick hide selected objects";

        protected override bool IsInContext(IContext context, MapDocument document)
        {
            return base.IsInContext(context, document) && !document.Selection.IsEmpty;
        }

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            var transaction = new Transaction();

            foreach (var mo in document.Selection.ToList())
            {
                var ex = mo.Data.GetOne<QuickHidden>();
                if (ex != null) transaction.Add(new RemoveMapObjectData(mo.ID, ex));
                transaction.Add(new AddMapObjectData(mo.ID, new QuickHidden()));
            }

            transaction.Add(new Deselect(document.Selection));

            await MapDocumentOperation.Perform(document, transaction);
        }
    }
}
