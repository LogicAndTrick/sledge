using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Common;
using Sledge.Editor.Brushes.Controls;
using Sledge.Extensions;

namespace Sledge.Editor.Brushes
{
    public class ArchBrush : IBrush
    {
        private readonly NumericControl _arc;
        private readonly NumericControl _startAngle;
        private readonly NumericControl _wallWidth;
        private readonly NumericControl _numSides;
        private readonly NumericControl _addHeight;

        public ArchBrush()
        {
            _arc = new NumericControl(this) { LabelText = "Arc", Minimum = 1, Maximum = 360*4, Value = 360 };
            _wallWidth = new NumericControl(this) { LabelText = "Wall width", Minimum = 1, Maximum = 1024, Value = 16};
            _numSides = new NumericControl(this) { LabelText = "Num. sides" };
            _startAngle = new NumericControl(this) { LabelText = "Start Angle", Minimum = 0, Maximum = 359, Value = 0 };
            _addHeight = new NumericControl(this) { LabelText = "Add height", Minimum = -1024, Maximum = 1024, Value = 0};
        }

        public string Name
        {
            get { return "Arch"; }
        }

        public IEnumerable<BrushControl> GetControls()
        {
            yield return _numSides;
            yield return _startAngle;
            yield return _arc;
            yield return _wallWidth;
            yield return _addHeight;
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

        public IEnumerable<MapObject> Create(IDGenerator generator, Box box, ITexture texture)
        {
            var numsides = (int)_numSides.GetValue();
            if (numsides < 3) yield break;
            var startAngle = (int)_startAngle.GetValue();
            if (startAngle < 0 || startAngle > 359) yield break;
            var arc = (int)_arc.GetValue();
            if (arc < 1) yield break;
            var wallWidth = _wallWidth.GetValue();
            if (wallWidth < 1) yield break;
            var archHeight = _addHeight.GetValue();

            // Very similar to the pipe brush, except with options for start angle, arc, and height
            var width = box.Width;
            var length = box.Length;
            var height = box.Height;
            var majorOut = width / 2;
            var majorIn = majorOut - wallWidth;
            var minorOut = length / 2;
            var minorIn = minorOut - wallWidth;

            var start = DMath.DegreesToRadians(startAngle);
            var angle = DMath.DegreesToRadians(arc) / numsides;
            var addHeight = archHeight / numsides;

            var colour = Colour.GetRandomBrushColour();

            // Calculate the X and Y points for the inner and outer ellipses
            var outer = new Coordinate[numsides + 1];
            var inner = new Coordinate[numsides + 1];
            for (var i = 0; i < numsides + 1; i++)
            {
                var a = start + i * angle;
                var xval = box.Center.X + majorOut * DMath.Cos(a);
                var yval = box.Center.Y + minorOut * DMath.Sin(a);
                var zval = box.Start.Z;
                outer[i] = new Coordinate(xval, yval, zval).Round(0);
                xval = box.Center.X + majorIn * DMath.Cos(a);
                yval = box.Center.Y + minorIn * DMath.Sin(a);
                inner[i] = new Coordinate(xval, yval, zval).Round(0);
            }

            // Create the solids
            var z = new Coordinate(0, 0, height);
            for (var i = 0; i < numsides; i++)
            {
                var vertical = Coordinate.UnitZ * addHeight * i;
                var faces = new List<Coordinate[]>();
                var next = i + 1;
                faces.Add(new[] { outer[i], outer[i] + z, outer[next] + z, outer[next] }.Select(x => x + vertical).ToArray());
                faces.Add(new[] { inner[next], inner[next] + z, inner[i] + z, inner[i] }.Select(x => x + vertical).ToArray());
                faces.Add(new[] { outer[next], outer[next] + z, inner[next] + z, inner[next] }.Select(x => x + vertical).ToArray());
                faces.Add(new[] { inner[i], inner[i] + z, outer[i] + z, outer[i] }.Select(x => x + vertical).ToArray());
                faces.Add(new[] { inner[next] + z, outer[next] + z, outer[i] + z, inner[i] + z }.Select(x => x + vertical).ToArray());
                faces.Add(new[] { inner[i], outer[i], outer[next], inner[next] }.Select(x => x + vertical).ToArray());
                yield return MakeSolid(generator, faces, texture, colour);
            }
        }
    }
}
