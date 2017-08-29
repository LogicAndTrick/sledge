using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Modification.Operations.Mutation
{
    /// <summary>
    /// Scale transform all ITextured data objects in a collection
    /// </summary>
    public class TransformTexturesScale : IOperation
    {
        private readonly List<long> _idsToTransform;
        private readonly Matrix _matrix;

        public bool Trivial => false;
        
        public TransformTexturesScale(Matrix matrix, params IMapObject[] objectsToTransform)
        {
            _matrix = matrix;
            _idsToTransform = objectsToTransform.Select(x => x.ID).ToList();
        }

        public TransformTexturesScale(Matrix matrix, IEnumerable<IMapObject> objectsToTransform)
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
                foreach (var it in o.Data.OfType<ITextured>())
                {
                    it.Texture.TransformScale(_matrix);
                    ch.Update(o);
                }
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
                foreach (var it in o.Data.OfType<ITextured>())
                {
                    it.Texture.TransformScale(inv);
                    ch.Update(o);
                }
            }

            return Task.FromResult(ch);
        }
    }
}