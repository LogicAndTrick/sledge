using System;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Graphics.Arrays
{
    public class VertexArray : IDisposable
    {
        public int ElementArrayID { get; private set; }
        public int ArrayID { get; private set; }
        public int ArrayObjectID { get; private set; }
        public ArraySpecification Specification { get; private set; }
        public byte[] Buffer { get; private set; }
        public short[] Indices { get; private set; }
        public BeginMode Mode { get; private set; }
        public int Count { get; private set; }

        public VertexArray(ArraySpecification specification, BeginMode mode, int count, byte[] buffer, short[] indices)
        {
            Specification = specification;
            Buffer = buffer;
            Indices = indices;
            Mode = mode;
            Count = count;

            // Generate IDs
            int id;
            GL.GenBuffers(1, out id);
            ArrayID = id;
            GL.GenBuffers(1, out id);
            ElementArrayID = id;
            GL.GenVertexArrays(1, out id);
            ArrayObjectID = id;

            // Create buffers
            GL.BindBuffer(BufferTarget.ArrayBuffer, ArrayID);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(Buffer.Length), Buffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementArrayID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(sizeof(short) * Indices.Length), Indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            // Create VAO
            GL.BindVertexArray(ArrayObjectID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, ArrayID);
            var stride = Specification.Stride;
            for (var i = 0; i < Specification.Indices.Count; i++)
            {
                var ai = Specification.Indices[i];
                GL.EnableVertexAttribArray(i);
                GL.VertexAttribPointer(i, ai.Length, ai.Type, false, stride, ai.Offset);
            }
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementArrayID);
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void DrawElements()
        {
            DrawElements(0, Count);
        }

        public void DrawElements(int offset, int count)
        {
            GL.BindVertexArray(ArrayObjectID);
            GL.DrawElements(Mode, count, DrawElementsType.UnsignedShort, offset);
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            GL.DeleteBuffers(2, new[] {ArrayID, ElementArrayID});
        }
    }
}