using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Properties;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using Sledge.QuickForms;

namespace Sledge.BspEditor.Editing.Commands.View
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [CommandID("BspEditor:View:GoToLocation")]
    [MenuItem("View", "", "GoTo", "H")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_GoToCoordinates))]
    public class GoToLocation : BaseCommand
    {
        public override string Name { get; set; } = "Go to location";
        public override string Details { get; set; } = "Center views on a specific set of coordinates.";

        public string Title { get; set; }
        public string OK { get; set; }
        public string Cancel { get; set; }

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            using (var qf = new QuickForm(Title) { UseShortcutKeys = true }.TextBox("X", "X", "0").TextBox("Y", "Y", "0").TextBox("Z", "Z", "0").OkCancel(OK, Cancel))
            {
                qf.ClientSize = new Size(180, qf.ClientSize.Height);

                if (await qf.ShowDialogAsync() != DialogResult.OK) return;

                if (!Decimal.TryParse(qf.String("X"), out var x)) return;
                if (!Decimal.TryParse(qf.String("Y"), out var y)) return;
                if (!Decimal.TryParse(qf.String("Z"), out var z)) return;

                var coordinate = new Vector3((float) x, (float) y, (float) z);
                
                var box = new Box(coordinate - (Vector3.One * 10), coordinate + (Vector3.One * 10));

                await Task.WhenAll(
                    Oy.Publish("MapDocument:Viewport:Focus3D", box),
                    Oy.Publish("MapDocument:Viewport:Focus2D", box)
                );
            }
        }
    }
}