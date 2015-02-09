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
        private const int Point = 2;

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

        public void RenderPoints(IRenderer renderer)
        {
            foreach (var subset in GetSubsets(Point))
            {
                Render(PrimitiveType.Points, subset);
            }
        }

        public void UpdatePartial(IEnumerable<RenderableObject> objects)
        {
            foreach (var obj in objects)
            {
                var offset = GetOffset(obj);
                if (offset < 0) continue;
                if (obj is Face) Update(offset, Convert((Face)obj));
                if (obj is Line) Update(offset, Convert((Line)obj));
            }
        }

        public void DeletePartial(IEnumerable<RenderableObject> objects)
        {
            foreach (var obj in objects)
            {
                var offset = GetOffset(obj);
                if (offset < 0) continue;
                if (obj is Face) Update(offset, Convert((Face)obj, VertexFlags.Invisible));
                if (obj is Line) Update(offset, Convert((Line)obj, VertexFlags.Invisible));
            }
        }

        protected override void CreateArray(IEnumerable<RenderableObject> data)
        {
            Items = new HashSet<RenderableObject>(data);

            StartSubset(Wireframe);
            StartSubset(Point);

            foreach (var g in Items.Where(x => x.RenderFlags != RenderFlags.None).GroupBy(x => x.Material.UniqueIdentifier))
            {
                StartSubset(Textured);
                foreach (var face in g.OfType<Face>())
                {
                    PushOffset(face);
                    var index = PushData(Convert(face));
                    if (face.RenderFlags.HasFlag(RenderFlags.Polygon)) PushIndex(Textured, index, Triangulate(face.Vertices.Count));
                    if (face.RenderFlags.HasFlag(RenderFlags.Wireframe)) PushIndex(Wireframe, index, Linearise(face.Vertices.Count));
                    if (face.RenderFlags.HasFlag(RenderFlags.Point)) PushIndex(Point, index, new[] { 0u });
                }
                foreach (var line in g.OfType<Line>())
                {
                    PushOffset(line);
                    var index = PushData(Convert(line));
                    if (line.RenderFlags.HasFlag(RenderFlags.Wireframe)) PushIndex(Wireframe, index, Linearise(line.Vertices.Count));
                    if (line.RenderFlags.HasFlag(RenderFlags.Point)) PushIndex(Point, index, new[] { 0u });
                }
                PushSubset(Textured, g.Key);
            }

            PushSubset(Wireframe, (object) null);
            PushSubset(Point, (object) null);
        }

        private IEnumerable<SimpleVertex> Convert(Face face, VertexFlags flags = VertexFlags.None)
        {
            return face.Vertices.Select((x, i) => new SimpleVertex
            {
                Position = x.Position.ToVector3(),
                Normal = face.Plane.Normal.ToVector3(),
                Texture = new Vector2((float)x.TextureU, (float)x.TextureV),
                MaterialColor = face.Material.Color.ToAbgr(),
                AccentColor = face.AccentColor.ToAbgr(),
                TintColor = face.AccentColor.ToAbgr(),
                Flags = flags
            });
        }

        private IEnumerable<SimpleVertex> Convert(Line line, VertexFlags flags = VertexFlags.None)
        {
            return line.Vertices.Select((x, i) => new SimpleVertex
            {
                Position = x.ToVector3(),
                Normal = Vector3.UnitZ,
                Texture = Vector2.Zero,
                MaterialColor = line.Material.Color.ToAbgr(),
                AccentColor = line.AccentColor.ToAbgr(),
                TintColor = line.AccentColor.ToAbgr(),
                Flags = flags
            });
        }
    }
}