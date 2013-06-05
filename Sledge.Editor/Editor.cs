using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;
using Sledge.Common.Mediator;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.MapObjects;
using Sledge.Database.Models;
using Sledge.Editor.Brushes;
using Sledge.Editor.Documents;
using Sledge.Editor.Menu;
using Sledge.Editor.Settings;
using Sledge.Editor.UI;
using Sledge.Editor.Visgroups;
using Sledge.Graphics.Helpers;
using Sledge.Providers;
using Sledge.Providers.GameData;
using Sledge.Providers.Map;
using Sledge.Database;
using Sledge.Editor.Tools;
using Sledge.Providers.Texture;
using Sledge.Settings;
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
            tsbNew.Click += (sender, e) => Mediator.Publish(HotkeysMediator.FileNew);
            tsbOpen.Click += (sender, e) => Mediator.Publish(HotkeysMediator.FileOpen);
            tsbSave.Click += (sender, e) => Mediator.Publish(HotkeysMediator.FileSave);
            Instance = this;
        }

        public void SelectTool(BaseTool t)
        {
            ToolManager.Activate(t);
            var at = ToolManager.ActiveTool;
            if (at == null) return;
            foreach (var tsb in from object item in tspTools.Items select ((ToolStripButton) item))
            {
                tsb.Checked = (tsb.Name == at.GetName());
            }
        }

        private static void LoadFile(string fileName)
        {
            using (var gsd = new GameSelectionForm())
            {
                gsd.ShowDialog();
                if (gsd.SelectedGameID < 0) return;
                var game = Context.DBContext.GetAllGames().Single(g => g.ID == gsd.SelectedGameID);
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
            if (TaskbarManager.IsPlatformSupported)
            {
                TaskbarManager.Instance.ApplicationId = Elevation.ProgramId;
                _jumpList = JumpList.CreateJumpList();
                _jumpList.KnownCategoryToDisplay = JumpListKnownCategoryType.Recent;
                _jumpList.Refresh();
            }

            UpdateRecentFiles();

            MenuManager.Init(mnuMain);
            MenuManager.Rebuild();

            ViewportManager.Init(tblQuadView);
            ToolManager.Init();
            BrushManager.Init();
            BrushManager.SetBrushControl(BrushCreatePanel);
            VisgroupManager.SetVisgroupPanel(VisgroupsPanel);

            foreach (var tool in ToolManager.Tools)
            {
                var tl = tool;
                tspTools.Items.Add(new ToolStripButton(
                    "",
                    tl.GetIcon(),
                    (s, ea) => SelectTool(tl),
                    tl.GetName())
                        {
                            Checked = (tl == ToolManager.ActiveTool)
                        }
                    );
            }

            MapProvider.Register(new RmfProvider());
            MapProvider.Register(new MapFormatProvider());
            MapProvider.Register(new VmfProvider());
            GameDataProvider.Register(new FgdProvider());
            TextureProvider.Register(new WadProvider());
            TextureProvider.Register(new SprProvider());

            var spritesFolder = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Sprites");
            //TexturePackage.Load(spritesFolder);
            //TexturePackage.LoadTextureData(TexturePackage.GetLoadedItems().Select(x => x.Name));

            Subscribe();
        }

        #region Mediator

        private void Subscribe()
        {
            Mediator.Subscribe(HotkeysMediator.FourViewAutosize, this);
            Mediator.Subscribe(HotkeysMediator.FourViewFocusBottomLeft, this);
            Mediator.Subscribe(HotkeysMediator.FourViewFocusBottomRight, this);
            Mediator.Subscribe(HotkeysMediator.FourViewFocusTopLeft, this);
            Mediator.Subscribe(HotkeysMediator.FourViewFocusTopRight, this);

            Mediator.Subscribe(HotkeysMediator.FileNew, this);
            Mediator.Subscribe(HotkeysMediator.FileOpen, this);

            Mediator.Subscribe(EditorMediator.FileOpened, this);
            Mediator.Subscribe(EditorMediator.FileSaved, this);

            Mediator.Subscribe(EditorMediator.Exit, this);

            Mediator.Subscribe(EditorMediator.OpenSettings, this);

            Mediator.Subscribe(EditorMediator.DocumentActivated, this);

            Mediator.Subscribe(EditorMediator.TextureSelected, this);
        }

        public static void FileNew()
        {
            using (var gsd = new GameSelectionForm())
            {
                gsd.ShowDialog();
                if (gsd.SelectedGameID < 0) return;
                var game = Context.DBContext.GetAllGames().Single(g => g.ID == gsd.SelectedGameID);
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
            foreach (var package in TexturePackage.GetLoadedPackages())
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
                .OrderBy(x => x.Name.StartsWith("info") ? 0 : 1)
                .FirstOrDefault();
            EntityTypeList.SelectedItem = selEnt;
        }

        private void TextureSelected(TextureItem selection)
        {
            TextureComboBox.SetSelectedTexture(selection);
            var dis = TextureSelectionPictureBox.Image;
            TextureSelectionPictureBox.Image = null;
            if (dis != null) dis.Dispose();
            TextureSizeLabel.Text = "";
            if (selection == null) return;
            using (var tp = TextureProvider.GetStreamSourceForPackages(new[] {selection.Package}))
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
            var recents = Context.DBContext.GetAllRecentFiles().OrderBy(x => x.Order).Where(x => x.Location != path).Take(9).ToList();
            recents.Insert(0, new RecentFile { Location = path});
            for (var i = 0; i < recents.Count; i++)
            {
                recents[i].Order = i;
            }
            Context.DBContext.SaveAllRecentFiles(recents);
            UpdateRecentFiles();
        }

        private void UpdateRecentFiles()
        {
            var recents = Context.DBContext.GetAllRecentFiles();
            MenuManager.RecentFiles.Clear();
            MenuManager.RecentFiles.AddRange(recents);
            MenuManager.Rebuild();
            //var exitPosition = MenuFile.DropDownItems.IndexOf(MenuFileExit) - 1;
            //while (MenuFile.DropDownItems[exitPosition] != MenuFileBottomSep)
            //{
            //    MenuFile.DropDownItems.RemoveAt(exitPosition--);
            //}
            //if (recents.Any())
            //{
            //    exitPosition++;
            //    foreach (var rf in recents.OrderBy(x => x.Order))
            //    {
            //        var loc = rf.Location;
            //        var mi = new ToolStripMenuItem(System.IO.Path.GetFileName(loc));
            //        mi.Click += (sender, e) => LoadFile(loc);
            //        mi.ToolTipText = loc;
            //        MenuFile.DropDownItems.Insert(exitPosition++, mi);
            //    }
            //    MenuFile.DropDownItems.Insert(exitPosition, new ToolStripSeparator());
            //}
        }

        private void TextureBrowseButtonClicked(object sender, EventArgs e)
        {
            using (var tb = new TextureBrowser())
            {
                tb.SetTextureList(TexturePackage.GetLoadedItems());
                tb.ShowDialog();
                if (tb.SelectedTexture != null)
                {
                    Mediator.Publish(EditorMediator.TextureSelected, tb.SelectedTexture);
                }
            }
        }

        private void MoveToWorldClicked(object sender, EventArgs e)
        {
            Mediator.Publish(HotkeysMediator.TieToWorld);
        }

        private void MoveToEntityClicked(object sender, EventArgs e)
        {
            Mediator.Publish(HotkeysMediator.TieToEntity);
        }
    }
}
