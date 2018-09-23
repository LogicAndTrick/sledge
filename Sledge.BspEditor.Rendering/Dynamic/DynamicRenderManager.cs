using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Resources;

namespace Sledge.BspEditor.Rendering.Dynamic
{
    [Export(typeof(IStartupHook))]
    public class DynamicRenderManager : IStartupHook, IUpdateable
    {
        [Import] private Lazy<EngineInterface> _engine;
        [ImportMany] private IDynamicRenderable[] _dynamicRenderables;
        [ImportMany] private IMapObjectDynamicRenderable[] _documentDynamicRenderables;
        [Import] private ResourceCollection _resourceCollection;

        private BufferBuilder _builder;
        private BufferBuilderRenderable _renderable;

        /// <inheritdoc />
        public Task OnStartup()
        {
            Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated);
            Oy.Subscribe<IDocument>("Document:Closed", DocumentClosed);

            _engine.Value.Add(this);

            return Task.FromResult(0);
        }

        private readonly WeakReference<MapDocument> _activeDocument = new WeakReference<MapDocument>(null);

        public void Update(long frame)
        {
            var builder = _engine.Value.CreateBufferBuilder(BufferSize.Small);
            var renderable = new BufferBuilderRenderable(builder);
            
            if (_activeDocument.TryGetTarget(out var md))
            {
                var resourceCollector = new ResourceCollector();

                foreach (var dr in _dynamicRenderables)
                {
                    dr.Render(builder, resourceCollector);
                }

                foreach (var ddr in _documentDynamicRenderables)
                {
                    ddr.Render(md, builder, resourceCollector);
                }

                var env = md.Environment;
                if (env != null) Task.Run(() => _resourceCollection.Upload(env, resourceCollector)).Wait();
            }

            builder.Complete();
            _engine.Value.Add(renderable);

            if (_builder != null)
            {
                _engine.Value.Remove(_renderable);
                _renderable.Dispose();
                _builder.Dispose();
            }

            _builder = builder;
            _renderable = renderable;
        }

        // Document events

        private Task DocumentActivated(IDocument doc)
        {
            _activeDocument.SetTarget(doc as MapDocument);
            return Task.CompletedTask;
        }

        private Task DocumentClosed(IDocument doc)
        {
            if (_activeDocument.TryGetTarget(out var md) && md == doc)
            {
                _activeDocument.SetTarget(null);
            }
            return Task.CompletedTask;
        }
    }
}
