using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.DataStructures.Models;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.OpenGL.Vertices;
using Sledge.Rendering.Scenes.Elements;

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

        protected override void CreateArray(IEnumerable<Element> data)
        {
            StartSubset(FaceWireframe);

            foreach (var line in data.OfType<LineElement>())
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
            if (position.Type == PositionType.World) return position.Location;
            if (position.Type == PositionType.Screen) return _viewport.Camera.WorldToScreen(position.Location, _viewport.Control.Width, _viewport.Control.Height);
            return Vector3.Zero;
        }
    }
}
