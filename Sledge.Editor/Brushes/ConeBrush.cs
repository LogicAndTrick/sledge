using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Common;
using Sledge.Editor.Brushes.Controls;
using Sledge.Extensions;

namespace Sledge.Editor.Brushes
{
    public class ConeBrush : IBrush
    {
        private readonly NumericControl _numSides;

        public ConeBrush()
        {
            _numSides = new NumericControl(this) {LabelText = "Num. sides"};
        }

        public string Name
        {
            get { return "Cone"; }
        }

        public IEnumerable<BrushControl> GetControls()
        {
            yield return _numSides;
        }

        public IEnumerable<MapObject> Create(Box box, ITexture texture)
        {
            var numsides = (int) _numSides.GetValue();
            if (numsides < 3) yield break;

            // This is all very similar to the cylinder brush.
            var width = box.Width;
            var length = box.Length;
            var major = width / 2;
            var minor = length / 2;
            var angle = 2 * DMath.PI / numsides;

            var points = new Coordinate[numsides];
            for (var i = 0; i < numsides; i++)
            {
                var a = i * angle;
                var xval = box.Center.X + major * DMath.Cos(a);
                var yval = box.Center.Y + minor * DMath.Sin(a);
                var zval = box.Start.Z;
                points[i] = new Coordinate(xval, yval, zval).Round(0);
            }

            var faces = new List<Coordinate[]>();

            var point = new Coordinate(box.Center.X, box.Center.Y, box.End.Z);
            for (var i = 0; i < numsides; i++)
            {
                var next = (i + 1) % numsides;
                faces.Add(new[] {points[i], point, points[next]});
            }
            faces.Add(points.ToArray());

            var solid = new Solid { Colour = Colour.GetRandomBrushColour() };
            foreach (var arr in faces)
            {
                var face = new Face
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
            yield return solid;
        }
    }
}
