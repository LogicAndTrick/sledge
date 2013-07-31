using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using Sledge.Common;
using Sledge.DataStructures.MapObjects;
using Sledge.Graphics.Arrays;

namespace Sledge.DataStructures.Rendering
{
    /// <summary>
    /// A solid vertex array collects and stores a VBO for all solids in the map.
    /// Faces are grouped by texture and then split into  subsets for optimised rendering later on.
    /// </summary>
    public class SolidVertexArray
    {
        private static readonly BeginMode[] Modes;
        private static readonly ArraySpecification Specification;
        private static readonly int SpecSize;

        static SolidVertexArray()
        {
            Modes = new[] { BeginMode.Triangles, BeginMode.Lines};
            Specification = new ArraySpecification(
                ArrayIndex.Vector3("Position"),
                ArrayIndex.Vector3("Normal"),
                ArrayIndex.Vector2("Texture"),
                ArrayIndex.Vector4("Colour"),
                ArrayIndex.Float("Selected"));
            SpecSize = Specification.Indices.Sum(x => x.Length);
        }

        public VertexBuffer<float> Array { get; private set; }
        public List<VertexArraySubset<ITexture>> TextureSubsets { get; private set; }
        public List<VertexArraySubset<object>> WireframeSubsets { get; private set; }
        public Dictionary<Face, int> FaceOffsets { get; private set; }
        public Dictionary<Entity, int> EntityOffsets { get; private set; }
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
        public SolidVertexArray(IEnumerable<MapObject> objects)
        {
            _arrays = new Dictionary<object, VertexArray<float>>();

            float[] array;
            uint[] indices;
            uint[] wireframeIndices;
            int count;
            TextureSubsets = new List<VertexArraySubset<ITexture>>();
            WireframeSubsets = new List<VertexArraySubset<object>>();
            FaceOffsets = new Dictionary<Face, int>();
            EntityOffsets = new Dictionary<Entity, int>();
            GetArrayData(objects, out count, out array, out indices, out wireframeIndices, TextureSubsets, WireframeSubsets, FaceOffsets, EntityOffsets);

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
            FaceOffsets.Clear();
            EntityOffsets.Clear();
            GetArrayData(objects, out count, out array, out indices, out wireframeIndices, TextureSubsets, WireframeSubsets, FaceOffsets, EntityOffsets);

            Array.Update(count, array, new[] {indices, wireframeIndices});
        }

        public void UpdatePartial(IEnumerable<MapObject> objects)
        {
            UpdatePartial(objects.OfType<Solid>().SelectMany(x => x.Faces));
            UpdatePartial(objects.OfType<Entity>().Where(x => x.Children.Count == 0));
        }

        public void UpdatePartial(IEnumerable<Face> faces)
        {
            var list = new float[128]; // 128 is large enough for most faces (up to 11 faces)
            foreach (var face in faces)
            {
                if (!FaceOffsets.ContainsKey(face)) continue;
                var offset = FaceOffsets[face];
                var count = face.Vertices.Count * SpecSize;
                if (list.Length < count) System.Array.Resize(ref list, count); // Increase the size of the array if needed
                WriteFace(list, 0, face);
                Array.UpdatePartial(offset, count, list);
            }
        }

        public void UpdatePartial(IEnumerable<Entity> entities)
        {
            var list = new float[6 * 4 * SpecSize];
            foreach (var entity in entities)
            {
                if (!EntityOffsets.ContainsKey(entity)) continue;
                var offset = EntityOffsets[entity];
                var idx = 0;
                foreach (var face in entity.GetBoxFaces())
                {
                    idx = WriteFace(list, idx, face);
                }
                Array.UpdatePartial(offset, list.Length, list);
            }
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
        /// <param name="faceOffsets"> </param>
        /// <param name="entityOffsets"> </param>
        private static void GetArrayData(IEnumerable<MapObject> objects, out int count, out float[] array,
            out uint[] indices, out uint[] wireframeIndices,
            ICollection<VertexArraySubset<ITexture>> subsets, ICollection<VertexArraySubset<object>> wireframeSubsets,
            Dictionary<Face, int> faceOffsets, Dictionary<Entity, int> entityOffsets)
        {
            var obj = objects.Where(x => !x.IsVisgroupHidden && !x.IsCodeHidden).ToList();
            var faces = obj.OfType<Solid>().SelectMany(x => x.Faces).ToList();
            var entities = obj.OfType<Entity>().Where(x => x.Children.Count == 0).ToList();
            var indexList = new List<uint>();
            var wireframeIndexList = new List<uint>();
            uint index = 0;
            var idx = 0;
            var numVerts = faces.Sum(x => x.Vertices.Count) + entities.Count * 6 * 4; // Entity is always a rec. prism (6 sides, quads)
            array = new float[SpecSize * numVerts];
            var subsetStart = 0;
            var wireframeSubsetStart = 0;
            foreach (var group in faces.GroupBy(x => new { x.Texture.Texture }))
            {
                foreach (var face in group)
                {
                    faceOffsets.Add(face, idx);
                    idx = WriteFace(array, idx, face);
                    if (!face.Parent.IsRenderHidden3D)
                    {
                        for (uint i = 1; i < face.Vertices.Count - 1; i++)
                        {
                            indexList.Add(index);
                            indexList.Add(index + i);
                            indexList.Add(index + i + 1);
                        }
                    }
                    if (!face.Parent.IsRenderHidden2D)
                    {
                        for (uint i = 0; i < face.Vertices.Count; i++)
                        {
                            var ni = (uint) ((i + 1) % face.Vertices.Count);
                            wireframeIndexList.Add(index + i);
                            wireframeIndexList.Add(index + ni);
                        }
                    }
                    index += (uint) face.Vertices.Count;
                }

                subsets.Add(new VertexArraySubset<ITexture>(group.Key.Texture, subsetStart, indexList.Count - subsetStart));
                subsetStart = indexList.Count;

                wireframeSubsets.Add(new VertexArraySubset<object>(null, wireframeSubsetStart, wireframeIndexList.Count - wireframeSubsetStart));
                wireframeSubsetStart = wireframeIndexList.Count;
            }
            foreach (var entity in entities)
            {
                entityOffsets.Add(entity, idx);
                foreach (var face in entity.GetBoxFaces())
                {
                    idx = WriteFace(array, idx, face);
                    if (!entity.IsRenderHidden3D)
                    {
                        for (uint i = 1; i < face.Vertices.Count - 1; i++)
                        {
                            indexList.Add(index);
                            indexList.Add(index + i);
                            indexList.Add(index + i + 1);
                        }
                    }
                    if (!entity.IsRenderHidden2D)
                    {
                        for (uint i = 0; i < face.Vertices.Count; i++)
                        {
                            var ni = (uint) ((i + 1) % face.Vertices.Count);
                            wireframeIndexList.Add(index + i);
                            wireframeIndexList.Add(index + ni);
                        }
                    }
                    index += (uint)face.Vertices.Count;
                }
            }
            if (entities.Any())
            {
                subsets.Add(new VertexArraySubset<ITexture>(null, subsetStart, indexList.Count - subsetStart));
                wireframeSubsets.Add(new VertexArraySubset<object>(null, wireframeSubsetStart, wireframeIndexList.Count - wireframeSubsetStart));
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
                  b = face.Colour.B / 255f,
                  a = face.Opacity;
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
                array[idx++] = (a);
                array[idx++] = (face.IsSelected || (face.Parent != null && face.Parent.IsSelected) ? 1 : 0);
            }
            return idx;
        }
    }
}