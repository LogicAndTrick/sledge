using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;
using Sledge.Shell.Properties;

namespace Sledge.Shell.Commands
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [CommandID("File:Open")]
    [DefaultHotkey("Ctrl+O")]
    [MenuItem("File", "", "File", "D")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_Open))]
    public class OpenFile : ICommand
    {
        private readonly IEnumerable<Lazy<IDocumentLoader>> _loaders;

        public string Name { get; set; } = "Open";
        public string Details { get; set; } = "Open...";

        [ImportingConstructor]
        public OpenFile([ImportMany] IEnumerable<Lazy<IDocumentLoader>> loaders)
        {
            _loaders = loaders;
        }

        public bool IsInContext(IContext context)
        {
            return true;
        }

        public async Task Invoke(IContext context, CommandParameters parameters)
        {
            var filter = _loaders.Select(x => x.Value).Select(x => x.FileTypeDescription + "|" + String.Join(";", x.SupportedFileExtensions.SelectMany(e => e.Extensions).Select(e => "*" + e))).ToList();
            filter.Add("All files|*.*");
            using (var ofd = new OpenFileDialog { Filter = String.Join("|", filter)})
            {
                if (ofd.ShowDialog() != DialogResult.OK) return;

                await Oy.Publish("Command:Run", new CommandMessage("Internal:OpenDocument", new
                {
                    Path = ofd.FileName
                }));
            }
        }
    }
}
