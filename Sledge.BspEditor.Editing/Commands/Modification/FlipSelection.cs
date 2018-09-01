using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Mutation;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Commands.Modification
{
    public abstract class FlipSelection : BaseCommand
    {
        public override string Name { get; set; } = "Flip";
        public override string Details { get; set; } = "Flip";

        protected override bool IsInContext(IContext context, MapDocument document)
        {
            return base.IsInContext(context, document) && !document.Selection.IsEmpty;
        }

        protected abstract Vector3 GetScale();

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            var selBox = document.Selection.GetSelectionBoundingBox();

            var tl = document.Map.Data.GetOne<TransformationFlags>() ?? new TransformationFlags();

            var transaction = new Transaction();

            var tform = Matrix4x4.CreateTranslation(-selBox.Center)
                        * Matrix4x4.CreateScale(GetScale())
                        * Matrix4x4.CreateTranslation(selBox.Center);

            var transformOperation = new BspEditor.Modification.Operations.Mutation.Transform(tform, document.Selection.GetSelectedParents());
            transaction.Add(transformOperation);

            transaction.Add(new FlipFaces(document.Selection));

            // Check for texture transform
            if (tl.TextureLock) transaction.Add(new TransformTexturesUniform(tform, document.Selection));

            await MapDocumentOperation.Perform(document, transaction);
        }

        private class FlipFaces : IOperation
        {
            private readonly List<long> _idsToTransform;

            public bool Trivial => false;

            public FlipFaces(IEnumerable<IMapObject> objectsToTransform)
            {
                _idsToTransform = objectsToTransform.Select(x => x.ID).ToList();
            }

            public Task<Change> Perform(MapDocument document)
            {
                var ch = new Change(document);

                var objects = _idsToTransform.Select(x => document.Map.Root.FindByID(x)).Where(x => x != null).ToList();

                foreach (var o in objects)
                {
                    foreach (var it in o.Data.OfType<Face>())
                    {
                        it.Vertices.Flip();
                        ch.Update(o);
                    }
                }

                return Task.FromResult(ch);
            }

            public Task<Change> Reverse(MapDocument document)
            {
                return Perform(document); // Reversing this operation means just performing it again
            }
        }
    }

    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Tools", "Flip", "FlipAlign", "B")]
    [CommandID("BspEditor:Tools:FlipX")]
    public class FlipSelectionX : FlipSelection
    {
        protected override Vector3 GetScale()
        {
            return new Vector3(-1, 1, 1);
        }
    }

    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Tools", "Flip", "FlipAlign", "D")]
    [CommandID("BspEditor:Tools:FlipY")]
    public class FlipSelectionY : FlipSelection
    {
        protected override Vector3 GetScale()
        {
            return new Vector3(1, -1, 1);
        }
    }

    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Tools", "Flip", "FlipAlign", "F")]
    [CommandID("BspEditor:Tools:FlipZ")]
    public class FlipSelectionZ : FlipSelection
    {
        protected override Vector3 GetScale()
        {
            return new Vector3(1, 1, -1);
        }
    }
}
