using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Veldrid;

namespace Sledge.Rendering.Renderables
{
    /// <summary>
    /// A buffer represents both a vertex buffer and an index buffer and a collection of renderables that use this buffer.
    /// </summary>
    public class Buffer : IDisposable
    {
        private readonly GraphicsDevice _device;
        private bool _created;

        internal DeviceBuffer VertexBuffer { get; private set; }
        internal DeviceBuffer IndexBuffer { get; private set; }

        public int VertexCount { get; private set; }
        public int IndexCount { get; private set; }

        private readonly List<IRenderable> _renderables;

        internal Buffer(GraphicsDevice device)
        {
            _device = device;
            _renderables = new List<IRenderable>();
            _created = false;
        }
        
        public void Update<T>(IEnumerable<T> vertices, IEnumerable<uint> indices) where T : struct
        {
            var verts = vertices.ToArray();
            var index = indices.ToArray();

            var vsize = (uint) (verts.Length * Unsafe.SizeOf<T>());
            var isize = (uint) index.Length * sizeof(uint);

            if (VertexBuffer == null || VertexBuffer.SizeInBytes < vsize)
            {
                VertexBuffer?.Dispose();
                VertexBuffer = _device.ResourceFactory.CreateBuffer(new BufferDescription(vsize, BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            }
            if (IndexBuffer == null || IndexBuffer.SizeInBytes < isize)
            {
                IndexBuffer?.Dispose();
                IndexBuffer = _device.ResourceFactory.CreateBuffer(new BufferDescription(isize, BufferUsage.IndexBuffer | BufferUsage.Dynamic));
            }

            _created = true;
            
            _device.UpdateBuffer(VertexBuffer, 0, verts);
            _device.UpdateBuffer(IndexBuffer, 0, index);

            VertexCount = verts.Length;
            IndexCount = index.Length;
        }

        public void Attach(IRenderable renderable) => _renderables.Add(renderable);
        public void Detatch(IRenderable renderable) => _renderables.Remove(renderable);
        public bool HasRenderables() => _renderables.Any();

        public virtual void Dispose()
        {
            if (_created)
            {
                VertexBuffer.Dispose();
                IndexBuffer.Dispose();
            }
        }
    }
}
