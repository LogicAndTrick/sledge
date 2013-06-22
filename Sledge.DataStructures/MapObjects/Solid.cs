using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.MapObjects
{
    public class Solid : MapObject
    {
        public List<Face> Faces { get; private set; }

        public Solid(long id) : base(id)
        {
            Faces = new List<Face>();
        }

        public override MapObject Copy(IDGenerator generator)
        {
            var e = new Solid(generator.GetNextObjectID());
            foreach (var f in Faces.Select(x => x.Copy(generator)))
            {
                f.Parent = e;
                e.Faces.Add(f);
                f.UpdateBoundingBox();
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

        public override void Transform(Transformations.IUnitTransformation transform)
        {
            Faces.ForEach(f => f.Transform(transform));

            // Handle flip transforms / negative scales
            var origin = GetOrigin();
            if (Faces.All(x => x.Plane.OnPlane(origin) >= 0))
            {
                // All planes are facing inwards - flip them all
                Faces.ForEach(x => x.Flip());
            }

            base.Transform(transform);

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
            front.Faces.Union(back.Faces).ToList().ForEach(x => x.CalculateTextureCoordinates());

            return true;
        }

        public static Solid CreateFromIntersectingPlanes(IEnumerable<Plane> planes, IDGenerator generator)
        {
            const short max = short.MaxValue;
            var list = planes.ToList();
            var verts = new List<List<Coordinate>>();
            var count = list.Count;
            verts.AddRange(Enumerable.Range(0, count).Select(x => new List<Coordinate>()));

            // Step 1: Intersect the planes
            for (var i = 0; i < count; i++)
            {
                for (var j = 0; j < count; j++)
                {
                    for (var k = 0; k < count; k++)
                    {
                        if (i == j || i == k || j == k) continue; // Don't bother if the planes are the same
                        var intersect = Plane.Intersect(list[i], list[j], list[k]);
                        if (intersect == null) continue; // If they don't intersect, skip it
                        var abs = intersect.Absolute();
                        if (abs.X > max || abs.Y > max || abs.Z > max) continue; // Make sure the intersect is in bounds
                        intersect = intersect.Round();
                        var legal = true;
                        for (var l = 0; l < count; l++)
                        {
                            if (l == i || l == j || l == k) continue;
                            if (list[l].OnPlane(intersect) > 0)
                            {
                                // If the point is above any of the plane, it is not a valid intersect
                                legal = false;
                                break;
                            }
                        }
                        if (!legal) continue;
                        // Add the point into the three faces corresponding to the planes
                        if (!verts[i].Contains(intersect)) verts[i].Add(intersect);
                        if (!verts[j].Contains(intersect)) verts[j].Add(intersect);
                        if (!verts[k].Contains(intersect)) verts[k].Add(intersect);
                    }
                }
            }

            // Step 2: Find the brush origin
            var sum = Coordinate.Zero;
            var cnt = 0;
            foreach (var v in verts.SelectMany(x => x))
            {
                sum += v;
                cnt++;
            }
            var origin = sum / cnt;

            var solid = new Solid(generator.GetNextObjectID());

            // Step 3: Sort the vertices
            for (var i = 0; i < count; i++)
            {
                var points = verts[i];
                var center = points.Aggregate(Coordinate.Zero, (x, y) => x + y) / points.Count;
                var plane = new Plane(center - origin, points[0]);
                for (var j = 0; j < points.Count - 2; j++)
                {
                    var a = (points[j] - center).Normalise();
                    var p = new Plane(points[j], center, center + list[i].Normal);
                    var smallestAngle = -1m;
                    var smallestIndex = -1;
                    for (var k = j + 1; k < points.Count; k++)
                    {
                        if (p.OnPlane(points[k]) < 0) continue;
                        var b = (points[k] - center).Normalise();
                        var angle = a.Dot(b);
                        if (angle <= smallestAngle) continue;
                        smallestAngle = angle;
                        smallestIndex = k;
                    }
                    if (smallestIndex == j + 1 || smallestIndex == -1) continue;
                    var temp = points[j + 1];
                    points[j + 1] = points[smallestIndex];
                    points[smallestIndex] = temp;
                }
                var tempPlane = new Plane(points[0], points[1], points[2]);
                if (tempPlane.Normal.Dot(plane.Normal) < 0) points.Reverse();
                var face = new Face(generator.GetNextFaceID()) { Parent = solid, Plane = new Plane(points[0], points[1], points[2]) };
                face.Vertices.AddRange(points.Select(x => new Vertex(x, face)));
                face.UpdateBoundingBox();
                face.AlignTextureToWorld();
                solid.Faces.Add(face);
            }
            solid.UpdateBoundingBox();
            return solid;
        }

        public bool IsValid()
        {
            // Check coplanar faces
            if (Faces.Any(f1 => Faces.Where(f2 => f2 != f1).Any(f2 => f2.Plane == f1.Plane)))
            {
                return false;
            }

            // Check face vertices are all on the plane
            if (Faces.Any(x => x.Vertices.Any(y => x.Plane.OnPlane(y.Location) != 0)))
            {
                return false;
            }

            // Check faces are pointing outwards
            var origin = GetOrigin();
            if (Faces.Any(x => x.Plane.OnPlane(origin) >= 0))
            {
                return false;
            }

            return true;
        }

        public Coordinate GetOrigin()
        {
            var points = Faces.SelectMany(x => x.Vertices.Select(y => y.Location)).ToList();
            var origin = points.Aggregate(Coordinate.Zero, (x, y) => x + y) / points.Count;
            return origin;
        }
    }
}
