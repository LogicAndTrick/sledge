using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Properties;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Commands.Pointfile
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Map", "", "Pointfile", "D")]
    [CommandID("BspEditor:Map:QuickLoadPointfile")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_QuickLoadPointfile))]
    public class QuickLoadPointfile : BaseCommand
    {
        public override string Name { get; set; } = "Quick load pointfile...";
        public override string Details { get; set; } = "Search for a pointfile for this map and load it if one was found.";
        
        public string InvalidPointfile { get; set; } = "{0} is not a valid pointfile!";
        public string NoPointfileFoundTitle { get; set; } = "Pointfile not found";
        public string NoPointfileFoundMessage { get; set; } = "No pointfile found. Would you like to browse for one?";

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            var dir = Path.GetDirectoryName(document.FileName);
            var file = Path.GetFileNameWithoutExtension(document.FileName);

            if (dir != null && file != null)
            {
                var lin = Path.Combine(dir, file + ".lin");
                if (File.Exists(lin))
                {
                    await LoadPointfile(document, lin);
                    return;
                }

                var pts = Path.Combine(dir, file + ".pts");
                if (File.Exists(pts))
                {
                    await LoadPointfile(document, pts);
                    return;
                }
            }

            if (MessageBox.Show(NoPointfileFoundMessage, NoPointfileFoundTitle, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                await Oy.Publish("Command:Run", new CommandMessage("BspEditor:Map:LoadPointfile"));
            }
        }

        private async Task LoadPointfile(MapDocument document, string file)
        {
            if (file != null && File.Exists(file))
            {
                var text = File.ReadAllLines(file);
                Pointfile point;
                try
                {
                    point = Pointfile.Parse(text);
                }
                catch
                {
                    MessageBox.Show(String.Format(InvalidPointfile, Path.GetFileName(file)));
                    return;
                }

                await MapDocumentOperation.Perform(document, new TrivialOperation(
                    d => d.Map.Data.Replace(point),
                    c => c.Add(c.Document.Map.Root)
                ));

                if (point.Lines.Any())
                {
                    var start = point.Lines[0].Start;
                    await Oy.Publish("MapDocument:Viewport:Focus2D", start);
                    await Oy.Publish("MapDocument:Viewport:Focus3D", start);
                }
            }
        }
    }
}