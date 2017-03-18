using System;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.Editor.Documents;
using Sledge.Providers.Texture;
using Sledge.Settings;

namespace Sledge.Editor.UI.Sidebar
{
    public partial class TextureSidebarPanel : UserControl, IMediatorListener
    {
        public TextureSidebarPanel()
        {
            InitializeComponent();
            SizeLabel.Text = "";
            NameLabel.Text = "";

            Mediator.Subscribe(EditorMediator.DocumentActivated, this);
            Mediator.Subscribe(EditorMediator.DocumentAllClosed, this);
            Mediator.Subscribe(EditorMediator.TextureSelected, this);
        }

        private void ApplyButtonClicked(object sender, EventArgs e)
        {
            Mediator.Publish(HotkeysMediator.ApplyCurrentTextureToSelection);
        }

        private void BrowseButtonClicked(object sender, EventArgs e)
        {
            if (DocumentManager.CurrentDocument == null) return;
            using (var tb = new TextureBrowser(DocumentManager.CurrentDocument))
            {
                tb.SetTextureList(DocumentManager.CurrentDocument.TextureCollection.GetAllTextures());
                tb.ShowDialog();
                if (tb.SelectedTexture != null)
                {
                    Mediator.Publish(EditorMediator.TextureSelected, tb.SelectedTexture);
                }
            }
        }

        private void ReplaceButtonClicked(object sender, EventArgs e)
        {
            Mediator.Publish(HotkeysMediator.ReplaceTextures);
        }

        public void Notify(string message, object data)
        {
            Mediator.ExecuteDefault(this, message, data);
        }

        private void DocumentActivated(Document doc)
        {
            TextureSelected(doc.TextureCollection.GetRecentTextures().FirstOrDefault());
        }

        private void DocumentAllClosed()
        {
            TextureSelected(null);
        }

        private void TextureSelected(string selection)
        {
            var dis = SelectionPictureBox.Image;
            SelectionPictureBox.Image = null;
            if (dis != null) dis.Dispose();

            SizeLabel.Text = "";
            NameLabel.Text = "";

            if (selection == null || DocumentManager.CurrentDocument == null) return;

            // todo texture
            //var bmp = DocumentManager.CurrentDocument.TextureCollection.GetImage(selection.Name, 128, 128);
            //if (bmp != null)
            //{
            //    if (bmp.Width > SelectionPictureBox.Width || bmp.Height > SelectionPictureBox.Height)
            //    {
            //        SelectionPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            //    }
            //    else
            //    {
            //        SelectionPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            //    }
            //}

            //SelectionPictureBox.Image = bmp;
            //SizeLabel.Text = String.Format("{0} x {1}", selection.Width, selection.Height);
            //NameLabel.Text = selection.Name;
        }
    }
}
