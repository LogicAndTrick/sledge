using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Grid;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Tools.Grid
{
    [Export(typeof(ICommand))]
    [CommandID("BspEditor:Grid:CycleGrid")]
    [DefaultHotkey("Shift+R")]
    [AutoTranslate]
    public class SwitchGrid : ICommand
    {
        [ImportMany] private IGridFactory[] _grids;

        public string Name => "Switch grids";
        public string Details => "Cycle through grid types";

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument _);
        }

        public async Task Invoke(IContext context, CommandParameters parameters)
        {
            if (context.TryGet("ActiveDocument", out MapDocument doc))
            {
                if (!_grids.Any()) return;

                var current = doc.Map.Data.GetOne<GridData>()?.Grid;
                var idx = current == null ? -1 : Array.FindIndex(_grids, x => x.IsInstance(current));
                idx = (idx + 1) % _grids.Length;
                
                var grid = await _grids[idx].Create(doc.Environment);

                var gd = new GridData(grid);
                var operation = new TrivialOperation(x => doc.Map.Data.Replace(gd), x => x.Update(gd));

                await MapDocumentOperation.Perform(doc, operation);
            }
        }
    }
}