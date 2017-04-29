using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    public class Face : IMapObjectData
    {
        public long ID { get; }
        public Plane Plane { get; set; }
        public Texture Texture { get; set; }
        public List<Coordinate> Vertices { get; set; }
        public bool IsSelected { get; set; }

        public Face(long id)
        {
            ID = id;
            Plane = new Plane(Coordinate.UnitZ, Coordinate.Zero);
            Texture = new Texture();
            Vertices = new List<Coordinate>();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", ID);
            info.AddValue("Plane", Plane);
            info.AddValue("Texture", Texture);
            info.AddValue("Vertices", Vertices.ToArray());
        }

        public IMapObjectData Clone()
        {
            var face = new Face(ID);
            face.Plane = Plane; // planes are immutable
            face.Texture = Texture.Clone();
            face.Vertices = Vertices.Select(x => x.Clone()).ToList();
            return face;
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject("Face");
            // todo !
            return so;
        }

        public virtual IEnumerable<Tuple<Coordinate, decimal, decimal>> GetTextureCoordinates(int width, int height)
        {
            if (width <= 0 || height <= 0 || Texture.XScale == 0 || Texture.YScale == 0)
            {
                return Vertices.Select(x => Tuple.Create(x, 0m, 0m));
            }

            var udiv = width * Texture.XScale;
            var uadd = Texture.XShift / width;
            var vdiv = height * Texture.YScale;
            var vadd = Texture.YShift / height;

            return Vertices.Select(x => Tuple.Create(x, x.Dot(Texture.UAxis) / udiv + uadd, x.Dot(Texture.VAxis) / vdiv + vadd));
        }
    }
}