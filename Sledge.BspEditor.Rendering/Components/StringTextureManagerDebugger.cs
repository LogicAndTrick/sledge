using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Rendering.Materials;
using Sledge.Shell;

namespace Sledge.BspEditor.Rendering.Components
{
    //[Export(typeof(IDialog))]
    public partial class StringTextureManagerDebugger : Form, IDialog
    {
        [Import("Shell", typeof(Form))] private Lazy<Form> _parent;
        [Import] private IContext _context;

        private List<Subscription> _subscriptions;

        private List<StringTextureManager.StringTexture> _textures;
        private Timer _timer;

        public StringTextureManagerDebugger()
        {
            InitializeComponent();
            CreateHandle();

            _timer = new Timer();
            _timer.Interval = 10000;
            _timer.Enabled = true;
            _timer.Tick += (s, a) => UpdateTextures();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
        }

        public bool IsInContext(IContext context)
        {
#if DEBUG
            return false;
#endif
            return false;
        }

        public void SetVisible(IContext context, bool visible)
        {
            this.InvokeLater(() =>
            {
                if (visible)
                {
                    if (!Visible) Show(_parent.Value);
                    Subscribe();
                    _timer.Start();
                }
                else
                {
                    Hide();
                    Unsubscribe();
                    _timer.Stop();
                }
            });
        }

        private void Subscribe()
        {
            if (_subscriptions != null) return;
            _subscriptions = new List<Subscription>
            {
                // 
            };
        }

        private void Unsubscribe()
        {
            if (_subscriptions == null) return;
            _subscriptions.ForEach(x => x.Dispose());
            _subscriptions = null;
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            UpdateTextures();
        }
        private void UpdateTextures()
        {
            _textures = new List<StringTextureManager.StringTexture>();
            TextureList.Items.Clear();

            if (Renderer.Instance == null || Renderer.Instance.Engine == null || Renderer.Instance.Engine.Renderer == null) return;


            var stm = Renderer.Instance.Engine.Renderer.StringTextureManager;
            _textures = stm.GetTextures().ToList();

            TextureList.BeginUpdate();
            foreach (var tex in _textures)
            {
                foreach (var stv in tex.GetValues().Where(x => !x.IsRemoved))
                {
                    TextureList.Items.Add(new ListViewItem(stv.Value)
                    {
                        SubItems =
                        {
                            tex.Font.Name,
                            stv.Rectangle.X + ", " + stv.Rectangle.Y,
                            stv.Rectangle.Width + " x " + stv.Rectangle.Height
                        },
                        Tag = tex
                    });
                }
            }
            TextureList.EndUpdate();
        }

        private void TextureList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var texValue = TextureList.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
            var tex = texValue?.Tag as StringTextureManager.StringTexture;
            if (tex == null) return;

            try
            {
                var value = tex.Get(texValue.Text);

                var im = new Bitmap(tex.Image.Width, tex.Image.Height);
                using (var g = Graphics.FromImage(im))
                {
                    g.FillRectangle(Brushes.Black, 0, 0, im.Width, im.Height);
                    g.DrawImageUnscaled(tex.Image, 0, 0);
                    g.DrawRectangle(Pens.Red, value.Rectangle);
                }

                ImageBox.Image = im;
            }
            catch
            {
                // who cares
            }
        }
    }
}
