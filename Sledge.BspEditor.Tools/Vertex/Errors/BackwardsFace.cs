using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Numerics;
using Sledge.BspEditor.Tools.Vertex.Selection;

namespace Sledge.BspEditor.Tools.Vertex.Errors
{
    [Export(typeof(IVertexErrorCheck))]
    public class BackwardsFace : IVertexErrorCheck
    {
        private const string Key = "Sledge.BspEditor.Tools.Vertex.Errors.BackwardsFace";

        private Vector3 GetOrigin(IEnumerable<MutableFace> faces)
        {
            var points = faces.SelectMany(x => x.Vertices).ToList();
            var origin = points.Aggregate(Vector3.Zero, (x, y) => x + y.Position) / points.Count;
            return origin;
        }

        private IEnumerable<MutableFace> GetBackwardsFaces(MutableSolid solid, float epsilon = 0.001f)
        {
            var faces = solid.Faces.ToList();
            var origin = GetOrigin(faces);
            return faces.Where(x => x.Plane.OnPlane(origin, epsilon) > 0);
        }

        public IEnumerable<VertexError> GetErrors(VertexSolid solid)
        {
            foreach (var face in GetBackwardsFaces(solid.Copy, 0.5f))
            {
                yield return new VertexError(Key, solid).Add(face);
            }
        }
    }
}