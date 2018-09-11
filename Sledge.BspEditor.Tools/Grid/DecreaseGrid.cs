using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Tools.Properties;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Tools.Grid
{
    [Export(typeof(ICommand))]
    [CommandID("BspEditor:Grid:DecreaseSpacing")]
    [DefaultHotkey("[")]
    [MenuItem("Map", "", "Grid", "G")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_SmallerGrid))]
    [AutoTranslate]
    public class DecreaseGrid : ICommand
    {
        public string Name { get; set; } = "Smaller Grid";
        public string Details { get; set; } = "Decrease the grid size";

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument _);
        }

        public async Task Invoke(IContext context, CommandParameters parameters)
        {
            if (context.TryGet("ActiveDocument", out MapDocument doc))
            {
                var activeGrid = doc.Map.Data.GetOne<GridData>();
                var grid = activeGrid?.Grid;
                if (grid != null)
                {
                    var operation = new TrivialOperation(x => grid.Spacing--, x => x.Update(activeGrid));
                    await MapDocumentOperation.Perform(doc, operation);
                }
            }
        }
    }
}