using System.Collections.Generic;
using System.Linq;
using Sledge.Rendering.DataStructures;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Cameras;

namespace Sledge.Rendering.Scenes.Renderables
{
    public class Face : RenderableObject
    {
        private List<Vertex> _vertices;
        private Plane _plane;

        public List<Vertex> Vertices
        {
            get { return _vertices; }
            set
            {
                if (_vertices != null && value != null && _vertices.Count == value.Count && _vertices.Select((x, i) => x.Position == value[i].Position && x.TextureU == value[i].TextureU && x.TextureV == value[i].TextureV).All(x => x)) return;

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
            RenderFlags = RenderFlags.Polygon | RenderFlags.Wireframe;
        }
    }
}