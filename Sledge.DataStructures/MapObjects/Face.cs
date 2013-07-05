using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.Transformations;
using Sledge.Extensions;

namespace Sledge.DataStructures.MapObjects
{
    public class Face
    {
        public long ID { get; set; }
        public Color Colour { get; set; }
        public Plane Plane { get; set; }

        public bool IsSelected { get; set; }
        public bool IsHidden { get; set; }

        public TextureReference Texture { get; set; }
        public List<Vertex> Vertices { get; set; }

        public Solid Parent { get; set; }

        public Box BoundingBox { get; set; }

        public Face(long id)
        {
            ID = id;
            Texture = new TextureReference();
            Vertices = new List<Vertex>();
            IsSelected = false;
        }

        public Face Copy(IDGenerator generator)
        {
            var f = new Face(generator.GetNextFaceID())
                        {
                            Plane = Plane.Clone(),
                            Colour = Colour,
                            IsSelected = IsSelected,
                            Texture = Texture.Clone(),
                            Parent = Parent,
                            BoundingBox = BoundingBox.Clone()
                        };
            foreach (var v in Vertices.Select(x => x.Clone()))
            {
                v.Parent = f;
                f.Vertices.Add(v);
            }
            return f;
        }

        public Face Clone()
        {
            var f = Copy(new IDGenerator());
            f.ID = ID;
            return f;
        }

        public void Paste(Face f)
        {
            Plane = f.Plane.Clone();
            Colour = f.Colour;
            IsSelected = f.IsSelected;
            Texture = f.Texture.Clone();
            Parent = f.Parent;
            BoundingBox = f.BoundingBox.Clone();
            Vertices.Clear();
            foreach (var v in f.Vertices.Select(x => x.Clone()))
            {
                v.Parent = this;
                Vertices.Add(v);
            }
        }

        public void Unclone(Face f)
        {
            Paste(f);
            ID = f.ID;
        }

        public virtual IEnumerable<Line> GetLines()
        {
            return GetEdges();
        }

        public IEnumerable<Line> GetEdges()
        {
            for (var i = 0; i < Vertices.Count; i++)
            {
                yield return new Line(Vertices[i].Location, Vertices[(i + 1) % Vertices.Count].Location);
            }
        }

        public virtual IEnumerable<Vertex[]> GetTriangles()
        {
            for (var i = 1; i < Vertices.Count - 1; i++)
            {
                yield return new[]
                                 {
                                     Vertices[0],
                                     Vertices[i],
                                     Vertices[i + 1]
                                 };
            }
        }

        #region Textures

        public enum BoxAlignMode
        {
            Left,
            Right,
            Center,
            Top,
            Bottom
        }

        public virtual void CalculateTextureCoordinates()
        {
            MinimiseTextureShiftValues();
            Vertices.ForEach(c => c.TextureU = c.TextureV = 0);

            if (Texture.Texture == null) return;

            var udiv = Texture.Texture.Width * Texture.XScale;
            var uadd = Texture.XShift / Texture.Texture.Width;
            var vdiv = Texture.Texture.Height * Texture.YScale;
            var vadd = Texture.YShift / Texture.Texture.Height;

            foreach (var v in Vertices)
            {
                v.TextureU = (v.Location.Dot(Texture.UAxis) / udiv) + uadd;
                v.TextureV = (v.Location.Dot(Texture.VAxis) / vdiv) + vadd;
            }
        }

        public void AlignTextureToWorld()
        {
            // Set the U and V axes to match the X, Y, or Z axes
            // How they are calculated depends on which direction the plane is facing

            var direction = Plane.GetClosestAxisToNormal();

            // VHE behaviour:
            // U axis: If the closest axis to the normal is the X axis,
            //         the U axis is UnitY. Otherwise, the U axis is UnitX.
            // V axis: If the closest axis to the normal is the Z axis,
            //         the V axis is -UnitY. Otherwise, the V axis is -UnitZ.

            Texture.UAxis = direction == Coordinate.UnitX ? Coordinate.UnitY : Coordinate.UnitX;
            Texture.VAxis = direction == Coordinate.UnitZ ? -Coordinate.UnitY : -Coordinate.UnitZ;
            Texture.Rotation = 0;

            CalculateTextureCoordinates();
        }

        public void AlignTextureToFace()
        {
            // Set the U and V axes to match the plane's normal
            // Need to start with the world alignment on the V axis so that we don't align backwards.
            // Then we can calculate U based on that, and the real V afterwards.

            var direction = Plane.GetClosestAxisToNormal();

            var tempV = direction == Coordinate.UnitZ ? -Coordinate.UnitY : -Coordinate.UnitZ;
            Texture.UAxis = tempV.Cross(Plane.Normal).Normalise();
            Texture.VAxis = Plane.Normal.Cross(Texture.UAxis).Normalise();
            Texture.Rotation = 0;

            CalculateTextureCoordinates();
        }

        public void AlignTextureWithFace(Face face)
        {
            // Get reference values for the axes
            var refU = face.Texture.UAxis;
            var refV = face.Texture.VAxis;
            // Reference points in the texture plane to use for shifting later on
            var refX = face.Texture.UAxis * face.Texture.XShift * face.Texture.XScale;
            var refY = face.Texture.VAxis * face.Texture.YShift * face.Texture.YScale;

            // Two non-parallel planes intersect at an edge. We want the textures on this face
            // to line up with the textures on the provided face. To do this, we rotate the texture 
            // normal on the provided face around the intersection edge to get the new texture axes.
            // Then we rotate the texture reference point around this edge as well to get the new shift values.
            // The scale values on both faces will always end up being the same value.

            // Find the intersection edge vector
            var intersectionEdge = face.Plane.Normal.Cross(Plane.Normal);
            // Create a plane using the intersection edge as the normal
            var intersectionPlane = new Plane(intersectionEdge, 0);
            
            // If the planes are parallel, the texture doesn't need any rotation - just different shift values.
            var intersect = Plane.Intersect(face.Plane, Plane, intersectionPlane);
            if (intersect != null)
            {
                var texNormal = face.Texture.GetNormal();

                // Since the intersection plane is perpendicular to both face planes, we can find the angle
                // between the two planes (the original texture plane and the plane of this face) by projecting
                // the normals of the planes onto the perpendicular plane and taking the cross product.

                // Project the two normals onto the perpendicular plane
                var ptNormal = intersectionPlane.Project(texNormal).Normalise();
                var ppNormal = intersectionPlane.Project(Plane.Normal).Normalise();

                // Get the angle between the projected normals
                var dot = Math.Round(ptNormal.Dot(ppNormal), 4);
                var angle = DMath.Acos(dot); // A.B = cos(angle)

                // Rotate the texture axis by the angle around the intersection edge
                var transform = new UnitRotate(angle, new Line(Coordinate.Zero, intersectionEdge));
                refU = transform.Transform(refU);
                refV = transform.Transform(refV);

                // Rotate the texture reference points as well, but around the intersection line, not the origin
                refX = transform.Transform(refX + intersect) - intersect;
                refY = transform.Transform(refY + intersect) - intersect;
            }

            // Convert the reference points back to get the final values
            Texture.Rotation = 0;
            Texture.UAxis = refU;
            Texture.VAxis = refV;
            Texture.XShift = refU.Dot(refX) / face.Texture.XScale;
            Texture.YShift = refV.Dot(refY) / face.Texture.YScale;
            Texture.XScale = face.Texture.XScale;
            Texture.YScale = face.Texture.YScale;

            CalculateTextureCoordinates();
        }

        private void MinimiseTextureShiftValues()
        {
            if (Texture.Texture == null) return;
            // Keep the shift values to a minimum
            Texture.XShift = Texture.XShift % Texture.Texture.Width;
            Texture.YShift = Texture.YShift % Texture.Texture.Height;
            if (Texture.XShift < -Texture.Texture.Width / 2m) Texture.XShift += Texture.Texture.Width;
            if (Texture.YShift < -Texture.Texture.Height / 2m) Texture.YShift += Texture.Texture.Height;
        }

        public void FitTextureToPointCloud(Cloud cloud)
        {
            if (Texture.Texture == null) return;

            // Scale will change, no need to use it in the calculations
            var xvals = cloud.GetExtents().Select(x => x.Dot(Texture.UAxis)).ToList();
            var yvals = cloud.GetExtents().Select(x => x.Dot(Texture.VAxis)).ToList();

            var minU = xvals.Min();
            var minV = yvals.Min();
            var maxU = xvals.Max();
            var maxV = yvals.Max();

            Texture.XScale = (maxU - minU) / Texture.Texture.Width;
            Texture.YScale = (maxV - minV) / Texture.Texture.Height;
            Texture.XShift = -minU / Texture.XScale;
            Texture.YShift = -minV / Texture.YScale;

            CalculateTextureCoordinates();
        }

        public void AlignTextureWithPointCloud(Cloud cloud, BoxAlignMode mode)
        {
            if (Texture.Texture == null) return;

            var xvals = cloud.GetExtents().Select(x => x.Dot(Texture.UAxis) / Texture.XScale).ToList();
            var yvals = cloud.GetExtents().Select(x => x.Dot(Texture.VAxis) / Texture.YScale).ToList();

            var minU = xvals.Min();
            var minV = yvals.Min();
            var maxU = xvals.Max();
            var maxV = yvals.Max();

            switch (mode)
            {
                case BoxAlignMode.Left:
                    Texture.XShift = -minU;
                    break;
                case BoxAlignMode.Right:
                    Texture.XShift = -maxU + Texture.Texture.Width;
                    break;
                case BoxAlignMode.Center:
                    var avgU = (minU + maxU) / 2;
                    var avgV = (minV + maxV) / 2;
                    Texture.XShift = -avgU + Texture.Texture.Width / 2m;
                    Texture.YShift = -avgV + Texture.Texture.Height / 2m;
                    break;
                case BoxAlignMode.Top:
                    Texture.YShift = -minV;
                    break;
                case BoxAlignMode.Bottom:
                    Texture.YShift = -maxV + Texture.Texture.Height;
                    break;
            }
            CalculateTextureCoordinates();
        }

        /// <summary>
        /// Rotate the texture around the texture normal.
        /// </summary>
        /// <param name="rotate">The rotation angle in degrees</param>
        public void SetTextureRotation(decimal rotate)
        {
            var rads = DMath.DegreesToRadians(Texture.Rotation - rotate);
            // Rotate around the texture normal
            var texNorm = Texture.VAxis.Cross(Texture.UAxis).Normalise();
            var transform = new UnitRotate(rads, new Line(Coordinate.Zero, texNorm));
            Texture.UAxis = transform.Transform(Texture.UAxis);
            Texture.VAxis = transform.Transform(Texture.VAxis);
            Texture.Rotation = rotate;

            CalculateTextureCoordinates();
        }

        #endregion

        public virtual void UpdateBoundingBox()
        {
            BoundingBox = new Box(Vertices.Select(x => x.Location));
        }

        public virtual void Transform(IUnitTransformation transform, TransformFlags flags)
        {
            foreach (var t in Vertices)
            {
                t.Location = transform.Transform(t.Location);
            }
            Plane = new Plane(Vertices[0].Location, Vertices[1].Location, Vertices[2].Location);
            Colour = Colour;
            if (flags.HasFlag(TransformFlags.TextureScalingLock))
            {
                // Make a best-effort guess of retaining scaling. All bets are off during skew operations.
                // Transform the current texture axes
                var origin = transform.Transform(Coordinate.Zero);
                var ua = transform.Transform(Texture.UAxis) - origin;
                var va = transform.Transform(Texture.VAxis) - origin;
                // Multiply the scales by the magnitudes (they were normals before the transform operation)
                Texture.XScale *= ua.VectorMagnitude();
                Texture.YScale *= va.VectorMagnitude();
            }
            if (flags.HasFlag(TransformFlags.TextureLock))
            {
                // Transform the texture axes and move them back to the origin
                var origin = transform.Transform(Coordinate.Zero);
                var ua = transform.Transform(Texture.UAxis) - origin;
                var va = transform.Transform(Texture.VAxis) - origin;
                // Only do the transform if the axes end up being not perpendicular
                // Otherwise just make a best-effort guess, same as the scaling lock
                if (Math.Abs(ua.Dot(va)) < 0.0001m)
                {
                    Texture.UAxis = ua;
                    Texture.VAxis = va;
                }
                // Calculate the new shift values based on the UV values of the vertices
                var vtx = Vertices[0];
                Texture.XShift = Texture.Texture.Width * vtx.TextureU - (vtx.Location.Dot(Texture.UAxis)) / Texture.XScale;
                Texture.YShift = Texture.Texture.Height * vtx.TextureV - (vtx.Location.Dot(Texture.VAxis)) / Texture.YScale;
            }
            CalculateTextureCoordinates();
            UpdateBoundingBox();
        }

        public virtual void Flip()
        {
            Vertices.Reverse();
            Plane = new Plane(Vertices[0].Location, Vertices[1].Location, Vertices[2].Location);
            UpdateBoundingBox();
        }

        /// <summary>
        /// Returns the point that this line intersects with this face.
        /// </summary>
        /// <param name="line">The intersection line</param>
        /// <returns>The point of intersection between the face and the line.
        /// Returns null if the line does not intersect this face.</returns>
        public virtual Coordinate GetIntersectionPoint(Line line)
        {
            return GetIntersectionPoint(Vertices.Select(x => x.Location).ToList(), line);
        }

        /// <summary>
        /// Test all the edges of this face against a bounding box to see if they intersect.
        /// </summary>
        /// <param name="box">The box to intersect</param>
        /// <returns>True if one of the face's edges intersects with the box.</returns>
        public bool IntersectsWithLine(Box box)
        {
            // Shortcut through the bounding box to avoid the line computations if they aren't needed
            return BoundingBox.IntersectsWith(box) && GetLines().Any(box.IntersectsWith);
        }

        /// <summary>
        /// Test this face to see if the given bounding box intersects with it
        /// </summary>
        /// <param name="box">The box to test against</param>
        /// <returns>True if the box intersects</returns>
        public bool IntersectsWithBox(Box box)
        {
            var verts = Vertices.Select(x => x.Location).ToList();
            return box.GetBoxLines().Any(x => GetIntersectionPoint(verts, x, true) != null);
        }

        /// <summary>
        /// Determines if this face is behind, in front, or spanning a plane.
        /// </summary>
        /// <param name="p">The plane to test against</param>
        /// <returns>A PlaneClassification value.</returns>
        public PlaneClassification ClassifyAgainstPlane(Plane p)
        {
            int front = 0, back = 0, onplane = 0, count = Vertices.Count;

            foreach (var test in Vertices.Select(v => v.Location).Select(p.OnPlane))
            {
                // Vertices on the plane are both in front and behind the plane in this context
                if (test <= 0) back++;
                if (test >= 0) front++;
                if (test == 0) onplane++;
            }

            if (onplane == count) return PlaneClassification.OnPlane;
            if (front == count) return PlaneClassification.Front;
            if (back == count) return PlaneClassification.Back;
            return PlaneClassification.Spanning;
        }

        protected static Coordinate GetIntersectionPoint(IList<Coordinate> coordinates, Line line, bool ignoreDirection = false)
        {
            var plane = new Plane(coordinates[0], coordinates[1], coordinates[2]);
            var intersect = plane.GetIntersectionPoint(line, ignoreDirection);
            if (intersect == null) return null;

            // http://paulbourke.net/geometry/insidepoly/

            // The angle sum will be 2 * PI if the point is inside the face
            double sum = 0;
            for (var i = 0; i < coordinates.Count; i++)
            {
                var i1 = i;
                var i2 = (i + 1) % coordinates.Count;

                // Translate the vertices so that the intersect point is on the origin
                var v1 = coordinates[i1] - intersect;
                var v2 = coordinates[i2] - intersect;

                var m1 = v1.VectorMagnitude();
                var m2 = v2.VectorMagnitude();
                var nom = m1 * m2;
                if (nom < 0.001m)
                {
                    // intersection is at a vertex
                    return intersect;
                }
                sum += Math.Acos((double)(v1.Dot(v2) / nom));
            }

            var delta = Math.Abs(sum - Math.PI * 2);
            return (delta < 0.001d) ? intersect : null;
        }
    }
}
