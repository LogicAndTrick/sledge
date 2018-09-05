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
    [CommandID("BspEditor:Edit:Group")]
    [DefaultHotkey("Ctrl+G")]
    [MenuItem("Tools", "", "Group", "D")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_Group))]
    public class Group : BaseCommand
    {
        public override string Name { get; set; } = "Group";
        public override string Details { get; set; } = "Group selected objects";

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            var sel = document.Selection.GetSelectedParents().ToList();
            if (sel.Count > 1)
            {
                var group = new Primitives.MapObjects.Group(document.Map.NumberGenerator.Next("MapObject")) { IsSelected = true };

                var tns = new Transaction();
                foreach (var grp in sel.GroupBy(x => x.Hierarchy.Parent.ID))
                {
                    tns.Add(new Detatch(grp.Key, grp));
                }
                tns.Add(new Attach(document.Map.Root.ID, group));
                tns.Add(new Attach(group.ID, sel));

                await MapDocumentOperation.Perform(document, tns);
            }
        }
    }
}
