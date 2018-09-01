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
        private long _currentGroup;

        public BufferBuilder MainBuffer => _bufferBuilders[_currentGroup];

        public IEnumerable<BufferBuilder> BufferBuilders => _bufferBuilders.Values;
        public IRenderable SceneBuilderRenderable { get; }

        public SceneBuilder(EngineInterface engine)
        {
            _engine = engine;
            _bufferBuilders = new Dictionary<long, BufferBuilder>();
            SceneBuilderRenderable = new SceneBuilderRenderable(this);
            SetCurrentGroup(0);
        }

        public void Clear()
        {
            foreach (var bb in _bufferBuilders) bb.Value.Dispose();
            _bufferBuilders.Clear();
            SetCurrentGroup(0);
        }

        public void DeleteGroup(long group)
        {
            if (_bufferBuilders.ContainsKey(group))
            {
                var bb = _bufferBuilders[group];
                bb.Dispose();
                _bufferBuilders.Remove(group);
            }

            if (_currentGroup == group)
            {
                if (_bufferBuilders.Any()) _currentGroup = _bufferBuilders.Keys.First();
                else SetCurrentGroup(0);
            }
        }

        public void SetCurrentGroup(long group)
        {
            if (!_bufferBuilders.ContainsKey(group)) _bufferBuilders[group] = _engine.CreateBufferBuilder(BufferSize.Medium);
            _currentGroup = group;
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
        }
    }
}