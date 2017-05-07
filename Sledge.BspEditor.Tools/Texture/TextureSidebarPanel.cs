using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Translations;
using Sledge.Shell;

namespace Sledge.BspEditor.Tools.Texture
{
    [AutoTranslate]
    [Export(typeof(ISidebarComponent))]
    [Export(typeof(IInitialiseHook))]
    [SidebarComponent(OrderHint = "B")]
    public partial class TextureSidebarPanel : UserControl, ISidebarComponent, IInitialiseHook
    {
        public Task OnInitialise()
        {
            Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated);
            Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged);
            return Task.FromResult(0);
        }

        public string Title => "Texture";
        public object Control => this;

        private WeakReference<MapDocument> _activeDocument;

        public TextureSidebarPanel()
        {
            InitializeComponent();
            SizeLabel.Text = "";
            NameLabel.Text = "";
            _activeDocument = new WeakReference<MapDocument>(null);
        }

        private void ApplyButtonClicked(object sender, EventArgs e)
        {
            // Mediator.Publish(HotkeysMediator.ApplyCurrentTextureToSelection);
        }

        private void BrowseButtonClicked(object sender, EventArgs e)
        {
            Oy.Publish("Command:Run", new CommandMessage("BspEditor:BrowseActiveTexture"));
        }

        private void ReplaceButtonClicked(object sender, EventArgs e)
        {
            // Mediator.Publish(HotkeysMediator.ReplaceTextures);
        }

        private async Task DocumentActivated(IDocument doc)
        {
            var md = doc as MapDocument;
            _activeDocument = new WeakReference<MapDocument>(md);
            if (md != null)
            {
                await TextureSelected(md.Map.Data.GetOne<ActiveTexture>()?.Name);
            }
        }

        private async Task DocumentChanged(Change change)
        {
            if (_activeDocument.TryGetTarget(out MapDocument t) && change.Document == t)
            {
                await TextureSelected(t.Map.Data.GetOne<ActiveTexture>()?.Name);
            }
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument _);
        }

        private async Task TextureSelected(string selection)
        {
            var dis = SelectionPictureBox.Image;
            SelectionPictureBox.Image = null;
            dis?.Dispose();

            SizeLabel.Text = "";
            NameLabel.Text = "";
            
            if (selection == null) return;
            if (!_activeDocument.TryGetTarget(out MapDocument doc)) return;

            var tc = await doc.Environment.GetTextureCollection();
            var texItem = await tc.GetTextureItem(selection);

            if (texItem == null) return;

            Bitmap bmp;
            using (var ss = await tc.GetStreamSource())
            {
                bmp = await ss.GetImage(selection, 256, 256);
            }

            Task.Factory.StartNew(() =>
            {
                this.Invoke(() =>
                {
                    if (bmp != null)
                    {
                        if (bmp.Width > SelectionPictureBox.Width || bmp.Height > SelectionPictureBox.Height)
                        {
                            SelectionPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                        else
                        {
                            SelectionPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
                        }
                    }
                    SelectionPictureBox.Image = bmp;
                    NameLabel.Text = selection ?? "";
                    SizeLabel.Text = $"{texItem.Width} x {texItem.Height}";
                    NameLabel.Text = texItem.Name;
                });
            });
        }
    }
}
