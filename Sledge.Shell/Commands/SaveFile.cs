using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
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
    [CommandID("File:Save")]
    [DefaultHotkey("Ctrl+S")]
    [MenuItem("File", "", "File", "H")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_Save))]
    public class SaveFile : ICommand
    {
        private readonly IEnumerable<Lazy<IDocumentLoader>> _loaders;

        public string Name { get; set; } = "Save";
        public string Details { get; set; } = "Save";

        [ImportingConstructor]
        public SaveFile([ImportMany] IEnumerable<Lazy<IDocumentLoader>> loaders)
        {
            _loaders = loaders;
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out IDocument _);
        }

        public async Task Invoke(IContext context, CommandParameters parameters)
        {
            var doc = context.Get<IDocument>("ActiveDocument");
            if (doc != null)
            {
                var loaders = _loaders.Select(x => x.Value).Where(x => x.CanSave(doc)).ToList();

                var filename = doc.FileName;

                if (filename == null || !Directory.Exists(Path.GetDirectoryName(filename)))
                {
                    var filter = loaders.SelectMany(x => x.SupportedFileExtensions).Select(x => x.Description + "|" + String.Join(";", x.Extensions.Select(e => "*" + e))).ToList();

                    using (var sfd = new SaveFileDialog {Filter = String.Join("|", filter)})
                    {
                        if (sfd.ShowDialog() != DialogResult.OK) return;
                        filename = sfd.FileName;
                    }
                }

                await Oy.Publish("Command:Run", new CommandMessage("Internal:SaveDocument", new
                {
                    Document = doc,
                    Path = filename
                }));
            }
        }
    }
}