using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.DataStructures.Geometric;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Modification.Operations.Mutation
{
    public class Transform : IOperation
    {
        private readonly List<long> _idsToTransform;
        private readonly Matrix _matrix;

        public bool Trivial => false;
        
        public Transform(Matrix matrix, params IMapObject[] objectsToTransform)
        {
            _matrix = matrix;
            _idsToTransform = objectsToTransform.Select(x => x.ID).ToList();
        }

        public Transform(Matrix matrix, IEnumerable<IMapObject> objectsToTransform)
        {
            _matrix = matrix;
            _idsToTransform = objectsToTransform.Select(x => x.ID).ToList();
        }

        public Task<Change> Perform(MapDocument document)
        {
            var ch = new Change(document);

            var objects = _idsToTransform.Select(x => document.Map.Root.FindByID(x)).Where(x => x != null).ToList();

            foreach (var o in objects)
            {
                o.Transform(_matrix);
                ch.UpdateRange(o.FindAll());
            }

            return Task.FromResult(ch);
        }

        public Task<Change> Reverse(MapDocument document)
        {
            var inv = _matrix.Inverse();
            var ch = new Change(document);

            var objects = _idsToTransform.Select(x => document.Map.Root.FindByID(x)).Where(x => x != null).ToList();

            foreach (var o in objects)
            {
                o.Transform(inv);
                ch.UpdateRange(o.FindAll());
            }

            return Task.FromResult(ch);
        }
    }
}
