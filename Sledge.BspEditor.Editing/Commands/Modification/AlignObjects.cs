using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
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
    public abstract class AlignObjects : BaseCommand
    {
        public override string Name { get; set; } = "Align";
        public override string Details { get; set; } = "Align";
        
        protected override bool IsInContext(IContext context, MapDocument document)
        {
            return base.IsInContext(context, document) && !document.Selection.IsEmpty;
        }

        protected abstract Coordinate GetTranslation(Box selectionBox, Box objectBox);

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            var selBox = document.Selection.GetSelectionBoundingBox();

            var tl = document.Map.Data.GetOne<TransformationFlags>() ?? new TransformationFlags();

            var transaction = new Transaction();

            foreach (var mo in document.Selection.GetSelectedParents().ToList())
            {
                var objBox = mo.BoundingBox;
                var translation = GetTranslation(selBox, objBox);
                if (translation == Coordinate.Zero) continue;

                var tform = Matrix.Translation(translation);

                var transformOperation = new BspEditor.Modification.Operations.Mutation.Transform(tform, mo);
                transaction.Add(transformOperation);

                // Check for texture transform
                if (tl.TextureLock) transaction.Add(new TransformTexturesUniform(tform, mo.FindAll()));
            }

            if (!transaction.IsEmpty)
            {
                await MapDocumentOperation.Perform(document, transaction);
            }
        }
    }

    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Tools", "", "Align", "A")]
    [CommandID("BspEditor:Tools:AlignXMin")]
    public class AlignObjectsXMin : AlignObjects
    {
        protected override Coordinate GetTranslation(Box selectionBox, Box objectBox)
        {
            return new Coordinate(selectionBox.Start.X - objectBox.Start.X, 0, 0);
        }
    }

    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Tools", "", "Align", "C")]
    [CommandID("BspEditor:Tools:AlignYMin")]
    public class AlignObjectsYMin : AlignObjects
    {
        protected override Coordinate GetTranslation(Box selectionBox, Box objectBox)
        {
            return new Coordinate(0, selectionBox.Start.Y - objectBox.Start.Y, 0);
        }
    }

    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Tools", "", "Align", "E")]
    [CommandID("BspEditor:Tools:AlignZMin")]
    public class AlignObjectsZMin : AlignObjects
    {
        protected override Coordinate GetTranslation(Box selectionBox, Box objectBox)
        {
            return new Coordinate(0, 0, selectionBox.Start.Z - objectBox.Start.Z);
        }
    }

    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Tools", "", "Align", "B")]
    [CommandID("BspEditor:Tools:AlignXMax")]
    public class AlignObjectsXMax : AlignObjects
    {
        protected override Coordinate GetTranslation(Box selectionBox, Box objectBox)
        {
            return new Coordinate(selectionBox.End.X - objectBox.End.X, 0, 0);
        }
    }

    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Tools", "", "Align", "D")]
    [CommandID("BspEditor:Tools:AlignYMax")]
    public class AlignObjectsYMax : AlignObjects
    {
        protected override Coordinate GetTranslation(Box selectionBox, Box objectBox)
        {
            return new Coordinate(0, selectionBox.End.Y - objectBox.End.Y, 0);
        }
    }

    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Tools", "", "Align", "F")]
    [CommandID("BspEditor:Tools:AlignZMax")]
    public class AlignObjectsZMax : AlignObjects
    {
        protected override Coordinate GetTranslation(Box selectionBox, Box objectBox)
        {
            return new Coordinate(0, 0, selectionBox.End.Z - objectBox.End.Z);
        }
    }
}