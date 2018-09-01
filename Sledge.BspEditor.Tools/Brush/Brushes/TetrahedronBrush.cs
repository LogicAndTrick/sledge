using System.Collections.Generic;
using System.ComponentModel.Composition;
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
    [OrderHint("B")]
    [AutoTranslate]
    public class TetrahedronBrush : IBrush, IInitialiseHook
    {
        private BooleanControl _useCentroid;
        
        public string TopVertexAtCentroid { get; set; }

        public Task OnInitialise()
        {
            _useCentroid = new BooleanControl(this) { LabelText = TopVertexAtCentroid, Checked = false };
            return Task.CompletedTask;
        }

        public string Name { get; set; } = "Tetrahedron";

        public bool CanRound => true;

        public IEnumerable<BrushControl> GetControls()
        {
            yield return _useCentroid;
        }

        public IEnumerable<IMapObject> Create(UniqueNumberGenerator generator, Box box, string texture, int roundDecimals)
        {
            var useCentroid = _useCentroid.GetValue();

            // The lower Z plane will be the triangle, with the lower Y value getting the two corners
            var c1 = new Vector3(box.Start.X, box.Start.Y, box.Start.Z).Round(roundDecimals);
            var c2 = new Vector3(box.End.X, box.Start.Y, box.Start.Z).Round(roundDecimals);
            var c3 = new Vector3(box.Center.X, box.End.Y, box.Start.Z).Round(roundDecimals);
            var centroid = new Vector3((c1.X + c2.X + c3.X) / 3, (c1.Y + c2.Y + c3.Y) / 3, box.End.Z);
            var c4 = (useCentroid ? centroid : new Vector3(box.Center.X, box.Center.Y, box.End.Z)).Round(roundDecimals);

            var faces = new[] {
                new[] { c1, c2, c3 },
                new[] { c4, c1, c3 },
                new[] { c4, c3, c2 },
                new[] { c4, c2, c1 }
            };

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
