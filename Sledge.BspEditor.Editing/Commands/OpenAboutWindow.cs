using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Sledge.BspEditor.Editing.Components;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Commands
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Help", "", "About", "Z")]
    [CommandID("BspEditor:Help:About")]
    public class OpenAboutWindow : ICommand
    {
        public string Name { get; set; } = "About Sledge";
        public string Details { get; set; } = "View information about this application";

        public bool IsInContext(IContext context)
        {
            return true;
        }

        public async Task Invoke(IContext context, CommandParameters parameters)
        {
            using (var vg = new AboutDialog())
            {
                vg.ShowDialog();
            }
        }
    }
}