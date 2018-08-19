using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Rendering.Overlay
{
    [Export(typeof(IStartupHook))]
    public class OverlayManager : IStartupHook
    {
        [Import] private Lazy<EngineInterface> _engine;
        [ImportMany] private IEnumerable<Lazy<IOverlayRenderable>> _overlayRenderables;

        private readonly object _lock = new object();

        /// <inheritdoc />
        public Task OnStartup()
        {
            Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated);
            Oy.Subscribe<IDocument>("Document:Closed", DocumentClosed);
            Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged);

            foreach (var or in _overlayRenderables)
            {
                _engine.Value.Add(or.Value);
            }

            _engine.Value.ViewportCreated += ViewportCreated;
            _engine.Value.ViewportDestroyed += ViewportDestroyed;

            return Task.FromResult(0);
        }

        private WeakReference<MapDocument> _activeDocument = new WeakReference<MapDocument>(null);

        // Document events

        private async Task DocumentChanged(Change change)
        {
            //
        }

        private async Task DocumentActivated(IDocument doc)
        {
            //
        }

        private async Task DocumentClosed(IDocument doc)
        {
            //
        }

        private void ViewportCreated(object sender, IViewport viewport)
        {
            //
        }

        private void ViewportDestroyed(object sender, IViewport viewport)
        {
            //
        }
    }
}