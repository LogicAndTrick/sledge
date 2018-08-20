using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Converters;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.Rendering.Engine;

namespace Sledge.BspEditor.Rendering.Scene
{
    /// <summary>
    /// The entry point for the rendering infrastructure.
    /// Handles when map documents are opened and closed, changed, and activated.
    /// </summary>
    [Export(typeof(IStartupHook))]
    public class SceneManager : IStartupHook
    {
        [Import] private Lazy<MapObjectConverter> _converter;
        [Import] private Lazy<EngineInterface> _engine;

        private readonly object _lock = new object();
        private SceneBuilder _sceneBuilder;

        /// <inheritdoc />
        public Task OnStartup()
        {
            Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated);
            Oy.Subscribe<IDocument>("Document:Closed", DocumentClosed);
            Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged);

            return Task.FromResult(0);
        }

        private WeakReference<MapDocument> _activeDocument = new WeakReference<MapDocument>(null);

        // Document events

        private async Task DocumentChanged(Change change)
        {
            if (_activeDocument.TryGetTarget(out var md) && change.Document == md)
            {
                if (change.AffectedData.Any(x => x.AffectsRendering))
                {
                    await UpdateScene(change.Document, null);
                }
                else if (change.HasObjectChanges)
                {
                    await UpdateScene(change.Document, change.Added.Union(change.Updated).Union(change.Removed));
                }
            }
        }

        private async Task DocumentActivated(IDocument doc)
        {
            var md = doc as MapDocument;
            _activeDocument = new WeakReference<MapDocument>(md);
            await UpdateScene(md, null);
        }

        private async Task DocumentClosed(IDocument doc)
        {
            if (_activeDocument.TryGetTarget(out var md) && md == doc)
            {
                await UpdateScene(null, null);
            }
        }

        // Scene handling

        private Task UpdateScene(MapDocument md, IEnumerable<IMapObject> affected)
        {
            lock (_lock)
            {
                if (_sceneBuilder == null)
                {
                    _sceneBuilder = new SceneBuilder(_engine.Value);
                    _engine.Value.Add(_sceneBuilder.SceneBuilderRenderable);
                    affected = null;
                }

                using (_engine.Value.Pause())
                {
                    if (affected == null || md == null)
                    {
                        _sceneBuilder.Clear();
                    }

                    if (md != null)
                    {
                        _converter.Value.Convert(md, _sceneBuilder, affected).Wait();
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
