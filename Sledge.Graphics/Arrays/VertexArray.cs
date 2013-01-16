using System;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Graphics.Arrays
{
    public class VertexArrayByte : VertexBuffer<byte>
    {
        public VertexArrayByte(ArraySpecification specification, BeginMode[] modes, int count, byte[] array, short[][] elementArrays)
            : base(specification, modes, count, sizeof(byte), array, elementArrays)
        {
        }
    }
    public class VertexArrayFloat : VertexBuffer<float>
    {
        public VertexArrayFloat(ArraySpecification specification, BeginMode[] modes, int count, float[] array, short[][] elementArrays)
            : base(specification, modes, count, sizeof(float), array, elementArrays)
        {
        }
    }

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

    /// <summary>
    /// A vertex buffer is a wrapper around an OpenGL object/element array set.
    /// Only one object array is allowed however multiple element arrays are allowed.
    /// </summary>
    /// <typeparam name="T">The data type of the vertex array</typeparam>
    public class VertexBuffer<T> : IDisposable where T : struct 
    {
        public ArraySpecification Specification { get; private set; }
        public BeginMode[] Modes { get; private set; }
        public int Count { get; private set; }
        public int ElementArrayCount { get; private set; }

        public int ArrayID { get; private set; }
        public int[] ElementArrayIDs { get; private set; }
        private int _arrayLength;
        private readonly int[] _elementArrayLengths;
        private readonly int _typeSize;

        /// <summary>
        /// Create a new vertex buffer
        /// </summary>
        /// <param name="specification">The array specification</param>
        /// <param name="modes">The drawing modes of these arrays</param>
        /// <param name="count">The number of vertices</param>
        /// <param name="typeSize">The size of the type in bytes</param>
        /// <param name="array">The data array. Must follow the specification of this array</param>
        /// <param name="elementArrays">The element arrays</param>
        public VertexBuffer(ArraySpecification specification, BeginMode[] modes, int count, int typeSize, T[] array, short[][] elementArrays)
        {
            ElementArrayCount = elementArrays.Length;

            ElementArrayIDs = new int[ElementArrayCount];
            _elementArrayLengths = new int[ElementArrayCount];

            _typeSize = typeSize;

            Specification = specification;
            Modes = modes;

            Update(count, array, elementArrays);
        }

        /// <summary>
        /// Update the vertex array with a new vertex count, data array, and element array.
        /// </summary>
        /// <param name="count">The number of vertices</param>
        /// <param name="array">The data array. Must follow the specification of this array</param>
        /// <param name="elementArrays">The element arrays</param>
        /// <returns>True if the ids of the arrays changed</returns>
        public bool Update(int count, T[] array, short[][] elementArrays)
        {
            if (elementArrays.Length != ElementArrayCount) throw new Exception("The element array set must contain " + ElementArrayCount + " element arrays.");

            Count = count;
            int len, oaid = ArrayID;

            ArrayID = Update(BufferTarget.ArrayBuffer, ArrayID, array, _typeSize, _arrayLength, out len);
            _arrayLength = len;

            var changed = false;
            for (var i = 0; i < ElementArrayCount; i++)
            {
                var oldId = ElementArrayIDs[i];
                ElementArrayIDs[i] = Update(BufferTarget.ElementArrayBuffer, ElementArrayIDs[i], elementArrays[i], sizeof(short), _elementArrayLengths[i], out len);
                _elementArrayLengths[i] = len;
                changed |= oldId != ElementArrayIDs[i];
            }

            return oaid != ArrayID || changed;
        }

        /// <summary>
        /// Update a buffer
        /// </summary>
        /// <typeparam name="TType">The buffer array type</typeparam>
        /// <param name="target">The buffer target</param>
        /// <param name="id">The current ID of the buffer</param>
        /// <param name="array">The new data array</param>
        /// <param name="typeSize">The size of the data type</param>
        /// <param name="currentLength">The current length of the buffer</param>
        /// <param name="newLength">The new length of the buffer</param>
        /// <returns>The new ID of the buffer</returns>
        private static int Update<TType>(BufferTarget target, int id, TType[] array, int typeSize, int currentLength, out int newLength) where TType : struct
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
        public void DrawElements(int index)
        {
            DrawElements(index, 0, Count);
        }

        /// <summary>
        /// Draw a subset of the array
        /// </summary>
        /// <param name="index">The index of the element array to use</param>
        /// <param name="offset">The subset offset</param>
        /// <param name="count">The subset length</param>
        public void DrawElements(int index, int offset, int count)
        {
            GL.DrawElements(Modes[index], count, DrawElementsType.UnsignedShort, offset * sizeof(short));
        }

        /// <summary>
        /// Destroy the array
        /// </summary>
        public void Dispose()
        {
            var aid = ArrayID;
            GL.DeleteBuffers(1, ref aid);
            GL.DeleteBuffers(ElementArrayCount, ElementArrayIDs);
        }
    }
}