using System.ComponentModel.Composition;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Threading;

namespace Sledge.BspEditor.Modification
{
    [Export(typeof(IInitialiseHook))]
    public class OperationHandler : IInitialiseHook
    {
        private readonly TaskQueue _queue = new TaskQueue();

        public Task OnInitialise()
        {
            Oy.Subscribe<MapDocumentOperation>("MapDocument:Perform:Bypass", Perform);
            Oy.Subscribe<MapDocumentOperation>("MapDocument:Perform", Perform);
            Oy.Subscribe<MapDocumentOperation>("MapDocument:Reverse", Reverse);
            return Task.FromResult(0);
        }

        private async Task Perform(MapDocumentOperation operation)
        {
            await _queue.Enqueue(async () =>
            {
                var change = await operation.Operation.Perform(operation.Document);
                await SendChange(change);
            });
        }

        private async Task Reverse(MapDocumentOperation operation)
        {
            await _queue.Enqueue(async () =>
            {
                var change = await operation.Operation.Reverse(operation.Document);
                await SendChange(change);
            });
        }

        private async Task SendChange(Change change)
        {
            await Oy.Publish("MapDocument:Changed:Early", change);
            await Oy.Publish("MapDocument:Changed", change);
            await Oy.Publish("MapDocument:Changed:Late", change);
        }
    }
}
