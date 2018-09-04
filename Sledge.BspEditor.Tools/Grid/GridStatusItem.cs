using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.ChangeHandling;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Tools.Grid
{
    [Export(typeof(IStatusItem))]
    [Export(typeof(IMapDocumentChangeHandler))]
    [AutoTranslate]
    [OrderHint("L")]
    public class GridStatusItem : IStatusItem, IMapDocumentChangeHandler
    {
        public event EventHandler<string> TextChanged;
        public string OrderHint => "M";

        public string ID => "Sledge.BspEditor.Tools.Grid.GridStatusItem";
        public int Width => 120;
        public bool HasBorder => true;
        public string Text { get; set; }

        public string Snap { get; set; }
        public string NoSnap { get; set; }
        public string Grid { get; set; }

        public GridStatusItem()
        {
            Oy.Subscribe<string>("MapDocument:GridStatus:UpdateText", UpdateText);
        }

        public Task Changed(Change change)
        {
            if (change.HasDataChanges && change.AffectedData.OfType<GridData>().Any())
            {
                var grid = change.Document.Map.Data.GetOne<GridData>();
                if (grid != null)
                {
                    var snap = grid.SnapToGrid ? Snap : NoSnap;
                    UpdateText(Grid + ": " + grid.Grid.Description + ", " + snap);
                }
            }

            return Task.CompletedTask;
        }

        private async Task UpdateText(string text)
        {
            Text = text;
            TextChanged?.Invoke(this, Text);
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument _);
        }
    }
}
