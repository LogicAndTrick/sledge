using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;
using Plane = Sledge.DataStructures.Geometric.Plane;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    public class Face : IMapObjectData, ITransformable, ITextured
    {
        public long ID { get; }
        public Texture Texture { get; set; }
        public VertexCollection Vertices { get; }

        public Plane Plane
        {
            get => Vertices.Plane;
            set => Vertices.Plane = value;
        }

        public Vector3 Origin => Vertices.Origin;

        public Face(long id)
        {
            ID = id;
            Texture = new Texture();
            Vertices = new VertexCollection();
        }

        public Face(SerialisedObject obj)
        {
            ID = obj.Get("ID", ID);

            var t = obj.Children.FirstOrDefault(x => x.Name == "Texture");
            Texture = new Texture();
            
            if (t != null)
            {
                Texture.Name = t.Get("Name", "");
                Texture.Rotation = t.Get("Rotation", 0f);
                Texture.UAxis = t.Get("UAxis", -Vector3.UnitZ);
                Texture.VAxis = t.Get("VAxis", Vector3.UnitX);
                Texture.XScale = t.Get("XScale", 1f);
                Texture.XShift = t.Get("XShift", 0f);
                Texture.YScale = t.Get("YScale", 1f);
                Texture.YShift = t.Get("YShift", 0f);
            }

            Vertices = new VertexCollection();
            Vertices.AddRange(obj.Children.Where(x => x.Name == "Vertex").Select(x => x.Get<Vector3>("Position")));
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
            face.Texture = Texture.Clone();
            face.Vertices.Reset(Vertices.Select(x => x));
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

        public virtual IEnumerable<Tuple<Vector3, float, float>> GetTextureCoordinates(int width, int height)
        {
            if (width <= 0 || height <= 0 || Texture.XScale == 0 || Texture.YScale == 0)
            {
                return Vertices.Select(x => Tuple.Create(x, 0f, 0f));
            }

            var udiv = width * Texture.XScale;
            var uadd = Texture.XShift / width;
            var vdiv = height * Texture.YScale;
            var vadd = Texture.YShift / height;

            return Vertices.Select(x => Tuple.Create(x, x.Dot(Texture.UAxis) / udiv + uadd, x.Dot(Texture.VAxis) / vdiv + vadd));
        }

        public void Transform(Matrix4x4 matrix)
        {
            Vertices.Transform(x => Vector3.Transform(x, matrix));
        }

        public Polygon ToPolygon()
        {
            return new Polygon(Vertices);
        }

        public virtual IEnumerable<Line> GetEdges()
        {
            for (var i = 0; i < Vertices.Count; i++)
            {
                yield return new Line(Vertices[i], Vertices[(i + 1) % Vertices.Count]);
            }
        }

        public class VertexCollection : IList<Vector3>
        {
            private readonly List<Vector3> _list = new List<Vector3>();
            private Plane _plane = new Plane(Vector3.UnitZ, Vector3.Zero);

            internal Plane Plane
            {
                get => _plane;
                set
                {
                    if (_list.Count < 3) _plane = value;
                }
            }

            public int Count => _list.Count;
            public bool IsReadOnly => false;

            internal Vector3 Origin => !_list.Any()
                ? Vector3.Zero
                : _list.Aggregate(Vector3.Zero, (a, b) => a + b) / _list.Count;

            public Vector3 this[int index]
            {
                get => _list[index];
                set
                {
                    _list[index] = value;
                    UpdatePlane();
                }
            }

            public void Add(Vector3 item)
            {
                _list.Add(item);
                UpdatePlane();
            }

            public void AddRange(IEnumerable<Vector3> items)
            {
                _list.AddRange(items);
                UpdatePlane();
            }

            public void Flip()
            {
                _list.Reverse();
                UpdatePlane();
            }

            public void Clear()
            {
                _list.Clear();
                UpdatePlane();
            }

            public bool Remove(Vector3 item)
            {
                var r = _list.Remove(item);
                UpdatePlane();
                return r;
            }

            public void Insert(int index, Vector3 item)
            {
                _list.Insert(index, item);
                UpdatePlane();
            }

            public void RemoveAt(int index)
            {
                _list.RemoveAt(index);
                UpdatePlane();
            }

            public void Transform(Func<Vector3, Vector3> tranform)
            {
                for (var i = 0; i < _list.Count; i++)
                {
                    _list[i] = tranform(_list[i]);
                }
                UpdatePlane();
            }

            public void Reset(IEnumerable<Vector3> values)
            {
                _list.Clear();
                _list.AddRange(values);
                UpdatePlane();
            }

            public void UpdatePlane()
            {
                _plane = _list.Count < 3 ? _plane : new Plane(_list[0], _list[1], _list[2]);
            }

            public IEnumerator<Vector3> GetEnumerator() => _list.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
            public bool Contains(Vector3 item) => _list.Contains(item);
            public void CopyTo(Vector3[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
            public int IndexOf(Vector3 item) => _list.IndexOf(item);
            public void ForEach(Action<Vector3> action) => _list.ForEach(action);
        }
    }
}