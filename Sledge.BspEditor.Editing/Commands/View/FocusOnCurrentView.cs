using System.ComponentModel.Composition;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Commands.View
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [CommandID("BspEditor:View:FocusCurrent")]
    [MenuItem("View", "", "SplitView", "B")]
    [DefaultHotkey("Shift+Z")]
    public class FocusOnCurrentView : BaseCommand
    {
        public override string Name { get; set; } = "Focus on current view";
        public override string Details { get; set; } = "Maximise the current view in the viewport grid";

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            await Oy.Publish("BspEditor:SplitView:FocusCurrent");
        }
    }
}