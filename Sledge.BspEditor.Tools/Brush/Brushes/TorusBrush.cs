using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Tools.Brush.Brushes.Controls;
using Sledge.Common;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using Plane = Sledge.DataStructures.Geometric.Plane;

namespace Sledge.BspEditor.Tools.Brush.Brushes
{
    [Export(typeof(IBrush))]
    [Export(typeof(IInitialiseHook))]
    [OrderHint("J")]
    [AutoTranslate]
    public class TorusBrush : IBrush, IInitialiseHook
    {
        private NumericControl _crossSides;
        private NumericControl _crossRadius;
        private BooleanControl _crossMakeHollow;
        private NumericControl _crossArc;
        private NumericControl _crossStartAngle;
        private NumericControl _crossWallWidth;

        private NumericControl _ringSides;
        private NumericControl _ringArc;
        private NumericControl _ringStartAngle;

        private NumericControl _rotationHeight;

        public string CrossSectionSides { get; set; }
        public string RingWidth { get; set; }
        public string CrossSectionStart { get; set; }
        public string MakeHollow { get; set; }
        public string CrossSectionArc { get; set; }
        public string HollowWallWidth { get; set; }
        public string RingSides { get; set; }
        public string RingArc { get; set; }
        public string RingStart { get; set; }
        public string RotationHeight { get; set; }

        public Task OnInitialise()
        {
            _crossSides = new NumericControl(this) { LabelText = CrossSectionSides };
            _crossRadius = new NumericControl(this) { LabelText = RingWidth, Minimum = 16, Maximum = 1024, Value = 32, Precision = 1 };
            _crossStartAngle = new NumericControl(this) { LabelText = CrossSectionStart, Minimum = 0, Maximum = 359, Value = 0 };
            _crossMakeHollow = new BooleanControl(this) { LabelText = MakeHollow, Checked = false };
            _crossArc = new NumericControl(this) { LabelText = CrossSectionArc, Minimum = 1, Maximum = 360, Value = 360, Enabled = false };
            _crossWallWidth = new NumericControl(this) { LabelText = HollowWallWidth, Minimum = 1, Maximum = 1024, Value = 16, Precision = 1, Enabled = false};
            _ringSides = new NumericControl(this) { LabelText = RingSides };
            _ringArc = new NumericControl(this) { LabelText = RingArc, Minimum = 1, Maximum = 1080, Value = 360 };
            _ringStartAngle = new NumericControl(this) { LabelText = RingStart, Minimum = 0, Maximum = 359, Value = 0 };
            _rotationHeight = new NumericControl(this) { LabelText = RotationHeight, Minimum = -1024, Maximum = 1024, Value = 0, Precision = 1};
            _crossMakeHollow.ValuesChanged += (s, b) => _crossWallWidth.Enabled = _crossArc.Enabled = _crossMakeHollow.GetValue();

            return Task.CompletedTask;
        }

        public string Name { get; set; } = "Torus";

        public bool CanRound => false;

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

        private Solid MakeSolid(UniqueNumberGenerator generator, IEnumerable<Vector3[]> faces, string texture, Color col)
        {
            var solid = new Solid(generator.Next("MapObject"));
            solid.Data.Add(new ObjectColor(Colour.GetRandomBrushColour()));

            foreach (var arr in faces)
            {
                var face = new Face(generator.Next("Face"))
                {
                    Plane = new Plane(arr[0], arr[1], arr[2]),
                    Texture = { Name = texture}
                };
                face.Vertices.AddRange(arr);
                solid.Data.Add(face);
            }

            solid.DescendantsChanged();
            return solid;
        }

        public IEnumerable<IMapObject> Create(UniqueNumberGenerator generator, Box box, string texture, int roundDecimals)
        {
            roundDecimals = 2; // don't support rounding

            var crossSides = (int)_crossSides.GetValue();
            if (crossSides < 3) yield break;

            var crossWidth = (float) _crossRadius.GetValue() * 2;
            if (crossWidth < 1) yield break;

            var crossMakeHollow = _crossMakeHollow.GetValue();
            var crossArc = !crossMakeHollow ? 360 : (float)_crossArc.GetValue();
            if (crossArc < 1) yield break;

            var crossStartAngle = (float)_crossStartAngle.GetValue();
            if (crossStartAngle < 0 || crossStartAngle > 359) yield break;

            var crossWallWidth = (float) _crossWallWidth.GetValue();
            if (crossWallWidth < 1) yield break;

            var ringSides = (int)_ringSides.GetValue();
            if (ringSides < 3) yield break;

            var ringArc = (float)_ringArc.GetValue();
            if (ringArc < 1) yield break;

            var ringStartAngle = (float)_ringStartAngle.GetValue();
            if (ringStartAngle < 0 || ringStartAngle > 359) yield break;

            var rotationHeight = (float) _rotationHeight.GetValue();

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

            var ringStart = (float) MathHelper.DegreesToRadians(ringStartAngle);
            var ringAngle = (float) MathHelper.DegreesToRadians(ringArc) / ringSides;
            var crossStart = (float) MathHelper.DegreesToRadians(crossStartAngle);
            var crossAngle = (float) MathHelper.DegreesToRadians(crossArc) / crossSides;
            var heightAdd = rotationHeight / ringSides;

            // Rotate around the ring, generating each cross section
            var ringOuterSections = new List<Vector3[]>();
            var ringInnerSections = new List<Vector3[]>();
            for (var i = 0; i < ringSides + 1; i++)
            {
                var ring = ringStart + i * ringAngle;
                var rxval = box.Center.X + majorPrimary * (float) Math.Cos(ring);
                var ryval = box.Center.Y + minorPrimary * (float) Math.Sin(ring);
                var rzval = box.Center.Z;
                var crossSecOuter = new Vector3[crossSides + 1];
                var crossSecInner = new Vector3[crossSides + 1];
                for (var j = 0; j < crossSides + 1; j++)
                {
                    var cross = crossStart + j * crossAngle;
                    var xval = majorSecondaryOuter * (float) Math.Cos(cross) * (float) Math.Cos(ring);
                    var yval = majorSecondaryOuter * (float) Math.Cos(cross) * (float) Math.Sin(ring);
                    var zval = minorSecondaryOuter * (float) Math.Sin(cross);
                    crossSecOuter[j] = new Vector3(xval + rxval, yval + ryval, zval + rzval).Round(roundDecimals);
                    if (!crossMakeHollow) continue;

                    xval = majorSecondaryInner * (float) Math.Cos(cross) * (float) Math.Cos(ring);
                    yval = majorSecondaryInner * (float) Math.Cos(cross) * (float) Math.Sin(ring);
                    zval = minorSecondaryInner * (float) Math.Sin(cross);
                    crossSecInner[j] = new Vector3(xval + rxval, yval + ryval, zval + rzval).Round(roundDecimals);
                }
                ringOuterSections.Add(crossSecOuter);
                ringInnerSections.Add(crossSecInner);
            }

            // Create the solids
            var colour = Colour.GetRandomBrushColour();
            for (var i = 0; i < ringSides; i++)
            {
                var vertical = Vector3.UnitZ * heightAdd * i;
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
                        var faces = new List<Vector3[]>();
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
                    var faces = new List<Vector3[]>();
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
