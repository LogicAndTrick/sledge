using System.ComponentModel.Composition;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;
using Sledge.Shell.Properties;

namespace Sledge.Shell.Commands
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [CommandID("File:Close")]
    [MenuItem("File", "", "File", "F")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_Close))]
    public class CloseFile : ICommand
    {
        public string Name { get; set; } = "Close";
        public string Details { get; set; } = "Close";

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out IDocument _);
        }

        public async Task Invoke(IContext context, CommandParameters parameters)
        {
            var doc = context.Get<IDocument>("ActiveDocument");
            if (doc != null) await Oy.Publish("Document:RequestClose", doc);
        }
    }
}