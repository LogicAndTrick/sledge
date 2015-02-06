using System.Collections.Generic;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Materials;

namespace Sledge.Rendering
{
    public class Face : RenderableObject
    {
        public Material Material { get; set; }
        public List<Vertex> Vertices { get; set; }
        public Plane Plane { get; private set; }

        public Face(Material material, List<Vertex> vertices)
        {
            Material = material;
            Vertices = vertices;
            Plane = new Plane(vertices[0].Position, vertices[1].Position, vertices[2].Position);
            Opacity = byte.MaxValue;
        }
    }
}