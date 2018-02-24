using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Tools.Vertex.Selection;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Tools.Vertex.Errors
{
    [Export(typeof(IVertexErrorCheck))]
    public class BackwardsFace : IVertexErrorCheck
    {
        private const string Key = "Sledge.BspEditor.Tools.Vertex.Errors.BackwardsFace";

        private Coordinate GetOrigin(IEnumerable<Face> faces)
        {
            var points = faces.SelectMany(x => x.Vertices).ToList();
            var origin = points.Aggregate(Coordinate.Zero, (x, y) => x + y) / points.Count;
            return origin;
        }

        private IEnumerable<Face> GetBackwardsFaces(Solid solid, decimal epsilon = 0.001m)
        {
            var faces = solid.Faces.ToList();
            var origin = GetOrigin(faces);
            return faces.Where(x => x.Plane.OnPlane(origin, epsilon) > 0);
        }

        public IEnumerable<VertexError> GetErrors(VertexSolid solid)
        {
            foreach (var face in GetBackwardsFaces(solid.Copy, 0.5m))
            {
                yield return new VertexError(Key, solid).Add(face);
            }
        }
    }
}