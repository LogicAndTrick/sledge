using System;
using System.ComponentModel.Composition;
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
                await UpdateScene(change.Document);
            }
        }

        private async Task DocumentActivated(IDocument doc)
        {
            var md = doc as MapDocument;
            _activeDocument = new WeakReference<MapDocument>(md);
            await UpdateScene(md);
        }

        private async Task DocumentClosed(IDocument doc)
        {
            if (_activeDocument.TryGetTarget(out var md) && md == doc)
            {
                await UpdateScene(null);
            }
        }

        // Scene handling

        private async Task UpdateScene(MapDocument md)
        {
            SceneBuilder builder = null;
            if (md != null)
            {
                builder = await _converter.Value.Convert(md, md.Map.Root.FindAll());
            }

            lock (_lock)
            {
                if (builder != null)
                {
                    _engine.Value.Add(builder.MainRenderable);
                    builder.Renderables.ForEach(x => _engine.Value.Add(x));
                }

                if (_sceneBuilder != null)
                {
                    _engine.Value.Remove(_sceneBuilder.MainRenderable);
                    _sceneBuilder.Renderables.ForEach(x => _engine.Value.Remove(x));
                    _sceneBuilder.Dispose();
                }

                _sceneBuilder = builder;
            }
        }
    }
}
