using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.Providers.Texture;

namespace Sledge.Editor.UI
{
    public partial class TextureBrowser : Form
    {
        public TextureBrowser()
        {
            InitializeComponent();
            TextureList.TextureSelected += TextureSelected;
            SizeCombo.SelectedIndex = 1;
            _textures = new List<TextureItem>();
            _packages = new List<TexturePackage>();
            SelectedTexture = null;
        }

        public TextureItem SelectedTexture { get; set; }
        private readonly List<TextureItem> _textures;
        private readonly List<TexturePackage> _packages;

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
            UpdateTextureList();
        }

        private void SelectedPackageChanged(object sender, TreeViewEventArgs e)
        {
            UpdateTextureList();
        }

        private void UpdatePackageList()
        {
            var selected = PackageTree.SelectedNode;
            var selectedKey = selected == null ? null : selected.Name;
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
            TextureList.SetTextureList(list);
        }

        private void SizeValueChanged(object sender, EventArgs e)
        {
            TextureList.ImageSize = Convert.ToInt32(SizeCombo.SelectedItem);
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
    }
}
