using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using Sledge.Shell;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Editing.Components
{
    [Export(typeof(IDialog))]
    [AutoTranslate]
    public partial class MapInformationDialog : Form, IDialog
    {
        [Import("Shell", typeof(Form))] private Lazy<Form> _parent;
        [Import] private IContext _context;
        
        private List<Subscription> _subscriptions;

        #region Translations

        public string Title { set => this.InvokeLater(() => Text = value); }
        public string Solids { set => this.InvokeLater(() => SolidsLabel.Text = value); }
        public string Faces { set => this.InvokeLater(() => FacesLabel.Text = value); }
        public string PointEntities { set => this.InvokeLater(() => PointEntitiesLabel.Text = value); }
        public string SolidEntities { set => this.InvokeLater(() => SolidEntitiesLabel.Text = value); }
        public string UniqueTextures { set => this.InvokeLater(() => UniqueTexturesLabel.Text = value); }
        public string TextureMemory { set => this.InvokeLater(() => TextureMemoryLabel.Text = value); }
        public string TexturePackagesUsed { set => this.InvokeLater(() => TexturePackagesUsedLabel.Text = value); }
        public string CloseButton { set => this.InvokeLater(() => CloseDialogButton.Text = value); }
        public string CalculatingTextureMemoryUsage { get; set; }

        #endregion

        public MapInformationDialog()
        {
            InitializeComponent();
            CreateHandle();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Oy.Publish("Context:Remove", new ContextInfo("BspEditor:MapInformation"));
        }

        protected override void OnMouseEnter(EventArgs e)
		{
            Focus();
            base.OnMouseEnter(e);
        }

        public bool IsInContext(IContext context)
        {
            return context.HasAny("BspEditor:MapInformation");
        }

        public void SetVisible(IContext context, bool visible)
        {
            this.InvokeLater(() =>
            {
                if (visible)
                {
                    if (!Visible) Show(_parent.Value);
                    Subscribe();
                    CalculateStats();
                }
                else
                {
                    Hide();
                    Unsubscribe();
                }
            });
        }

        private void Subscribe()
        {
            if (_subscriptions != null) return;
            _subscriptions = new List<Subscription>
            {
                Oy.Subscribe<Change>("MapDocument:Changed", _ => CalculateStats()),
                Oy.Subscribe<MapDocument>("Document:Activated", _ => CalculateStats())
            };
        }

        private void Unsubscribe()
        {
            if (_subscriptions == null) return;
            _subscriptions.ForEach(x => x.Dispose());
            _subscriptions = null;
        }

        private Task CalculateStats()
        {
            var doc = _context.Get<MapDocument>("ActiveDocument");

            if (doc == null)
            {
                return this.InvokeLaterAsync(() =>
                {
                    NumSolids.Text = "\u2014";
                    NumFaces.Text = "\u2014";
                    NumPointEntities.Text = "\u2014";
                    NumSolidEntities.Text = "\u2014";
                    NumUniqueTextures.Text = "\u2014";
                    TextureMemoryValue.Text = "\u2014";
                    TexturePackages.Items.Clear();
                });
            }

            var all = doc.Map.Root.FindAll();
            var solids = all.OfType<Solid>().ToList();
            var faces = solids.SelectMany(x => x.Faces).ToList();
            var entities = all.OfType<Entity>().ToList();
            var numSolids = solids.Count;
            var numFaces = faces.Count;
            var numPointEnts = entities.Count(x => !x.Hierarchy.HasChildren);
            var numSolidEnts = entities.Count(x => x.Hierarchy.HasChildren);
            var uniqueTextures = new HashSet<string>(faces.Select(x => x.Texture.Name));
            var numUniqueTextures = uniqueTextures.Count;

            return this.InvokeLaterAsync(() =>
            {
                NumSolids.Text = numSolids.ToString(CultureInfo.CurrentCulture);
                NumFaces.Text = numFaces.ToString(CultureInfo.CurrentCulture);
                NumPointEntities.Text = numPointEnts.ToString(CultureInfo.CurrentCulture);
                NumSolidEntities.Text = numSolidEnts.ToString(CultureInfo.CurrentCulture);
                NumUniqueTextures.Text = numUniqueTextures.ToString(CultureInfo.CurrentCulture);
                TextureMemoryValue.Text = CalculatingTextureMemoryUsage;
            }).ContinueWith(async _ =>
            {
                var tc = await doc.Environment.GetTextureCollection();
                var usedPackages = tc.Packages.Where(x => x.Textures.Overlaps(uniqueTextures));

                this.InvokeLater(() =>
                {
                    TexturePackages.Items.Clear();
                    foreach (var tp in usedPackages)
                    {
                        TexturePackages.Items.Add(tp);
                    }
                });

                long texUsage = 0;
                foreach (var ut in uniqueTextures)
                {
                    var tex = await tc.GetTextureItem(ut);
                    // todo BETA: Other engines: the texture size operation will need to be outsourced to the provider to properly calculate usage for non-24-bit textures
                    texUsage += tex.Width * tex.Height * 3; // 3 bytes per pixel
                }
                var textureMemoryMb = texUsage / (1024m * 1024m);
                this.InvokeLater(() =>
                {
                    TextureMemoryValue.Text = $@"{textureMemoryMb:0.00} MB";
                });
            });
        }

        private void CloseButtonClicked(object sender, EventArgs e)
        {
            Close();
        }

        private void ComputeTextureUsage(object sender, EventArgs e)
        {
            TextureMemoryValue.Text = CalculatingTextureMemoryUsage;
            // ...
        }
    }
}
