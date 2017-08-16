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
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Commands
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Tools", "", "Evil", "B")]
    [CommandID("BspEditor:Tools:Carve")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_Carve))]
    [DefaultHotkey("Ctrl+Shift+C")]
    public class Carve : BaseCommand
    {
        public override string Name { get; set; } = "Carve";
        public override string Details { get; set; } = "Carve";

        protected override bool IsInContext(IContext context, MapDocument document)
        {
            return base.IsInContext(context, document)
                && document.Selection.Any(x => x is Solid);
        }

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            var carver = document.Selection.OfType<Solid>().FirstOrDefault();
            if (carver == null) return;

            var objects = document.Map.Root
                .Find(x => x is Solid && x.BoundingBox.IntersectsWith(carver.BoundingBox))
                .OfType<Solid>()
                .Where(x => !ReferenceEquals(x, carver));

            var ops = new List<IOperation>();

            foreach (var obj in objects)
            {
                var split = false;
                var solid = obj;

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
                    if (solid.IsSelected) back.IsSelected = true;
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
