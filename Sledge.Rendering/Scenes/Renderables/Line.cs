using System.Collections.Generic;
using System.Drawing;

namespace Sledge.Rendering.Scenes.Renderables
{
    public class Line : RenderableObject
    {
        public Color Color { get; set; }
        public int Width { get; set; }
        public List<Vertex> Vertices { get; set; }
    }
}