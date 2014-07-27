using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.MapObjects
{
    [Serializable]
    public class Solid : MapObject
    {
        public List<Face> Faces { get; private set; }

        public override Color Colour {
            get { return base.Colour; }
            set
            {
                base.Colour = value;
                Faces.ForEach(x => x.Colour = value);
            }
        }

        public Solid(long id) : base(id)
        {
            Faces = new List<Face>();
        }

        protected Solid(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Faces = ((Face[]) info.GetValue("Faces", typeof (Face[]))).ToList();
            Faces.ForEach(x => x.Parent = this);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Faces", Faces.ToArray());
        }

        public override MapObject Copy(IDGenerator generator)
        {
            var e = new Solid(generator.GetNextObjectID());
            foreach (var f in Faces.Select(x => x.Copy(generator)))
            {
                f.Parent = e;
                e.Faces.Add(f);
                f.UpdateBoundingBox();
                f.CalculateTextureCoordinates(true);
            }
            CopyBase(e, generator);
            return e;
        }

        public override void Paste(MapObject o, IDGenerator generator)
        {
            PasteBase(o, generator);
            var e = o as Solid;
            if (e == null) return;
            Faces.Clear();
            foreach (var f in e.Faces.Select(x => x.Copy(generator)))
            {
                f.Parent = this;
                Faces.Add(f);
                f.UpdateBoundingBox();
            }
        }

        public override MapObject Clone()
        {
            var e = new Solid(ID);
            foreach (var f in Faces.Select(x => x.Clone()))
            {
                f.Parent = e;
                e.Faces.Add(f);
                f.UpdateBoundingBox();
            }
            CopyBase(e, null, true);
            return e;
        }

        public override void Unclone(MapObject o)
        {
            PasteBase(o, null, true);
            var e = o as Solid;
            if (e == null) return;
            Faces.Clear();
            foreach (var f in e.Faces.Select(x => x.Clone()))
            {
                f.Parent = this;
                Faces.Add(f);
                f.UpdateBoundingBox();
            }
            UpdateBoundingBox();
        }

        public override void UpdateBoundingBox(bool cascadeToParent = true)
        {
            BoundingBox = new Box(Faces.Select(x => x.BoundingBox));
            base.UpdateBoundingBox(cascadeToParent);
        }

        public override void Transform(Transformations.IUnitTransformation transform, TransformFlags flags)
        {
            Faces.ForEach(f => f.Transform(transform, flags));

            // Handle flip transforms / negative scales
            var origin = GetOrigin();
            if (Faces.All(x => x.Plane.OnPlane(origin) >= 0))
            {
                // All planes are facing inwards - flip them all
                Faces.ForEach(x => x.Flip());
            }

            base.Transform(transform, flags);

        }

        /// <summary>
        /// Returns the intersection point closest to the start of the line.
        /// </summary>
        /// <param name="line">The intersection line</param>
        /// <returns>The closest intersecting point, or null if the line doesn't intersect.</returns>
        public override Coordinate GetIntersectionPoint(Line line)
        {
            return Faces.Select(x => x.GetIntersectionPoint(line))
                .Where(x => x != null)
                .OrderBy(x => (x - line.Start).VectorMagnitude())
                .FirstOrDefault();
        }

        /// <summary>
        /// Splits this solid into two solids by intersecting against a plane.
        /// </summary>
        /// <param name="plane">The splitting plane</param>
        /// <param name="back">The back side of the solid</param>
        /// <param name="front">The front side of the solid</param>
        /// <param name="generator">The IDGenerator to use</param>
        /// <returns>True if the plane splits the solid, false if the plane doesn't intersect</returns>
        public bool Split(Plane plane, out Solid back, out Solid front, IDGenerator generator)
        {
            back = front = null;
            // Check that this solid actually spans the plane
            var classify = Faces.Select(x => x.ClassifyAgainstPlane(plane)).Distinct().ToList();
            if (classify.All(x => x != PlaneClassification.Spanning))
            {
                if (classify.Any(x => x == PlaneClassification.Back)) back = this;
                else if (classify.Any(x => x == PlaneClassification.Front)) front = this;
                return false;
            }

            var backPlanes = new List<Plane> { plane };
            var frontPlanes = new List<Plane> { new Plane(-plane.Normal, -plane.DistanceFromOrigin) };

            foreach (var face in Faces)
            {
                var classification = face.ClassifyAgainstPlane(plane);
                if (classification != PlaneClassification.Back) frontPlanes.Add(face.Plane);
                if (classification != PlaneClassification.Front) backPlanes.Add(face.Plane);
            }

            back = CreateFromIntersectingPlanes(backPlanes, generator);
            front = CreateFromIntersectingPlanes(frontPlanes, generator);
            CopyBase(back, generator);
            CopyBase(front, generator);

            front.Faces.Union(back.Faces).ToList().ForEach(x =>
                                    {
                                        x.Texture = Faces[0].Texture.Clone();
                                        x.AlignTextureToFace();
                                        x.Colour = Colour;
                                    });
            // Restore textures (match the planes up on each face)
            foreach (var orig in Faces)
            {
                foreach (var face in back.Faces)
                {
                    var classification = face.ClassifyAgainstPlane(orig.Plane);
                    if (classification != PlaneClassification.OnPlane) continue;
                    face.Texture = orig.Texture.Clone();
                    break;
                }
                foreach (var face in front.Faces)
                {
                    var classification = face.ClassifyAgainstPlane(orig.Plane);
                    if (classification != PlaneClassification.OnPlane) continue;
                    face.Texture = orig.Texture.Clone();
                    break;
                }
            }
            front.Faces.Union(back.Faces).ToList().ForEach(x => x.CalculateTextureCoordinates(true));

            return true;
        }

        public static Solid CreateFromIntersectingPlanes(IEnumerable<Plane> planes, IDGenerator generator)
        {
            var solid = new Solid(generator.GetNextObjectID());
            var list = planes.ToList();
            for (var i = 0; i < list.Count; i++)
            {
                // Split the polygon by all the other planes
                var poly = new Polygon(list[i]);
                for (var j = 0; j < list.Count; j++)
                {
                    if (i != j) poly.Split(list[j]);
                }

                // The final polygon is the face
                var face = new Face(generator.GetNextFaceID()) { Plane = poly.Plane , Parent = solid };
                face.Vertices.AddRange(poly.Vertices.Select(x => new Vertex(x.Round(2), face))); // Round vertices a bit for sanity
                face.UpdateBoundingBox();
                face.AlignTextureToWorld();
                solid.Faces.Add(face);
            }

            // Ensure all the faces point outwards
            var origin = solid.GetOrigin();
            foreach (var face in solid.Faces)
            {
                if (face.Plane.OnPlane(origin) >= 0) face.Flip();
            }

            solid.UpdateBoundingBox();
            return solid;
        }

        public IEnumerable<Face> GetCoplanarFaces()
        {
            return Faces.Where(f1 => Faces.Where(f2 => f2 != f1).Any(f2 => f2.Plane == f1.Plane));
        }

        public IEnumerable<Face> GetBackwardsFaces(decimal epsilon = 0.001m)
        {
            var origin = GetOrigin();
            return Faces.Where(x => x.Plane.OnPlane(origin, epsilon) > 0);
        }

        public bool IsValid(decimal epsilon = 0.5m)
        {
            return !GetCoplanarFaces().Any() // Check coplanar faces
                   && !GetBackwardsFaces(epsilon).Any() // Check faces are pointing outwards
                   && !Faces.Any(x => x.GetNonPlanarVertices(epsilon).Any()) // Check face vertices are all on the plane
                   && Faces.All(x => x.IsConvex()); // Check all faces are convex
        }

        public Coordinate GetOrigin()
        {
            var points = Faces.SelectMany(x => x.Vertices.Select(y => y.Location)).ToList();
            var origin = points.Aggregate(Coordinate.Zero, (x, y) => x + y) / points.Count;
            return origin;
        }
    }
}
