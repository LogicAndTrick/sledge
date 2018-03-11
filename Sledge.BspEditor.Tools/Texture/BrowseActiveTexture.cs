using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Tools.Texture
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [CommandID("BspEditor:BrowseActiveTexture")]
    public class BrowseActiveTexture : ICommand
    {
        [Import] private Lazy<ITranslationStringProvider> _translation;

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
                await tb.Initialise(_translation.Value);
                if (tb.ShowDialog() == DialogResult.OK && !String.IsNullOrWhiteSpace(tb.SelectedTexture))
                {
                    var tex = tb.SelectedTexture;
                    var at = new ActiveTexture {Name = tex};
                    MapDocumentOperation.Perform(md, new TrivialOperation(x => x.Map.Data.Replace(at), x => x.Update(at)));
                }
            }
        }
    }
}
