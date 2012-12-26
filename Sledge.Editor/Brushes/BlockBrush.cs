using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Common;
using Sledge.Editor.Brushes.Controls;

namespace Sledge.Editor.Brushes
{
    public class BlockBrush : IBrush
    {
        public string Name
        {
            get { return "Block"; }
        }

        public IEnumerable<BrushControl> GetControls()
        {
            return new List<BrushControl>();
        }

        public IEnumerable<MapObject> Create(Box box, ITexture texture)
        {
            var solid = new Solid { Colour = Colour.GetRandomBrushColour() };
            foreach (var arr in box.GetBoxFaces())
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
