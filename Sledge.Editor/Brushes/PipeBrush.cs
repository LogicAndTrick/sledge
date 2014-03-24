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
    public class PipeBrush : IBrush
    {
        private readonly NumericControl _numSides;
        private readonly NumericControl _width;

        public PipeBrush()
        {
            _numSides = new NumericControl(this) { LabelText = "Num. sides" };
            _width = new NumericControl(this) { LabelText = "Wall width", Minimum = 1, Maximum = 1024, Value = 32, Precision = 1 };
        }

        public string Name
        {
            get { return "Pipe"; }
        }

        public IEnumerable<BrushControl> GetControls()
        {
            yield return _numSides;
            yield return _width;
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
                face.AlignTextureToFace();
                solid.Faces.Add(face);
            }
            solid.UpdateBoundingBox();
            return solid;
        }

        public IEnumerable<MapObject> Create(IDGenerator generator, Box box, ITexture texture, int roundDecimals)
        {
            var wallWidth = _width.GetValue();
            if (wallWidth < 1) yield break;
            var numsides = (int) _numSides.GetValue();
            if (numsides < 3) yield break;

            // Very similar to the cylinder, except we have multiple solids this time
            var width = box.Width;
            var length = box.Length;
            var height = box.Height;
            var majorOut = width / 2;
            var majorIn = majorOut - wallWidth;
            var minorOut = length / 2;
            var minorIn = minorOut - wallWidth;
            var angle = 2 * DMath.PI / numsides;

            var colour = Colour.GetRandomBrushColour();

            // Calculate the X and Y points for the inner and outer ellipses
            var outer = new Coordinate[numsides];
            var inner = new Coordinate[numsides];
            for (var i = 0; i < numsides; i++)
            {
                var a = i * angle;
                var xval = box.Center.X + majorOut * DMath.Cos(a);
                var yval = box.Center.Y + minorOut * DMath.Sin(a);
                var zval = box.Start.Z;
                outer[i] = new Coordinate(xval, yval, zval).Round(roundDecimals);
                xval = box.Center.X + majorIn * DMath.Cos(a);
                yval = box.Center.Y + minorIn * DMath.Sin(a);
                inner[i] = new Coordinate(xval, yval, zval).Round(roundDecimals);
            }

            // Create the solids
            var z = new Coordinate(0, 0, height);
            for (var i = 0; i < numsides; i++)
            {
                var faces = new List<Coordinate[]>();
                var next = (i + 1) % numsides;
                faces.Add(new[] { outer[i], outer[i] + z, outer[next] + z, outer[next] });
                faces.Add(new[] { inner[next], inner[next] + z, inner[i] + z, inner[i] });
                faces.Add(new[] { outer[next], outer[next] + z, inner[next] + z, inner[next] });
                faces.Add(new[] { inner[i], inner[i] + z, outer[i] + z, outer[i] });
                faces.Add(new[] { inner[next] + z, outer[next] + z, outer[i] + z, inner[i] + z });
                faces.Add(new[] { inner[i], outer[i], outer[next], inner[next] });
                yield return MakeSolid(generator, faces, texture, colour);
            }
        }
    }
}
