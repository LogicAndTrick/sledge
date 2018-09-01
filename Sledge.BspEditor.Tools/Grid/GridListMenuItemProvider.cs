using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
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
        [ImportMany] private IEnumerable<Lazy<IGridFactory>> _grids;

        public event EventHandler MenuItemsChanged;

        public IEnumerable<IMenuItem> GetMenuItems()
        {
            foreach (var grid in _grids)
            {
                yield return new GridMenuItem(grid.Value);
            }
        }

        private class GridMenuItem : IMenuItem
        {
            public string ID => "Sledge.BspEditor.Tools.Grid.GridMenuItem." + GridFactory.GetType().Name;
            public string Name => GridFactory.Name;
            public string Description => GridFactory.Details;
            public Image Icon => GridFactory.Icon;
            public bool AllowedInToolbar => false;
            public string Section => "Map";
            public string Path => "";
            public string Group => "GridTypes";
            public string OrderHint => Group.GetType().Name;
            public string ShortcutText => "";
            public bool IsToggle => false;

            public IGridFactory GridFactory { get; set; }

            public GridMenuItem(IGridFactory gridFactory)
            {
                GridFactory = gridFactory;
            }

            public bool IsInContext(IContext context)
            {
                return context.TryGet("ActiveDocument", out MapDocument _);
            }

            public async Task Invoke(IContext context)
            {
                if (context.TryGet("ActiveDocument", out MapDocument doc))
                {
                    var grid = await GridFactory.Create(doc.Environment);

                    var gd = new GridData(grid);
                    var operation = new TrivialOperation(x => doc.Map.Data.Replace(gd), x => x.Update(gd));

                    await MapDocumentOperation.Perform(doc, operation);
                    
                }
            }

            public bool GetToggleState(IContext context)
            {
                return false;
            }
        }
    }
}
