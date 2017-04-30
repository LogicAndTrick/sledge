using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Grid;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Menu;

namespace Sledge.BspEditor.Tools.Grid
{
    [Export(typeof(IMenuItemProvider))]
    public class GridListMenuItemProvider : IMenuItemProvider
    {
        [ImportMany] private IEnumerable<Lazy<IGrid>> _grids;

        public IEnumerable<IMenuItem> GetMenuItems()
        {
            foreach (var grid in _grids)
            {
                yield return new GridMenuItem(grid.Value);
            }
        }

        private class GridMenuItem : IMenuItem
        {
            public string ID => "Sledge.BspEditor.Tools.Grid.GridMenuItem." + Grid.GetType().Name;
            public string Name => Grid.Name;
            public string Description => Grid.GetType().Name;
            public string Section => "Map";
            public string Path => ""; // todo !menu proper grid path
            public string Group => "";
            public string OrderHint => Group.GetType().Name;

            public IGrid Grid { get; set; }

            public GridMenuItem(IGrid grid)
            {
                Grid = grid;
            }

            public bool IsInContext(IContext context)
            {
                return context.TryGet("ActiveDocument", out MapDocument _);
            }

            public async Task Invoke(IContext context)
            {
                if (context.TryGet("ActiveDocument", out MapDocument doc))
                {
                    var operation = new TrivialOperation(x =>
                    {
                        var activeGrid = doc.Map.Data.GetOne<GridData>();
                        if (activeGrid != null) doc.Map.Data.Remove(activeGrid);
                        doc.Map.Data.Add(new GridData(Grid));
                    }, x => x.UpdateDocument());
                    await MapDocumentOperation.Perform(doc, operation);
                    
                }
            }
        }
    }
}
