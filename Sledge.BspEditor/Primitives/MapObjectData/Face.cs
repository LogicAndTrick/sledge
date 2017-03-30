using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    public class Face : IMapObjectData
    {
        public long ID { get; }
        public Plane Plane { get; set; }
        public Texture Texture { get; set; }
        public List<Coordinate> Vertices { get; set; }

        public Face(long id)
        {
            ID = id;
            Plane = new Plane(Coordinate.UnitZ, Coordinate.Zero);
            Texture = new Texture();
            Vertices = new List<Coordinate>();
        }

        public IMapObjectData Clone()
        {
            var face = new Face(ID);
            face.Plane = Plane; // planes are immutable
            face.Texture = Texture.Clone();
            face.Vertices = Vertices.Select(x => x.Clone()).ToList();
            return face;
        }
    }
}