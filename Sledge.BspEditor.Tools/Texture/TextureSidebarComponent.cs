using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Environment;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;
using Sledge.Providers.Texture;
using Sledge.Shell.Properties;

namespace Sledge.BspEditor.Tools.Texture
{
    [AutoTranslate]
    [Export(typeof(ISidebarComponent))]
    [Export(typeof(IInitialiseHook))]
    public class TextureSidebarComponent : UserControl, ISidebarComponent, IInitialiseHook
    {
        public Task OnInitialise()
        {
            Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated);
            return Task.FromResult(0);
        }

        public string Title => "Texture";
        public object Control => this;

        public TextureSidebarComponent()
        {
            var button = new Button();
            button.Click += OpenTextureBrowser;
            button.Text = "Browse";
            Controls.Add(button);
        }

        private async void OpenTextureBrowser(object sender, EventArgs e)
        {
            await Oy.Publish("Command:Run", new CommandMessage("BspEditor:OpenTextureBrowser"));
        }

        private async Task DocumentActivated(IDocument doc)
        {
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument _);
        }
    }
    
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [CommandID("BspEditor:OpenTextureBrowser")]
    public class OpenTextureBrowser : ICommand
    {
        public string Name { get; set; } = "Open texture browser";
        public string Details { get; set; } = "Open texture browser";

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument _);
        }

        public async Task Invoke(IContext context, CommandParameters parameters)
        {
            var md = context.Get<MapDocument>("ActiveDocument");
            if (md == null) return;
            using (var tb = new TextureBrowser(md))
            {
                await tb.Initialise();
                tb.ShowDialog();
            }
        }
    }
}
