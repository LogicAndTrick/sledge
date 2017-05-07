using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.Common.Mediator;

namespace Sledge.BspEditor.Tools.Texture
{
    public partial class TextureBrowser : Form
    {
        private readonly MapDocument _document;

        public TextureBrowser(MapDocument document)
        {
            _document = document;
            var sz = GetMemory("SizeMode", 1);
            var so = GetMemory("SortBy", 0);

            InitializeComponent();
            TextureList.TextureSelected += TextureSelected;
            TextureList.SelectionChanged += SelectionChanged;
            SizeCombo.SelectedIndex = 1;
            _textures = new List<string>();
            SelectedTexture = null;

            SortOrderCombo.Items.Clear();
            SortOrderCombo.Items.Add("Name");
            SortOrderCombo.Enabled = false;
            SortOrderCombo.SelectedIndex = 0;

            FilterTextbox.Text = GetMemory("Filter", "");
            UsedTexturesOnlyBox.Checked = GetMemory("UsedTexturesOnly", false);
            SizeCombo.SelectedIndex = sz;
            SortOrderCombo.SelectedIndex = so;
            SortDescendingCheckbox.Checked = GetMemory("SortDescending", false);

            SelectionChanged(null, TextureList.GetSelectedTextures());
        }

        public async Task Initialise()
        {
            TextureList.Collection = await _document.Environment.GetTextureCollection();

            _textures.Clear();
            _textures.AddRange(TextureList.Collection.GetAllTextures());

            TextureList.SetTextureList(_textures);
            TextureList.SortTextureList(x => x, GetMemory("SortDescending", false));
            UpdatePackageList();
        }

        protected override void OnLoad(EventArgs e)
        {
            FilterTextbox.SelectAll();
            base.OnLoad(e);
        }

        private void SelectionChanged(object sender, IEnumerable<string> selection)
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
                //todo TextureNameLabel.Text = t.Name;
                // todo TextureSizeLabel.Text = t.Width + " x " + t.Height;
            }
            else
            {
                TextureNameLabel.Text = list.Count + " textures selected";
                TextureSizeLabel.Text = "";
            }
        }

        public string SelectedTexture { get; set; }
        private readonly List<string> _textures;

        private void SetMemory<T>(string name, T value)
        {
            name = GetType().Name + '.' + name;
            // todo
        }

        private T GetMemory<T>(string name, T def = default(T))
        {
            name = GetType().Name + '.' + name;
            return def;
            // todo return _document != null ? _document.GetMemory(name, def) : def;
        }

        private void TextureSelected(object sender, string item)
        {
            SelectedTexture = item;
            DialogResult = DialogResult.OK;
            Close();
        }

        public void SetTextureList(IEnumerable<string> items)
        {
            _textures.Clear();
            _textures.AddRange(items);

            UpdatePackageList();
            UpdateFavouritesList();
            UpdateTextureList();
        }

        public void SetSelectedTextures(IEnumerable<string> items)
        {
            TextureList.SetSelectedTextures(items);
        }

        public void SetFilterText(string text)
        {
            if (text != null) FilterTextbox.Text = text;
        }

        private void FilterTextboxKeyUp(object sender, KeyEventArgs e)
        {
            SetMemory("Filter", FilterTextbox.Text);
            UpdateTextureList();
        }

        private void SelectedPackageChanged(object sender, TreeViewEventArgs e)
        {
            FavouritesTree.SelectedNode = null;
            var package = PackageTree.SelectedNode;
            var key = package?.Name;
            if (String.IsNullOrWhiteSpace(key)) key = null;
            SetMemory("SelectedPackage", key);
            SetMemory("SelectedFavourite", (string) null);

            UpdateTextureList();
        }

        private void SelectedFavouriteChanged(object sender, TreeViewEventArgs e)
        {
            PackageTree.SelectedNode = null;
            var favourite = FavouritesTree.SelectedNode;
            var key = favourite == null ? null : favourite.Name;
            if (String.IsNullOrWhiteSpace(key)) key = null;
            SetMemory("SelectedFavourite", key);
            SetMemory("SelectedPackage", (string)null);

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
            var packages = TextureList.Collection.Packages;
            PackageTree.Nodes.Clear();
            var parent = PackageTree.Nodes.Add("", "All Packages");
            TreeNode reselect = null;
            foreach (var tp in packages.OrderBy(x => x.ToString()))
            {
                var node = parent.Nodes.Add(tp.ToString(), tp + " (" + tp.Textures.Count + ")");
                if (selectedKey == node.Name) reselect = node;
            }
            PackageTree.SelectedNode = reselect;
            PackageTree.ExpandAll();
        }

        private void UpdateFavouritesList()
        {
            //var selected = FavouritesTree.SelectedNode;
            //var selectedKey = selected == null ? GetMemory<string>("SelectedFavourite") : selected.Name;
            //var favourites = SettingsManager.FavouriteTextureFolders;
            //FavouritesTree.Nodes.Clear();
            //var parent = FavouritesTree.Nodes.Add("", "All Favourites");
            //TreeNode reselect;
            //AddFavouriteTextureFolders(parent, favourites, selectedKey, out reselect);
            //FavouritesTree.SelectedNode = reselect;
            //FavouritesTree.ExpandAll();
        }

        //private void AddFavouriteTextureFolders(TreeNode parent, IEnumerable<FavouriteTextureFolder> folders, string selectedKey, out TreeNode reselect)
        //{
        //    reselect = null;
        //    foreach (var fav in folders)
        //    {
        //        var items = GetTexturesInFavourite(fav);
        //        var node = parent.Nodes.Add(parent.Tag + "/" + fav.Name, fav.Name + " (" + items.Count + ")");
        //        AddFavouriteTextureFolders(node, fav.Children, selectedKey, out reselect);
        //        if (selectedKey == node.Name) reselect = node;
        //        node.Tag = fav;
        //    }
        //}

        //private List<string> GetTexturesInFavourite(FavouriteTextureFolder fav)
        //{
        //    return _textures.Where(x => InFavouriteList(fav.Items, x)).ToList();
        //}

        private IEnumerable<string> GetPackageTextures()
        {
            var package = PackageTree.SelectedNode;
            var key = package?.Name;
            if (String.IsNullOrWhiteSpace(key)) key = null;
            var p = TextureList.Collection.Packages.FirstOrDefault(x => x.ToString() == key);
            var set = new HashSet<string>(_textures);
            if (p != null) set.IntersectWith(p.Textures);
            return set;
        }

        private IEnumerable<string> GetFavouriteFolderTextures()
        {
            var folder = FavouritesTree.SelectedNode;
            return _textures;
            //var node = folder == null ? null : folder.Tag as FavouriteTextureFolder;
            //var nodes = new List<FavouriteTextureFolder>();
            //CollectNodes(nodes, node == null ? SettingsManager.FavouriteTextureFolders : node.Children);
            //if (node != null) nodes.Add(node);
            //var favs = nodes.SelectMany(x => x.Items).ToList();
            //return _textures.Where(x => InFavouriteList(favs, x));
        }

        //private bool InFavouriteList(IEnumerable<string> favs, string ti)
        //{
        //    return favs.Contains(ti, StringComparer.InvariantCultureIgnoreCase);
        //}

        //private void CollectNodes(List<FavouriteTextureFolder> favs, IEnumerable<FavouriteTextureFolder> folders)
        //{
        //    foreach (var f in folders)
        //    {
        //        favs.Add(f);
        //        CollectNodes(favs, f.Children);
        //    }
        //} 

        private async Task UpdateTextureList()
        {
            var list = FavouritesTree.SelectedNode != null ? GetFavouriteFolderTextures() : GetPackageTextures();
            if (!String.IsNullOrEmpty(FilterTextbox.Text))
            {
                list = list.Where(x => x.ToLower().Contains(FilterTextbox.Text.ToLower()));
            }
            if (UsedTexturesOnlyBox.Checked && _document != null)
            {
                // todo
                //var used = _document.GetUsedTextures().ToList();
                //list = list.Where(x => used.Any(y => String.Equals(x, y, StringComparison.InvariantCultureIgnoreCase)));
            }
            var l = list.ToList();
            await TextureList.SetTextureList(l);

            //var sel = _document?.TextureCollection.SelectedTexture;
            //if (sel != null)
            //{
            //    TextureList.SetSelectedTextures(new[] { sel });
            //    TextureList.ScrollToItem(sel);
            //}
        }

        private void SizeValueChanged(object sender, EventArgs e)
        {
            SetMemory("SizeMode", SizeCombo.SelectedIndex);
            TextureList.ImageSize = Convert.ToInt32(SizeCombo.SelectedItem);
        }

        private static readonly char[] AllowedSpecialChars = "!@#$%^&*()-_=+<>,.?/'\"\\;:[]{}`~".ToCharArray();

        private void TextureBrowserKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!TextureList.Focused) return;

            if (e.KeyChar == 8 && FilterTextbox.Text.Length > 0)
            {
                if (FilterTextbox.SelectionLength > 0)
                {
                    FilterTextbox.Text = FilterTextbox.Text.Substring(0, FilterTextbox.SelectionStart) +
                                         FilterTextbox.Text.Substring(FilterTextbox.SelectionStart + FilterTextbox.SelectionLength);
                }
                else
                {
                    FilterTextbox.Text = FilterTextbox.Text.Substring(0, FilterTextbox.Text.Length - 1);
                }
                FilterTextboxKeyUp(null, null);
            }
            else if ((e.KeyChar >= 'a' && e.KeyChar <= 'z')
                || (e.KeyChar >= '0' && e.KeyChar <= '9')
                || AllowedSpecialChars.Contains(e.KeyChar))
            {
                if (FilterTextbox.SelectionLength > 0)
                {

                    FilterTextbox.Text = FilterTextbox.Text.Substring(0, FilterTextbox.SelectionStart) +
                                         e.KeyChar +
                                         FilterTextbox.Text.Substring(FilterTextbox.SelectionStart + FilterTextbox.SelectionLength);
                }
                else
                {
                    FilterTextbox.Text += e.KeyChar;
                }
                FilterTextboxKeyUp(null, null);
            }
        }

        private void TextureBrowserKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && FilterTextbox.SelectionLength > 0)
            {
                FilterTextbox.Text = FilterTextbox.Text.Substring(0, FilterTextbox.SelectionStart) +
                                         FilterTextbox.Text.Substring(FilterTextbox.SelectionStart + FilterTextbox.SelectionLength);
                FilterTextboxKeyUp(null, null);
            }
        }

        private void SortOrderComboIndexChanged(object sender, EventArgs e)
        {

        }

        private void SortDescendingCheckboxChanged(object sender, EventArgs e)
        {
            SetMemory("SortDescending", SortDescendingCheckbox.Checked);
            TextureList.SortTextureList(x => x, SortDescendingCheckbox.Checked);
        }

        private void DeleteFavouriteFolderButtonClicked(object sender, EventArgs e)
        {
            //FavouriteTextureFolder parent = null;
            //var selected = FavouritesTree.SelectedNode;
            //if (selected != null && selected.Parent != null)
            //{
            //    parent = selected.Parent.Tag as FavouriteTextureFolder;
            //    var siblings = parent != null ? parent.Children : SettingsManager.FavouriteTextureFolders;
            //    siblings.Remove(selected.Tag as FavouriteTextureFolder);
            //    UpdateFavouritesList();
            //    UpdateTextureList();
            //}
        }

        private void AddFavouriteFolderButtonClicked(object sender, EventArgs e)
        {
            //FavouriteTextureFolder parent = null;
            //var selected = FavouritesTree.SelectedNode;
            //if (selected != null) parent = selected.Tag as FavouriteTextureFolder;
            //var siblings = parent != null ? parent.Children : SettingsManager.FavouriteTextureFolders;
            //using (var qf = new QuickForm("Enter Folder Name") { UseShortcutKeys = true}.TextBox("Name").OkCancel())
            //{
            //    if (qf.ShowDialog() != DialogResult.OK) return;

            //    var name = qf.String("Name");
            //    var uniqName = name;
            //    if (String.IsNullOrWhiteSpace(name)) return;

            //    var counter = 1;
            //    while (siblings.Any(x => x.Name == uniqName))
            //    {
            //        uniqName = name + "_" + counter;
            //        counter++;
            //    }

            //    siblings.Add(new FavouriteTextureFolder { Name = uniqName });
            //    UpdateFavouritesList();
            //}
        }

        private TreeNode _highlightedNode;

        private void FavouritesTreeDragEnter(object sender, DragEventArgs e)
        {
            //if (!e.Data.GetDataPresent(typeof(TextureItem)) && !e.Data.GetDataPresent(typeof(List<TextureItem>))) return;

            //var pt = FavouritesTree.PointToClient(new Point(e.X, e.Y));
            //var highlightedNode = FavouritesTree.GetNodeAt(pt);
            //if (highlightedNode == null || !(highlightedNode.Tag is FavouriteTextureFolder)) return;

            //_highlightedNode = highlightedNode;
            //_highlightedNode.BackColor = Color.LightSkyBlue;
            //e.Effect = DragDropEffects.Copy;
        }

        private void FavouritesTreeDragDrop(object sender, DragEventArgs e)
        {
            //if (e.Data.GetDataPresent(typeof(TextureItem)) || e.Data.GetDataPresent(typeof(List<TextureItem>)))
            //{
            //    var pt = FavouritesTree.PointToClient(new Point(e.X, e.Y));
            //    var dest = FavouritesTree.GetNodeAt(pt);
            //    if (dest != null && dest.Tag is FavouriteTextureFolder)
            //    {
            //        var data = e.Data.GetDataPresent(typeof (TextureItem))
            //            ? new List<TextureItem> {(TextureItem) e.Data.GetData(typeof (TextureItem))}
            //            : (List<TextureItem>)e.Data.GetData(typeof(List<TextureItem>));
            //        var folder = (FavouriteTextureFolder) dest.Tag;
            //        foreach (var ti in data)
            //        {
            //            if (!folder.Items.Contains(ti.Name, StringComparer.InvariantCultureIgnoreCase)) folder.Items.Add(ti.Name);
            //        }
            //        UpdateFavouritesList();
            //    }
            //}
            //if (_highlightedNode != null) _highlightedNode.BackColor = Color.Transparent;
            //_highlightedNode = null;
        }

        private void FavouritesTreeDragLeave(object sender, EventArgs e)
        {
            if (_highlightedNode != null) _highlightedNode.BackColor = Color.Transparent;
            _highlightedNode = null;
        }

        private void FavouritesTreeDragOver(object sender, DragEventArgs e)
        {
            //if (!e.Data.GetDataPresent(typeof (TextureItem)) && !e.Data.GetDataPresent(typeof (List<TextureItem>))) return;

            //var pt = FavouritesTree.PointToClient(new Point(e.X, e.Y));
            //var highlightedNode = FavouritesTree.GetNodeAt(pt);
            //if (highlightedNode == null || !(highlightedNode.Tag is FavouriteTextureFolder))
            //{
            //    if (_highlightedNode != null) _highlightedNode.BackColor = Color.Transparent;
            //    _highlightedNode = null;
            //    e.Effect = DragDropEffects.None;
            //    return;
            //}

            //if (_highlightedNode != null) _highlightedNode.BackColor = Color.Transparent;
            //_highlightedNode = highlightedNode;
            //_highlightedNode.BackColor = Color.LightSkyBlue;
            //e.Effect = DragDropEffects.Copy;
        }

        private void RemoveFavouriteItemButtonClicked(object sender, EventArgs e)
        {
            //var selection = TextureList.GetSelectedTextures().Select(x => x);

            //var folder = FavouritesTree.SelectedNode;
            //var node = folder == null ? null : folder.Tag as FavouriteTextureFolder;
            //var nodes = new List<FavouriteTextureFolder>();
            //CollectNodes(nodes, node == null ? SettingsManager.FavouriteTextureFolders : node.Children);
            //if (node != null) nodes.Add(node);

            //nodes.ForEach(x => x.Items.RemoveAll(selection.Contains));
            //UpdateFavouritesList();
            //UpdateTextureList();
        }

        private void SelectButtonClicked(object sender, EventArgs e)
        {
            var sel = TextureList.GetSelectedTextures().ToList();
            if (!sel.Any()) return;
            // todo Mediator.Publish(EditorMediator.SelectMatchingTextures, sel.Select(x => x).ToList());
            Close();
        }
    }
}
