using System;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Graphics.Arrays
{
    public class VertexArray : IDisposable
    {
        public ArraySpecification Specification { get; private set; }
        public BeginMode Mode { get; private set; }
        public int Count { get; private set; }

        private readonly int _vertexArrayID;

        private int _elementArrayID;
        private int _arrayID;
        private int _arrayLength;
        private int _elementArrayLength;

        /// <summary>
        /// Create a new vertex array
        /// </summary>
        /// <param name="specification">The array specification</param>
        /// <param name="mode">The drawing mode of this array</param>
        /// <param name="count">The number of vertices</param>
        /// <param name="array">The data array. Must follow the specification of this array</param>
        /// <param name="elementArray">The element array</param>
        public VertexArray(ArraySpecification specification, BeginMode mode, int count, byte[] array, short[] elementArray)
        {
            int id;
            GL.GenVertexArrays(1, out id);
            _vertexArrayID = id;

            Specification = specification;
            Mode = mode;

            Update(count, array, elementArray);
        }

        /// <summary>
        /// Update the vertex array with a new vertex count, data array, and element array.
        /// </summary>
        /// <param name="count">The number of vertices</param>
        /// <param name="array">The data array. Must follow the specification of this array</param>
        /// <param name="elementArray">The element array</param>
        public void Update(int count, byte[] array, short[] elementArray)
        {
            Count = count;
            int len, oaid = _arrayID, oeaid = _elementArrayID;

            _arrayID = Update(BufferTarget.ArrayBuffer, _arrayID, array, sizeof(byte), _arrayLength, out len);
            _arrayLength = len;

            _elementArrayID = Update(BufferTarget.ElementArrayBuffer, _elementArrayID, elementArray, sizeof(short), _elementArrayLength, out len);
            _elementArrayLength = len;

            if (oaid == _arrayID && oeaid == _elementArrayID) return;

            // Create VAO
            GL.BindVertexArray(_vertexArrayID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _arrayID);
            var stride = Specification.Stride;
            for (var i = 0; i < Specification.Indices.Count; i++)
            {
                var ai = Specification.Indices[i];
                GL.EnableVertexAttribArray(i);
                GL.VertexAttribPointer(i, ai.Length, ai.Type, false, stride, ai.Offset);
            }
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementArrayID);
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        /// <summary>
        /// Update a buffer
        /// </summary>
        /// <typeparam name="T">The buffer array type</typeparam>
        /// <param name="target">The buffer target</param>
        /// <param name="id">The current ID of the buffer</param>
        /// <param name="array">The new data array</param>
        /// <param name="typeSize">The size of the data type</param>
        /// <param name="currentLength">The current length of the buffer</param>
        /// <param name="newLength">The new length of the buffer</param>
        /// <returns>The new ID of the buffer</returns>
        private static int Update<T>(BufferTarget target, int id, T[] array, int typeSize, int currentLength, out int newLength) where T : struct
        {
            if (id > 0 && array.Length <= currentLength)
            {
                GL.BindBuffer(target, id);
                GL.BufferSubData(target, IntPtr.Zero, new IntPtr(array.Length * typeSize), array);
                GL.BindBuffer(target, 0);
                newLength = currentLength;
            }
            else
            {
                if (id > 0) GL.DeleteBuffers(1, ref id);
                GL.GenBuffers(1, out id);

                GL.BindBuffer(BufferTarget.ArrayBuffer, id);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(array.Length * typeSize), array, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                newLength = array.Length;
            }
            return id;
        }

        /// <summary>
        /// Draw the entire array
        /// </summary>
        public void DrawElements()
        {
            DrawElements(0, Count);
        }

        /// <summary>
        /// Draw a subset of the array
        /// </summary>
        /// <param name="offset">The subset offset</param>
        /// <param name="count">The subset length</param>
        public void DrawElements(int offset, int count)
        {
            GL.BindVertexArray(_vertexArrayID);
            GL.DrawElements(Mode, count, DrawElementsType.UnsignedShort, offset);
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Destroy the array
        /// </summary>
        public void Dispose()
        {
            GL.DeleteVertexArrays(1, new[] {_vertexArrayID});
            GL.DeleteBuffers(2, new[] {_arrayID, _elementArrayID});
        }
    }
}