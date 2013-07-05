using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.Common;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Actions.MapObjects.Selection;
using Sledge.Editor.Documents;
using Sledge.Providers.Texture;

namespace Sledge.Editor.UI
{
    public partial class TextureReplaceDialog : Form
    {
        public TextureReplaceDialog(Document document)
        {
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

            if (Editor.Instance != null && Editor.Instance.GetSelectedTexture() != null)
            {
                var tex = Editor.Instance.GetSelectedTexture();
                Find.Text = tex.Name;
            }
        }

        private IEnumerable<MapObject> GetObjects(Document doc)
        {
            if (ReplaceSelection.Checked) return doc.Selection.GetSelectedObjects();
            if (ReplaceVisible.Checked) return doc.Map.WorldSpawn.Find(x => !x.IsVisgroupHidden);
            return doc.Map.WorldSpawn.FindAll();
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

        private IEnumerable<Tuple<string, TextureItem, ITexture>> GetReplacements(IEnumerable<string> names)
        {
            var list = new List<Tuple<string, TextureItem, ITexture>>();
            var substitute = ActionSubstitute.Checked;
            var find = Find.Text.ToLowerInvariant();
            var replace = Replace.Text.ToLowerInvariant();

            foreach (var name in names.Select(x => x.ToLowerInvariant()).Distinct())
            {
                var n = substitute ? name.Replace(find, replace) : replace;

                var item = TexturePackage.GetLoadedItems().FirstOrDefault(x => x.Name.ToLowerInvariant().Equals(n));
                if (item == null) continue;
                
                list.Add(Tuple.Create(name, item, item.GetTexture()));
            }
            return list;
        }

        public IAction GetAction(Document document)
        {
            var faces = GetObjects(document).OfType<Solid>().SelectMany(x => x.Faces).Where(x => MatchTextureName(x.Texture.Name)).ToList();
            if (ActionSelect.Checked)
            {
                return new ChangeSelection(faces.Select(x => x.Parent).Distinct(), document.Selection.GetSelectedObjects());
            }
            var rescale = RescaleTextures.Checked;
            var replacements = GetReplacements(faces.Select(x => x.Texture.Name));
            Action<Document, Face> action = (doc, face) =>
                                                {
                                                    var repl = replacements.FirstOrDefault(x => x.Item1 == face.Texture.Name.ToLowerInvariant());
                                                    if (repl == null) return;
                                                    if (rescale)
                                                    {
                                                        var item = TexturePackage.GetLoadedItems().FirstOrDefault(x => x.Name.ToLowerInvariant().Equals(face.Texture.Name.ToLowerInvariant()));
                                                        if (item != null)
                                                        {
                                                            face.Texture.XScale *= item.Width / (decimal)repl.Item2.Width;
                                                            face.Texture.YScale *= item.Height / (decimal)repl.Item2.Height;
                                                        }
                                                    }
                                                    face.Texture.Name = repl.Item2.Name;
                                                    face.Texture.Texture = repl.Item3;
                                                    face.CalculateTextureCoordinates();
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
            using (var tb = new TextureBrowser())
            {
                tb.SetTextureList(TexturePackage.GetLoadedItems());
                tb.ShowDialog();
                if (tb.SelectedTexture != null)
                {
                    box.Text = tb.SelectedTexture.Name;
                }
            }
        }

        private void UpdateTexture(string text, PictureBox image, Label info)
        {
            var item = TexturePackage.GetLoadedItems().FirstOrDefault(x => String.Equals(x.Name, text, StringComparison.InvariantCultureIgnoreCase));
            if (item == null)
            {
                image.Image = null;
                info.Text = "No Image";
                return;
            }

            using (var tp = TextureProvider.GetStreamSourceForPackages(new[] { item.Package }))
            {
                var bmp = tp.GetImage(item);
                image.SizeMode = bmp.Width > image.Width || bmp.Height > image.Height
                                     ? PictureBoxSizeMode.Zoom
                                     : PictureBoxSizeMode.CenterImage;
                image.Image = bmp;
            }

            info.Text = string.Format("{0} x {1}", item.Width, item.Height);
        }
    }
}
