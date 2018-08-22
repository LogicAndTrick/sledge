using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
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
    [OrderHint("G")]
    [AutoTranslate]
    public class PipeBrush : IBrush, IInitialiseHook
    {
        private NumericControl _numSides;
        private NumericControl _wallWidth;
        
        public string NumberOfSides { get; set; }
        public string WallWidth { get; set; }

        public async Task OnInitialise()
        {
            _numSides = new NumericControl(this) { LabelText = NumberOfSides };
            _wallWidth = new NumericControl(this) { LabelText = WallWidth, Minimum = 1, Maximum = 1024, Value = 32, Precision = 1 };
        }

        public string Name { get; set; } = "Pipe";
        public bool CanRound => true;

        public IEnumerable<BrushControl> GetControls()
        {
            yield return _numSides;
            yield return _wallWidth;
        }

        private Solid MakeSolid(UniqueNumberGenerator generator, IEnumerable<Vector3[]> faces, string texture, Color col)
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
            var wallWidth = (float) _wallWidth.GetValue();
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
            var angle = 2 * Math.PI / numSides;

            // Calculate the X and Y points for the inner and outer ellipses
            var outer = new Vector3[numSides];
            var inner = new Vector3[numSides];
            for (var i = 0; i < numSides; i++)
            {
                var a = i * angle;
                var xval = box.Center.X + majorOut * (float) Math.Cos(a);
                var yval = box.Center.Y + minorOut * (float) Math.Sin(a);
                var zval = box.Start.Z;
                outer[i] = new Vector3(xval, yval, zval).Round(roundDecimals);
                xval = box.Center.X + majorIn * (float) Math.Cos(a);
                yval = box.Center.Y + minorIn * (float) Math.Sin(a);
                inner[i] = new Vector3(xval, yval, zval).Round(roundDecimals);
            }

            // Create the solids
            var colour = Colour.GetRandomBrushColour();
            var z = new Vector3(0, 0, height).Round(roundDecimals);
            for (var i = 0; i < numSides; i++)
            {
                var faces = new List<Vector3[]>();
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
