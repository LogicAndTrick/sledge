using System.ComponentModel.Composition;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Properties;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Commands.View
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [CommandID("BspEditor:View:AutosizeViews")]
    [MenuItem("View", "", "SplitView", "B")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_AutosizeViews))]
    public class AutosizeViews : BaseCommand
    {
        public override string Name { get; set; } = "Autosize views";
        public override string Details { get; set; } = "Automatically resize the split views to be the same size.";

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            await Oy.Publish("BspEditor:SplitView:Autosize");
        }
    }
}