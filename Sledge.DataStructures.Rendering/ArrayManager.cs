using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.MapObjects;
using Sledge.Graphics.Arrays;

namespace Sledge.DataStructures.Rendering
{
    public class ArrayManager
    {

    }

    public class SolidVertexArray
    {
        private static readonly ArraySpecification Specification;

        static SolidVertexArray()
        {
            Specification = new ArraySpecification(
                ArrayIndex.Vector3("Position"),
                ArrayIndex.Vector3("Normal"),
                ArrayIndex.Vector2("Texture"));
        }

        private const int BytesPerVertex = sizeof(float);
        private VertexArray _array;
        private Solid _solid;

        public SolidVertexArray(Solid solid)
        {
            _solid = solid;
            byte[] array;
            short[] indices;
            int count;
            GetArrayData(out count, out array, out indices);

            _array = new VertexArray(Specification, BeginMode.Triangles, count, array, indices);
        }

        private void GetArrayData(out int count, out byte[] array, out short[] indices)
        {
            var size = (int)Math.Ceiling(_solid.Faces.Sum(x => x.Vertices.Count) * 1.1f); // 10% flexible space
            var indexList = new List<short>();
            var c = 0;
            using(var ms = new MemoryStream(size * Specification.Stride))
            {
                using (var bw = new BinaryWriter(ms))
                {
                    foreach (var face in _solid.Faces)
                    {
                        foreach (var vert in face.Vertices)
                        {
                            bw.Write((float)vert.Location.X);
                            bw.Write((float)vert.Location.Y);
                            bw.Write((float)vert.Location.Z);
                            bw.Write((float)face.Plane.Normal.X);
                            bw.Write((float)face.Plane.Normal.Y);
                            bw.Write((float)face.Plane.Normal.Z);
                            bw.Write((float)vert.TextureU);
                            bw.Write((float)vert.TextureV);
                        }
                        var start = c;
                        for (short i = 1; i < face.Vertices.Count - 1; i++)
                        {
                            indexList.Add((short) start);
                            indexList.Add((short) (start + i));
                            indexList.Add((short) (start + i + 1));
                            c += 3;
                        }
                    }
                    array = ms.ToArray();
                    indices = indexList.ToArray();
                    count = c;
                }
            }
        }
    }
}
