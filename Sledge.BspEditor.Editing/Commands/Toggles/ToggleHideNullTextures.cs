using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Properties;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Commands.Toggles
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [CommandID("BspEditor:Map:ToggleHideNullTextures")]
    [MenuItem("Map", "", "Texture", "H")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_HideNullTextures))]
    public class ToggleHideNullTextures : BaseCommand, IMenuItemExtendedProperties
    {
        public override string Name { get; set; } = "Hide Null Textures";
        public override string Details { get; set; } = "Toggle null texture display.";
        public bool IsToggle => true;

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            var tl = document.Map.Data.GetOne<DisplayFlags>() ?? new DisplayFlags();
            tl.HideNullTextures = !tl.HideNullTextures;

            var tc = await document.Environment.GetTextureCollection();

            await MapDocumentOperation.Perform(document,
                new TrivialOperation(
                    x => x.Map.Data.Replace(tl),
                    x =>
                    {
                        x.Update(tl);
                        x.UpdateRange(x.Document.Map.Root.Find(s => s is Solid).Where(s => s.Data.Get<Face>().Any(f => tc.IsNullTexture(f.Texture.Name))));
                    })
            );
        }

        public bool GetToggleState(IContext context)
        {
            if (!context.TryGet("ActiveDocument", out MapDocument doc)) return false;
            var tf = doc.Map.Data.GetOne<DisplayFlags>() ?? new DisplayFlags();
            return tf.HideNullTextures;
        }
    }
}