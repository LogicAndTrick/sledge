using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Tools.Properties;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Tools.Cordon
{
    [Export(typeof(ICommand))]
    [CommandID("BspEditor:Cordon:ToggleCordon")]
    [MenuItem("Tools", "", "Cordon", "B")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_Cordon))]
    [AutoTranslate]
    public class ToggleCordon : ICommand
    {
        public string Name { get; set; } = "Cordon Bounds";
        public string Details { get; set; } = "Toggle cordon bounds";

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument _);
        }

        public async Task Invoke(IContext context, CommandParameters parameters)
        {
            if (context.TryGet("ActiveDocument", out MapDocument doc))
            {
                var cordon = doc.Map.Data.GetOne<CordonBounds>() ?? new CordonBounds {Enabled = false};
                cordon.Enabled = !cordon.Enabled;
                await MapDocumentOperation.Perform(doc, new TrivialOperation(x => x.Map.Data.Replace(cordon), x => x.Update(cordon).UpdateRange(doc.Map.Root.FindAll())));
            }
        }
    }
}