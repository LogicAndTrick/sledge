using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Translations;
using Sledge.Shell;

namespace Sledge.BspEditor.Tools.Texture
{
    public partial class TextureBrowser : Form, IManualTranslate
    {
        private readonly MapDocument _document;
        private TextureListPanel _textureList;

        public TextureBrowser(MapDocument document)
        {
            _document = document;
            InitializeComponent();
            InitialiseTextureList();

            // Setup memory & other controls
            var sz = GetMemory("SizeMode", 1);
            var so = GetMemory("SortBy", 0);

            SortOrderCombo.Items.Clear();
            SortOrderCombo.Items.Add("Name");
            SortOrderCombo.Enabled = false;
            SortOrderCombo.SelectedIndex = 0;

            FilterTextbox.Text = GetMemory("Filter", "");
            UsedTexturesOnlyBox.Checked = GetMemory("UsedTexturesOnly", false);
            SizeCombo.SelectedIndex = sz;
            SortOrderCombo.SelectedIndex = so;
            SortDescendingCheckbox.Checked = GetMemory("SortDescending", false);
            
            _textureList.TextureSelected += TextureSelected;
            _textureList.HighlightedTexturesChanged += HighlightedTexturesChanged;
            SizeCombo.SelectedIndex = 1;
            _textures = new List<string>();
            SelectedTexture = null;

            HighlightedTexturesChanged(null, _textureList.GetHighlightedTextures());
        }

        private void InitialiseTextureList()
        {
            _textureList = new TextureListPanel
            {
                AllowMultipleHighlighting = true,
                AllowHighlighting = true,
                AutoScroll = true,
                BackColor = Color.Black,
                Dock = DockStyle.Fill,
                EnableDrag = true,
                ImageSize = 128,
                Location = new Point(226, 0),
                Name = "_textureList",
                Size = new Size(714, 495),
                TabIndex = 0
            };
            TextureListPanel.Controls.Add(_textureList);
        }

        public void Translate(ITranslationStringProvider strings)
        {
            CreateHandle();
            var prefix = GetType().FullName;
            this.InvokeLater(() =>
            {
                Text = strings.GetString(prefix, "Title");
                FavouriteTexturesLabel.Text = strings.GetString(prefix, "FavouriteTextures");
                AddFavouriteFolderButton.Text = strings.GetString(prefix, "AddFolder");
                DeleteFavouriteFolderButton.Text = strings.GetString(prefix, "DeleteFolder");
                RemoveFavouriteItemButton.Text = strings.GetString(prefix, "RemoveSelected");

                FilterLabel.Text = strings.GetString(prefix, "Filter");
                SizeLabel.Text = strings.GetString(prefix, "Size");

                UsedTexturesOnlyBox.Text = strings.GetString(prefix, "UsedTexturesOnly");
                SelectButton.Text = strings.GetString(prefix, "Select");
                SortByLabel.Text = strings.GetString(prefix, "SortBy");
                SortDescendingCheckbox.Text = strings.GetString(prefix, "SortDescending");
            });
        }

        public async Task Initialise(ITranslationStringProvider translation)
        {
            _textureList.Collection = await _document.Environment.GetTextureCollection();

            _textures.Clear();
            _textures.AddRange(_textureList.Collection.GetBrowsableTextures());
            
            UpdatePackageList();
            UpdateTextureList();
            UpdateFavouritesList();
            _textureList.SortTextureList(x => x, GetMemory("SortDescending", false));

            translation.Translate(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            FilterTextbox.SelectAll();
            base.OnLoad(e);
        }

        private void HighlightedTexturesChanged(object sender, IEnumerable<string> selection)
        {
            TextureNameLabel.Text = "";
            TextureSizeLabel.Text = "";

            var list = selection.ToList();
            
            if (list.Count == 1 && _document != null)
            {
                var t = list[0];
                TextureNameLabel.Text = t;
                _document.Environment.GetTextureCollection().ContinueWith(tc =>
                {
                    tc.Result.GetTextureItem(t).ContinueWith(ti =>
                    {
                        this.InvokeLater(() =>
                        {
                            TextureNameLabel.Text = ti == null ? t : ti.Result.Name;
                            TextureSizeLabel.Text = ti == null ? "" : $@"{ti.Result.Width} x {ti.Result.Height}";
                        });
                    });
                });
            }
            else if (list.Count > 1)
            {
                TextureNameLabel.Text = list.Count + " textures selected";
                TextureSizeLabel.Text = "";
            }
        }

        public string SelectedTexture { get; set; }
        private readonly List<string> _textures;

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
            _textureList.SetHighlightedTextures(items);
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
            var key = favourite?.Name;
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
            var packages = _textureList.Collection.Packages.Where(p => _textures.Any(p.HasTexture));
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

        private IEnumerable<string> GetPackageTextures()
        {
            var package = PackageTree.SelectedNode;
            var key = package?.Name;
            if (String.IsNullOrWhiteSpace(key)) key = null;
            var p = _textureList.Collection.Packages.FirstOrDefault(x => x.ToString() == key);
            var set = new HashSet<string>(_textures);
            if (p != null) set.IntersectWith(p.Textures);
            return set;
        }

        private void UpdateFavouritesList()
        {
            FavouritesTree.Nodes.Clear();
            FavouritesTree.Nodes.Add("", "(Not implemented yet)");
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

        private async Task UpdateTextureList()
        {
            var list = FavouritesTree.SelectedNode != null ? GetFavouriteFolderTextures() : GetPackageTextures();
            if (!String.IsNullOrEmpty(FilterTextbox.Text))
            {
                list = list.Where(x => x.ToLower().Contains(FilterTextbox.Text.ToLower()));
            }
            if (UsedTexturesOnlyBox.Checked && _document != null)
            {
                var textureNames = new HashSet<string>(_document.Map.Root.FindAll().SelectMany(x => x.Data.OfType<ITextured>()).Select(x => x.Texture.Name).Distinct());
                list = list.Where(x => textureNames.Contains(x, StringComparer.InvariantCultureIgnoreCase));
            }
            var l = list.ToList();
            await _textureList.SetTextureList(l);

            var sel = _document?.Map.Data.GetOne<ActiveTexture>()?.Name;
            if (sel != null)
            {
                _textureList.SetHighlightedTextures(new[] { sel });
                _textureList.ScrollToTexture(sel);
            }
        }

        //private List<string> GetTexturesInFavourite(FavouriteTextureFolder fav)
        //{
        //    return _textures.Where(x => InFavouriteList(fav.Items, x)).ToList();
        //}

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

        private void SizeValueChanged(object sender, EventArgs e)
        {
            SetMemory("SizeMode", SizeCombo.SelectedIndex);
            _textureList.ImageSize = Convert.ToInt32(SizeCombo.SelectedItem);
        }

        private static readonly char[] AllowedSpecialChars = "!@#$%^&*()-_=+<>,.?/'\"\\;:[]{}`~".ToCharArray();

        private void TextureBrowserKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!_textureList.Focused) return;

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
            // Nothing, for now
        }

        private void SortDescendingCheckboxChanged(object sender, EventArgs e)
        {
            SetMemory("SortDescending", SortDescendingCheckbox.Checked);
            _textureList.SortTextureList(x => x, SortDescendingCheckbox.Checked);
        }

        private void SelectButtonClicked(object sender, EventArgs e)
        {
            var textures = _textureList.GetHighlightedTextures().ToHashSet(StringComparer.InvariantCultureIgnoreCase);
            if (!textures.Any()) return;

            var sel = _document.Map.Root.Find(x => x.Data.OfType<ITextured>().Any(t => textures.Contains(t.Texture.Name))).ToList();
            var des = _document.Selection.Except(sel).ToList();

            var transaction = new Transaction(new Select(sel), new Deselect(des));
            MapDocumentOperation.Perform(_document, transaction);

            Close();
        }

        // Favourite list management

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
            //var selection = _textureList.GetHighlightedTextures().Select(x => x);

            //var folder = FavouritesTree.SelectedNode;
            //var node = folder == null ? null : folder.Tag as FavouriteTextureFolder;
            //var nodes = new List<FavouriteTextureFolder>();
            //CollectNodes(nodes, node == null ? SettingsManager.FavouriteTextureFolders : node.Children);
            //if (node != null) nodes.Add(node);

            //nodes.ForEach(x => x.Items.RemoveAll(selection.Contains));
            //UpdateFavouritesList();
            //UpdateTextureList();
        }

        // Memory (per environment; transient)

        private static readonly Dictionary<string, Memory> _memory = new Dictionary<string, Memory>();

        private class Memory
        {
            public Dictionary<string, object> Values { get; } = new Dictionary<string, object>();

            public void Set<T>(string name, T value)
            {
                Values[GetType().Name + '.' + name] = value;
            }

            public T Get<T>(string name, T def = default(T))
            {
                if (!Values.TryGetValue(GetType().Name + '.' + name, out var v)) return def;

                try
                {
                    return (T)Convert.ChangeType(v, typeof(T));
                }
                catch
                {
                    return def;
                }

            }
        }

        private void SetMemory<T>(string name, T value)
        {
            var id = _document?.Environment?.ID;
            if (id == null) return;
            if (!_memory.TryGetValue(id, out var m)) _memory[id] = m = new Memory();
            m.Set(name, value);
        }

        private T GetMemory<T>(string name, T def = default(T))
        {
            var id = _document?.Environment?.ID;
            if (id == null) return def;
            if (_memory.TryGetValue(id, out var m)) return m.Get(name, def);
            return def;
        }
    }
}
