using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using Sledge.Common;
using Sledge.DataStructures.MapObjects;
using Sledge.Graphics.Arrays;

namespace Sledge.DataStructures.Rendering
{
    /// <summary>
    /// A decal face vertex array collects and stores a VBO for all decal faces in the map.
    /// Faces are grouped by texture and then split into subsets for optimised rendering later on.
    /// Decals are separated from the solid vertex array because extra decal faces can be added by
    /// simple translations, which would break partial updating.
    /// </summary>
    public class DecalFaceVertexArray
    {
        private static readonly BeginMode[] Modes;
        private static readonly ArraySpecification Specification;
        private static readonly int SpecSize;

        static DecalFaceVertexArray()
        {
            Modes = new[] { BeginMode.Triangles, BeginMode.Lines};
            Specification = new ArraySpecification(
                ArrayIndex.Vector3("Position"),
                ArrayIndex.Vector3("Normal"),
                ArrayIndex.Vector2("Texture"),
                ArrayIndex.Vector3("Colour"),
                ArrayIndex.Float("Selected"));
            SpecSize = Specification.Indices.Sum(x => x.Length);
        }

        public VertexBuffer<float> Array { get; private set; }
        public List<VertexArraySubset<ITexture>> TextureSubsets { get; private set; }
        public List<VertexArraySubset<object>> WireframeSubsets { get; private set; }
        private readonly Dictionary<object, VertexArray<float>> _arrays;

        public void Bind(object context, int index)
        {
            if (!_arrays.ContainsKey(context))
            {
                _arrays.Add(context, new VertexArray<float>(Array));
            }
            _arrays[context].Bind(index);
        }

        public void Unbind()
        {
            VertexArray<float>.Unbind();
        }

        /// <summary>
        /// Create a new vertex array for a solid.
        /// </summary>
        /// <param name="objects">The array objects</param>
        public DecalFaceVertexArray(IEnumerable<MapObject> objects)
        {
            _arrays = new Dictionary<object, VertexArray<float>>();

            float[] array;
            uint[] indices;
            uint[] wireframeIndices;
            int count;
            TextureSubsets = new List<VertexArraySubset<ITexture>>();
            WireframeSubsets = new List<VertexArraySubset<object>>();
            GetArrayData(objects, out count, out array, out indices, out wireframeIndices, TextureSubsets, WireframeSubsets);

            Array = new VertexBuffer<float>(Specification, Modes, count, sizeof(float), array, new[] { indices, wireframeIndices});
        }

        /// <summary>
        /// Update the array with new data.
        /// </summary>
        /// <param name="objects">List containing the data to update</param>
        public void Update(IEnumerable<MapObject> objects)
        {
            _arrays.Clear();
            float[] array;
            uint[] indices;
            uint[] wireframeIndices;
            int count;
            TextureSubsets.Clear();
            WireframeSubsets.Clear();
            GetArrayData(objects, out count, out array, out indices, out wireframeIndices, TextureSubsets, WireframeSubsets);

            Array.Update(count, array, new[] {indices, wireframeIndices});
        }

        /// <summary>
        /// Does a loop around the map objects and calculates array data and the subsets
        /// </summary>
        /// <param name="objects">The objects in the array</param>
        /// <param name="count">Outputs the number of verts in the array</param>
        /// <param name="array">Outputs the array data</param>
        /// <param name="indices">Outputs the triangle drawing indices</param>
        /// <param name="wireframeIndices">Outputs the line drawing indices</param>
        /// <param name="subsets">The collection of textured subsets to populate</param>
        /// <param name="wireframeSubsets">The collection of wireframe subsets to populate</param>
        private static void GetArrayData(IEnumerable<MapObject> objects, out int count, out float[] array, out uint[] indices, out uint[] wireframeIndices, ICollection<VertexArraySubset<ITexture>> subsets, ICollection<VertexArraySubset<object>> wireframeSubsets)
        {
            var obj = objects.Where(x => !x.IsVisgroupHidden && !x.IsCodeHidden).ToList();
            var faces = obj.OfType<Entity>().SelectMany(x => x.GetTexturedFaces()).ToList();
            var indexList = new List<uint>();
            var wireframeIndexList = new List<uint>();
            uint index = 0;
            var idx = 0;
            var numVerts = faces.Sum(x => x.Vertices.Count);
            array = new float[SpecSize * numVerts];
            var subsetStart = 0;
            var wireframeSubsetStart = 0;
            foreach (var group in faces.GroupBy(x => new { x.Texture.Texture }))
            {
                foreach (var face in group)
                {
                    idx = WriteFace(array, idx, face);
                    for (uint i = 1; i < face.Vertices.Count - 1; i++)
                    {
                        indexList.Add(index);
                        indexList.Add(index + i);
                        indexList.Add(index + i + 1);
                    }
                    for (uint i = 0; i < face.Vertices.Count; i++)
                    {
                        var ni = (uint) ((i + 1) % face.Vertices.Count);
                        wireframeIndexList.Add(index + i);
                        wireframeIndexList.Add(index + ni);
                    }
                    index += (uint) face.Vertices.Count;
                }

                subsets.Add(new VertexArraySubset<ITexture>(group.Key.Texture, subsetStart, indexList.Count - subsetStart));
                subsetStart = indexList.Count;

                wireframeSubsets.Add(new VertexArraySubset<object>(null, wireframeSubsetStart, wireframeIndexList.Count - wireframeSubsetStart));
                wireframeSubsetStart = wireframeIndexList.Count;
            }
            indices = indexList.ToArray();
            wireframeIndices = wireframeIndexList.ToArray();
            count = indices.Length;
        }

        private static int WriteFace(float[] array, int idx, Face face)
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
                array[idx++] = (face.IsSelected || (face.Parent != null && face.Parent.IsSelected) ? 1 : 0);
            }
            return idx;
        }
    }
}