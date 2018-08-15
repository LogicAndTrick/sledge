using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Sledge.Rendering.Pipelines;
using Veldrid;

namespace Sledge.Rendering.Renderables
{
    public class BufferBuilder : IDisposable
    {
        private readonly GraphicsDevice _device;

        private bool _inBuffer;

        private DeviceBuffer _currentVertexBuffer;
        private MappedResource _currentVertexMap;
        private uint _currentVertexOffset;

        private DeviceBuffer _currentIndexBuffer;
        private MappedResource _currentIndexMap;
        private uint _currentIndexOffset;

        private List<(PipelineType, string, IndirectDrawIndexedArguments)> _currentIndirectArguments;

        public List<DeviceBuffer> VertexBuffers { get; }
        public List<DeviceBuffer> IndexBuffers { get; }
        public List<DeviceBuffer> IndirectBuffers { get; }
        public List<List<BufferGroup>> IndirectBufferGroups { get; }

        public int NumBuffers => VertexBuffers.Count;

        private const uint VertexBufferSize = 1 * 1024 * 1024; // 4mb
        private const uint IndexBufferSize = 1 * 1024 * 1024; // 1mb

        internal BufferBuilder(GraphicsDevice device)
        {
            _device = device;
            VertexBuffers = new List<DeviceBuffer>();
            IndexBuffers = new List<DeviceBuffer>();
            IndirectBuffers = new List<DeviceBuffer>();
            IndirectBufferGroups = new List<List<BufferGroup>>();
        }

        private void AllocateBuffer(uint vsize, uint isize)
        {
            if (_inBuffer)
            {
                if (_currentVertexOffset + vsize > _currentVertexBuffer.SizeInBytes ||
                    _currentIndexOffset + isize > _currentIndexBuffer.SizeInBytes)
                {
                    CommitCurrentBuffers();
                }
            }

            if (_inBuffer) return;

            _currentVertexBuffer = _device.ResourceFactory.CreateBuffer(new BufferDescription(VertexBufferSize, BufferUsage.Dynamic | BufferUsage.VertexBuffer));
            _currentIndexBuffer = _device.ResourceFactory.CreateBuffer(new BufferDescription(IndexBufferSize, BufferUsage.Dynamic | BufferUsage.IndexBuffer));
            _currentVertexMap = _device.Map(_currentVertexBuffer, MapMode.Write);
            _currentIndexMap = _device.Map(_currentIndexBuffer, MapMode.Write);
            _currentIndirectArguments = new List<(PipelineType, string, IndirectDrawIndexedArguments)>();
            _inBuffer = true;
        }

        private void CommitCurrentBuffers()
        {
            if (!_inBuffer) return;

            _device.Unmap(_currentVertexBuffer);
            _device.Unmap(_currentIndexBuffer);

            var numInd = _currentIndirectArguments.Count;
            var indSize = Unsafe.SizeOf<IndirectDrawIndexedArguments>();

            var bufferGroups = new List<BufferGroup>();
            var indirectBuffer = _device.ResourceFactory.CreateBuffer(new BufferDescription((uint) (numInd * indSize), BufferUsage.IndirectBuffer));
            uint indirOffset = 0;
            foreach (var g in _currentIndirectArguments.GroupBy(x => new { x.Item1, x.Item2 }))
            {
                var indir = g.Select(x => x.Item3).ToArray();
                var indirCount = (uint) indir.Length;

                _device.UpdateBuffer(indirectBuffer, (uint) (indirOffset * indSize), indir);

                var bg = new BufferGroup(g.Key.Item1, g.Key.Item2, indirOffset, indirCount);
                bufferGroups.Add(bg);

                indirOffset += indirCount;
            }
            
            VertexBuffers.Add(_currentVertexBuffer);
            IndexBuffers.Add(_currentIndexBuffer);
            IndirectBuffers.Add(indirectBuffer);
            IndirectBufferGroups.Add(bufferGroups);

            _currentIndexBuffer = _currentVertexBuffer = null;
            _currentVertexOffset = _currentIndexOffset = 0;
            _currentIndirectArguments = null;
            _inBuffer = false;
        }

        public void Append<T>(IEnumerable<T> vertices, IEnumerable<uint> indices, IEnumerable<BufferGroup> groups) where T : struct
        {
            var verts = vertices.ToArray();
            var index = indices.ToArray();

            var structSize = Unsafe.SizeOf<T>();

            var vsize = (uint)(verts.Length * structSize);
            var isize = (uint)index.Length * sizeof(uint);

            AllocateBuffer(vsize, isize);

            CopyToBuffer(_currentVertexMap, _currentVertexOffset, verts, vsize);
            CopyToBuffer(_currentIndexMap, _currentIndexOffset, index, isize);

            foreach (var bg in groups)
            {
                _currentIndirectArguments.Add((bg.Pipeline, bg.Binding, new IndirectDrawIndexedArguments
                {
                    IndexCount = bg.Count,
                    InstanceCount = 1,
                    FirstIndex = (_currentIndexOffset / 4) + bg.Offset,
                    VertexOffset = (int) (_currentVertexOffset / structSize),
                    FirstInstance = 0
                }));
                
            }

            _currentVertexOffset += vsize;
            _currentIndexOffset += isize;
        }

        public void Complete()
        {
            CommitCurrentBuffers();
        }

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        private static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        private void CopyToBuffer(MappedResource map, uint offset, Array data, uint size)
        {
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                var source = handle.AddrOfPinnedObject();
                var destination = IntPtr.Add(map.Data, (int) offset);
                CopyMemory(destination, source, size);
            }
            finally
            {
                if (handle.IsAllocated) handle.Free();
            }
        }

        public void Dispose()
        {
            VertexBuffers.ForEach(x => x.Dispose());
            IndexBuffers.ForEach(x => x.Dispose());
            IndirectBuffers.ForEach(x => x.Dispose());
        }
    }
}