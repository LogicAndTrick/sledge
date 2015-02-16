using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.DataStructures;

namespace Sledge.Rendering.Scenes.Renderables
{
    public class Line : RenderableObject
    {
        private List<Vector3> _vertices;
        public int Width { get; set; }

        public List<Vector3> Vertices
        {
            get { return _vertices; }
            set
            {
                _vertices = value;
                BoundingBox = new Box(value);
                OnPropertyChanged("Vertices");
            }
        }

        public Line(Color color, params Vector3[] vertices)
        {
            Material = Materials.Material.Flat(color);
            AccentColor = color;
            TintColor = Color.White;
            Vertices = vertices.ToList();
            Width = 1; // todo change line widths?
            CameraFlags = CameraFlags.All;
            RenderFlags = RenderFlags.Wireframe;
        }
    }
}