using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Common;
using Sledge.DataStructures.Transformations;
using Sledge.Editor.Brushes.Controls;
using Sledge.Extensions;

namespace Sledge.Editor.Brushes
{
    public class TorusBrush : IBrush
    {
        private readonly NumericControl _crossSides;
        private readonly NumericControl _crossRadius;
        private readonly BooleanControl _crossMakeHollow;
        private readonly NumericControl _crossArc;
        private readonly NumericControl _crossStartAngle;
        private readonly NumericControl _crossWallWidth;

        private readonly NumericControl _ringSides;
        private readonly NumericControl _ringArc;
        private readonly NumericControl _ringStartAngle;

        private readonly NumericControl _rotationHeight;

        public TorusBrush()
        {
            _crossSides = new NumericControl(this) { LabelText = "Cross sec. sides" };
            _crossRadius = new NumericControl(this) { LabelText = "Ring width", Minimum = 16, Maximum = 1024, Value = 32, Precision = 1 };
            _crossStartAngle = new NumericControl(this) { LabelText = "Cross sec. start", Minimum = 0, Maximum = 359, Value = 0 };
            _crossMakeHollow = new BooleanControl(this) { LabelText = "Make hollow", Checked = false };
            _crossArc = new NumericControl(this) { LabelText = "Cross sec. arc", Minimum = 1, Maximum = 360, Value = 360, Enabled = false };
            _crossWallWidth = new NumericControl(this) { LabelText = "Hollow wall width", Minimum = 1, Maximum = 1024, Value = 16, Precision = 1, Enabled = false};
            _ringSides = new NumericControl(this) { LabelText = "Ring sides" };
            _ringArc = new NumericControl(this) { LabelText = "Ring arc", Minimum = 1, Maximum = 1080, Value = 360 };
            _ringStartAngle = new NumericControl(this) { LabelText = "Ring start", Minimum = 0, Maximum = 359, Value = 0 };
            _rotationHeight = new NumericControl(this) { LabelText = "Rotation height", Minimum = -1024, Maximum = 1024, Value = 0, Precision = 1};
            _crossMakeHollow.ValuesChanged += (s, b) => _crossWallWidth.Enabled = _crossArc.Enabled = _crossMakeHollow.GetValue();
        }

        public string Name
        {
            get { return "Torus"; }
        }

        public bool CanRound { get { return true; } }

        public IEnumerable<BrushControl> GetControls()
        {
            yield return _crossSides;
            yield return _crossRadius;
            yield return _crossStartAngle;
            yield return _crossMakeHollow;
            yield return _crossArc;
            yield return _crossWallWidth;
            yield return _ringSides;
            yield return _ringArc;
            yield return _ringStartAngle;
            yield return _rotationHeight;
        }

        private Solid MakeSolid(IDGenerator generator, IEnumerable<Coordinate[]> faces, ITexture texture, Color col)
        {
            var solid = new Solid(generator.GetNextObjectID()) { Colour = col };
            foreach (var arr in faces)
            {
                var face = new Face(generator.GetNextFaceID())
                {
                    Parent = solid,
                    Plane = new Plane(arr[0], arr[1], arr[2]),
                    Colour = solid.Colour,
                    Texture = { Texture = texture }
                };
                face.Vertices.AddRange(arr.Select(x => new Vertex(x, face)));
                face.UpdateBoundingBox();
                face.AlignTextureToWorld();
                solid.Faces.Add(face);
            }
            solid.UpdateBoundingBox();
            return solid;
        }

        public IEnumerable<MapObject> Create(IDGenerator generator, Box box, ITexture texture, int roundDecimals)
        {
            var crossSides = (int)_crossSides.GetValue();
            if (crossSides < 3) yield break;
            var crossWidth = _crossRadius.GetValue() * 2;
            if (crossWidth < 1) yield break;
            var crossMakeHollow = _crossMakeHollow.GetValue();
            var crossArc = !crossMakeHollow ? 360 : (int)_crossArc.GetValue();
            if (crossArc < 1) yield break;
            var crossStartAngle = (int)_crossStartAngle.GetValue();
            if (crossStartAngle < 0 || crossStartAngle > 359) yield break;
            var crossWallWidth = _crossWallWidth.GetValue();
            if (crossWallWidth < 1) yield break;
            var ringSides = (int)_ringSides.GetValue();
            if (ringSides < 3) yield break;
            var ringArc = (int)_ringArc.GetValue();
            if (ringArc < 1) yield break;
            var ringStartAngle = (int)_ringStartAngle.GetValue();
            if (ringStartAngle < 0 || ringStartAngle > 359) yield break;
            var rotationHeight = _rotationHeight.GetValue();

            // Sort of a combination of cylinder and pipe brushes
            var width = box.Width;
            var length = box.Length;
            var height = box.Height;
            var majorPrimary = (width - crossWidth) / 2; // Primary = donut circle
            var minorPrimary = (length - crossWidth) / 2;
            var majorSecondaryOuter = crossWidth / 2; // Secondary = cross section circle
            var minorSecondaryOuter = height / 2; // Outer = Outer ring
            var majorSecondaryInner = (crossWidth - crossWallWidth) / 2; // Inner = inner ring (hollow only)
            var minorSecondaryInner = (height - crossWallWidth) / 2;

            var ringStart = DMath.DegreesToRadians(ringStartAngle);
            var ringAngle = DMath.DegreesToRadians(ringArc) / ringSides;
            var crossStart = DMath.DegreesToRadians(crossStartAngle);
            var crossAngle = DMath.DegreesToRadians(crossArc) / crossSides;
            var heightAdd = rotationHeight / ringSides;

            // Rotate around the ring, generating each cross section
            var ringOuterSections = new List<Coordinate[]>();
            var ringInnerSections = new List<Coordinate[]>();
            for (var i = 0; i < ringSides + 1; i++)
            {
                var ring = ringStart + i * ringAngle;
                var rxval = box.Center.X + majorPrimary * DMath.Cos(ring);
                var ryval = box.Center.Y + minorPrimary * DMath.Sin(ring);
                var rzval = box.Center.Z;
                var crossSecOuter = new Coordinate[crossSides + 1];
                var crossSecInner = new Coordinate[crossSides + 1];
                for (var j = 0; j < crossSides + 1; j++)
                {
                    var cross = crossStart + j * crossAngle;
                    var xval = majorSecondaryOuter * DMath.Cos(cross) * DMath.Cos(ring);
                    var yval = majorSecondaryOuter * DMath.Cos(cross) * DMath.Sin(ring);
                    var zval = minorSecondaryOuter * DMath.Sin(cross);
                    crossSecOuter[j] = new Coordinate(xval + rxval, yval + ryval, zval + rzval).Round(roundDecimals);
                    if (!crossMakeHollow) continue;
                    xval = majorSecondaryInner * DMath.Cos(cross) * DMath.Cos(ring);
                    yval = majorSecondaryInner * DMath.Cos(cross) * DMath.Sin(ring);
                    zval = minorSecondaryInner * DMath.Sin(cross);
                    crossSecInner[j] = new Coordinate(xval + rxval, yval + ryval, zval + rzval).Round(roundDecimals);
                }
                ringOuterSections.Add(crossSecOuter);
                ringInnerSections.Add(crossSecInner);
            }

            // Create the solids
            var colour = Colour.GetRandomBrushColour();
            for (var i = 0; i < ringSides; i++)
            {
                var vertical = Coordinate.UnitZ * heightAdd * i;
                var nexti = i + 1;
                if (crossMakeHollow)
                {
                    // Use pipe cross sections
                    var outerPoints = ringOuterSections[i];
                    var nextOuterPoints = ringOuterSections[nexti];
                    var innerPoints = ringInnerSections[i];
                    var nextInnerPoints = ringInnerSections[nexti];
                    for (var j = 0; j < crossSides; j++)
                    {
                        var nextj = j + 1;
                        var faces = new List<Coordinate[]>();
                        faces.Add(new[] { outerPoints[j], outerPoints[nextj], nextOuterPoints[nextj], nextOuterPoints[j] }.Select(x => x + vertical).ToArray());
                        faces.Add(new[] { nextInnerPoints[j], nextInnerPoints[nextj], innerPoints[nextj], innerPoints[j] }.Select(x => x + vertical).ToArray());
                        faces.Add(new[] { innerPoints[nextj], nextInnerPoints[nextj], nextOuterPoints[nextj], outerPoints[nextj] }.Select(x => x + vertical).ToArray());
                        faces.Add(new[] { outerPoints[j], nextOuterPoints[j], nextInnerPoints[j], innerPoints[j] }.Select(x => x + vertical).ToArray());
                        faces.Add(new[] { innerPoints[j], innerPoints[nextj], outerPoints[nextj], outerPoints[j] }.Select(x => x + vertical).ToArray());
                        faces.Add(new[] { nextOuterPoints[j], nextOuterPoints[nextj], nextInnerPoints[nextj], nextInnerPoints[j] }.Select(x => x + vertical).ToArray());
                        yield return MakeSolid(generator, faces, texture, colour);
                    }
                }
                else
                {
                    // Use cylindrical cross sections
                    var faces = new List<Coordinate[]>();
                    var points = ringOuterSections[i];
                    var nextPoints = ringOuterSections[nexti];
                    // Add the outer faces
                    for (var j = 0; j < crossSides; j++)
                    {
                        var nextj = (j + 1) % crossSides;
                        faces.Add(new[] { points[j], points[nextj], nextPoints[nextj], nextPoints[j] }.Select(x => x + vertical).ToArray());
                    }
                    // Add the cross section faces
                    faces.Add(points.Reverse().Select(x => x + vertical).ToArray());
                    faces.Add(nextPoints.Select(x => x + vertical).ToArray());
                    yield return MakeSolid(generator, faces, texture, colour);
                }
            }
        }
    }
}
