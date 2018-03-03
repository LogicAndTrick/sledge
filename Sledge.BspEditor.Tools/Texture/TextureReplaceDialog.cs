using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Data;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Translations;
using Sledge.Rendering.Materials;
using Sledge.Shell;

namespace Sledge.BspEditor.Tools.Texture
{
    public partial class TextureReplaceDialog : Form, IManualTranslate
    {
        private readonly MapDocument _document;

        public TextureReplaceDialog(MapDocument document)
        {
            _document = document;
            InitializeComponent();

            BindTextureControls(Find, FindImage, FindBrowse, FindInfo);
            BindTextureControls(Replace, ReplaceImage, ReplaceBrowse, ReplaceInfo);

            ReplaceSelection.Checked = true;
            ActionExact.Checked = true;

            if (document.Selection.IsEmpty)
            {
                ReplaceSelection.Enabled = false;
                ReplaceVisible.Checked = true;
            }

            var at = _document.Map.Data.GetOne<ActiveTexture>()?.Name;
            if (at != null)
            {
                Find.Text = at;
            }
            else if (!document.Selection.IsEmpty)
            {
                var first = document.Selection.FirstOrDefault(x => x is Solid) as Solid;
                var face = first?.Faces.FirstOrDefault();
                var texture = face?.Texture.Name;
                if (texture != null) Find.Text = texture;
            }
        }

        public void Translate(TranslationStringsCollection strings)
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

        private IEnumerable<IMapObject> GetObjects()
        {
            if (ReplaceSelection.Checked) return _document.Selection.ToList();
            if (ReplaceVisible.Checked) return _document.Map.Root.Find(x => !x.Data.OfType<IObjectVisibility>().Any(y => y.IsHidden));
            return _document.Map.Root.FindAll();
        }

        private bool MatchTextureName(string name)
        {
            if (String.IsNullOrWhiteSpace(name)) return false;

            var match = Find.Text;
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
            var find = Find.Text.ToLowerInvariant();
            var replace = Replace.Text.ToLowerInvariant();

            foreach (var name in names.Select(x => x.ToLowerInvariant()).Distinct())
            {
                var n = substitute ? name.Replace(find, replace) : replace;
                list.Add(new TextureReplacement(name, n));
            }
            return list;
        }

        public async Task<IOperation> GetOperation()
        {
            if (ActionSelect.Checked)
            {
                return new Transaction(
                    new Deselect(_document.Selection.ToList()),
                    new Select(GetObjects())
                );
            }

            var faces = GetObjects().OfType<Solid>()
                .SelectMany(x => x.Faces.Select(f => new { Face = f, Parent = x }))
                .Where(x => MatchTextureName(x.Face.Texture.Name))
                .ToList();

            var rescale = RescaleTextures.Checked;
            var tc = rescale ? await _document.Environment.GetTextureCollection() : null;
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
                        clone.Texture.XScale *= find.Width / (decimal) replace.Width;
                        clone.Texture.YScale *= find.Height / (decimal) replace.Height;
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
            using (var tb = new TextureBrowser(_document))
            {
                await tb.Initialise();
                if (await tb.ShowDialogAsync() != DialogResult.OK) return;
                if (tb.SelectedTexture == null) return;
                box.Text = tb.SelectedTexture;
            }
        }

        private async void UpdateTexture(string text, PictureBox image, Label info)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                image.Image = null;
                info.Text = "No Image";
                return;
            }
            
            var tc = await _document.Environment.GetTextureCollection();
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

                var format = item.Flags.HasFlag(TextureFlags.Missing) ? "Invalid texture" : "{0} x {1}";
                info.Text = string.Format(format, item.Width, item.Height);
            }
            else
            {
                image.Image = null;
                info.Text = "No Image";
            }
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
