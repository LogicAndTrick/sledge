using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Providers.Texture;

namespace Sledge.Editor.UI
{
    public partial class TextureBrowser : Form
    {
        public TextureBrowser()
        {
            var sz = GetMemory("SizeMode", 2);
            var so = GetMemory("SortBy", 0);

            InitializeComponent();
            TextureList.TextureSelected += TextureSelected;
            TextureList.SelectionChanged += SelectionChanged;
            SizeCombo.SelectedIndex = 2;
            _textures = new List<TextureItem>();
            _packages = new List<TexturePackage>();
            SelectedTexture = null;

            SortOrderCombo.Items.Clear();
            foreach (var tso in Enum.GetValues(typeof(TextureListPanel.TextureSortOrder)))
            {
                SortOrderCombo.Items.Add(tso);
            }
            SortOrderCombo.SelectedIndex = 0;

            FilterTextbox.Text = GetMemory("Filter", "");
            UsedTexturesOnlyBox.Checked = GetMemory("UsedTexturesOnly", false);
            SizeCombo.SelectedIndex = sz;
            SortOrderCombo.SelectedIndex = so;
            SortDescendingCheckbox.Checked = GetMemory("SortDescending", false);

            SelectionChanged(null, TextureList.GetSelectedTextures());
        }

        private void SelectionChanged(object sender, IEnumerable<TextureItem> selection)
        {
            var list = selection.ToList();
            if (!list.Any())
            {
                TextureNameLabel.Text = "";
                TextureSizeLabel.Text = "";
            }
            else if (list.Count == 1)
            {
                var t = list[0];
                TextureNameLabel.Text = t.Name;
                TextureSizeLabel.Text = t.Width + " x " + t.Height;
            }
            else
            {
                TextureNameLabel.Text = list.Count + " textures selected";
                TextureSizeLabel.Text = "";
            }
        }

        public TextureItem SelectedTexture { get; set; }
        private readonly List<TextureItem> _textures;
        private readonly List<TexturePackage> _packages;

        private void SetMemory<T>(string name, T value)
        {
            name = GetType().Name + '.' + name;
            if (DocumentManager.CurrentDocument != null) DocumentManager.CurrentDocument.SetMemory(name, value);
        }

        private T GetMemory<T>(string name, T def = default(T))
        {
            name = GetType().Name + '.' + name;
            return DocumentManager.CurrentDocument != null ? DocumentManager.CurrentDocument.GetMemory(name, def) : def;
        }

        private void TextureSelected(object sender, TextureItem item)
        {
            SelectedTexture = item;
            Close();
        }

        public void SetTextureList(IEnumerable<TextureItem> items)
        {
            _textures.Clear();
            _textures.AddRange(items);
            _packages.Clear();
            _packages.AddRange(_textures.Select(x => x.Package).Distinct());
            UpdatePackageList();
            UpdateTextureList();
        }

        public void SetSelectedTextures(IEnumerable<TextureItem> items)
        {
            TextureList.SetSelectedTextures(items);
        }

        private void FilterTextboxKeyUp(object sender, KeyEventArgs e)
        {
            SetMemory("Filter", FilterTextbox.Text);
            UpdateTextureList();
        }

        private void SelectedPackageChanged(object sender, TreeViewEventArgs e)
        {
            var package = PackageTree.SelectedNode;
            var key = package == null ? null : package.Name;
            if (String.IsNullOrWhiteSpace(key)) key = null;
            SetMemory("SelectedPackage", key);

            UpdateTextureList();
        }

        private void UsedTexturesOnlyChanged(object sender, EventArgs e)
        {
            SetMemory("UsedTexturesOnly", UsedTexturesOnlyBox.Checked);
            UpdateTextureList();
        }

        private void UpdatePackageList()
        {
            var selected = PackageTree.SelectedNode;
            var selectedKey = selected == null ? GetMemory<string>("SelectedPackage") : selected.Name;
            var packages = _textures.Select(x => x.Package).Distinct();
            PackageTree.Nodes.Clear();
            var parent = PackageTree.Nodes.Add("", "All Packages");
            var reselect = parent;
            foreach (var tp in packages)
            {
                var node = parent.Nodes.Add(tp.PackageFile, tp.ToString());
                if (selectedKey == node.Name) reselect = node;
            }
            PackageTree.SelectedNode = reselect;
            PackageTree.ExpandAll();
        }

        private IEnumerable<TextureItem> GetPackageTextures()
        {
            var package = PackageTree.SelectedNode;
            var key = package == null ? null : package.Name;
            if (String.IsNullOrWhiteSpace(key)) key = null;
            return _textures.Where(x => key == null || key == x.Package.PackageFile);
        }

        private void UpdateTextureList()
        {
            var list = GetPackageTextures();
            if (!String.IsNullOrEmpty(FilterTextbox.Text))
            {
                list = list.Where(x => x.Name.ToLower().Contains(FilterTextbox.Text.ToLower()));
            }
            if (UsedTexturesOnlyBox.Checked && DocumentManager.CurrentDocument != null)
            {
                var used = DocumentManager.CurrentDocument.Map.WorldSpawn.Find(x => x is Solid).OfType<Solid>()
                    .SelectMany(x => x.Faces).Select(x => x.Texture.Name).Distinct().ToList();
                list = list.Where(x => used.Any(y => String.Equals(x.Name, y, StringComparison.InvariantCultureIgnoreCase)));
            }
            var l = list.ToList();
            TextureList.SetTextureList(l);

            var sel = DocumentManager.CurrentDocument == null ? null : DocumentManager.CurrentDocument.TextureCollection.SelectedTexture;
            if (sel != null)
            {
                TextureList.SetSelectedTextures(new [] { sel });
                TextureList.ScrollToItem(sel);
            }
        }

        private void SizeValueChanged(object sender, EventArgs e)
        {
            SetMemory("SizeMode", SizeCombo.SelectedIndex);
            TextureList.ImageSize = SizeCombo.SelectedIndex == 0 ? 0 : Convert.ToInt32(SizeCombo.SelectedItem);
        }

        private static readonly char[] AllowedSpecialChars = "!@#$%^&*()-_=+<>,.?/'\"\\;:[]{}`~".ToCharArray();

        private void TextureBrowserKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!TextureList.Focused) return;

            if (e.KeyChar == 8 && FilterTextbox.Text.Length > 0)
            {
                FilterTextbox.Text = FilterTextbox.Text.Substring(0, FilterTextbox.Text.Length - 1);
                UpdateTextureList();
            }
            else if ((e.KeyChar >= 'a' && e.KeyChar <= 'z')
                || (e.KeyChar >= '0' && e.KeyChar <= '9')
                || AllowedSpecialChars.Contains(e.KeyChar))
            {
                FilterTextbox.Text += e.KeyChar;
                UpdateTextureList();
            }
        }

        private void SortOrderComboIndexChanged(object sender, EventArgs e)
        {
            SetMemory("SortBy", SortOrderCombo.SelectedIndex);
            TextureList.SortOrder = (TextureListPanel.TextureSortOrder) SortOrderCombo.SelectedItem;
        }

        private void SortDescendingCheckboxChanged(object sender, EventArgs e)
        {
            SetMemory("SortDescending", SortDescendingCheckbox.Checked);
            TextureList.SortDescending = SortDescendingCheckbox.Checked;
        }
    }
}
