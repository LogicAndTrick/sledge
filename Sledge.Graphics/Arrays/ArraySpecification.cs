using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Graphics.Arrays
{
    /// <summary>
    /// An array specification collects array indices into a structure ready for computation on the VBO layer.
    /// </summary>
    public class ArraySpecification
    {
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
                switch (field.FieldType.Name)
                {
                    case "Vector2":
                        Indices.Add(ArrayIndex.Vector2(field.Name));
                        break;
                    case "Vector3":
                        Indices.Add(ArrayIndex.Vector3(field.Name));
                        break;
                    case "Vector4":
                    case "Color4":
                        Indices.Add(ArrayIndex.Vector4(field.Name));
                        break;
                    case "Single":
                        Indices.Add(ArrayIndex.Float(field.Name));
                        break;
                    case "Int32":
                        Indices.Add(ArrayIndex.Integer(field.Name));
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
