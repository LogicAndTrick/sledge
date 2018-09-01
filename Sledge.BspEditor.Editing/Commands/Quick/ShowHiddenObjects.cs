using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Properties;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Data;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Commands.Quick
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("View", "", "Quick", "F")]
    [CommandID("BspEditor:View:ShowHidden")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_ShowHidden))]
    [DefaultHotkey("U")]
    public class ShowHiddenObjects : BaseCommand
    {
        public override string Name { get; set; } = "Show hidden objects";
        public override string Details { get; set; } = "Show objects hidden with quick hide";

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            var transaction = new Transaction();

            foreach (var mo in document.Map.Root.Find(x => x.Data.Get<QuickHidden>().Any()))
            {
                transaction.Add(new RemoveMapObjectData(mo.ID, mo.Data.GetOne<QuickHidden>()));
            }

            await MapDocumentOperation.Perform(document, transaction);
        }
    }
}