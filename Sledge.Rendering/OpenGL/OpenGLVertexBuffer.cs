using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using Sledge.Graphics.Arrays;

namespace Sledge.Rendering.OpenGL
{
    /// <summary>
    /// A vertex buffer that can store arbitrary data. Keeps the object indices in memory so it can return the index for use in an element array.
    /// </summary>
    /// <remarks>
    /// The buffer starts at 1024 vertices and doubles in size until it reaches 4096,
    /// after which it increases by 4096 until it reaches the maximum size of 65536 elements.
    /// The buffer can shrink by the same values.
    /// </remarks>
    /// <typeparam name="T">The vertex type struct to store in the buffer. Must be valid for a <code>ArraySpecification</code> object.</typeparam>
    public class OpenGLVertexBuffer<T> : IDisposable where T : struct 
    {
        private ArraySpecification Specification { get; set; }
        public IEnumerable<T> Elements {get { return _elements.Keys; }}

        private readonly int _structSize;
        private readonly int _buffer;

        private ushort _nextIndex;
        private int _arraySize;
        private Dictionary<T, ushort> _elements;
        
        public OpenGLVertexBuffer()
        {
            Specification = new ArraySpecification(typeof(T));
            _structSize = Marshal.SizeOf(typeof(T));
            _buffer = GL.GenBuffer();

            _nextIndex = 0;
            _arraySize = 1024;
            _elements = new Dictionary<T, ushort>();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(_arraySize * _structSize), IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public ushort IndexOf(T obj)
        {
            return _elements[obj];
        }

        public void Bind()
        {
            // Note: Unlike buffers, VAOs cannot be shared across multiple opengl contexts!
            // That makes them really hard to deal with and probably not worth the effort...
            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);
            var stride = Specification.Stride;
            for (var j = 0; j < Specification.Indices.Count; j++)
            {
                var ai = Specification.Indices[j];
                GL.EnableVertexAttribArray(j);
                GL.VertexAttribPointer(j, ai.Length, ai.Type, false, stride, ai.Offset);
            }
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Update(IEnumerable<T> add, IEnumerable<T> remove)
        {
            foreach (var obj in remove)
            {
                _elements.Remove(obj);
            }

            var index = _nextIndex;
            var list = add.ToList();
            foreach (var obj in list)
            {
                _elements.Add(obj, _nextIndex);
                _nextIndex++;
            }
            if (_nextIndex == index) return;

            if (_nextIndex > _arraySize) Rebuild();
            else Append(index, list);
        }

        private void Append(int startIndex, List<T> list)
        {
            var d = list.ToArray();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);
            GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(startIndex * _structSize), new IntPtr(d.Length * _structSize), d);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void Rebuild()
        {
            var data = _elements.OrderBy(x => x.Value).Select(x => x.Key).ToArray();
            if (_arraySize < data.Length) _arraySize = GetNewSize(data.Length);
            _elements = data.Select((x, i) => new {x, i}).ToDictionary(x => x.x, x => (ushort) x.i);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(_arraySize * _structSize), data, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private static int GetNewSize(int required)
        {
            var ok = 1024;
            while (ok < required)
            {
                if (ok < 4096) ok *= 2;
                else ok += 4096;
            }
            return ok;
        }

        public void Dispose()
        {
            _elements.Clear();
            GL.DeleteBuffer(_buffer);
        }
    }
}