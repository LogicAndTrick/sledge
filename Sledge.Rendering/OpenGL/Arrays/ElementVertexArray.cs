using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.DataStructures.Models;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.OpenGL.Shaders;
using Sledge.Rendering.OpenGL.Vertices;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.OpenGL.Arrays
{
    public class ElementVertexArray : VertexArray<Element, SimpleVertex>
    {
        private const int FacePolygons = 0;
        private const int FaceWireframe = 1;

        private readonly IViewport _viewport;

        public ElementVertexArray(IViewport viewport, IEnumerable<Element> data) : base(data)
        {
            _viewport = viewport;
        }

        public void Render(IRenderer renderer, Passthrough shader, IViewport viewport)
        {
            var camera = viewport.Camera;
            const float near = -1000000;
            const float far = 1000000;
            var vpMatrix = Matrix4.CreateOrthographicOffCenter(0, viewport.Control.Width, 0, viewport.Control.Height, near, far);

            var options = camera.RenderOptions;

            shader.Bind();
            shader.SelectionTransform = Matrix4.Identity;
            shader.ModelMatrix = Matrix4.Identity;
            shader.CameraMatrix = Matrix4.Identity;
            shader.ViewportMatrix = vpMatrix;
            shader.Orthographic = false;
            shader.UseAccentColor = false;

            // Render polygons
            string last = null;
            foreach (var subset in GetSubsets<string>(FacePolygons).Where(x => x.Instance != null).OrderBy(x => (string)x.Instance))
            {
                var mat = (string)subset.Instance;
                if (mat != last) renderer.Materials.Bind(mat);
                last = mat;

                Render(PrimitiveType.Triangles, subset);
            }
            
            shader.UseAccentColor = true;

            // Render wireframe
            foreach (var subset in GetSubsets(FaceWireframe))
            {
                Render(PrimitiveType.Lines, subset);
            }

            shader.Unbind();
        }

        protected override void CreateArray(IEnumerable<Element> data)
        {
            StartSubset(FaceWireframe);

            var list = data.Where(x => x.Viewport == null || x.Viewport == _viewport).ToList();

            foreach (var g in list.SelectMany(x => x.GetFaces()).GroupBy(x => x.Material.UniqueIdentifier))
            {
                StartSubset(FacePolygons);

                foreach (var face in g)
                {
                    PushOffset(face);
                    var index = PushData(Convert(face));
                    if (face.RenderFlags.HasFlag(RenderFlags.Polygon)) PushIndex(FacePolygons, index, Triangulate(face.Vertices.Count));
                    if (face.RenderFlags.HasFlag(RenderFlags.Wireframe)) PushIndex(FaceWireframe, index, Linearise(face.Vertices.Count));
                }

                PushSubset(FacePolygons, g.Key);
            }

            foreach (var line in list.SelectMany(x => x.GetLines()))
            {
                var index = PushData(Convert(line));
                PushIndex(FaceWireframe, index, Line(line.Vertices.Count));
            }

            PushSubset(FaceWireframe, (object) null);
        }

        private IEnumerable<uint> Line(int num)
        {
            for (uint i = 0; i < num - 1; i++)
            {
                yield return i;
                yield return i + 1;
            }
        }

        private VertexFlags ConvertVertexFlags(CameraFlags cameraFlags)
        {
            var flags = VertexFlags.None;
            if (!cameraFlags.HasFlag(CameraFlags.Orthographic)) flags |= VertexFlags.InvisibleOrthographic;
            if (!cameraFlags.HasFlag(CameraFlags.Perspective)) flags |= VertexFlags.InvisiblePerspective;
            return flags;
        }

        private IEnumerable<SimpleVertex> Convert(FaceElement face)
        {
            return face.Vertices.Select(vert => new SimpleVertex
            {
                Position = Convert(vert.Position),
                Texture = new Vector2(vert.TextureU, vert.TextureV),
                MaterialColor = face.Material.Color.ToAbgr(),
                AccentColor = face.AccentColor.ToAbgr(),
                TintColor = Color.White.ToAbgr(),
                Flags = ConvertVertexFlags(face.CameraFlags)
            });
        }

        private IEnumerable<SimpleVertex> Convert(LineElement line)
        {
            return line.Vertices.Select(vert => new SimpleVertex
            {
                Position = Convert(vert),
                MaterialColor = line.Color.ToAbgr(),
                AccentColor = line.Color.ToAbgr(),
                TintColor = Color.White.ToAbgr(),
                Flags = ConvertVertexFlags(line.CameraFlags)
            });
        }

        private Vector3 Convert(Position position)
        {
            if (position.Type == PositionType.World)
            {
                return _viewport.Camera.WorldToScreen(position.Location, _viewport.Control.Width, _viewport.Control.Height) + position.Offset;
            }
            else if (position.Type == PositionType.Screen)
            {
                if (position.Normalised) return new Vector3(position.Location.X * _viewport.Control.Width, position.Location.Y * _viewport.Control.Height, 0) + position.Offset;
                else return position.Location + position.Offset;
            }
            return Vector3.Zero;
        }
    }
}
