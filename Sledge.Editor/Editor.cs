using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;
using Sledge.Common.Mediator;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Brushes;
using Sledge.Editor.Documents;
using Sledge.Editor.Menu;
using Sledge.Editor.Settings;
using Sledge.Editor.UI;
using Sledge.Providers;
using Sledge.Providers.GameData;
using Sledge.Providers.Map;
using Sledge.Editor.Tools;
using Sledge.Providers.Texture;
using Sledge.Settings;
using Sledge.Settings.Models;
using Hotkeys = Sledge.Editor.UI.Hotkeys;

namespace Sledge.Editor
{
    public partial class Editor : Form, IMediatorListener
    {
        private JumpList _jumpList;
        public static Editor Instance { get; private set; }

        public bool CaptureAltPresses { get; set; }

        public Editor()
        {
            InitializeComponent();
            Instance = this;
        }

        public void SelectTool(BaseTool t)
        {
            ToolManager.Activate(t);
        }

        private static void LoadFile(string fileName)
        {
            using (var gsd = new GameSelectionForm())
            {
                gsd.ShowDialog();
                if (gsd.SelectedGameID < 0) return;
                var game = SettingsManager.Games.Single(g => g.ID == gsd.SelectedGameID);
                try
                {
                    var map = MapProvider.GetMapFromFile(fileName);
                    DocumentManager.AddAndSwitch(new Document(fileName, map, game));
                }
                catch (ProviderException e)
                {
                    Error.Warning("The map file could not be opened:\n" + e.Message);
                }
            }
        }

        private void EditorLoad(object sender, EventArgs e)
        {
            SettingsManager.Read();

            if (TaskbarManager.IsPlatformSupported)
            {
                TaskbarManager.Instance.ApplicationId = Elevation.ProgramId;
                _jumpList = JumpList.CreateJumpList();
                _jumpList.KnownCategoryToDisplay = JumpListKnownCategoryType.Recent;
                _jumpList.Refresh();
            }

            UpdateRecentFiles();

            MenuManager.Init(mnuMain, tscToolStrip);
            MenuManager.Rebuild();

            ViewportManager.Init(tblQuadView);
            ToolManager.Init();
            BrushManager.Init();
            BrushManager.SetBrushControl(BrushCreatePanel);

            foreach (var tool in ToolManager.Tools)
            {
                var tl = tool;
                tspTools.Items.Add(new ToolStripButton(
                    "",
                    tl.GetIcon(),
                    (s, ea) => SelectTool(tl),
                    tl.GetName())
                        {
                            Checked = (tl == ToolManager.ActiveTool),
                            ToolTipText = tl.GetName()
                        }
                    );
            }

            MapProvider.Register(new RmfProvider());
            MapProvider.Register(new MapFormatProvider());
            MapProvider.Register(new VmfProvider());
            GameDataProvider.Register(new FgdProvider());
            TextureProvider.Register(new WadProvider());
            TextureProvider.Register(new SprProvider());

            // Sprites are loaded on startup and always retained
            var spritesFolder = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Sprites");
            var collection = TextureProvider.CreateCollection(new[] { spritesFolder });
            collection.LoadTextureItems(collection.GetAllItems());

            Subscribe();

            Mediator.MediatorException += (msg, ex) => Logging.Logger.ShowException(ex, "Mediator Error: " + msg);
        }

        #region Updates

        private void CheckForUpdates()
        {
            #if DEBUG
                return;
            #endif

            var sources = GetUpdateSources();
            var version = GetCurrentVersion();
            foreach (var source in sources)
            {
                var result = GetUpdateCheckResult(source, version);
                if (result == null) continue;
                if (!String.Equals(result.Version, version, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (MessageBox.Show("An update is available, would you like to update now?", "New version detected!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Process.Start(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(Editor).Assembly.Location), "Sledge.Editor.Updater.exe"));
                        Application.Exit();
                    }
                }
                return;
            }
        }

        private class UpdateSource
        {
            public string Name { get; set; }
            public string Url { get; set; }

            public string GetUrl(string version)
            {
                return String.Format(Url, version);
            }
        }

        private class UpdateCheckResult
        {
            public string Version { get; set; }
            public DateTime Date { get; set; }
            public string DownloadUrl { get; set; }
        }

        private IEnumerable<UpdateSource> GetUpdateSources()
        {
            var dir = System.IO.Path.GetDirectoryName(typeof(Editor).Assembly.Location);
            if (dir == null) yield break;
            var file = System.IO.Path.Combine(dir, "UpdateSources.txt");
            if (!File.Exists(file)) yield break;
            var lines = File.ReadAllLines(file);
            foreach (var line in lines)
            {
                if (String.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
                var split = line.Split(':');
                if (split.Length < 2) continue;
                var us = new UpdateSource
                {
                    Name = split[0],
                    Url = String.Join(":", split.Skip(1))
                };
                yield return us;
            }
        }

        private String GetCurrentVersion()
        {
            var info = FileVersionInfo.GetVersionInfo(typeof(Editor).Assembly.Location);
            return info.FileVersion;
        }

        private UpdateCheckResult GetUpdateCheckResult(UpdateSource source, string version)
        {
            try
            {
                using (var downloader = new WebClient())
                {
                    var str = downloader.DownloadString(source.GetUrl(version)).Split('\n', '\r');
                    if (str.Length < 3 || String.IsNullOrWhiteSpace(str[0]))
                    {
                        return null;
                    }
                    return new UpdateCheckResult { Version = str[0], Date = DateTime.Parse(str[1]), DownloadUrl = str[2] };
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion

        private void EditorClosing(object sender, FormClosingEventArgs e)
        {
            if (DocumentManager.CurrentDocument != null)
            {
                if (DocumentManager.CurrentDocument.History.TotalActionsSinceLastSave > 0)
                {
                    var result = MessageBox.Show("Would you like to save your changes to this map?", "Changes Detected", MessageBoxButtons.YesNoCancel);
                    if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }
                    if (result == DialogResult.Yes) Mediator.Publish(HotkeysMediator.FileSave);
                }
                DocumentManager.Remove(DocumentManager.CurrentDocument);
                DocumentManager.SwitchTo(null);
            }
        }

        #region Mediator

        private void Subscribe()
        {
            Mediator.Subscribe(HotkeysMediator.FourViewAutosize, this);
            Mediator.Subscribe(HotkeysMediator.FourViewFocusBottomLeft, this);
            Mediator.Subscribe(HotkeysMediator.FourViewFocusBottomRight, this);
            Mediator.Subscribe(HotkeysMediator.FourViewFocusTopLeft, this);
            Mediator.Subscribe(HotkeysMediator.FourViewFocusTopRight, this);
            Mediator.Subscribe(HotkeysMediator.FourViewFocusCurrent, this);

            Mediator.Subscribe(HotkeysMediator.FileNew, this);
            Mediator.Subscribe(HotkeysMediator.FileOpen, this);

            Mediator.Subscribe(EditorMediator.FileOpened, this);
            Mediator.Subscribe(EditorMediator.FileSaved, this);

            Mediator.Subscribe(EditorMediator.Exit, this);

            Mediator.Subscribe(EditorMediator.OpenSettings, this);
            Mediator.Subscribe(EditorMediator.SettingsChanged, this);

            Mediator.Subscribe(EditorMediator.DocumentActivated, this);
            Mediator.Subscribe(EditorMediator.DocumentClosed, this);

            Mediator.Subscribe(EditorMediator.MouseCoordinatesChanged, this);
            Mediator.Subscribe(EditorMediator.SelectionBoxChanged, this);
            Mediator.Subscribe(EditorMediator.SelectionChanged, this);
            Mediator.Subscribe(EditorMediator.ViewZoomChanged, this);
            Mediator.Subscribe(EditorMediator.ViewFocused, this);
            Mediator.Subscribe(EditorMediator.ViewUnfocused, this);
            Mediator.Subscribe(EditorMediator.DocumentGridSpacingChanged, this);

            Mediator.Subscribe(EditorMediator.TextureSelected, this);
            Mediator.Subscribe(EditorMediator.ToolSelected, this);
            Mediator.Subscribe(EditorMediator.ResetSelectedBrushType, this);

            Mediator.Subscribe(EditorMediator.OpenWebsite, this);
            Mediator.Subscribe(EditorMediator.CheckForUpdates, this);
            Mediator.Subscribe(EditorMediator.About, this);
        }

        private void OpenWebsite(string url)
        {
            Process.Start(url);
        }

        private void About()
        {
            using (var ad = new AboutDialog())
            {
                ad.ShowDialog();
            }
        }

        public static void FileNew()
        {
            using (var gsd = new GameSelectionForm())
            {
                gsd.ShowDialog();
                if (gsd.SelectedGameID < 0) return;
                var game = SettingsManager.Games.Single(g => g.ID == gsd.SelectedGameID);
                DocumentManager.AddAndSwitch(new Document(null, new Map(), game));
            }
        }

        private static void FileOpen()
        {
            using (var ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() != DialogResult.OK) return;
                LoadFile(ofd.FileName);
            }
        }

        private static void OpenSettings()
        {
            using (var sf = new SettingsForm())
            {
                sf.ShowDialog();
            }
        }

        private static void SettingsChanged()
        {
            foreach (var vp in ViewportManager.Viewports.OfType<Sledge.UI.Viewport3D>())
            {
                vp.Camera.FOV = Sledge.Settings.View.CameraFOV;
                vp.Camera.ClipDistance = Sledge.Settings.View.BackClippingPane;
            }
            ViewportManager.RefreshClearColour();
        }

        private void Exit()
        {
            Close();
        }

        private void DocumentActivated(Document doc)
        {
            // Textures
            var index = TextureGroupComboBox.SelectedIndex;
            TextureGroupComboBox.Items.Clear();
            TextureGroupComboBox.Items.Add("All Textures");
            foreach (var package in doc.TextureCollection.Packages)
            {
                TextureGroupComboBox.Items.Add(package);
            }
            if (index < 0 || index >= TextureGroupComboBox.Items.Count) index = 0;
            TextureGroupComboBox.SelectedIndex = index;
            TextureSelected(TextureComboBox.GetSelectedTexture());

            // Entities
            var selEnt = EntityTypeList.SelectedItem;
            var def = doc.Game.DefaultPointEntity;
            EntityTypeList.Items.Clear();
            foreach (var gdo in doc.GameData.Classes.Where(x => x.ClassType == ClassType.Point))
            {
                EntityTypeList.Items.Add(gdo);
                if (selEnt == null && gdo.Name == def) selEnt = gdo;
            }
            if (selEnt == null) selEnt = doc.GameData.Classes
                .Where(x => x.ClassType == ClassType.Point)
                .OrderBy(x => x.Name.StartsWith("info_player_start") ? 0 : 1)
                .FirstOrDefault();
            EntityTypeList.SelectedItem = selEnt;

            // Status bar
            StatusSelectionLabel.Text = "";
            StatusCoordinatesLabel.Text = "";
            StatusBoxLabel.Text = "";
            StatusZoomLabel.Text = "";
            StatusSnapLabel.Text = "";
            StatusTextLabel.Text = "";

            SelectionChanged();
            DocumentGridSpacingChanged(doc.Map.GridSpacing);

            Text = "Sledge - " + (String.IsNullOrWhiteSpace(doc.MapFile) ? "Untitled" : System.IO.Path.GetFileName(doc.MapFile));
        }

        private void DocumentClosed()
        {
            TextureGroupComboBox.Items.Clear();
            TextureComboBox.Items.Clear();
            EntityTypeList.Items.Clear();
            VisgroupToolbarPanel.Clear();
            TextureSelected(null);

            StatusSelectionLabel.Text = "";
            StatusCoordinatesLabel.Text = "";
            StatusBoxLabel.Text = "";
            StatusZoomLabel.Text = "";
            StatusSnapLabel.Text = "";
            StatusTextLabel.Text = "";
            Text = "Sledge";
        }

        private void MouseCoordinatesChanged(Coordinate coord)
        {
            if (DocumentManager.CurrentDocument != null)
            {
                coord = DocumentManager.CurrentDocument.Snap(coord);
            }
            StatusCoordinatesLabel.Text = coord.X.ToString("0") + " " + coord.Y.ToString("0") + " " + coord.Z.ToString("0");
        }

        private void SelectionBoxChanged(Box box)
        {
            if (box == null || box.IsEmpty()) StatusBoxLabel.Text = "";
            else StatusBoxLabel.Text = box.Width.ToString("0") + " x " + box.Length.ToString("0") + " x " + box.Height.ToString("0");
        }

        private void SelectionChanged()
        {
            StatusSelectionLabel.Text = "";
            if (DocumentManager.CurrentDocument == null) return;

            var sel  = DocumentManager.CurrentDocument.Selection.GetSelectedParents().ToList();
            var count = sel.Count;
            if (count == 0)
            {
                StatusSelectionLabel.Text = "No objects selected";
            }
            else if (count == 1)
            {
                StatusSelectionLabel.Text = sel[0].GetType().Name;
            }
            else
            {
                StatusSelectionLabel.Text = count.ToString(CultureInfo.InvariantCulture) + " objects selected";
            }
        }

        private void ViewZoomChanged(decimal zoom)
        {
            StatusZoomLabel.Text = "Zoom: " + zoom.ToString("0.00");
        }

        private void ViewFocused()
        {

        }

        private void ViewUnfocused()
        {
            StatusCoordinatesLabel.Text = "";
            StatusZoomLabel.Text = "";
        }

        private void DocumentGridSpacingChanged(decimal spacing)
        {
            StatusSnapLabel.Text = "Grid: " + spacing.ToString("0.##");
        }

        private void TextureSelected(TextureItem selection)
        {
            var dis = TextureSelectionPictureBox.Image;
            TextureSelectionPictureBox.Image = null;
            if (dis != null) dis.Dispose();
            TextureSizeLabel.Text = "";
            if (selection == null || DocumentManager.CurrentDocument == null) return;
            TextureComboBox.SetSelectedTexture(selection);
            using (var tp = DocumentManager.CurrentDocument.TextureCollection.GetStreamSource())
            {
                var bmp = tp.GetImage(selection);
                if (bmp.Width > TextureSelectionPictureBox.Width || bmp.Height > TextureSelectionPictureBox.Height)
                    TextureSelectionPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                else
                    TextureSelectionPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
                TextureSelectionPictureBox.Image = bmp;
            }
            TextureSizeLabel.Text = string.Format("{0} x {1}", selection.Width, selection.Height);
        }

        private void TextureGroupSelected(object sender, EventArgs e)
        {
            var tp = TextureGroupComboBox.SelectedItem as TexturePackage;
            TextureComboBox.Update(tp == null ? null : tp.PackageFile);
        }

        private void TextureSelectionChanged(object sender, EventArgs e)
        {
            Mediator.Publish(EditorMediator.TextureSelected, TextureComboBox.GetSelectedTexture());
        }

        public TextureItem GetSelectedTexture()
        {
            return TextureComboBox.GetSelectedTexture();
        }

        public GameDataObject GetSelectedEntity()
        {
            return (GameDataObject) EntityTypeList.SelectedItem;
        }

        public void ToolSelected()
        {
            var at = ToolManager.ActiveTool;
            if (at == null) return;
            foreach (var tsb in from object item in tspTools.Items select ((ToolStripButton)item))
            {
                tsb.Checked = (tsb.Name == at.GetName());
            }
        }

        public void ResetSelectedBrushType()
        {
            if (BrushTypeList.Items.Count > 0)
            {
                BrushTypeList.SelectedIndex = 0;
            }
        }

        public void FileOpened(string path)
        {
            RecentFile(path);
        }

        public void FileSaved(string path)
        {
            RecentFile(path);
        }

        public void FourViewAutosize()
        {
            tblQuadView.ResetViews();
        }

        public void FourViewFocusTopLeft()
        {
            tblQuadView.FocusOn(0, 0);
        }

        public void FourViewFocusTopRight()
        {
            tblQuadView.FocusOn(0, 1);
        }

        public void FourViewFocusBottomLeft()
        {
            tblQuadView.FocusOn(1, 0);
        }

        public void FourViewFocusBottomRight()
        {
            tblQuadView.FocusOn(1, 1);
        }

        public void FourViewFocusCurrent()
        {
            if (tblQuadView.IsFocusing())
            {
                tblQuadView.Unfocus();
            }
            else
            {
                var focused = ViewportManager.Viewports.FirstOrDefault(x => x.IsFocused);
                if (focused != null)
                {
                    tblQuadView.FocusOn(focused);
                }
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            // Suppress presses of the alt key if required
            if (CaptureAltPresses && (keyData & Keys.Alt) == Keys.Alt)
            {
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return Hotkeys.HotkeyDown(keyData) || base.ProcessCmdKey(ref msg, keyData);
        }

        public void Notify(string message, object data)
        {
            Mediator.ExecuteDefault(this, message, data);
        }

        #endregion

        private void RecentFile(string path)
        {
            if (TaskbarManager.IsPlatformSupported)
            {
                Elevation.RegisterFileType(System.IO.Path.GetExtension(path));
                JumpList.AddToRecent(path);
                _jumpList.Refresh();
            }
            var recents = SettingsManager.RecentFiles.OrderBy(x => x.Order).Where(x => x.Location != path).Take(9).ToList();
            recents.Insert(0, new RecentFile { Location = path});
            for (var i = 0; i < recents.Count; i++)
            {
                recents[i].Order = i;
            }
            SettingsManager.RecentFiles.Clear();
            SettingsManager.RecentFiles.AddRange(recents);
            SettingsManager.Write();
            UpdateRecentFiles();
        }

        private void UpdateRecentFiles()
        {
            var recents = SettingsManager.RecentFiles;
            MenuManager.RecentFiles.Clear();
            MenuManager.RecentFiles.AddRange(recents);
            MenuManager.Rebuild();
        }

        private void TextureBrowseButtonClicked(object sender, EventArgs e)
        {
            if (DocumentManager.CurrentDocument == null) return;
            using (var tb = new TextureBrowser())
            {
                tb.SetTextureList(DocumentManager.CurrentDocument.TextureCollection.GetAllItems());
                tb.ShowDialog();
                if (tb.SelectedTexture != null)
                {
                    Mediator.Publish(EditorMediator.TextureSelected, tb.SelectedTexture);
                }
            }
        }

        private void TextureReplaceButtonClicked(object sender, EventArgs e)
        {
            Mediator.Publish(HotkeysMediator.ReplaceTextures);
        }

        private void MoveToWorldClicked(object sender, EventArgs e)
        {
            Mediator.Publish(HotkeysMediator.TieToWorld);
        }

        private void MoveToEntityClicked(object sender, EventArgs e)
        {
            Mediator.Publish(HotkeysMediator.TieToEntity);
        }

        private void EditorShown(object sender, EventArgs e)
        {
            System.Threading.Tasks.Task.Factory.StartNew(CheckForUpdates);
        }
    }
}
