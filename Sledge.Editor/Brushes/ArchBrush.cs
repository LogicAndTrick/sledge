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
        private readonly NumericControl _tiltAngle;
        private readonly NumericControl _wallWidth;
        private readonly NumericControl _numSides;
        private readonly NumericControl _addHeight;
        private readonly BooleanControl _curvedRamp;

        public ArchBrush()
        {
            _arc = new NumericControl(this) { LabelText = "Arc", Minimum = 1, Maximum = 360*4, Value = 360 };
            _wallWidth = new NumericControl(this) { LabelText = "Wall width", Minimum = 1, Maximum = 1024, Value = 16};
            _numSides = new NumericControl(this) { LabelText = "Num. sides" };
            _startAngle = new NumericControl(this) { LabelText = "Start Angle", Minimum = 0, Maximum = 359, Value = 0 };
            _tiltAngle = new NumericControl(this) { LabelText = "Tilt angle", Minimum = -90, Maximum = 90, Value = 0 };
            _addHeight = new NumericControl(this) { LabelText = "Add height", Minimum = -1024, Maximum = 1024, Value = 0};
            _curvedRamp = new BooleanControl(this) { LabelText = "Curved ramp", Checked = false };
        }

        public string Name
        {
            get { return "Arch"; }
        }

        public IEnumerable<BrushControl> GetControls()
        {
            yield return _numSides;
            yield return _startAngle;
            yield return _tiltAngle;
            yield return _arc;
            yield return _wallWidth;
            yield return _addHeight;
            yield return _curvedRamp;
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
            var tiltAngle = (int)_tiltAngle.GetValue();
            if (tiltAngle < -90 || startAngle > 90) yield break;
            var arc = (int)_arc.GetValue();
            if (arc < 1) yield break;
            var wallWidth = _wallWidth.GetValue();
            if (wallWidth < 1) yield break;
            var addHeight = _addHeight.GetValue() / numsides;
            var curvedRamp = _curvedRamp.GetValue();

            // Very similar to the pipe brush, except with options for start angle, tilt, arc, and height
            var width = box.Width;
            var length = box.Length;
            var height = box.Height;

            var majorOut = width / 2;
            var majorIn = majorOut - wallWidth;
            var minorOut = length / 2;
            var minorIn = minorOut - wallWidth;

            var start = DMath.DegreesToRadians(startAngle);
            var tilt = DMath.DegreesToRadians(tiltAngle * Math.Sign(addHeight)); // Sign depends on curve direction
            var angle = DMath.DegreesToRadians(arc) / numsides;

            var colour = Colour.GetRandomBrushColour();

            // Calculate the coordinates of the inner and outer ellipses' points
            var outer = new Coordinate[numsides + 1];
            var inner = new Coordinate[numsides + 1];
            for (var i = 0; i < numsides + 1; i++)
            {
                var a = start + i * angle;
                var h = i * addHeight
                var hypot = 0.5 * wallWidth

                var xval = box.Center.X + majorOut * DMath.Cos(a);
                var yval = box.Center.Y + minorOut * DMath.Sin(a);
                var zval = box.Start.Z + h + hypot * DMath.Sin(tiltAngle); // TODO: Interpolation
                outer[i] = new Coordinate(xval, yval, zval).Round(0);

                xval = box.Center.X + majorIn * DMath.Cos(a);
                yval = box.Center.Y + minorIn * DMath.Sin(a);
                zval = box.Start.Z + h - hypot * DMath.Sin(tiltAngle);     // TODO: Interpolation
                inner[i] = new Coordinate(xval, yval, zval).Round(0);
            }

            // Create the solids
            var numSolids = curvedRamp ? 2 * numsides : numsides;
            for (var i = 0; i < numSolids; i++)
            {
                var faces = new List<Coordinate[]>();
                var z = new Coordinate(0, 0, height);

                if (curvedRamp)
                {
                    // Since we are triangulating/splitting each arch segment, we need to generate 2 brushes per side
                    // TODO: Different split orientations for the segments, depending on the arch's curving direction
                    if (i % 2 == 0)
                    {
                        faces.Add(new[] { inner[i],       outer[i],       inner[i] + z   });
                        faces.Add(new[] { inner[i],       inner[i+1],     inner[i] + z   });
                        faces.Add(new[] { outer[i],       inner[i+1],     outer[i] + z   });
                        faces.Add(new[] { inner[i],       outer[i],       inner[i+1]     });
                        faces.Add(new[] { inner[i] + z,   outer[i] + z,   inner[i+1] + z });
                    }
                    else
                    {
                        faces.Add(new[] { outer[i],       inner[i+1],     outer[i] + z   });
                        faces.Add(new[] { outer[i],       outer[i+1],     outer[i] + z   });
                        faces.Add(new[] { inner[i+1],     outer[i+1],     inner[i+1] + z });
                        faces.Add(new[] { outer[i],       inner[i+1],     outer[i+1]     });
                        faces.Add(new[] { outer[i] + z,   inner[i+1] + z, outer[i+1] + z });
                    }
                }
                else
                {
                    faces.Add(new[] { outer[i],       outer[i] + z,   outer[i+1] + z, outer[i+1]   });
                    faces.Add(new[] { inner[i+1],     inner[i+1] + z, inner[i] + z,   inner[i]     });
                    faces.Add(new[] { outer[i+1],     outer[i+1] + z, inner[i+1] + z, inner[i+1]   });
                    faces.Add(new[] { inner[i],       inner[i] + z,   outer[i] + z,   outer[i]     });
                    faces.Add(new[] { inner[i+1] + z, outer[i+1] + z, outer[i] + z,   inner[i] + z });
                    faces.Add(new[] { inner[i],       outer[i],       outer[i+1],     inner[i+1]   });
                }

                yield return MakeSolid(generator, faces, texture, colour);
            }
        }
    }
}
