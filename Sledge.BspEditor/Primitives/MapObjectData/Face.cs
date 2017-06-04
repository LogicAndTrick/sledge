using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    public class Face : IMapObjectData, ITransformable, ITextured
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

        public Face(SerialisedObject obj)
        {
            ID = obj.Get("ID", ID);

            var pln = obj.Children.First(x => x.Name == "Plane");
            Plane = new Plane(pln.Get<Coordinate>("Normal"), pln.Get<decimal>("DistanceFromOrigin"));

            var t = obj.Children.FirstOrDefault(x => x.Name == "Texture");
            Texture = new Texture();
            
            if (t != null)
            {
                Texture.Name = t.Get("Name", "");
                Texture.Rotation = t.Get("Rotation", 0m);
                Texture.UAxis = t.Get("UAxis", -Coordinate.UnitZ);
                Texture.VAxis = t.Get("VAxis", Coordinate.UnitX);
                Texture.XScale = t.Get("XScale", 1m);
                Texture.XShift = t.Get("XShift", 0m);
                Texture.YScale = t.Get("YScale", 1m);
                Texture.YShift = t.Get("YShift", 0m);
            }

            Vertices = new List<Coordinate>();
            foreach (var so in obj.Children.Where(x => x.Name == "Vertex"))
            {
                Vertices.Add(so.Get<Coordinate>("Position"));
            }
        }

        [Export(typeof(IMapElementFormatter))]
        public class ActiveTextureFormatter : StandardMapElementFormatter<Face> { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", ID);
            info.AddValue("Plane", Plane);
            info.AddValue("Texture", Texture);
            info.AddValue("Vertices", Vertices.ToArray());
        }

        private void CopyBase(Face face)
        {
            face.Plane = Plane; // planes are immutable
            face.Texture = Texture.Clone();
            face.Vertices = Vertices.Select(x => x.Clone()).ToList();
        }

        public IMapElement Clone()
        {
            var face = new Face(ID);
            CopyBase(face);
            return face;
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            var face = new Face(numberGenerator.Next("Face"));
            CopyBase(face);
            return face;
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject("Face");
            so.Set("ID", ID);

            var p = new SerialisedObject("Plane");
            p.Set("Normal", Plane.Normal);
            p.Set("DistanceFromOrigin", Plane.DistanceFromOrigin);
            so.Children.Add(p);

            if (Texture != null)
            {
                var t = new SerialisedObject("Texture");
                t.Set("Name", Texture.Name);
                t.Set("Rotation", Texture.Rotation);
                t.Set("UAxis", Texture.UAxis);
                t.Set("VAxis", Texture.VAxis);
                t.Set("XScale", Texture.XScale);
                t.Set("XShift", Texture.XShift);
                t.Set("YScale", Texture.YScale);
                t.Set("YShift", Texture.YShift);
                so.Children.Add(t);
            }
            foreach (var c in Vertices)
            {
                var v = new SerialisedObject("Vertex");
                v.Set("Position", c);
                so.Children.Add(v);
            }
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

        public void Transform(Matrix matrix)
        {
            Vertices = Vertices.Select(x => x * matrix).ToList();
        }
    }
}