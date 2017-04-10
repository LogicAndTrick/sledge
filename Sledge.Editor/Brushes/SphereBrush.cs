using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Common;
using Sledge.Editor.Brushes.Controls;
using Sledge.Providers.Texture;

namespace Sledge.Editor.Brushes
{
    public class SphereBrush : IBrush
    {
        private readonly NumericControl _numSides;

        public SphereBrush()
        {
            _numSides = new NumericControl(this) { LabelText = "Number of sides" };
        }

        public string Name
        {
            get { return "Sphere"; }
        }

        public bool CanRound { get { return false; } }

        public IEnumerable<BrushControl> GetControls()
        {
            yield return _numSides;
        }

        private Solid MakeSolid(IDGenerator generator, IEnumerable<Coordinate[]> faces, string texture, Color col)
        {
            var solid = new Solid(generator.GetNextObjectID()) { Colour = col };
            foreach (var arr in faces)
            {
                var face = new Face(generator.GetNextFaceID())
                {
                    Parent = solid,
                    Plane = new Plane(arr[0], arr[1], arr[2]),
                    Colour = solid.Colour,
                    Texture = { Name = texture  }
                };
                face.Vertices.AddRange(arr.Select(x => new Vertex(x, face)));
                face.UpdateBoundingBox();
                face.AlignTextureToWorld();
                solid.Faces.Add(face);
            }
            solid.UpdateBoundingBox();
            return solid;
        }

        public IEnumerable<MapObject> Create(IDGenerator generator, Box box, string texture, int roundDecimals)
        {
            var numSides = (int)_numSides.GetValue();
            if (numSides < 3) yield break;

            roundDecimals = 2; // don't support rounding

            var width = box.Width;
            var length = box.Length;
            var height = box.Height;
            var major = width / 2;
            var minor = length / 2;
            var heightRadius = height / 2;
            
            var angleV = DMath.DegreesToRadians(180) / numSides;
            var angleH = DMath.DegreesToRadians(360) / numSides;

            var faces = new List<Coordinate[]>();
            var bottom = new Coordinate(box.Center.X, box.Center.Y, box.Start.Z).Round(roundDecimals);
            var top = new Coordinate(box.Center.X, box.Center.Y, box.End.Z).Round(roundDecimals);
            
            for (var i = 0; i < numSides; i++)
            {
                // Top -> bottom
                var zAngleStart = angleV * i;
                var zAngleEnd = angleV * (i + 1);
                var zStart = heightRadius * DMath.Cos(zAngleStart);
                var zEnd = heightRadius * DMath.Cos(zAngleEnd);
                var zMultStart = DMath.Sin(zAngleStart);
                var zMultEnd = DMath.Sin(zAngleEnd);
                for (var j = 0; j < numSides; j++)
                {
                    // Go around the circle in X/Y
                    var xyAngleStart = angleH * j;
                    var xyAngleEnd = angleH * ((j + 1) % numSides);
                    var xyStartX = major * DMath.Cos(xyAngleStart);
                    var xyStartY = minor * DMath.Sin(xyAngleStart);
                    var xyEndX = major * DMath.Cos(xyAngleEnd);
                    var xyEndY = minor * DMath.Sin(xyAngleEnd);
                    var one = (new Coordinate(xyStartX * zMultStart, xyStartY * zMultStart, zStart) + box.Center).Round(roundDecimals);
                    var two = (new Coordinate(xyEndX * zMultStart, xyEndY * zMultStart, zStart) + box.Center).Round(roundDecimals);
                    var three = (new Coordinate(xyEndX * zMultEnd, xyEndY * zMultEnd, zEnd) + box.Center).Round(roundDecimals);
                    var four = (new Coordinate(xyStartX * zMultEnd, xyStartY * zMultEnd, zEnd) + box.Center).Round(roundDecimals);
                    if (i == 0)
                    {
                        // Top faces are triangles
                        faces.Add(new[] { top, three, four });
                    }
                    else if (i == numSides - 1)
                    {
                        // Bottom faces are also triangles
                        faces.Add(new[] { bottom, one, two });
                    }
                    else
                    {
                        // Inner faces are quads
                        faces.Add(new[] { one, two, three, four });
                    }
                }
            }
            yield return MakeSolid(generator, faces, texture, Colour.GetRandomBrushColour());
        }
    }
}
