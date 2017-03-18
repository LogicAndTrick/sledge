using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using LogicAndTrick.Gimme;
using Microsoft.WindowsAPICodePack.Taskbar;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Brushes;
using Sledge.Editor.Compiling;
using Sledge.Editor.Documents;
using Sledge.Editor.Menu;
using Sledge.Editor.Rendering;
using Sledge.Editor.Settings;
using Sledge.Editor.UI;
using Sledge.Editor.UI.Sidebar;
using Sledge.Providers;
using Sledge.Providers.GameData;
using Sledge.Providers.Map;
using Sledge.Editor.Tools;
using Sledge.Providers.Model;
using Sledge.Providers.Texture;
using Sledge.Providers.Texture.Wad;
using Sledge.Rendering.Cameras;
using Sledge.Settings;
using Sledge.Settings.Models;
using LayoutSettings = Sledge.Editor.UI.Layout.LayoutSettings;
using Path = System.IO.Path;

namespace Sledge.Editor
{
    public partial class Editor : HotkeyForm, IMediatorListener
    {
        private JumpList _jumpList;
        public static Editor Instance { get; private set; }

        public bool CaptureAltPresses { get; set; }

        public Editor()
        {
            PreventSimpleHotkeyPassthrough = false;
            InitializeComponent();
            Instance = this;
        }

        public void SelectTool(BaseTool t)
        {
            ToolManager.Activate(t);
        }

        public static void ProcessArguments(IEnumerable<string> args)
        {
            foreach (var file in args.Skip(1).Where(File.Exists))
            {
                Mediator.Publish(EditorMediator.LoadFile, file);
            }
        }

        private static void LoadFileGame(string fileName, Game game)
        {
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

        private static void LoadFile(string fileName)
        {
            using (var gsd = new GameSelectionForm())
            {
                gsd.ShowDialog();
                if (gsd.SelectedGameID < 0) return;
                var game = SettingsManager.Games.Single(g => g.ID == gsd.SelectedGameID);
                LoadFileGame(fileName, game);
            }
        }

        private void EditorLoad(object sender, EventArgs e)
        {
            FileTypeRegistration.RegisterFileTypes();

            SettingsManager.Read();

            if (TaskbarManager.IsPlatformSupported)
            {
                TaskbarManager.Instance.ApplicationId = FileTypeRegistration.ProgramId;
                
                _jumpList = JumpList.CreateJumpList();
                _jumpList.KnownCategoryToDisplay = JumpListKnownCategoryType.Recent;
                _jumpList.Refresh();
            }

            UpdateDocumentTabs();
            UpdateRecentFiles();

            DockBottom.Hidden = DockLeft.Hidden = DockRight.Hidden = true;

            MenuManager.Init(mnuMain, tscToolStrip);
            MenuManager.Rebuild();

            BrushManager.Init();
            SidebarManager.Init(RightSidebar);

            ViewportManager.Init(TableSplitView);
            ToolManager.Init();

            foreach (var tool in ToolManager.Tools)
            {
                var tl = tool;
                var hotkey = Sledge.Settings.Hotkeys.GetHotkeyForMessage(HotkeysMediator.SwitchTool, tool.GetHotkeyToolType());
                tspTools.Items.Add(new ToolStripButton(
                    "",
                    tl.GetIcon(),
                    (s, ea) => Mediator.Publish(HotkeysMediator.SwitchTool, tl.GetHotkeyToolType()),
                    tl.GetName())
                        {
                            Checked = (tl == ToolManager.ActiveTool),
                            ToolTipText = tl.GetName() + (hotkey != null ? " (" +hotkey.Hotkey + ")" : ""),
                            DisplayStyle = ToolStripItemDisplayStyle.Image,
                            ImageScaling = ToolStripItemImageScaling.None,
                            AutoSize = false,
                            Width = 36,
                            Height = 36
                        }
                    );
            }

            MapProvider.Register(new RmfProvider());
            MapProvider.Register(new MapFormatProvider());
            MapProvider.Register(new VmfProvider());
            MapProvider.Register(new ObjProvider());
            GameDataProvider.Register(new FgdProvider());

            Gimme.Register(new WadTexturePackageProvider());
            Gimme.Register(new WadTextureItemProvider());
            Gimme.Register(new WadTextureStreamSourceProvider());

            ModelProvider.Register(new MdlProvider());

            Subscribe();

            Mediator.MediatorException += (mthd, ex) => Logging.Logger.ShowException(ex.Exception, "Mediator Error: " + ex.Message);

            if (Sledge.Settings.View.LoadSession)
            {
                foreach (var session in SettingsManager.LoadSession())
                {
                    LoadFileGame(session.Item1, session.Item2);
                }
            }

            ProcessArguments(System.Environment.GetCommandLineArgs());
        }

        #region Updates

        private void CheckForUpdates()
        {
            DoUpdateCheck(true);
        }

        private void DoUpdateCheck(bool notify)
        {
            #if DEBUG
                return;
            #endif

            try
            {
                var version = GetCurrentVersion();
                var result = GetUpdateCheckResult(SledgeWebsiteUpdateSource);
                if (result == null) return;
                if (String.Equals(result.Version, version, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (notify)
                    {
                        NotifyUpdateError("This version of Sledge is currently up-to-date.", "No Updates Found");
                    }
                    return;
                }

                var details = GetLatestReleaseDetails();
                if (!details.Exists)
                {
                    if (notify)
                    {
                        NotifyUpdateError("There was a problem downloading the update details, please try again later.", "Update Error");
                    }
                    return;
                }

                if (InvokeRequired) BeginInvoke(new Action(() => NotifyUpdate(details)));
                else NotifyUpdate(details);
            }
            catch (Exception ex)
            {
                if (notify)
                {
                    NotifyUpdateError("An error occurred during the update: " + ex.Message, "Update Failed!");
                }
            }
        }

        private void NotifyUpdateError(string message, string title)
        {
            if (InvokeRequired) BeginInvoke(new Action(() => MessageBox.Show(message, title)));
            else MessageBox.Show(message, title);
        }

        private void NotifyUpdate(UpdateReleaseDetails details)
        {
            var file = Path.Combine(Path.GetTempPath(), details.FileName);
            using (var dialog = new UpdaterForm(details, file))
            {
                dialog.ShowDialog(this);
                if (dialog.Completed)
                {
                    _updateExecutable = file;
                    Close();
                }
            }
        }

        private string _updateExecutable = null;
        private const string GithubReleasesApiUrl = "https://api.github.com/repos/LogicAndTrick/sledge/releases?page=1&per_page=1";
        private const string SledgeWebsiteUpdateSource = "http://sledge-editor.com/version.txt";

        private UpdateReleaseDetails GetLatestReleaseDetails()
        {
            using (var wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.UserAgent, "LogicAndTrick/Sledge-Editor");
                var str = wc.DownloadString(GithubReleasesApiUrl);
                return new UpdateReleaseDetails(str);
            }
        }

        private class UpdateCheckResult
        {
            public string Version { get; set; }
            public DateTime Date { get; set; }
        }

        private String GetCurrentVersion()
        {
            var info = typeof (Editor).Assembly.GetName().Version;
            return info.ToString();
        }

        private UpdateCheckResult GetUpdateCheckResult(string url)
        {
            try
            {
                using (var downloader = new WebClient())
                {
                    var str = downloader.DownloadString(url).Split('\n', '\r');
                    if (str.Length < 2 || String.IsNullOrWhiteSpace(str[0]))
                    {
                        return null;
                    }
                    return new UpdateCheckResult { Version = str[0], Date = DateTime.Parse(str[1]) };
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion

        private bool PromptForChanges(Document doc)
        {
            if (doc.History.TotalActionsSinceLastSave > 0)
            {
                var result = MessageBox.Show("Would you like to save your changes to " + doc.MapFileName + "?", "Changes Detected", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Cancel)
                {
                    return false;
                }
                if (result == DialogResult.Yes)
                {
                    if (!doc.SaveFile())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void EditorClosing(object sender, FormClosingEventArgs e)
        {
            foreach(var doc in DocumentManager.Documents.ToArray())
            {
                if (!PromptForChanges(doc))
                {
                    e.Cancel = true;
                    return;
                }
            }
            SidebarManager.SaveLayout();
            ViewportManager.SaveLayout();
            SettingsManager.SaveSession(DocumentManager.Documents.Select(x => Tuple.Create(x.MapFile, x.Game)));
            SettingsManager.Write();
            if (_updateExecutable != null && File.Exists(_updateExecutable))
            {
                var loc = Path.GetDirectoryName(typeof (Editor).Assembly.Location);
                Process.Start(_updateExecutable, "/S" + (loc != null ? " /D=" + loc : ""));
            }
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var supported = FileTypeRegistration.GetSupportedExtensions();
                var files = (drgevent.Data.GetData(DataFormats.FileDrop) as IEnumerable<string> ?? new string[0])
                    .Where(x => supported.Any(f => x.EndsWith(f.Extension, StringComparison.InvariantCultureIgnoreCase)))
                    .ToList();
                foreach (var file in files) LoadFile(file);
            }
            base.OnDragDrop(drgevent);
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var supported = FileTypeRegistration.GetSupportedExtensions();
                var files = (drgevent.Data.GetData(DataFormats.FileDrop) as IEnumerable<string> ?? new string[0])
                    .Where(x => supported.Any(f => x.EndsWith(f.Extension, StringComparison.InvariantCultureIgnoreCase)))
                    .ToList();
                drgevent.Effect = files.Any() ? DragDropEffects.Link : DragDropEffects.None;
            }
            base.OnDragEnter(drgevent);
        }

        #region Mediator

        private void Subscribe()
        {
            Mediator.Subscribe(HotkeysMediator.ViewportAutosize, this);
            Mediator.Subscribe(HotkeysMediator.FourViewFocusBottomLeft, this);
            Mediator.Subscribe(HotkeysMediator.FourViewFocusBottomRight, this);
            Mediator.Subscribe(HotkeysMediator.FourViewFocusTopLeft, this);
            Mediator.Subscribe(HotkeysMediator.FourViewFocusTopRight, this);
            Mediator.Subscribe(HotkeysMediator.FourViewFocusCurrent, this);

            Mediator.Subscribe(HotkeysMediator.ScreenshotViewport, this);

            Mediator.Subscribe(HotkeysMediator.FileNew, this);
            Mediator.Subscribe(HotkeysMediator.FileOpen, this);

            Mediator.Subscribe(HotkeysMediator.PreviousTab, this);
            Mediator.Subscribe(HotkeysMediator.NextTab, this);

            Mediator.Subscribe(EditorMediator.FileOpened, this);
            Mediator.Subscribe(EditorMediator.FileSaved, this);

            Mediator.Subscribe(EditorMediator.LoadFile, this);

            Mediator.Subscribe(EditorMediator.Exit, this);

            Mediator.Subscribe(EditorMediator.OpenSettings, this);
            Mediator.Subscribe(EditorMediator.SettingsChanged, this);

            Mediator.Subscribe(EditorMediator.CreateNewLayoutWindow, this);
            Mediator.Subscribe(EditorMediator.OpenLayoutSettings, this);

            Mediator.Subscribe(EditorMediator.DocumentActivated, this);
            Mediator.Subscribe(EditorMediator.DocumentSaved, this);
            Mediator.Subscribe(EditorMediator.DocumentOpened, this);
            Mediator.Subscribe(EditorMediator.DocumentClosed, this);
            Mediator.Subscribe(EditorMediator.DocumentAllClosed, this);
            Mediator.Subscribe(EditorMediator.HistoryChanged, this);

            Mediator.Subscribe(EditorMediator.CompileStarted, this);
            Mediator.Subscribe(EditorMediator.CompileFinished, this);
            Mediator.Subscribe(EditorMediator.CompileFailed, this);

            Mediator.Subscribe(EditorMediator.MouseCoordinatesChanged, this);
            Mediator.Subscribe(EditorMediator.SelectionBoxChanged, this);
            Mediator.Subscribe(EditorMediator.SelectionChanged, this);
            Mediator.Subscribe(EditorMediator.ViewZoomChanged, this);
            Mediator.Subscribe(EditorMediator.ViewFocused, this);
            Mediator.Subscribe(EditorMediator.ViewUnfocused, this);
            Mediator.Subscribe(EditorMediator.DocumentGridSpacingChanged, this);

            Mediator.Subscribe(EditorMediator.ToolSelected, this);

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
                var filter = String.Join("|", FileTypeRegistration.GetSupportedExtensions()
                        .Select(x => x.Description + " (*" + x.Extension + ")|*" + x.Extension));
                var all = FileTypeRegistration.GetSupportedExtensions().Select(x => "*" + x.Extension).ToArray();
                ofd.Filter = "All supported formats (" + String.Join(", ", all) + ")|" + String.Join(";", all) + "|" + filter;

                if (ofd.ShowDialog() != DialogResult.OK) return;
                LoadFile(ofd.FileName);
            }
        }

        private void PreviousTab()
        {
            var count = DocumentTabs.TabCount;
            if (count <= 1) return;
            var sel = DocumentTabs.SelectedIndex;
            var prev = sel - 1;
            if (prev < 0) prev = count - 1;
            DocumentTabs.SelectedIndex = prev;
        }

        private void NextTab()
        {
            var count = DocumentTabs.TabCount;
            if (count <= 1) return;
            var sel = DocumentTabs.SelectedIndex;
            var next = sel + 1;
            if (next >= count) next = 0;
            DocumentTabs.SelectedIndex = next;
        }

        private static void OpenSettings()
        {
            using (var sf = new SettingsForm())
            {
                sf.ShowDialog();
            }
        }

        private static void CreateNewLayoutWindow()
        {
            ViewportManager.CreateNewWindow();
        }

        private static void OpenLayoutSettings()
        {
            using (var dlg = new LayoutSettings(ViewportManager.GetWindowConfigurations()))
            {
                dlg.ShowDialog();
            }
        }

        private static void SettingsChanged()
        {
            foreach (var cam in ViewportManager.Viewports.Select(x => x.Viewport.Camera).OfType<PerspectiveCamera>())
            {
                cam.FOV = Sledge.Settings.View.CameraFOV;
                cam.ClipDistance = Sledge.Settings.View.BackClippingPane;
            }
        }

        private void Exit()
        {
            Close();
        }

        private void DocumentActivated(Document doc)
        {
            // Status bar
            StatusSelectionLabel.Text = "";
            StatusCoordinatesLabel.Text = "";
            StatusBoxLabel.Text = "";
            StatusZoomLabel.Text = "";
            StatusSnapLabel.Text = "";
            StatusTextLabel.Text = "";

            SelectionChanged();
            DocumentGridSpacingChanged(doc.Map.GridSpacing);

            DocumentTabs.SelectedIndex = DocumentManager.Documents.IndexOf(doc);

            UpdateTitle();
        }

        private void UpdateDocumentTabs()
        {
            if (DocumentTabs.TabPages.Count != DocumentManager.Documents.Count)
            {
                DocumentTabs.TabPages.Clear();
                foreach (var doc in DocumentManager.Documents)
                {
                    DocumentTabs.TabPages.Add(doc.MapFileName);
                }
            }
            else
            {
                for (var i = 0; i < DocumentManager.Documents.Count; i++)
                {
                    var doc = DocumentManager.Documents[i];
                    DocumentTabs.TabPages[i].Text = doc.MapFileName + (doc.History.TotalActionsSinceLastSave > 0 ? " *" : "");
                }
            }
            if (DocumentManager.CurrentDocument != null)
            {
                var si = DocumentManager.Documents.IndexOf(DocumentManager.CurrentDocument);
                if (si >= 0 && si != DocumentTabs.SelectedIndex) DocumentTabs.SelectedIndex = si;
            }
        }

        private void HistoryChanged()
        {
            UpdateDocumentTabs();
        }

        private void CompileStarted(Batch batch)
        {
            if (DockBottom.Hidden && Sledge.Settings.View.CompileOpenOutput) DockBottom.Hidden = false;
        }

        private void CompileFinished(Batch batch)
        {
            if (batch.Build.AfterRunGame)
            {
                if (batch.Build.AfterAskBeforeRun)
                {
                    if (MessageBox.Show(
                        "The compile of " + batch.Document.MapFileName + " completed successfully.\n" +
                        "Would you like to run the game now?",
                        "Compile Successful!",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return;
                    }
                    var exe = batch.Game.GetExecutable();
                    if (!File.Exists(exe))
                    {
                        MessageBox.Show(
                            "The location of the game executable is incorrect. Please ensure that the game configuration has been set up correctly.",
                            "Failed to launch!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var flags = String.Format("{0} +map \"{1}\" {2}", batch.Game.GetGameLaunchArgument(), batch.MapFileName, batch.Game.ExecutableParameters);
                    try
                    {
                        Process.Start(exe, flags);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Launching game failed: " + ex.Message, "Failed to launch!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }

        private void CompileFailed(Batch batch)
        {
            if (batch.Build.AfterRunGame && batch.Build.AfterAskBeforeRun)
            {
                MessageBox.Show(
                    "The compile of " + batch.Document.MapFileName + " failed. If any errors were generated, " +
                    "they will appear in the compile output panel.",
                    "Compile Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (DockBottom.Hidden) DockBottom.Hidden = false;
            }
        }

        private void DocumentTabsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_closingDocumentTab) return;
            var si = DocumentTabs.SelectedIndex;
            if (si >= 0 && si < DocumentManager.Documents.Count)
            {
                DocumentManager.SwitchTo(DocumentManager.Documents[si]);
            }
        }

        private bool _closingDocumentTab = false;

        private void DocumentTabsRequestClose(object sender, int index)
        {
            if (index < 0 || index >= DocumentManager.Documents.Count) return;

            var doc = DocumentManager.Documents[index];
            if (!PromptForChanges(doc))
            {
                return;
            }
            _closingDocumentTab = true;
            DocumentManager.Remove(doc);
            _closingDocumentTab = false;
        }

        private void DocumentOpened(Document doc)
        {
            UpdateDocumentTabs();
        }

        private void DocumentSaved(Document doc)
        {
            FileOpened(doc.MapFile);
            UpdateDocumentTabs();
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            if (DocumentManager.CurrentDocument != null)
            {
                var doc = DocumentManager.CurrentDocument;
                Text = "Sledge - " + (String.IsNullOrWhiteSpace(doc.MapFile) ? "Untitled" : System.IO.Path.GetFileName(doc.MapFile));
            }
            else
            {
                Text = "Sledge";
            }
        }

        private void DocumentClosed(Document doc)
        {
            UpdateDocumentTabs();
        }

        private void DocumentAllClosed()
        {
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
                var obj = sel[0];
                var ed = obj.GetEntityData();
                if (ed != null)
                {
                    var name = ed.GetPropertyValue("targetname");
                    StatusSelectionLabel.Text = ed.Name + (String.IsNullOrWhiteSpace(name) ? "" : " - " + name);
                }
                else
                {
                    StatusSelectionLabel.Text = sel[0].GetType().Name;
                }
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

        public void ToolSelected()
        {
            var at = ToolManager.ActiveTool;
            if (at == null) return;
            foreach (var tsb in from object item in tspTools.Items select ((ToolStripButton)item))
            {
                tsb.Checked = (tsb.Name == at.GetName());
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

        public void ViewportAutosize()
        {
            TableSplitView.ResetViews();
        }

        public void FourViewFocusTopLeft()
        {
            TableSplitView.FocusOn(0, 0);
        }

        public void FourViewFocusTopRight()
        {
            TableSplitView.FocusOn(0, 1);
        }

        public void FourViewFocusBottomLeft()
        {
            TableSplitView.FocusOn(1, 0);
        }

        public void FourViewFocusBottomRight()
        {
            TableSplitView.FocusOn(1, 1);
        }

        public void FourViewFocusCurrent()
        {
            if (TableSplitView.IsFocusing())
            {
                TableSplitView.Unfocus();
            }
            else
            {
                var focused = ViewportManager.Viewports.FirstOrDefault(x => x.Viewport.IsFocused);
                if (focused != null)
                {
                    TableSplitView.FocusOn(focused.Control);
                }
            }
        }

        // todo viewport: redo this
        public void ScreenshotViewport(object parameter)
        {
            //var focused = (parameter as MapViewport) ?? ViewportManager.Viewports.FirstOrDefault(x => x.IsFocused);
            //if (focused == null) return;

            //var screen = Screen.FromControl(this);
            //var area = screen.Bounds;

            //using (var qf = new QuickForm("Select screenshot size") {UseShortcutKeys = true}
            //    .NumericUpDown("Width", 640, 5000, 0, area.Width)
            //    .NumericUpDown("Height", 480, 5000, 0, area.Height)
            //    .OkCancel())
            //{
            //    if (qf.ShowDialog() != DialogResult.OK) return;

            //    var shot = ViewportManager.CreateScreenshot(focused, (int) qf.Decimal("Width"), (int) qf.Decimal("Height"));
            //    if (shot == null) return;

            //    var ext = focused is MapViewport || (focused is MapViewport && ((MapViewport)focused).Type != MapViewport.ViewType.Textured) ? ".png" : ".jpg";

            //    using (var sfd = new SaveFileDialog())
            //    {
            //        sfd.FileName = "Sledge - "
            //                       + (DocumentManager.CurrentDocument != null ? DocumentManager.CurrentDocument.MapFileName : "untitled")
            //                       + " - " + DateTime.Now.ToString("yyyy-MM-ddThh-mm-ss") + ext;
            //        sfd.Filter = "Image Files (*.png, *.jpg, *.bmp)|*.png;*.jpg;*.bmp";
            //        if (sfd.ShowDialog() == DialogResult.OK)
            //        {
            //            if (sfd.FileName.EndsWith("jpg"))
            //            {
            //                var encoder = GetJpegEncoder();
            //                if (encoder != null)
            //                {
            //                    var p = new EncoderParameter(Encoder.Quality, 90L);
            //                    var ep = new EncoderParameters(1);
            //                    ep.Param[0] = p;
            //                    shot.Save(sfd.FileName, encoder, ep);
            //                }
            //                else
            //                {
            //                    shot.Save(sfd.FileName);
            //                }
            //            }
            //            else
            //            {
            //                shot.Save(sfd.FileName);
            //            }
            //        }
            //    }
            //    shot.Dispose();
            //}
        }

        private ImageCodecInfo GetJpegEncoder()
        {
            return ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == ImageFormat.Jpeg.Guid);
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

        public void Notify(string message, object data)
        {
            Mediator.ExecuteDefault(this, message, data);
        }

        #endregion

        private void RecentFile(string path)
        {
            if (TaskbarManager.IsPlatformSupported)
            {
                //Elevation.RegisterFileType(System.IO.Path.GetExtension(path));
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
            MenuManager.UpdateRecentFilesMenu();
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
            System.Threading.Tasks.Task.Factory.StartNew(() => DoUpdateCheck(false));
        }
    }
}
