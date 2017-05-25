using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Components;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Components.Visgroup;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Commands
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [CommandID("BspEditor:Map:Visgroups")]
    [DefaultHotkey("Shift+Y")]
    public class EditVisgroups : BaseCommand
    {
        [Import] private Lazy<ITranslationStringProvider> _translator;

        public override string Name { get; set; } = "Edit visgroups";
        public override string Details { get; set; } = "View and edit map visgroups";

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            using (var vg = new VisgroupEditForm(document))
            {
                _translator.Value.Translate(vg);
                if (vg.ShowDialog() == DialogResult.OK)
                {
                    // todo
                }
            }
        }
    }
}