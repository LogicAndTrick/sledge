using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.MapObjects;
using Sledge.Graphics.Arrays;

namespace Sledge.DataStructures.Rendering
{
    public class SolidVertexArray
    {

        private static readonly BeginMode[] Modes;
        private static readonly ArraySpecification Specification;
        private const int SpecSize = 11;

        static SolidVertexArray()
        {
            Modes = new[] { BeginMode.Triangles, BeginMode.Lines};
            Specification = new ArraySpecification(
                ArrayIndex.Vector3("Position"),
                ArrayIndex.Vector3("Normal"),
                ArrayIndex.Vector2("Texture"),
                ArrayIndex.Vector3("Colour"));
        }

        public VertexArrayFloat Array { get; private set; }
        public List<SolidVertexArraySubset> Subsets { get; private set; } 

        public SolidVertexArray(Solid solid)
        {
            float[] array;
            short[] indices;
            short[] wireframeIndices;
            int count;
            Subsets = new List<SolidVertexArraySubset>();
            GetArrayData(solid, out count, out array, out indices, out wireframeIndices, Subsets);

            Array = new VertexArrayFloat(Specification, Modes, count, array, new[] { indices, wireframeIndices});
        }

        public void Update(Solid solid)
        {
            float[] array;
            short[] indices;
            int count;
            Subsets.Clear();
            short[] wireframeIndices;
            GetArrayData(solid, out count, out array, out indices, out wireframeIndices, Subsets);
            Array.Update(count, array, new[] { indices, wireframeIndices });
        }

        private void GetArrayData(Solid solid, out int count, out float[] array, out short[] indices, out short[] wireframeIndices, ICollection<SolidVertexArraySubset> subsets)
        {
            var indexList = new List<short>();
            var wireframeIndexList = new List<short>();
            var index = 0;
            var idx = 0;
            array = new float[SpecSize * solid.Faces.Sum(x => x.Vertices.Count)];
            var subsetStart = 0;
            foreach (var group in solid.Faces.GroupBy(x => new { Selected = solid.IsSelected || x.IsSelected, x.Texture.Texture }))
            {
                foreach (var face in group)
                {
                    float nx = (float) face.Plane.Normal.DX,
                          ny = (float) face.Plane.Normal.DY,
                          nz = (float) face.Plane.Normal.DZ;
                    float r = face.Colour.R / 255f,
                          g = face.Colour.G / 255f,
                          b = face.Colour.B / 255f;
                    foreach (var vert in face.Vertices)
                    {
                        array[idx++] = ((float) vert.Location.DX);
                        array[idx++] = ((float) vert.Location.DY);
                        array[idx++] = ((float) vert.Location.DZ);
                        array[idx++] = (nx);
                        array[idx++] = (ny);
                        array[idx++] = (nz);
                        array[idx++] = ((float) vert.TextureU);
                        array[idx++] = ((float) vert.TextureV);
                        array[idx++] = (r);
                        array[idx++] = (g);
                        array[idx++] = (b);
                    }
                    wireframeIndexList.Add((short) index);
                    wireframeIndexList.Add((short) (index + 1));
                    wireframeIndexList.Add((short) (index + 1));
                    for (short i = 1; i < face.Vertices.Count - 1; i++)
                    {
                        wireframeIndexList.Add((short) (index + i + 1));
                        wireframeIndexList.Add((short) (index + i + 1));

                        indexList.Add((short) index);
                        indexList.Add((short) (index + i));
                        indexList.Add((short) (index + i + 1));
                    }
                    wireframeIndexList.Add((short)index);
                    index += face.Vertices.Count;
                }

                subsets.Add(new SolidVertexArraySubset(this, subsetStart,
                                              indexList.Count - subsetStart,
                                              group.Key.Texture,
                                              group.Key.Selected));
                subsetStart = indexList.Count;
            }
            indices = indexList.ToArray();
            wireframeIndices = wireframeIndexList.ToArray();
            count = indices.Length;
        }
    }
}