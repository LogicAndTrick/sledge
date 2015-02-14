using System.Collections.Generic;
using System.Linq;
using OpenTK;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Materials;

namespace Sledge.Rendering.DataStructures.Models
{
    public class Mesh : IBounded
    {
        public Vector3 Origin { get { return BoundingBox.Center; } }
        public Box BoundingBox { get; private set; }

        public Material Material { get; private set; }
        public List<MeshVertex> Vertices { get; private set; }

        public Mesh(Material material, List<MeshVertex> vertices)
        {
            Material = material;
            Vertices = vertices;
            BoundingBox = new Box(Vertices.Select(x => x.Location));
        }
    }
}