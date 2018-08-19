using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Rendering.Grid
{
    /// <summary>
    /// Handles rendering the grid for each viewport.
    /// The grid is separate from the rest of the scene as it doesn't update at the same time as the document.
    /// i.e. the grid will update when the user simply scrolls the viewport, but will not when the user edits an object.
    /// </summary>
    [Export(typeof(IStartupHook))]
    public class GridManager : IStartupHook
    {
        [Import] private Lazy<EngineInterface> _engine;

        private readonly object _lock = new object();
        private readonly Dictionary<int, GridRenderable> _viewportRenderables;

        /// <inheritdoc />
        public Task OnStartup()
        {
            Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated);
            Oy.Subscribe<IDocument>("Document:Closed", DocumentClosed);
            Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged);

            _engine.Value.ViewportCreated += ViewportCreated;
            _engine.Value.ViewportDestroyed += ViewportDestroyed;

            return Task.FromResult(0);
        }

        public GridManager()
        {
            _viewportRenderables = new Dictionary<int, GridRenderable>();
        }

        private WeakReference<MapDocument> _activeDocument = new WeakReference<MapDocument>(null);

        // Document events

        private async Task DocumentChanged(Change change)
        {
            if (_activeDocument.TryGetTarget(out var md) && change.Document == md)
            {
                if (change.HasDataChanges && change.AffectedData.OfType<GridData>().Any())
                {
                    await UpdateGrid(change.Document);
                }
            }
        }

        private async Task DocumentActivated(IDocument doc)
        {
            var md = doc as MapDocument;
            _activeDocument = new WeakReference<MapDocument>(md);
            await UpdateGrid(md);
        }

        private async Task DocumentClosed(IDocument doc)
        {
            if (_activeDocument.TryGetTarget(out var md) && md == doc)
            {
                await UpdateGrid(null);
            }
        }

        private void ViewportCreated(object sender, IViewport viewport)
        {
            lock (_lock)
            {
                var gr = new GridRenderable(viewport, _engine.Value);
                if (_activeDocument.TryGetTarget(out var md)) gr.SetGrid(md);
                _viewportRenderables[viewport.ID] = gr;
                _engine.Value.Add(gr);
            }
        }

        private void ViewportDestroyed(object sender, IViewport viewport)
        {
            lock (_lock)
            {
                if (!_viewportRenderables.ContainsKey(viewport.ID)) return;

                var gr = _viewportRenderables[viewport.ID];
                _viewportRenderables.Remove(viewport.ID);
                _engine.Value.Remove(gr);
                gr.Dispose();
            }
        }

        // Grid handling

        private Task UpdateGrid(MapDocument md)
        {
            lock (_lock)
            {
                foreach (var gr in _viewportRenderables.Values)
                {
                    gr.SetGrid(md);
                }
            }

            return Task.FromResult(0);
        }
    }
}
