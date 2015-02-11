using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Cameras;

namespace Sledge.Rendering.Scenes.Renderables
{
    public class Face : Primitive
    {
        private List<Vertex> _vertices;
        private Plane _plane;

        public List<Vertex> Vertices
        {
            get { return _vertices; }
            set
            {
                _vertices = value;
                _plane = new Plane(value[0].Position, value[1].Position, value[2].Position);
                BoundingBox = new Box(value.Select(x => x.Position));
                OnPropertyChanged("Vertices");
                OnPropertyChanged("Plane");
            }
        }

        public Plane Plane
        {
            get { return _plane; }
        }

        public Face(Material material, List<Vertex> vertices)
        {
            Material = material;
            Vertices = vertices;
            CameraFlags = CameraFlags.All;
        }
    }
}