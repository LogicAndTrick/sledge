using System.ComponentModel.Composition;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.Common.Shell.Hooks;

namespace Sledge.BspEditor.Editing.History
{
    /// <summary>
    /// Deals with storing history and undo/redo on map documents
    /// </summary>
    [Export(typeof(IInitialiseHook))]
    public class HistoryHandler : IInitialiseHook
    {
        private int MaximumSize { get; set; } = 50;

        public async Task OnInitialise()
        {
            Oy.Subscribe<MapDocument>("Document:Opened", Opened);
            Oy.Subscribe<MapDocumentOperation>("MapDocument:Perform", Performed);
            Oy.Subscribe<MapDocumentOperation>("MapDocument:Reverse", Reversed);
        }

        private async Task Opened(MapDocument doc)
        {
            doc.Map.Data.Replace(new HistoryStack(MaximumSize));
        }

        private async Task Performed(MapDocumentOperation operation)
        {
            if (operation.Operation.Trivial) return;

            var stack = operation.Document.Map.Data.GetOne<HistoryStack>();
            stack?.Add(operation.Operation);

            Oy.Publish("MapDocument:HistoryChanged", operation.Document);
        }

        private async Task Reversed(MapDocumentOperation operation)
        {
            var stack = operation.Document.Map.Data.GetOne<HistoryStack>();
            stack?.Remove(operation.Operation);

            Oy.Publish("MapDocument:HistoryChanged", operation.Document);
        }
    }
}
