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

        public string Solids { set => this.InvokeLater(() => SolidsLabel.Text = value); }
        public string Faces { set => this.InvokeLater(() => FacesLabel.Text = value); }
        public string PointEntities { set => this.InvokeLater(() => PointEntitiesLabel.Text = value); }
        public string SolidEntities { set => this.InvokeLater(() => SolidEntitiesLabel.Text = value); }
        public string UniqueTextures { set => this.InvokeLater(() => UniqueTexturesLabel.Text = value); }
        public string TextureMemory { set => this.InvokeLater(() => TextureMemoryLabel.Text = value); }
        public string TexturePackagesUsed { set => this.InvokeLater(() => TexturePackagesUsedLabel.Text = value); }
        public string CloseButton { set => this.InvokeLater(() => CloseDialogButton.Text = value); }

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

        private async Task CalculateStats()
        {
            var doc = _context.Get<MapDocument>("ActiveDocument");
            if (doc == null)
            {
                NumSolids.Text = "\u2014";
                NumFaces.Text = "\u2014";
                NumPointEntities.Text = "\u2014";
                NumSolidEntities.Text = "\u2014";
                NumUniqueTextures.Text = "\u2014";
                TextureMemoryValue.Text = "\u2014";
                TexturePackages.Items.Clear();
                return;
            }


            var all = doc.Map.Root.FindAll();
            var solids = all.OfType<Solid>().ToList();
            var faces = solids.SelectMany(x => x.Faces).ToList();
            var entities = all.OfType<Entity>().ToList();
            var numSolids = solids.Count;
            var numFaces = faces.Count;
            var numPointEnts = entities.Count(x => !x.Hierarchy.HasChildren);
            var numSolidEnts = entities.Count(x => x.Hierarchy.HasChildren);
            var uniqueTextures = faces.Select(x => x.Texture.Name).Distinct().ToList();
            var numUniqueTextures = uniqueTextures.Count;
            //var textureMemory = faces.Select(x => x.Texture.Name)
            //    .Distinct()
            //    .Select(document.GetTextureSize)
            //    .Sum(x => x.Width * x.Height * 3); // 3 bytes per pixel
            //var textureMemoryMb = textureMemory / (1024m * 1024m);
            // todo texture memory, texture packages

            NumSolids.Text = numSolids.ToString(CultureInfo.CurrentCulture);
            NumFaces.Text = numFaces.ToString(CultureInfo.CurrentCulture);
            NumPointEntities.Text = numPointEnts.ToString(CultureInfo.CurrentCulture);
            NumSolidEntities.Text = numSolidEnts.ToString(CultureInfo.CurrentCulture);
            NumUniqueTextures.Text = numUniqueTextures.ToString(CultureInfo.CurrentCulture);
            TextureMemoryValue.Text = "?";
            // TextureMemory.Text = textureMemory.ToString(CultureInfo.CurrentCulture);
            //TextureMemory.Text = textureMemory.ToString("#,##0", CultureInfo.CurrentCulture)
            //    + " bytes (" + textureMemoryMb.ToString("0.00", CultureInfo.CurrentCulture) + " MB)";
            TexturePackages.Items.Clear();
            //foreach (var tp in document.GetUsedTexturePackages())
            //{
            //    TexturePackages.Items.Add(tp);
            //}
        }

        private void CloseButtonClicked(object sender, EventArgs e)
        {
            Close();
        }
    }
}
