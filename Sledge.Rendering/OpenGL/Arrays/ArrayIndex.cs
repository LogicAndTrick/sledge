using System;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Rendering.OpenGL.Arrays
{
    /// <summary>
    /// An array index is a point within an array specification.
    /// It defines the name (for display purposes), type, length, size, and offset of an array data point.
    /// </summary>
    public class ArrayIndex
    {
        public static ArrayIndex Float(string name, int length = 1)
        {
            return new ArrayIndex(name, VertexAttribPointerType.Float, length);
        }

        public static ArrayIndex Integer(string name, int length = 1)
        {
            return new ArrayIndex(name, VertexAttribPointerType.Int, 1);
        }

        public string Name { get; private set; }
        public bool IsIntegerType { get; private set; }
        public VertexAttribPointerType Type { get; private set; }
        public VertexAttribIntegerType IntegerType { get; private set; }
        public int Length { get; private set; }
        public int Size { get; private set; }
        public bool Normalised { get; set; }
        public int Offset { get; set; }

        public ArrayIndex(string name, VertexAttribPointerType type, int length)
        {
            IsIntegerType = false;
            Name = name;
            Type = type;
            Length = length;
            Size = GetSize(type) * length;
            Normalised = false;
        }

        public ArrayIndex(string name, VertexAttribIntegerType type, int length)
        {
            IsIntegerType = true;
            Name = name;
            IntegerType = type;
            Length = length;
            Size = GetSize(type) * length;
            Normalised = false;
        }

        private static int GetSize(VertexAttribIntegerType type)
        {
            switch (type)
            {
                case VertexAttribIntegerType.Byte:
                case VertexAttribIntegerType.UnsignedByte:
                    return sizeof (byte);
                case VertexAttribIntegerType.Short:
                    return sizeof(short);
                case VertexAttribIntegerType.UnsignedShort:
                    return sizeof(ushort);
                case VertexAttribIntegerType.Int:
                    return sizeof(int);
                case VertexAttribIntegerType.UnsignedInt:
                    return sizeof(uint);
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        private static int GetSize(VertexAttribPointerType type)
        {
            switch (type)
            {
                case VertexAttribPointerType.Byte:
                case VertexAttribPointerType.UnsignedByte:
                    return sizeof(byte);
                case VertexAttribPointerType.Short:
                    return sizeof(short);
                case VertexAttribPointerType.UnsignedShort:
                    return sizeof(ushort);
                case VertexAttribPointerType.Int:
                    return sizeof(int);
                case VertexAttribPointerType.UnsignedInt:
                    return sizeof(uint);
                case VertexAttribPointerType.Float:
                    return sizeof(float);
                case VertexAttribPointerType.Double:
                    return sizeof(double);
                case VertexAttribPointerType.HalfFloat:
                    return sizeof(float) / 2;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }
    }
}