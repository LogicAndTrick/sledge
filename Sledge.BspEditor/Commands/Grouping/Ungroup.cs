using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Properties;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Commands.Grouping
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [CommandID("BspEditor:Edit:Ungroup")]
    [DefaultHotkey("Ctrl+U")]
    [MenuItem("Tools", "", "Group", "D")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_Ungroup))]
    public class Ungroup : BaseCommand
    {
        public override string Name { get; set; } = "Ungroup";
        public override string Details { get; set; } = "Ungroup selected objects";

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            var sel = document.Selection.GetSelectedParents().OfType<Primitives.MapObjects.Group>().ToList();
            if (sel.Count > 0)
            {
                var tns = new Transaction();
                foreach (var grp in sel)
                {
                    var list = grp.Hierarchy.ToList();
                    tns.Add(new Detatch(grp.ID, list));
                    tns.Add(new Attach(document.Map.Root.ID, list));
                    tns.Add(new Detatch(grp.Hierarchy.Parent.ID, grp));
                }
                await MapDocumentOperation.Perform(document, tns);
            }
        }
    }
}