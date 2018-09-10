using System;
using System.Runtime.CompilerServices;
using Sledge.Rendering.Engine;
using Veldrid;

namespace Sledge.Rendering.Resources
{
    /// <summary>
    /// A wrapper around an indirect buffer which works when running DirectX10 by falling back to a loop over an array
    /// </summary>
    public class IndirectIndirectBuffer : IDisposable
    {
        public static bool UseFallback => !Features.IndirectBuffers;
        private static readonly int ArgSize = Unsafe.SizeOf<IndirectDrawIndexedArguments>();

        private readonly GraphicsDevice _device;

        private readonly DeviceBuffer _deviceBuffer;
        private readonly IndirectDrawIndexedArguments[] _arguments;

        public IndirectIndirectBuffer(GraphicsDevice device, int sizeInBytes)
        {
            _device = device;

            if (UseFallback)
            {
                _arguments = new IndirectDrawIndexedArguments[sizeInBytes / ArgSize];
            }
            else
            {
                _deviceBuffer = _device.ResourceFactory.CreateBuffer(new BufferDescription((uint) sizeInBytes, BufferUsage.IndirectBuffer));
            }
        }

        public void Update(long offset, IndirectDrawIndexedArguments arguments)
        {
            if (UseFallback)
            {
                _arguments[offset / ArgSize] = arguments;
            }
            else
            {
                _device.UpdateBuffer(_deviceBuffer, (uint) offset, arguments);
            }
        }

        public void Update(long offset, IndirectDrawIndexedArguments[] arguments)
        {
            if (UseFallback)
            {
                Array.Copy(arguments, 0, _arguments, offset / ArgSize, arguments.LongLength);
            }
            else
            {
                _device.UpdateBuffer(_deviceBuffer, (uint) offset, arguments);
            }
        }

        public void DrawIndexed(CommandList commandList, uint offset, uint count, uint stride)
        {
            if (UseFallback)
            {
                var offs = offset / ArgSize;
                for (var i = 0; i < count; i++)
                {
                    var arg = _arguments[i + offs];
                    commandList.DrawIndexed(arg.IndexCount, arg.InstanceCount, arg.FirstIndex, arg.VertexOffset, arg.FirstInstance);
                }
            }
            else
            {
                commandList.DrawIndexedIndirect(_deviceBuffer, offset, count, stride);
            }
        }

        public void Dispose()
        {
            _deviceBuffer?.Dispose();
        }
    }
}