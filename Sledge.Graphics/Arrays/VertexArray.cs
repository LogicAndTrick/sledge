using System;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Graphics.Arrays
{
    public class VertexArray<T> : IDisposable where T : struct
    {
        private readonly VertexBuffer<T> _buffer;
        private int[] _arrays;

        public VertexArray(VertexBuffer<T> buffer)
        {
            _buffer = buffer;
            CreateArrays();
        }

        public void CreateArrays()
        {
            if (_arrays != null) GL.DeleteVertexArrays(_buffer.ElementArrayCount, _arrays);
            else _arrays = new int[_buffer.ElementArrayCount];
            GL.GenVertexArrays(_buffer.ElementArrayCount, _arrays);

            for (var i = 0; i < _buffer.ElementArrayCount; i++)
            {
                GL.BindVertexArray(_arrays[i]);
                GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer.ArrayID);
                var stride = _buffer.Specification.Stride;
                for (var j = 0; j < _buffer.Specification.Indices.Count; j++)
                {
                    var ai = _buffer.Specification.Indices[j];
                    GL.EnableVertexAttribArray(j);
                    GL.VertexAttribPointer(j, ai.Length, ai.Type, false, stride, ai.Offset);
                }
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _buffer.ElementArrayIDs[i]);
                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }
        }

        public void Dispose()
        {
            GL.DeleteVertexArrays(_buffer.ElementArrayCount, _arrays);
        }

        public void Bind(int index)
        {
            GL.BindVertexArray(_arrays[index]);
        }

        public static void Unbind()
        {
            GL.BindVertexArray(0);
        }
    }
}