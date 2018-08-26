using System.ComponentModel.Composition;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Hooks;

namespace Sledge.BspEditor.Modification
{
    [Export(typeof(IInitialiseHook))]
    public class ChangeHandler : IInitialiseHook
    {
        [ImportMany] private IMapDocumentChangeHandler[] _changeHandlers;

        public Task OnInitialise()
        {
            Oy.Subscribe<Change>("MapDocument:Changed", Changed);
            Oy.Subscribe<MapDocument>("Document:Opened", Opened);
            return Task.CompletedTask;
        }

        private Task Opened(MapDocument doc)
        {
            var ch = new Change(doc);
            ch.AddRange(doc.Map.Root.FindAll());
            return Changed(ch);
        }

        private async Task Changed(Change change)
        {
            foreach (var ch in _changeHandlers)
            {
                await ch.Changed(change);
            }
        }
    }
}