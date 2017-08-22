using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Components;
using Sledge.BspEditor.Editing.Components.Compile;
using Sledge.BspEditor.Editing.Components.Compile.Profiles;
using Sledge.BspEditor.Editing.Components.Compile.Specification;
using Sledge.BspEditor.Editing.Properties;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;
using Sledge.Shell;

namespace Sledge.BspEditor.Editing.Commands
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("File", "", "Build", "B")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_Run))]
    [CommandID("BspEditor:File:Compile")]
    [DefaultHotkey("F9")]
    public class OpenCompileWindow : BaseCommand
    {
        [Import] private CompileSpecificationRegister _compileSpecificationRegister;
        [Import] private BuildProfileRegister _buildProfileRegister;

        public override string Name { get; set; } = "Run...";
        public override string Details { get; set; } = "Open the compile dialog";

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            var spec = _compileSpecificationRegister.GetCompileSpecificationsForEngine(document.Environment.Engine).FirstOrDefault();
            if (spec == null) return;

            using (var cd = new CompileDialog(spec, _buildProfileRegister))
            {
                if (await cd.ShowDialogAsync() == DialogResult.OK)
                {
                    var arguments = cd.SelectedBatchArguments;
                    var batch = await document.Environment.CreateBatch(arguments);
                    if (batch == null) return;
                    
                    await batch.Run(document);
                }
            }
        }
    }
}