using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.OpenGL.Vertices;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.OpenGL.Arrays
{
    public class RenderableVertexArray : VertexArray<RenderableObject, SimpleVertex>
    {
        private const int Textured = 0;
        private const int Transparent = 1;

        public HashSet<RenderableObject> Items { get; private set; }

        public RenderableVertexArray(ICollection<RenderableObject> data) : base(data)
        {
            Items = new HashSet<RenderableObject>(data);
        }

        public IEnumerable<string> GetMaterials()
        {
            return GetSubsets<string>(Textured).Where(x => x.Instance != null).Select(x => x.Instance).OfType<string>();
        }

        public void RenderTextured(IRenderer renderer)
        {
            foreach (var subset in GetSubsets<string>(Textured).Where(x => x.Instance != null))
            {
                var mat = (string)subset.Instance;
                renderer.Materials.Bind(mat);
                Render(PrimitiveType.Triangles, subset);
            }
        }

        public void RenderMaterial(IRenderer renderer, string material)
        {
            foreach (var subset in GetSubsets<string>(Textured).Where(x => Equals(x.Instance, material)))
            {
                Render(PrimitiveType.Triangles, subset);
            }
        }

        protected override void CreateArray(IEnumerable<RenderableObject> data)
        {
            foreach (var g in data.OfType<Face>().GroupBy(x => x.Material.UniqueIdentifier))
            {
                StartSubset(Textured);
                foreach (var face in g)
                {
                    var index = PushData(Convert(face));
                    PushIndex(Textured, index, Triangulate(face.Vertices.Count));
                }
                PushSubset(Textured, g.Key);
            }
        }

        private IEnumerable<SimpleVertex> Convert(Face face)
        {
            return face.Vertices.Select((x, i) => new SimpleVertex
            {
                Position = x.Position.ToVector3(),
                Normal = face.Plane.Normal.ToVector3(),
                Texture = new Vector2((float)x.TextureU, (float)x.TextureV),
                Color = face.Material.Color.ToAbgr(face.Opacity)
            });
        }
    }
}