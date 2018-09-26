using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Resources;

namespace Sledge.BspEditor.Rendering.Scene
{
    public class SceneBuilder : IDisposable
    {
        private readonly EngineInterface _engine;

        private readonly Dictionary<long, BufferBuilder> _bufferBuilders;
        public IEnumerable<BufferBuilder> BufferBuilders => _bufferBuilders.Values;
        public IRenderable SceneBuilderRenderable { get; }

        private readonly Dictionary<long, HashSet<IRenderable>> _renderables;

        public SceneBuilder(EngineInterface engine)
        {
            _engine = engine;
            _bufferBuilders = new Dictionary<long, BufferBuilder>();
            _renderables = new Dictionary<long, HashSet<IRenderable>>();

            SceneBuilderRenderable = new SceneBuilderRenderable(this);
        }

        public void AddRenderablesToGroup(long group, IEnumerable<IRenderable> renderables)
        {
            EnsureGroupExists(group);
            _renderables[group].UnionWith(renderables);
        }

        public void RemoveRenderablesFromGroup(long group, IEnumerable<IRenderable> renderables)
        {
            EnsureGroupExists(group);
            _renderables[group].ExceptWith(renderables);
        }

        public IEnumerable<IRenderable> GetRenderablesForGroup(long group)
        {
            if (_renderables.ContainsKey(group)) return _renderables[group];
            return new IRenderable[0];
        }

        public IEnumerable<IRenderable> GetAllRenderables() => _renderables.Values.SelectMany(x => x);

        public BufferBuilder GetBufferForGroup(long group)
        {
            return _bufferBuilders.ContainsKey(group) ? _bufferBuilders[group] : null;
        }

        public void Clear()
        {
            foreach (var bb in _bufferBuilders) bb.Value.Dispose();
            _bufferBuilders.Clear();
            _renderables.Clear();
        }

        public void DeleteGroup(long group)
        {
            if (_bufferBuilders.ContainsKey(group))
            {
                var bb = _bufferBuilders[group];
                bb.Dispose();
                _bufferBuilders.Remove(group);
            }

            if (_renderables.ContainsKey(group))
            {
                _renderables.Remove(group);
            }
        }

        public void EnsureGroupExists(long group)
        {
            if (!_bufferBuilders.ContainsKey(group)) _bufferBuilders[group] = _engine.CreateBufferBuilder(BufferSize.Medium);
            if (!_renderables.ContainsKey(group)) _renderables[group] = new HashSet<IRenderable>();
        }

        public void Complete()
        {
            foreach (var bb in _bufferBuilders)
            {
                bb.Value.Complete();
            }
        }

        public void Dispose()
        {
            SceneBuilderRenderable.Dispose();
            foreach (var bb in _bufferBuilders)
            {
                bb.Value.Dispose();
            }
            _bufferBuilders.Clear();
            _renderables.Clear();
        }
    }
}