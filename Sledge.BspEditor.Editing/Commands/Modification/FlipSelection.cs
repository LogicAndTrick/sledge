using System.ComponentModel.Composition;
using System.Numerics;
using System.Threading.Tasks;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Mutation;
using Sledge.BspEditor.Primitives.MapData;
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

            var tform = Matrix4x4.CreateTranslation(selBox.Center) * Matrix4x4.CreateScale(GetScale()) * Matrix4x4.CreateTranslation(-selBox.Center);

            var transformOperation = new BspEditor.Modification.Operations.Mutation.Transform(tform, document.Selection.GetSelectedParents());
            transaction.Add(transformOperation);

            // Check for texture transform
            if (tl.TextureLock) transaction.Add(new TransformTexturesUniform(tform, document.Selection));

            await MapDocumentOperation.Perform(document, transaction);
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
