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
    [CommandID("BspEditor:View:CenterSelectionAll")]
    [MenuItem("View", "", "Selection", "B")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_CenterSelectionAll))]
    public class CenterSelectionAll : BaseCommand
    {
        public override string Name { get; set; } = "Center all views on selection";
        public override string Details { get; set; } = "Move the cameras of all views to focus on the selected objects.";

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            if (document.Selection.IsEmpty) return;

            var box = document.Selection.GetSelectionBoundingBox();

            await Task.WhenAll(
                Oy.Publish("MapDocument:Viewport:Focus3D", box),
                Oy.Publish("MapDocument:Viewport:Focus2D", box)
            );
        }
    }
}