using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.Common.Shell.Hooks;

namespace Sledge.BspEditor.Tools
{
    [Export(typeof(IInitialiseHook))]
    public class ToolInitialiser : IInitialiseHook
    {
        public async Task OnInitialise()
        {
            Oy.Subscribe<MapViewport>("MapViewport:Created", MapViewportCreated);
        }

        private async Task MapViewportCreated(MapViewport viewport)
        {
            var itl = new ToolViewportListener(viewport);
            viewport.Listeners.Add(itl);
        }
    }
}
