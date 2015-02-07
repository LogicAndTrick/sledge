using System.Collections.Generic;
using System.Drawing;

namespace Sledge.Rendering.Scenes.Renderables
{
    public class Line : RenderableObject
    {
        public int Width { get; set; }
        public List<Vertex> Vertices { get; set; }
    }
}