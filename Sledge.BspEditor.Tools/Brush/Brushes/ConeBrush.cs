using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
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
    [OrderHint("F")]
    public class ConeBrush : IBrush
    {
        private readonly NumericControl _numSides;

        public ConeBrush()
        {
            _numSides = new NumericControl(this) { LabelText = "Number of sides" };
        }

        public string Name => "Cone";
        public bool CanRound => true;

        public IEnumerable<BrushControl> GetControls()
        {
            yield return _numSides;
        }

        public IEnumerable<IMapObject> Create(UniqueNumberGenerator generator, Box box, string texture, int roundDecimals)
        {
            var numSides = (int) _numSides.GetValue();
            if (numSides < 3) yield break;

            // This is all very similar to the cylinder brush.
            var width = box.Width;
            var length = box.Length;
            var major = width / 2;
            var minor = length / 2;
            var angle = 2 * DMath.PI / numSides;

            var points = new Coordinate[numSides];
            for (var i = 0; i < numSides; i++)
            {
                var a = i * angle;
                var xval = box.Center.X + major * DMath.Cos(a);
                var yval = box.Center.Y + minor * DMath.Sin(a);
                var zval = box.Start.Z;
                points[i] = new Coordinate(xval, yval, zval).Round(roundDecimals);
            }

            var faces = new List<Coordinate[]>();

            var point = new Coordinate(box.Center.X, box.Center.Y, box.End.Z).Round(roundDecimals);
            for (var i = 0; i < numSides; i++)
            {
                var next = (i + 1) % numSides;
                faces.Add(new[] {points[i], point, points[next]});
            }
            faces.Add(points.ToArray());

            var solid = new Solid(generator.Next("MapObject"));
            solid.Data.Add(new ObjectColor(Colour.GetRandomBrushColour()));
            foreach (var arr in faces)
            {
                var face = new Face(generator.Next("Face"))
                {
                    Plane = new Plane(arr[0], arr[1], arr[2]),
                    Texture = { Name = texture }
                };
                face.Vertices.AddRange(arr);
                solid.Data.Add(face);
            }
            solid.DescendantsChanged();
            yield return solid;
        }
    }
}
