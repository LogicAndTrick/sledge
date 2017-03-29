using System.Collections.Generic;
using System.Drawing;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives
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
        }
    }
}