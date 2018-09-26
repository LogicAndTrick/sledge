using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Converters;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Resources;
using Sledge.Shell.Registers;

namespace Sledge.BspEditor.Rendering.Scene
{
    /// <summary>
    /// The entry point for the rendering infrastructure.
    /// Handles when map documents are opened and closed, changed, and activated.
    /// </summary>
    [Export(typeof(IStartupHook))]
#if DEBUG_EXTRA
    [Export]
#endif
    public class SceneManager : IStartupHook
    {
        private readonly Lazy<MapObjectConverter> _converter;
        private readonly Lazy<EngineInterface> _engine;
        private readonly DocumentRegister _documentRegister;
        private readonly ResourceCollection _resourceCollection;

        private readonly object _lock = new object();
        private SceneBuilder _sceneBuilder;

        private WeakReference<MapDocument> _activeDocument = new WeakReference<MapDocument>(null);

        [ImportingConstructor]
        public SceneManager(
            [Import] Lazy<MapObjectConverter> converter,
            [Import] Lazy<EngineInterface> engine,
            [Import] DocumentRegister documentRegister,
            [Import] ResourceCollection resourceCollection
        )
        {
            _converter = converter;
            _engine = engine;
            _documentRegister = documentRegister;
            _resourceCollection = resourceCollection;
        }

        /// <inheritdoc />
        public Task OnStartup()
        {
            Oy.Subscribe<object>("SettingsChanged", SettingsChanged);
            Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated);
            Oy.Subscribe<IDocument>("Document:Closed", DocumentClosed);
            Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged);

            return Task.FromResult(0);
        }

        public List<List<BufferBuilder.BufferAllocation>> GetCurrentAllocationInformation()
        {
            return _sceneBuilder?.BufferBuilders.Select(x => x.AllocationInformation).ToList();
        }

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

        private async Task SettingsChanged(object o)
        {
            var doc = _activeDocument.TryGetTarget(out var md) ? md : null;
            await UpdateScene(doc, null);
        }

        private async Task DocumentActivated(IDocument doc)
        {
            var md = doc as MapDocument;
            _activeDocument = new WeakReference<MapDocument>(md);
            await UpdateScene(md, null);
        }

        private async Task DocumentClosed(IDocument doc)
        {
            var envs = _documentRegister.OpenDocuments.OfType<MapDocument>().Select(x => x.Environment).ToHashSet();
            _resourceCollection.DisposeOtherEnvironments(envs);
            if (_activeDocument.TryGetTarget(out var md) && md == doc)
            {
                await UpdateScene(null, null);
            }
        }

        // Scene handling

        private Task UpdateScene(MapDocument md, IEnumerable<IMapObject> affected)
        {
            var waitTask = Task.CompletedTask;
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
                        foreach (var r in _sceneBuilder.GetAllRenderables())
                        {
                            _engine.Value.Remove(r);
                            if (r is IUpdateable u) _engine.Value.Remove(u);
                        }
                        _sceneBuilder.Clear();
                    }

                    if (md != null)
                    {
                        var resourceCollector = new ResourceCollector();
                        waitTask = _converter.Value.Convert(md, _sceneBuilder, affected, resourceCollector)
                            .ContinueWith(t => HandleResources(md.Environment, resourceCollector));
                    }
                }
            }

            return waitTask;
        }

        private async Task HandleResources(IEnvironment environment, ResourceCollector resources)
        {
            var add = resources.GetRenderablesToAdd().ToHashSet();
            var rem = resources.GetRenderablesToRemove().ToHashSet();

            foreach (var r in add) _engine.Value.Add(r);
            foreach (var r in add.OfType<IUpdateable>()) _engine.Value.Add(r);

            foreach (var r in rem.OfType<IUpdateable>()) _engine.Value.Remove(r);
            foreach (var r in rem) _engine.Value.Remove(r);

            await _resourceCollection.Upload(environment, resources);
        }
    }
}
