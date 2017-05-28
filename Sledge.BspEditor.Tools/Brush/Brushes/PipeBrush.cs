using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Tools.Brush.Brushes.Controls;
using Sledge.Common;
using Sledge.Common.Shell.Components;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Tools.Brush.Brushes
{
    [Export(typeof(IBrush))]
    [OrderHint("G")]
    public class PipeBrush : IBrush
    {
        private readonly NumericControl _numSides;
        private readonly NumericControl _wallWidth;

        public PipeBrush()
        {
            _numSides = new NumericControl(this) { LabelText = "Number of sides" };
            _wallWidth = new NumericControl(this) { LabelText = "Wall width", Minimum = 1, Maximum = 1024, Value = 32, Precision = 1 };
        }

        public string Name => "Pipe";
        public bool CanRound => true;

        public IEnumerable<BrushControl> GetControls()
        {
            yield return _numSides;
            yield return _wallWidth;
        }

        private Solid MakeSolid(UniqueNumberGenerator generator, IEnumerable<Coordinate[]> faces, string texture, Color col)
        {
            var solid = new Solid(generator.Next("MapObject"));
            solid.Data.Add(new ObjectColor(col));
            foreach (var arr in faces)
            {
                var face = new Face(generator.Next("Face"))
                {
                    Plane = new Plane(arr[0], arr[1], arr[2]),
                    Texture = { Name = texture  }
                };
                face.Vertices.AddRange(arr);
                solid.Data.Add(face);
            }
            solid.DescendantsChanged();
            return solid;
        }

        public IEnumerable<IMapObject> Create(UniqueNumberGenerator generator, Box box, string texture, int roundDecimals)
        {
            var wallWidth = _wallWidth.GetValue();
            if (wallWidth < 1) yield break;
            var numSides = (int) _numSides.GetValue();
            if (numSides < 3) yield break;

            // Very similar to the cylinder, except we have multiple solids this time
            var width = box.Width;
            var length = box.Length;
            var height = box.Height;
            var majorOut = width / 2;
            var majorIn = majorOut - wallWidth;
            var minorOut = length / 2;
            var minorIn = minorOut - wallWidth;
            var angle = 2 * DMath.PI / numSides;

            // Calculate the X and Y points for the inner and outer ellipses
            var outer = new Coordinate[numSides];
            var inner = new Coordinate[numSides];
            for (var i = 0; i < numSides; i++)
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
            var colour = Colour.GetRandomBrushColour();
            var z = new Coordinate(0, 0, height).Round(roundDecimals);
            for (var i = 0; i < numSides; i++)
            {
                var faces = new List<Coordinate[]>();
                var next = (i + 1) % numSides;
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
