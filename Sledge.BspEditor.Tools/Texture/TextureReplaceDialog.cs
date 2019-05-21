using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Data;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Translations;
using Sledge.Shell;

namespace Sledge.BspEditor.Tools.Texture
{
    [Export(typeof(IDialog))]
    [Export(typeof(IInitialiseHook))]
    [AutoTranslate]
    public partial class TextureReplaceDialog : Form, IManualTranslate, IDialog, IInitialiseHook
    {
        [Import("Shell", typeof(Form))] private Lazy<Form> _parent;
        [Import] private IContext _context;
        [Import] private Lazy<ITranslationStringProvider> _translation;

        private List<Subscription> _subscriptions;

        public TextureReplaceDialog()
        {
            InitializeComponent();
        }

        public Task OnInitialise()
        {
            BindTextureControls(FindTextbox, FindImage, FindBrowse, FindInfo);
            BindTextureControls(ReplaceTextbox, ReplaceImage, ReplaceBrowse, ReplaceInfo);
            return Task.FromResult(0);
        }

        private void Reset()
        {
            this.InvokeLater(() =>
            {
                var document = GetDocument();
                
                FindTextbox.Text = "";
                ReplaceTextbox.Text = "";

                ReplaceSelection.Checked = true;
                ActionExact.Checked = true;

                if (document.Selection.IsEmpty)
                {
                    ReplaceSelection.Enabled = false;
                    ReplaceVisible.Checked = true;
                }

                var at = document.Map.Data.GetOne<ActiveTexture>()?.Name;
                if (at != null)
                {
                    FindTextbox.Text = at;
                }
                else if (!document.Selection.IsEmpty)
                {
                    var first = document.Selection.FirstOrDefault(x => x is Solid) as Solid;
                    var face = first?.Faces.FirstOrDefault();
                    var texture = face?.Texture.Name;
                    if (texture != null) FindTextbox.Text = texture;
                }
            });
        }

        public void Translate(ITranslationStringProvider strings)
        {
            CreateHandle();
            var prefix = GetType().FullName;
            this.InvokeLater(() =>
            {
                Text = strings.GetString(prefix, "Title");

                FindGroup.Text = strings.GetString(prefix, "Find");
                ReplaceGroup.Text = strings.GetString(prefix, "Replace");

                FindBrowse.Text = ReplaceBrowse.Text = strings.GetString(prefix, "Browse");
                
                ReplaceInGroup.Text = strings.GetString(prefix, "ReplaceIn");
                ReplaceSelection.Text = strings.GetString(prefix, "ReplaceSelection");
                ReplaceVisible.Text = strings.GetString(prefix, "ReplaceVisible");
                ReplaceEverything.Text = strings.GetString(prefix, "ReplaceEverything");

                RescaleTextures.Text = strings.GetString(prefix, "RescaleTextures");
                
                ActionGroup.Text = strings.GetString(prefix, "Action");
                ActionExact.Text = strings.GetString(prefix, "ActionExact");
                ActionPartial.Text = strings.GetString(prefix, "ActionPartial");
                ActionSubstitute.Text = strings.GetString(prefix, "ActionSubstitute");
                ActionSelect.Text = strings.GetString(prefix, "ActionSelect");

                OKButton.Text = strings.GetString(prefix, "OK");
                CancelButton.Text = strings.GetString(prefix, "Cancel");
            });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Oy.Publish("Context:Remove", new ContextInfo("BspEditor:TextureReplace"));
        }

		protected override void OnMouseEnter(EventArgs e)
		{
            Focus();
            base.OnMouseEnter(e);
        }

        public bool IsInContext(IContext context)
        {
            return context.HasAny("BspEditor:TextureReplace");
        }

        public void SetVisible(IContext context, bool visible)
        {
            this.InvokeLater(() =>
            {
                if (visible)
                {
                    if (!Visible) Show(_parent.Value);
                    Subscribe();
                    Reset();
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
                Oy.Subscribe<MapDocument>("Document:Activated", DocumentActivated),
                Oy.Subscribe<MapDocument>("MapDocument:SelectionChanged", SelectionChanged)
            };
        }

        private void Unsubscribe()
        {
            if (_subscriptions == null) return;
            _subscriptions.ForEach(x => x.Dispose());
            _subscriptions = null;
        }

        public async Task DocumentActivated(MapDocument document)
        {
            Reset();
        }

        public async Task SelectionChanged(MapDocument document)
        {
            this.InvokeLater(() =>
            {
                if (document.Selection.IsEmpty)
                {
                    if (ReplaceSelection.Checked) ReplaceVisible.Checked = true;
                    ReplaceSelection.Enabled = false;
                }
                else
                {
                    ReplaceSelection.Enabled = true;
                }
            });
        }

        public MapDocument GetDocument()
        {
            var doc = _context.Get<MapDocument>("ActiveDocument");
            return doc;
        }

        private IEnumerable<IMapObject> GetObjects(MapDocument doc)
        {
            if (ReplaceSelection.Checked) return doc.Selection.ToList();
            if (ReplaceVisible.Checked) return doc.Map.Root.Find(x => !x.Data.OfType<IObjectVisibility>().Any(y => y.IsHidden));
            return doc.Map.Root.FindAll();
        }

        private bool MatchTextureName(string name)
        {
            if (String.IsNullOrWhiteSpace(name)) return false;

            var match = FindTextbox.Text;
            if (!ActionExact.Checked)
            {
                return name.ToLowerInvariant().Contains(match.ToLowerInvariant());
            }
            return String.Equals(name, match, StringComparison.InvariantCultureIgnoreCase);
        }

        private List<TextureReplacement> GetReplacements(IEnumerable<string> names)
        {
            var list = new List<TextureReplacement>();
            var substitute = ActionSubstitute.Checked;
            var find = FindTextbox.Text.ToLowerInvariant();
            var replace = ReplaceTextbox.Text.ToLowerInvariant();

            foreach (var name in names.Select(x => x.ToLowerInvariant()).Distinct())
            {
                var n = substitute ? name.Replace(find, replace) : replace;
                list.Add(new TextureReplacement(name, n));
            }
            return list;
        }

        public async Task<IOperation> GetOperation(MapDocument doc)
        {
            if (ActionSelect.Checked)
            {
                return new Transaction(
                    new Deselect(doc.Selection.ToList()),
                    new Select(GetObjects(doc))
                );
            }

            var faces = GetObjects(doc).OfType<Solid>()
                .SelectMany(x => x.Faces.Select(f => new { Face = f, Parent = x }))
                .Where(x => MatchTextureName(x.Face.Texture.Name))
                .ToList();

            var rescale = RescaleTextures.Checked;
            var tc = rescale ? await doc.Environment.GetTextureCollection() : null;
            var replacements = GetReplacements(faces.Select(x => x.Face.Texture.Name));

            var tran = new Transaction();
            foreach (var fp in faces)
            {
                var face = fp.Face;
                var parent = fp.Parent;

                var clone = (Face) face.Clone();

                var repl = replacements.FirstOrDefault(x => x.Find == face.Texture.Name.ToLowerInvariant());
                if (repl == null) continue;

                if (rescale && tc != null)
                {
                    var find = await tc.GetTextureItem(face.Texture.Name);
                    var replace = await tc.GetTextureItem(repl.Replace);
                    if (find != null && replace != null)
                    {
                        clone.Texture.XScale *= find.Width / (float) replace.Width;
                        clone.Texture.YScale *= find.Height / (float) replace.Height;
                    }
                }

                clone.Texture.Name = repl.Replace;
                
                tran.Add(new RemoveMapObjectData(parent.ID, face));
                tran.Add(new AddMapObjectData(parent.ID, clone));
            }
            
            return tran;
        }

        private void BindTextureControls(TextBox box, PictureBox image, Button browse, Label info)
        {
            box.TextChanged += (sender, e) => UpdateTexture(box.Text, image, info);
            browse.Click += (sender, e) => BrowseTexture(box);
            UpdateTexture(box.Text, image, info);
        }

        private async void BrowseTexture(TextBox box)
        {
            var doc = GetDocument();
            if (doc == null) return;

            using (var tb = new TextureBrowser(doc))
            {
                await tb.Initialise(_translation.Value);
                if (await tb.ShowDialogAsync() != DialogResult.OK) return;
                if (tb.SelectedTexture == null) return;
                box.Text = tb.SelectedTexture;
            }
        }

        private async void UpdateTexture(string text, PictureBox image, Label info)
        {
            var doc = GetDocument();
            if (String.IsNullOrWhiteSpace(text) || doc == null)
            {
                image.Image = null;
                info.Text = "No Image";
                return;
            }
            
            var tc = await doc.Environment.GetTextureCollection();
            var item = await tc.GetTextureItem(text);

            if (item != null)
            {
                using (var tp = tc.GetStreamSource())
                {
                    var bmp = await tp.GetImage(text, 128, 128);
                    image.SizeMode = bmp.Width > image.Width || bmp.Height > image.Height
                        ? PictureBoxSizeMode.Zoom
                        : PictureBoxSizeMode.CenterImage;
                    image.Image = bmp;
                }

                var format = "{0} x {1}";
                info.Text = string.Format(format, item.Width, item.Height);
            }
            else
            {
                image.Image = null;
                info.Text = "No Image";
            }
        }

        private void OkClicked(object sender, EventArgs e)
        {
            ExecuteReplace();
            Close();
        }

        private async Task ExecuteReplace()
        {
            var doc = GetDocument();
            if (doc == null) return;

            var op = await GetOperation(doc);
            if (op == null) return;

            await MapDocumentOperation.Perform(doc, op);
        }

        private void CancelClicked(object sender, EventArgs e)
        {
            Close();
        }

        private class TextureReplacement
        {
            public string Find { get; set; }
            public string Replace { get; set; }

            public TextureReplacement(string find, string replace)
            {
                Find = find;
                Replace = replace;
            }
        }
    }
}
