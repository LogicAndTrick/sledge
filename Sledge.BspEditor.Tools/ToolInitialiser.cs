using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Grid;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;

namespace Sledge.BspEditor.Tools
{
    [Export(typeof(IInitialiseHook))]
    public class ToolInitialiser : IInitialiseHook
    {
        public async Task OnInitialise()
        {
            Oy.Subscribe<MapViewport>("MapViewport:Created", MapViewportCreated);
            Oy.Subscribe<IDocument>("Document:Opened", DocumentOpened);
        }

        private async Task MapViewportCreated(MapViewport viewport)
        {
            var itl = new ToolViewportListener(viewport);
            viewport.Listeners.Add(itl);
        }

        private async Task DocumentOpened(IDocument doc)
        {
            var md = doc as MapDocument;
            if (md == null) return;
            if (!md.Map.Data.Any(x => x is GridData))
            {
                MapDocumentOperation.Perform(md, new TrivialOperation(
                    x => x.Map.Data.Add(new GridData(new SquareGrid())),
                    x => x.UpdateDocument())
                );
            }
        }
    }
}
