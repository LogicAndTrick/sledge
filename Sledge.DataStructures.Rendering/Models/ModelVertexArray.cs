using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Models;
using Sledge.Graphics.Arrays;
using Sledge.Graphics.Renderables;

namespace Sledge.DataStructures.Rendering.Models
{
    public class ModelVertexArray : IRenderable, IDisposable
    {
        private static readonly BeginMode[] Modes;
        private static readonly ArraySpecification Specification;
        private static readonly int SpecSize;

        static ModelVertexArray()
        {
            Modes = new[] { BeginMode.Triangles };
            Specification = new ArraySpecification(
                ArrayIndex.Vector3("Position"),
                ArrayIndex.Vector3("Normal"),
                ArrayIndex.Vector2("Texture"));
            SpecSize = Specification.Indices.Sum(x => x.Length);
        }

        public List<VertexArraySubset<int>> TextureSubsets { get; private set; }
        public VertexBuffer<float> Array { get; private set; }
        private readonly Dictionary<object, VertexArray<float>> _arrays;

        public void Bind(object context)
        {
            if (!_arrays.ContainsKey(context))
            {
                _arrays.Add(context, new VertexArray<float>(Array));
            }
            _arrays[context].Bind(0);
        }

        public void Unbind()
        {
            VertexArray<float>.Unbind();
        }

        public ModelVertexArray(Model model)
        {
            _arrays = new Dictionary<object, VertexArray<float>>();

            float[] array;
            uint[] indices;
            int count;
            TextureSubsets = new List<VertexArraySubset<int>>();

            GetArrayData(model, out count, out array, out indices, TextureSubsets);
            Array = new VertexBuffer<float>(Specification, Modes, count, sizeof (float), array, new[] {indices});
        }

        public void Dispose()
        {
            foreach (var va in _arrays)
            {
                va.Value.Dispose();
            }
            _arrays.Clear();
            Array.Dispose();
        }

        private static void GetArrayData(Model model, out int count,
            out float[] array, out uint[] indices,
            List<VertexArraySubset<int>> textureSubsets)
        {
            var transforms = model.Bones.Select(x => x.Transform).ToList();
            
            var data = new List<float>();
            var indexList = new List<uint>();
            count = 0;
            foreach (var g in model.Meshes.GroupBy(x => x.SkinRef))
            {
                var sr = g.Key;
                var start = count;
                foreach (var mesh in g)
                {
                    foreach (var vertex in mesh.Vertices)
                    {
                        var transform = transforms[vertex.BoneWeightings.First().Bone.BoneIndex];
                        var c = vertex.Location * transform;
                        var n = vertex.Normal * transform;
                        data.Add(c.X);
                        data.Add(c.Y);
                        data.Add(c.Z);
                        data.Add(n.X);
                        data.Add(n.Y);
                        data.Add(n.Z);
                        data.Add(vertex.TextureU);
                        data.Add(vertex.TextureV);
                        indexList.Add((uint) count);
                        count++;
                    }
                }
                textureSubsets.Add(new VertexArraySubset<int>(sr, start, count - start));
            }
            array = data.ToArray();
            indices = indexList.ToArray();
        }

        public void Render(object sender)
        {
            Bind(sender);
            foreach (var subset in TextureSubsets)
            {
                var sr = subset.Instance;
                // Bind texture here
                Array.DrawElements(0, subset.Start, subset.Count);
            }
            Unbind();
        }
    }
}
