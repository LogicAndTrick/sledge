using System;
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
        private const int Wireframe = 1;

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
            // todo render transparent stuff last
            foreach (var subset in GetSubsets<string>(Textured).Where(x => x.Instance != null))
            {
                var mat = (string)subset.Instance;
                renderer.Materials.Bind(mat);
                Render(PrimitiveType.Triangles, subset);
            }
        }

        public void RenderWireframe(IRenderer renderer)
        {
            foreach (var subset in GetSubsets(Wireframe))
            {
                Render(PrimitiveType.Lines, subset);
            }
        }

        public void UpdatePartial(IEnumerable<RenderableObject> objects)
        {
            foreach (var face in objects.OfType<Face>())
            {
                var offset = GetOffset(face);
                if (offset < 0) continue;
                Update(offset, Convert(face));
            }
        }

        public void DeletePartial(IEnumerable<RenderableObject> objects)
        {
            foreach (var face in objects.OfType<Face>())
            {
                var offset = GetOffset(face);
                if (offset < 0) continue;
                Update(offset, Convert(face, VertexFlags.Invisible));
            }
        }

        protected override void CreateArray(IEnumerable<RenderableObject> data)
        {
            Items = new HashSet<RenderableObject>(data);

            StartSubset(Wireframe);
            
            foreach (var g in Items.OfType<Face>().GroupBy(x => x.Material.UniqueIdentifier))
            {
                StartSubset(Textured);
                foreach (var face in g)
                {
                    PushOffset(face);
                    var index = PushData(Convert(face));
                    PushIndex(Textured, index, Triangulate(face.Vertices.Count));
                    PushIndex(Wireframe, index, Linearise(face.Vertices.Count));
                }
                PushSubset(Textured, g.Key);
            }

            PushSubset(Wireframe, (object) null);
        }

        private IEnumerable<SimpleVertex> Convert(Face face, VertexFlags flags = VertexFlags.None)
        {
            return face.Vertices.Select((x, i) => new SimpleVertex
            {
                Position = x.Position.ToVector3(),
                Normal = face.Plane.Normal.ToVector3(),
                Texture = new Vector2((float) x.TextureU, (float) x.TextureV),
                Color = face.Material.Color.ToAbgr(),
                Flags = flags
            });
        }
    }
}