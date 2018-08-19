using System.ComponentModel.Composition;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.Common.Shell.Hooks;

namespace Sledge.BspEditor.Tools
{
    [Export(typeof(IInitialiseHook))]
    public class ToolInitialiser : IInitialiseHook
    {
        public Task OnInitialise()
        {
            Oy.Subscribe<MapViewport>("MapViewport:Created", MapViewportCreated);
            return Task.CompletedTask;
        }

        private Task MapViewportCreated(MapViewport viewport)
        {
            var itl = new ToolViewportListener(viewport);
            viewport.Listeners.Add(itl);
            return Task.CompletedTask;
        }
    }
}
