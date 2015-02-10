using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.OpenGL.Vertices;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.OpenGL.Arrays
{
    public class RenderableVertexArray : VertexArray<RenderableObject, SimpleVertex>
    {
        private const int FacePolygons = 0;
        private const int FaceWireframe = 1;
        private const int FacePoints = 2;

        private const int LineWireframe = 3;
        private const int LinePoints = 4;

        public HashSet<RenderableObject> Items { get; private set; }

        public RenderableVertexArray(ICollection<RenderableObject> data) : base(data)
        {
            Items = new HashSet<RenderableObject>(data);
        }

        public IEnumerable<string> GetMaterials()
        {
            return GetSubsets<string>(FacePolygons).Where(x => x.Instance != null).Select(x => x.Instance).OfType<string>();
        }

        public void RenderFacePolygons(IRenderer renderer)
        {
            // todo render transparent stuff last
            foreach (var subset in GetSubsets<string>(FacePolygons).Where(x => x.Instance != null))
            {
                var mat = (string)subset.Instance;
                renderer.Materials.Bind(mat);
                Render(PrimitiveType.Triangles, subset);
            }
        }

        public void RenderFaceWireframe(IRenderer renderer)
        {
            foreach (var subset in GetSubsets(FaceWireframe))
            {
                Render(PrimitiveType.Lines, subset);
            }
        }

        public void RenderFacePoints(IRenderer renderer)
        {
            foreach (var subset in GetSubsets(FacePoints))
            {
                Render(PrimitiveType.Points, subset);
            }
        }

        public void RenderLineWireframe(IRenderer renderer)
        {
            foreach (var subset in GetSubsets(LineWireframe))
            {
                Render(PrimitiveType.Lines, subset);
            }
        }

        public void RenderLinePoints(IRenderer renderer)
        {
            foreach (var subset in GetSubsets(LinePoints))
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
                if (obj is Face) Update(offset, Convert((Face)obj, VertexFlags.InvisibleOrthographic | VertexFlags.InvisiblePerspective));
                if (obj is Line) Update(offset, Convert((Line)obj, VertexFlags.InvisibleOrthographic | VertexFlags.InvisiblePerspective));
            }
        }

        protected override void CreateArray(IEnumerable<RenderableObject> data)
        {
            Items = new HashSet<RenderableObject>(data);

            StartSubset(LineWireframe);
            StartSubset(LinePoints);
            StartSubset(FaceWireframe);
            StartSubset(FacePoints);

            foreach (var g in Items.Where(x => x.RenderFlags != RenderFlags.None).GroupBy(x => x.Material.UniqueIdentifier))
            {
                StartSubset(FacePolygons);
                foreach (var face in g.OfType<Face>())
                {
                    PushOffset(face);
                    var index = PushData(Convert(face));
                    if (face.RenderFlags.HasFlag(RenderFlags.Polygon)) PushIndex(FacePolygons, index, Triangulate(face.Vertices.Count));
                    if (face.RenderFlags.HasFlag(RenderFlags.Wireframe)) PushIndex(FaceWireframe, index, Linearise(face.Vertices.Count));
                    if (face.RenderFlags.HasFlag(RenderFlags.Point)) PushIndex(FacePoints, index, new[] { 0u });
                }
                foreach (var line in g.OfType<Line>())
                {
                    PushOffset(line);
                    var index = PushData(Convert(line));
                    if (line.RenderFlags.HasFlag(RenderFlags.Wireframe)) PushIndex(LineWireframe, index, Linearise(line.Vertices.Count));
                    if (line.RenderFlags.HasFlag(RenderFlags.Point)) PushIndex(LinePoints, index, new[] { 0u });
                }
                PushSubset(FacePolygons, g.Key);
            }

            PushSubset(LineWireframe, (object)null);
            PushSubset(LinePoints, (object)null);
            PushSubset(FaceWireframe, (object)null);
            PushSubset(FacePoints, (object)null);
        }

        private VertexFlags ConvertVertexFlags(RenderableObject obj)
        {
            var flags = VertexFlags.None;
            if (!obj.CameraFlags.HasFlag(CameraFlags.Orthographic)) flags |= VertexFlags.InvisibleOrthographic;
            if (!obj.CameraFlags.HasFlag(CameraFlags.Perspective)) flags |= VertexFlags.InvisiblePerspective;
            return flags;
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
                Flags = ConvertVertexFlags(face) | flags
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
                Flags = ConvertVertexFlags(line) | flags
            });
        }
    }
}