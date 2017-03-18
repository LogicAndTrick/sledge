using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Actions.MapObjects.Selection;
using Sledge.Editor.Documents;
using Sledge.Providers.Texture;
using Sledge.Rendering.Materials;

namespace Sledge.Editor.UI
{
    public partial class TextureReplaceDialog : Form
    {
        private Document _document;

        public TextureReplaceDialog(Document document)
        {
            _document = document;
            InitializeComponent();
            BindTextureControls(Find, FindImage, FindBrowse, FindInfo);
            BindTextureControls(Replace, ReplaceImage, ReplaceBrowse, ReplaceInfo);

            ReplaceSelection.Checked = true;
            ActionExact.Checked = true;

            if (document.Selection.IsEmpty())
            {
                ReplaceSelection.Enabled = false;
                ReplaceVisible.Checked = true;
            }

            if (_document.TextureCollection.SelectedTexture != null)
            {
                var tex = _document.TextureCollection.SelectedTexture;
                Find.Text = tex;
            }
        }

        private IEnumerable<MapObject> GetObjects()
        {
            if (ReplaceSelection.Checked) return _document.Selection.GetSelectedObjects();
            if (ReplaceVisible.Checked) return _document.Map.WorldSpawn.Find(x => !x.IsVisgroupHidden);
            return _document.Map.WorldSpawn.FindAll();
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

        private IEnumerable<Tuple<string, string>> GetReplacements(IEnumerable<string> names)
        {
            var list = new List<Tuple<string, string>>();
            var substitute = ActionSubstitute.Checked;
            var find = Find.Text.ToLowerInvariant();
            var replace = Replace.Text.ToLowerInvariant();

            foreach (var name in names.Select(x => x.ToLowerInvariant()).Distinct())
            {
                var n = substitute ? name.Replace(find, replace) : replace;
                list.Add(Tuple.Create(name, n));
            }
            return list;
        }

        public IAction GetAction()
        {
            var faces = GetObjects().OfType<Solid>().SelectMany(x => x.Faces).Where(x => MatchTextureName(x.Texture.Name)).ToList();
            if (ActionSelect.Checked)
            {
                return new ChangeSelection(faces.Select(x => x.Parent).Distinct(), _document.Selection.GetSelectedObjects());
            }
            var rescale = RescaleTextures.Checked;
            var replacements = GetReplacements(faces.Select(x => x.Texture.Name));
            Action<Document, Face> action = (doc, face) =>
                                                {
                                                    var repl = replacements.FirstOrDefault(x => x.Item1 == face.Texture.Name.ToLowerInvariant());
                                                    if (repl == null) return;
                                                    if (rescale)
                                                    {
                                                        // todo
                                                        throw new NotImplementedException();
                                                        //var item = _document.TextureCollection.GetItem(face.Texture.Name);
                                                        //if (item != null)
                                                        //{
                                                        //    face.Texture.XScale *= item.Width / (decimal)repl.Item2.Width;
                                                        //    face.Texture.YScale *= item.Height / (decimal)repl.Item2.Height;
                                                        //}
                                                    }
                                                    face.Texture.Name = repl.Item2;
                                                };
            return new EditFace(faces, action, true);
        }

        private void BindTextureControls(TextBox box, PictureBox image, Button browse, Label info)
        {
            box.TextChanged += (sender, e) => UpdateTexture(box.Text, image, info);
            browse.Click += (sender, e) => BrowseTexture(box);
            UpdateTexture(box.Text, image, info);
        }

        private void BrowseTexture(TextBox box)
        {
            using (var tb = new TextureBrowser(_document))
            {
                throw new NotImplementedException();
                // todo tb.SetTextureList(_document.TextureCollection.GetAllBrowsableItems());
                tb.ShowDialog();
                if (tb.SelectedTexture != null)
                {
                    box.Text = tb.SelectedTexture;
                }
            }
        }

        private void UpdateTexture(string text, PictureBox image, Label info)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                image.Image = null;
                info.Text = "No Image";
                return;
            }

            var item = text;

            // todo texture
            throw new NotImplementedException();
            //using (var tp = _document.TextureCollection.GetStreamSource(128, 128))
            //{
            //    var bmp = tp.GetImage(item);
            //    image.SizeMode = bmp.Width > image.Width || bmp.Height > image.Height
            //                         ? PictureBoxSizeMode.Zoom
            //                         : PictureBoxSizeMode.CenterImage;
            //    image.Image = bmp;
            //}

            //var format = item.Flags.HasFlag(TextureFlags.Missing) ? "Invalid texture" : "{0} x {1}";
            //info.Text = string.Format(format, item.Width, item.Height);
        }
    }
}
