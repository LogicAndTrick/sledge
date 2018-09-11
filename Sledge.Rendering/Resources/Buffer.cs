using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Veldrid;

namespace Sledge.Rendering.Resources
{
    public class Buffer : IDisposable
    {
        private readonly GraphicsDevice _device;
        private bool _created;

        internal DeviceBuffer VertexBuffer { get; private set; }
        internal DeviceBuffer IndexBuffer { get; private set; }

        public int VertexCount { get; private set; }
        public int IndexCount { get; private set; }

        internal Buffer(GraphicsDevice device)
        {
            _device = device;
            _created = false;
        }

        public void Bind(CommandList cl, uint slot)
        {
            if (!_created) return;
            cl.SetVertexBuffer(slot, VertexBuffer);
            cl.SetIndexBuffer(IndexBuffer, IndexFormat.UInt32);
        }
        
        public void Update<T>(IEnumerable<T> vertices, IEnumerable<uint> indices) where T : struct
        {
            var verts = vertices.ToArray();
            var index = indices.ToArray();

            if (verts.Length == 0 || index.Length == 0) return;

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
