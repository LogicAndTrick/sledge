using System.ComponentModel.Composition;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Properties;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Commands
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Map", "", "Properties", "E")]
    [CommandID("BspEditor:Map:LogicalTree")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_ShowLogicalTree))]
    public class OpenMapTreeWindow : BaseCommand
    {
        public override string Name { get; set; } = "Show logical tree";
        public override string Details { get; set; } = "Show the logical tree of the current document.";

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            await Oy.Publish("Context:Add", new ContextInfo("BspEditor:MapTree"));
        }
    }
}