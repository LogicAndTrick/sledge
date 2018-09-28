using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Properties;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Commands
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Map", "", "Properties", "A")]
    [CommandID("BspEditor:Map:RootProperties")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_MapProperties))]
    public class OpenRootProperties : BaseCommand
    {
        public override string Name { get; set; } = "Map properties";
        public override string Details { get; set; } = "Open the map properties window";

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            await Oy.Publish("BspEditor:ObjectProperties:OpenWithSelection", new List<IMapObject> {document.Map.Root});
        }
    }
}