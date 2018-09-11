using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Tools.Properties;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Tools.Selection
{
    [Export(typeof(ICommand))]
    [CommandID("BspEditor:Map:ToggleIgnoreGrouping")]
    [DefaultHotkey("Ctrl+W")]
    [MenuItem("Map", "", "Grouping", "B")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_IgnoreGrouping))]
    [AutoTranslate]
    public class ToggleIgnoreGroupingCommand : BaseCommand
    {
        public override string Name { get; set; } = "Ignore grouping";
        public override string Details { get; set; } = "Toggle ignore grouping on and off";
        protected override Task Invoke(MapDocument document, CommandParameters parameters)
        {
            var opt = document.Map.Data.GetOne<SelectionOptions>() ?? new SelectionOptions();
            opt.IgnoreGrouping = !opt.IgnoreGrouping;
            MapDocumentOperation.Perform(document, new TrivialOperation(x => x.Map.Data.Replace(opt), x => x.Update(opt)));
            return Task.CompletedTask;
        }
    }
}