using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sledge.DataStructures.Geometric;

namespace Sledge.Rendering.Scenes.Renderables
{
    public class Line : RenderableObject
    {
        private List<Coordinate> _vertices;
        public int Width { get; set; }

        public List<Coordinate> Vertices
        {
            get { return _vertices; }
            set
            {
                _vertices = value;
                BoundingBox = new Box(value);
                OnPropertyChanged("Vertices");
            }
        }

        public Line(Color color, params Coordinate[] vertices)
        {
            Material = Materials.Material.Flat(color);
            AccentColor = color;
            TintColor = Color.White;
            Vertices = vertices.ToList();
            Width = 1; // todo change line widths?
        }
    }
}