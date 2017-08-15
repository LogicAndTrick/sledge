using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Properties;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Editing.Commands
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Tools", "", "Evil", "D")]
    [CommandID("BspEditor:Tools:Hollow")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_Hollow))]
    public class Hollow : BaseCommand
    {
        public override string Name { get; set; } = "Make Hollow...";
        public override string Details { get; set; } = "Make hollow";

        protected override bool IsInContext(IContext context, MapDocument document)
        {
            return base.IsInContext(context, document)
                   && document.Selection.Any(x => x is Solid);
        }

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            int _width = 16; // todo prompt

            var objects = document.Map.Root.Find(x => x is Solid).OfType<Solid>();

            var ops = new List<IOperation>();

            foreach (var obj in objects)
            {
                var split = false;
                var solid = obj;

                // Make a scaled version of the solid for the "inside" of the hollowed solid
                var origin = solid.BoundingBox.Center;
                var current = obj.BoundingBox.Dimensions;
                var target = current - new Coordinate(_width, _width, _width) * 2; // Double the width to take from both sides

                // Ensure we don't have any invalid target sizes
                if (target.X < 1) target.X = 1;
                if (target.Y < 1) target.Y = 1;
                if (target.Z < 1) target.Z = 1;

                // Clone and scale the solid
                var scale = target.ComponentDivide(current);
                var carver = (Solid) solid.Clone();
                carver.Transform(Matrix.Translation(origin) * Matrix.Scale(scale) * Matrix.Translation(-origin));

                // For a negative width, we want the original solid to be the inside instead
                if (_width < 0)
                {
                    var temp = carver;
                    carver = solid;
                    solid = temp;
                }

                // Carve the outside solid with the inside solid
                foreach (var plane in carver.Faces.Select(x => x.Plane))
                {
                    // Split solid by plane
                    Solid back, front;
                    try
                    {
                        if (!solid.Split(document.Map.NumberGenerator, plane, out back, out front)) continue;
                    }
                    catch
                    {
                        // We're not too fussy about over-complicated carving, just get out if we've broken it.
                        break;
                    }
                    split = true;

                    if (front != null)
                    {
                        // Retain the front solid
                        if (solid.IsSelected) front.IsSelected = true;
                        ops.Add(new Attach(obj.Hierarchy.Parent.ID, front));
                    }

                    if (back == null || !back.IsValid()) break;

                    // Use the back solid as the new clipping target
                    solid = back;
                }

                if (!split) continue;
                ops.Add(new Detatch(obj.Hierarchy.Parent.ID, obj));
            }

            if (ops.Any())
            {
                await MapDocumentOperation.Perform(document, new Transaction(ops));
            }
        }
    }
}