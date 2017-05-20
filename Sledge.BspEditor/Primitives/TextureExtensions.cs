using System;
using System.Linq;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.Transformations;

namespace Sledge.BspEditor.Primitives
{
    public static class TextureExtensions
    {
        public static void AlignToNormal(this Texture tex, Coordinate normal)
        {
            // Get the closest axis for this normal
            Coordinate axis;
            if (normal.X >= normal.Y && normal.X >= normal.Z) axis = Coordinate.UnitX;
            else if (normal.Y >= normal.Z) axis = Coordinate.UnitY;
            else axis = Coordinate.UnitZ;

            var tempV = axis == Coordinate.UnitZ ? -Coordinate.UnitY : -Coordinate.UnitZ;
            tex.UAxis = normal.Cross(tempV).Normalise();
            tex.VAxis = tex.UAxis.Cross(normal).Normalise();
            tex.Rotation = 0;
        }

        public static bool IsAlignedToNormal(this Texture tex, Coordinate normal)
        {
            var cp = tex.UAxis.Cross(tex.VAxis).Normalise();
            return cp.EquivalentTo(normal, 0.01m) || cp.EquivalentTo(-normal, 0.01m);
        }

        public static void AlignWithTexture(this Texture tex, Plane currentPlane, Plane alignToPlane, Texture alignToTexture)
        {
            // Get reference values for the axes
            var refU = alignToTexture.UAxis;
            var refV = alignToTexture.VAxis;
            // Reference points in the texture plane to use for shifting later on
            var refX = alignToTexture.UAxis * alignToTexture.XShift * alignToTexture.XScale;
            var refY = alignToTexture.VAxis * alignToTexture.YShift * alignToTexture.YScale;

            // Two non-parallel planes intersect at an edge. We want the textures on this face
            // to line up with the textures on the provided face. To do this, we rotate the texture 
            // normal on the provided face around the intersection edge to get the new texture axes.
            // Then we rotate the texture reference point around this edge as well to get the new shift values.
            // The scale values on both faces will always end up being the same value.

            // Find the intersection edge vector
            var intersectionEdge = alignToPlane.Normal.Cross(currentPlane.Normal);
            // Create a plane using the intersection edge as the normal
            var intersectionPlane = new Plane(intersectionEdge, 0);

            // If the planes are parallel, the texture doesn't need any rotation - just different shift values.
            var intersect = Plane.Intersect(alignToPlane, currentPlane, intersectionPlane);
            if (intersect != null)
            {
                var texNormal = alignToTexture.GetNormal();

                // Since the intersection plane is perpendicular to both face planes, we can find the angle
                // between the two planes (the original texture plane and the plane of this face) by projecting
                // the normals of the planes onto the perpendicular plane and taking the cross product.

                // Project the two normals onto the perpendicular plane
                var ptNormal = intersectionPlane.Project(texNormal).Normalise();
                var ppNormal = intersectionPlane.Project(currentPlane.Normal).Normalise();

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
            tex.Rotation = 0;
            tex.UAxis = refU;
            tex.VAxis = refV;
            tex.XShift = refU.Dot(refX) / alignToTexture.XScale;
            tex.YShift = refV.Dot(refY) / alignToTexture.YScale;
            tex.XScale = alignToTexture.XScale;
            tex.YScale = alignToTexture.YScale;
        }

        public static void MinimiseTextureShiftValues(this Texture tex, int width, int height)
        {
            if (width <= 0 || height <= 0) return;

            // Keep the shift values to a minimum
            tex.XShift = tex.XShift % width;
            tex.YShift = tex.YShift % height;
            if (tex.XShift < -width / 2m) tex.XShift += width;
            if (tex.YShift < -height / 2m) tex.YShift += height;
        }

        public static void FitToPointCloud(this Texture tex, int width, int height, Cloud cloud, int tileX, int tileY)
        {
            if (width <= 0 || height <= 0) return;
            if (tileX <= 0) tileX = 1;
            if (tileY <= 0) tileY = 1;

            // Scale will change, no need to use it in the calculations
            var xvals = cloud.GetExtents().Select(x => x.Dot(tex.UAxis)).ToList();
            var yvals = cloud.GetExtents().Select(x => x.Dot(tex.VAxis)).ToList();

            var minU = xvals.Min();
            var minV = yvals.Min();
            var maxU = xvals.Max();
            var maxV = yvals.Max();

            tex.XScale = (maxU - minU) / (width * tileX);
            tex.YScale = (maxV - minV) / (height * tileY);
            tex.XShift = -minU / tex.XScale;
            tex.YShift = -minV / tex.YScale;
        }

        public static void AlignWithPointCloud(this Texture tex, int width, int height, Cloud cloud, BoxAlignMode mode)
        {
            if (width <= 0 || height <= 0) return;

            var xvals = cloud.GetExtents().Select(x => x.Dot(tex.UAxis) / tex.XScale).ToList();
            var yvals = cloud.GetExtents().Select(x => x.Dot(tex.VAxis) / tex.YScale).ToList();

            var minU = xvals.Min();
            var minV = yvals.Min();
            var maxU = xvals.Max();
            var maxV = yvals.Max();

            switch (mode)
            {
                case BoxAlignMode.Left:
                    tex.XShift = -minU;
                    break;
                case BoxAlignMode.Right:
                    tex.XShift = -maxU + width;
                    break;
                case BoxAlignMode.Center:
                    var avgU = (minU + maxU) / 2;
                    var avgV = (minV + maxV) / 2;
                    tex.XShift = -avgU + width / 2m;
                    tex.YShift = -avgV + height / 2m;
                    break;
                case BoxAlignMode.Top:
                    tex.YShift = -minV;
                    break;
                case BoxAlignMode.Bottom:
                    tex.YShift = -maxV + height;
                    break;
            }
        }
        
        public static void SetRotation(this Texture tex, decimal rotate)
        {
            var rads = DMath.DegreesToRadians(tex.Rotation - rotate);

            // Rotate around the texture normal
            var texNorm = tex.VAxis.Cross(tex.UAxis).Normalise();
            var transform = new UnitRotate(rads, new Line(Coordinate.Zero, texNorm));
            tex.UAxis = transform.Transform(tex.UAxis);
            tex.VAxis = transform.Transform(tex.VAxis);
            tex.Rotation = rotate;
        }
    }
}