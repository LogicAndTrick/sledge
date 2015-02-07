using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Rendering.OpenGL.Arrays
{
    /// <summary>
    /// An array specification collects array indices into a structure ready for computation on the VBO layer.
    /// </summary>
    public class ArraySpecification
    {
        [ArrayIndex(VertexAttribPointerType.UnsignedByte, 4, true)]
        public List<ArrayIndex> Indices { get; private set; }
        public int Stride { get; private set; }

        public ArraySpecification(params ArrayIndex[] indices)
        {
            Indices = indices.ToList();
            Stride = indices.Sum(x => x.Size);
            var offset = 0;
            foreach (var ai in indices)
            {
                ai.Offset = offset;
                offset += ai.Size;
            }
        }

        public ArraySpecification(Type t)
        {
            Indices = new List<ArrayIndex>();
            foreach (var field in t.GetFields())
            {
                var attr = field.GetCustomAttributes(typeof(ArrayIndexAttribute), false).OfType<ArrayIndexAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    Indices.Add(new ArrayIndex(field.Name, attr.Type, attr.Length) { Normalised = attr.Normalised });
                    continue;
                }
                switch (field.FieldType.Name)
                {
                    case "Vector2":
                        Indices.Add(ArrayIndex.Float(field.Name, 2));
                        break;
                    case "Vector3":
                        Indices.Add(ArrayIndex.Float(field.Name, 3));
                        break;
                    case "Vector4":
                    case "Color4":
                        Indices.Add(ArrayIndex.Float(field.Name, 4));
                        break;
                    case "Single":
                        Indices.Add(ArrayIndex.Float(field.Name));
                        break;
                    case "Int32":
                        Indices.Add(ArrayIndex.Integer(field.Name));
                        break;
                    case "Int16":
                        Indices.Add(new ArrayIndex(field.Name, VertexAttribPointerType.Short, 1));
                        break;
                    case "Byte":
                        Indices.Add(new ArrayIndex(field.Name, VertexAttribPointerType.UnsignedByte, 1));
                        break;
                }
            }
            Stride = Indices.Sum(x => x.Size);
            var offset = 0;
            foreach (var ai in Indices)
            {
                ai.Offset = offset;
                offset += ai.Size;
            }
        }
    }
}
