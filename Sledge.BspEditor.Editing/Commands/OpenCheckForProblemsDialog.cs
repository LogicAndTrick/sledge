using System.ComponentModel.Composition;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Properties;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Commands
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Map", "", "Properties", "D")]
    [CommandID("BspEditor:Map:CheckForProblems")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_CheckForProblems))]
    [DefaultHotkey("Alt+P")]
    public class OpenCheckForProblemsDialog : BaseCommand
    {
        public override string Name { get; set; } = "Check for problems";
        public override string Details { get; set; } = "Open the check for problems window";

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            await Oy.Publish("Context:Add", new ContextInfo("BspEditor:CheckForProblems"));
        }
    }
}