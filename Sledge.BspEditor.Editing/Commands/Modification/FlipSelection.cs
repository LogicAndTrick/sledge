using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Mutation;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;

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

        protected abstract Coordinate GetScale();

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            var selBox = document.Selection.GetSelectionBoundingBox();

            var tl = document.Map.Data.GetOne<TransformationFlags>() ?? new TransformationFlags();

            var transaction = new Transaction();

            var tform = Matrix.Translation(selBox.Center) * Matrix.Scale(GetScale()) * Matrix.Translation(-selBox.Center);

            var transformOperation = new BspEditor.Modification.Operations.Mutation.Transform(tform, document.Selection.GetSelectedParents());
            transaction.Add(transformOperation);

            // Check for texture transform
            if (tl.TextureLock) transaction.Add(new TransformTexturesUniform(tform, document.Selection));

            await MapDocumentOperation.Perform(document, transaction);
        }
    }

    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Tools", "", "Flip", "B")]
    [CommandID("BspEditor:Tools:FlipX")]
    public class FlipSelectionX : FlipSelection
    {
        protected override Coordinate GetScale()
        {
            return new Coordinate(-1, 1, 1);
        }
    }

    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Tools", "", "Flip", "D")]
    [CommandID("BspEditor:Tools:FlipY")]
    public class FlipSelectionY : FlipSelection
    {
        protected override Coordinate GetScale()
        {
            return new Coordinate(1, -1, 1);
        }
    }

    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Tools", "", "Flip", "F")]
    [CommandID("BspEditor:Tools:FlipZ")]
    public class FlipSelectionZ : FlipSelection
    {
        protected override Coordinate GetScale()
        {
            return new Coordinate(1, 1, -1);
        }
    }
}
