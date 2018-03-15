using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Components;
using Sledge.BspEditor.Editing.Properties;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Mutation;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Editing.Commands.Modification
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Tools", "", "Snap", "B")]
    [CommandID("BspEditor:Tools:SnapToGrid")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_SnapSelection))]
    public class SnapToGrid : BaseCommand
    {
        public override string Name { get; set; } = "Snap to grid";
        public override string Details { get; set; } = "Snap selection to grid";

        protected override bool IsInContext(IContext context, MapDocument document)
        {
            return base.IsInContext(context, document) && !document.Selection.IsEmpty;
        }

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            var selBox = document.Selection.GetSelectionBoundingBox();
            var grid = document.Map.Data.GetOne<GridData>();
            if (grid == null) return;

            var start = selBox.Start;
            var snapped = grid.Grid.Snap(start);
            var trans = snapped - start;
            if (trans == Coordinate.Zero) return;

            var tform = Matrix.Translation(trans);

            var transaction = new Transaction();
            var transformOperation = new BspEditor.Modification.Operations.Mutation.Transform(tform, document.Selection.GetSelectedParents());
            transaction.Add(transformOperation);

            // Check for texture transform
            var tl = document.Map.Data.GetOne<TransformationFlags>() ?? new TransformationFlags();
            if (tl.TextureLock) transaction.Add(new TransformTexturesUniform(tform, document.Selection));

            await MapDocumentOperation.Perform(document, transaction);
        }
    }
}
