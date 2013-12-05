using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.DataStructures.GameData;
using Sledge.Providers.GameData;
using Sledge.QuickForms;
using Sledge.Settings;
using System.Linq;
using Sledge.Settings.Models;

namespace Sledge.Editor.Settings
{
	/// <summary>
	/// Description of SettingsForm.
	/// </summary>
	public partial class SettingsForm : Form
	{
	    private List<Game> _games;
	    private List<Build> _builds;

		public SettingsForm()
		{
            InitializeComponent();
            BindColourPicker(GridBackgroundColour);
            BindColourPicker(GridColour);
            BindColourPicker(GridZeroAxisColour);
            BindColourPicker(GridBoundaryColour);
            BindColourPicker(GridHighlight1Colour);
            BindColourPicker(GridHighlight2Colour);

		    UpdateData();

            tbcSettings.SelectTab(3);
		    BindConfigControls();
		}

        #region Initialisation
	    private void BindConfigControls()
	    {
            // Game Configurations
            SelectedGameName.TextChanged += (s, e) => CheckNull(_selectedGame, x => x.Name = SelectedGameName.Text);
            SelectedGameEngine.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.Engine = (Engine) SelectedGameEngine.SelectedItem);
            SelectedGameBuild.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.BuildID = _builds[SelectedGameBuild.SelectedIndex].ID);
            SelectedGameSteamInstall.CheckedChanged += (s, e) => CheckNull(_selectedGame, x => x.SteamInstall = SelectedGameSteamInstall.Checked);
            SelectedGameWonDir.TextChanged += (s, e) => CheckNull(_selectedGame, x => x.WonGameDir = SelectedGameWonDir.Text);
            SelectedGameSteamDir.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.SteamGameDir = SelectedGameSteamDir.Text);
            SelectedGameMod.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.ModDir = SelectedGameMod.Text);
            SelectedGameBase.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.BaseDir = SelectedGameBase.Text);
            SelectedGameMapDir.TextChanged += (s, e) => CheckNull(_selectedGame, x => x.MapDir = SelectedGameMapDir.Text);
            SelectedGameEnableAutosave.CheckedChanged += (s, e) => CheckNull(_selectedGame, x => x.Autosave = SelectedGameEnableAutosave.Checked);
            SelectedGameUseDiffAutosaveDir.CheckedChanged += (s, e) => CheckNull(_selectedGame, x => x.UseCustomAutosaveDir = SelectedGameUseDiffAutosaveDir.Checked);
            SelectedGameDiffAutosaveDir.TextChanged += (s, e) => CheckNull(_selectedGame, x => x.AutosaveDir = SelectedGameDiffAutosaveDir.Text);
            SelectedGameAutosaveTime.ValueChanged += (s, e) => CheckNull(_selectedGame, x => x.AutosaveTime = (int) SelectedGameAutosaveTime.Value);
            SelectedGameAutosaveLimit.ValueChanged += (s, e) => CheckNull(_selectedGame, x => x.AutosaveLimit = (int)SelectedGameAutosaveLimit.Value);
            SelectedGameAutosaveOnlyOnChange.CheckedChanged += (s, e) => CheckNull(_selectedGame, x => x.AutosaveOnlyOnChanged = SelectedGameAutosaveOnlyOnChange.Checked);
            SelectedGameDefaultPointEnt.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.DefaultPointEntity = SelectedGameDefaultPointEnt.Text);
            SelectedGameDefaultBrushEnt.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.DefaultBrushEntity = SelectedGameDefaultBrushEnt.Text);
            SelectedGameTextureScale.ValueChanged += (s, e) => CheckNull(_selectedGame, x => x.DefaultTextureScale = SelectedGameTextureScale.Value);
            SelectedGameLightmapScale.ValueChanged += (s, e) => CheckNull(_selectedGame, x => x.DefaultLightmapScale = SelectedGameLightmapScale.Value);
            SelectedGameOverrideMapSize.CheckedChanged += (s, e) => CheckNull(_selectedGame, x => x.OverrideMapSize = SelectedGameOverrideMapSize.Checked);
	        var sizes = new[] {4096, 8192, 16384, 32768, 65536};
            SelectedGameOverrideSizeLow.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.OverrideMapSizeLow = SelectedGameOverrideSizeLow.SelectedIndex < 0 ? 0 : -sizes[SelectedGameOverrideSizeLow.SelectedIndex]);
            SelectedGameOverrideSizeHigh.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.OverrideMapSizeHigh = SelectedGameOverrideSizeHigh.SelectedIndex < 0 ? 0 : sizes[SelectedGameOverrideSizeHigh.SelectedIndex]);

            // Build Configurations
            SelectedBuildName.TextChanged += (s, e) => CheckNull(_selectedBuild, x => x.Name = SelectedBuildName.Text);
            SelectedBuildEngine.SelectedIndexChanged += (s, e) => CheckNull(_selectedBuild, x => x.Engine = (Engine) SelectedBuildEngine.SelectedItem);
            SelectedBuildExeFolder.TextChanged += (s, e) => CheckNull(_selectedBuild, x => x.Path = SelectedBuildExeFolder.Text);
            SelectedBuildBsp.SelectedIndexChanged += (s, e) => CheckNull(_selectedBuild, x => x.Bsp = SelectedBuildBsp.Text);
            SelectedBuildCsg.SelectedIndexChanged += (s, e) => CheckNull(_selectedBuild, x => x.Csg = SelectedBuildCsg.Text);
            SelectedBuildVis.SelectedIndexChanged += (s, e) => CheckNull(_selectedBuild, x => x.Vis = SelectedBuildVis.Text);
            SelectedBuildRad.SelectedIndexChanged += (s, e) => CheckNull(_selectedBuild, x => x.Rad = SelectedBuildRad.Text);
            
	    }

	    private void BindColourPicker(Control panel)
        {
            panel.Click += (sender, e) =>
            {
                var p = (Control)sender;
                using (var cpd = new ColorDialog { Color = p.BackColor })
                {
                    if (cpd.ShowDialog() == DialogResult.OK) p.BackColor = cpd.Color;
                }
            };
        }

        private void CheckNull<T>(T obj, Action<T> act) where T : class
        {
            if (obj != null) act(obj);
        }

        #endregion

        #region Data Loading

	    private void UpdateData()
	    {
            _selectedGame = null;
            UpdateSelectedGame();

	        _selectedBuild = null;
            UpdateSelectedBuild();

            _games = new List<Game>(SettingsManager.Games);
            _builds = new List<Build>(SettingsManager.Builds);

            ReIndex();

            SelectedGameEngine.Items.Clear();
            SelectedGameEngine.Items.AddRange(Enum.GetValues(typeof(Engine)).OfType<object>().ToArray());

            SelectedBuildEngine.Items.Clear();
            SelectedBuildEngine.Items.AddRange(Enum.GetValues(typeof(Engine)).OfType<object>().ToArray());

            UpdateGameTree();
            UpdateBuildTree();
            UpdateSteamUsernames();
            SelectedGameUpdateSteamGames();
        }

        private void ReIndex()
        {
            for (var i = 0; i < _games.Count; i++)
            {
                _games[i].ID = i + 1;
                _games[i].BuildID = _builds.FindIndex(x => x.ID == _games[i].BuildID) + 1;
            }

            for (var i = 0; i < _builds.Count; i++)
            {
                _builds[i].ID = i + 1;
            }
        }

	    private void UpdateBuildTree()
	    {
	        BuildTree.Nodes.Clear();
            foreach (Engine engine in Enum.GetValues(typeof(Engine)))
            {
                var node = BuildTree.Nodes.Add(engine.ToString(), engine.ToString());
                var list = _builds.Where(x => x.Engine == engine).ToList();
                foreach (var build in list)
                {
                    var index = _builds.IndexOf(build);
                    node.Nodes.Add(index.ToString(CultureInfo.InvariantCulture), build.Name);
                }
            }
            BuildTree.ExpandAll();
            SelectedGameBuild.Items.Clear();
	        SelectedGameBuild.Items.AddRange(_builds.Select(x => x.Name).ToArray());
        }

	    private void UpdateGameTree()
	    {
            GameTree.Nodes.Clear();
            foreach (Engine engine in Enum.GetValues(typeof(Engine)))
	        {
                var node = GameTree.Nodes.Add(engine.ToString(), engine.ToString());
	            var list = _games.Where(x => x.Engine == engine).ToList();
	            foreach (var game in list)
	            {
	                var index = _games.IndexOf(game);
	                node.Nodes.Add(index.ToString(CultureInfo.InvariantCulture), game.Name);
	            }
	        }
            GameTree.ExpandAll();
	    }

        #endregion

        #region Load/Apply

	    private void SettingsFormLoad(object sender, EventArgs e)
        {
            // General
            SwitchToSelectAfterCreation.Checked = Sledge.Settings.Select.SwitchToSelectAfterCreation;
	        SwitchToSelectAfterEntity.Checked = Sledge.Settings.Select.SwitchToSelectAfterEntity;
            SelectCreatedBrush.Checked = Sledge.Settings.Select.SelectCreatedBrush;
	        SelectCreatedEntity.Checked = Sledge.Settings.Select.SelectCreatedEntity;
	        DeselectOthersWhenSelectingCreation.Checked = Sledge.Settings.Select.DeselectOthersWhenSelectingCreation;
	        ResetBrushTypeOnCreation.Checked = Sledge.Settings.Select.ResetBrushTypeOnCreation;

	        ApplyTextureImmediately.Checked = Sledge.Settings.Select.ApplyTextureImmediately;

            LoadSession.Checked = Sledge.Settings.View.LoadSession;
            KeepCameraPositions.Checked = Sledge.Settings.View.KeepCameraPositions;
            KeepSelectedTool.Checked = Sledge.Settings.View.KeepSelectedTool;
	        KeepViewportSplitterPosition.Checked = Sledge.Settings.View.KeepViewportSplitterPosition;

            RenderMode.SelectedIndex = 0;
            if (Sledge.Settings.View.Renderer == Sledge.Settings.RenderMode.OpenGL1DisplayLists) RenderMode.SelectedIndex = 1;
            if (Sledge.Settings.View.Renderer == Sledge.Settings.RenderMode.OpenGL1Immediate) RenderMode.SelectedIndex = 2;

            DisableWadTransparency.Checked = Sledge.Settings.View.DisableWadTransparency;
            DisableToolTransparency.Checked = Sledge.Settings.View.DisableToolTextureTransparency;
            GloballyDisableTransparency.Checked = Sledge.Settings.View.GloballyDisableTransparency;

            // 2D Views
            CrosshairCursorIn2DViews.Checked = Sledge.Settings.View.CrosshairCursorIn2DViews;
            AutoSelectBox.Checked = Sledge.Settings.Select.AutoSelectBox;
            KeepVisgroupsWhenCloning.Checked = Sledge.Settings.Select.KeepVisgroupsWhenCloning;
	        ScrollWheelZoomMultiplier.Value = Sledge.Settings.View.ScrollWheelZoomMultiplier;
	        SelectionBoxBackgroundOpacity.Value = Sledge.Settings.View.SelectionBoxBackgroundOpacity;

            DrawCenterHandles.Checked = Sledge.Settings.Select.DrawCenterHandles;
	        CenterHandlesActiveViewportOnly.Checked = Sledge.Settings.Select.CenterHandlesActiveViewportOnly;
	        CenterHandlesOnlyNearCursor.Checked = Sledge.Settings.Select.CenterHandlesFollowCursor;
            BoxSelectByHandlesOnly.Checked = Sledge.Settings.Select.BoxSelectByCenterHandlesOnly;
            ClickSelectByHandlesOnly.Checked = Sledge.Settings.Select.ClickSelectByCenterHandlesOnly;

            RotationStyle_SnapOnShift.Checked = Sledge.Settings.Select.RotationStyle == RotationStyle.SnapOnShift;
            RotationStyle_SnapOffShift.Checked = Sledge.Settings.Select.RotationStyle == RotationStyle.SnapOffShift;
            RotationStyle_SnapNever.Checked = Sledge.Settings.Select.RotationStyle == RotationStyle.SnapNever;

            SnapStyle_SnapOffAlt.Checked = Sledge.Settings.Select.SnapStyle == SnapStyle.SnapOffAlt;
            SnapStyle_SnapOnAlt.Checked = Sledge.Settings.Select.SnapStyle == SnapStyle.SnapOnAlt;

            ArrowKeysNudgeSelection.Checked = Sledge.Settings.Select.ArrowKeysNudgeSelection;
            NudgeUnits.Value = Sledge.Settings.Select.NudgeUnits;
            NudgeStyle_GridOffCtrl.Checked = Sledge.Settings.Select.NudgeStyle == NudgeStyle.GridOffCtrl;
            NudgeStyle_GridOnCtrl.Checked = Sledge.Settings.Select.NudgeStyle == NudgeStyle.GridOnCtrl;

            DefaultGridSize.SelectedItem = Grid.DefaultSize;
            HideGridLimit.Value = Grid.HideSmallerThan;
            HideGridOn.Checked = Grid.HideSmallerOn;
            HideGridFactor.SelectedItem = Grid.HideFactor;

            GridBackgroundColour.BackColor = Grid.Background;
            GridColour.BackColor = Grid.GridLines;
            GridZeroAxisColour.BackColor = Grid.ZeroLines;
            GridBoundaryColour.BackColor = Grid.BoundaryLines;
            GridHighlight1Colour.BackColor = Grid.Highlight1;
            GridHighlight2Colour.BackColor = Grid.Highlight2;
            GridHighlight1Distance.Value = Grid.Highlight1LineNum;
            GridHighlight2UnitNum.SelectedItem = Grid.Highlight2UnitNum;
            GridHighlight1On.Checked = Grid.Highlight1On;
            GridHighlight2On.Checked = Grid.Highlight2On;

            // 3D Views
            BackClippingPane.Value = Sledge.Settings.View.BackClippingPane;
            ModelRenderDistance.Value = Sledge.Settings.View.ModelRenderDistance;
            DetailRenderDistance.Value = Sledge.Settings.View.DetailRenderDistance;

            ForwardSpeed.Value = Sledge.Settings.View.ForwardSpeed;
            TimeToTopSpeed.Value = (int) (Sledge.Settings.View.TimeToTopSpeed / 10);
            InvertMouseX.Checked = Sledge.Settings.View.InvertX;
            InvertMouseY.Checked = Sledge.Settings.View.InvertY;

	        CameraFOV.Value = Sledge.Settings.View.CameraFOV;

            // Game Configurations
            // Build Programs
            // Steam
            SteamInstallDir.Text = Steam.SteamDirectory;
            SteamUsername.Text = Steam.SteamUsername;
            UpdateSteamUsernames();

            // Hotkeys
        }

        private void Apply()
        {
            // General
            Sledge.Settings.Select.SwitchToSelectAfterCreation = SwitchToSelectAfterCreation.Checked;
            Sledge.Settings.Select.SwitchToSelectAfterEntity = SwitchToSelectAfterEntity.Checked;
            Sledge.Settings.Select.SelectCreatedBrush = SelectCreatedBrush.Checked;
            Sledge.Settings.Select.SelectCreatedEntity = SelectCreatedEntity.Checked;
            Sledge.Settings.Select.DeselectOthersWhenSelectingCreation = DeselectOthersWhenSelectingCreation.Checked;
            Sledge.Settings.Select.ResetBrushTypeOnCreation = ResetBrushTypeOnCreation.Checked;

            Sledge.Settings.Select.ApplyTextureImmediately = ApplyTextureImmediately.Checked;

            Sledge.Settings.View.LoadSession = LoadSession.Checked;
            Sledge.Settings.View.KeepCameraPositions = KeepCameraPositions.Checked;
            Sledge.Settings.View.KeepSelectedTool = KeepSelectedTool.Checked;
            Sledge.Settings.View.KeepViewportSplitterPosition = KeepViewportSplitterPosition.Checked;

            Sledge.Settings.View.Renderer = Sledge.Settings.RenderMode.OpenGL3;
            if (RenderMode.SelectedIndex == 1) Sledge.Settings.View.Renderer = Sledge.Settings.RenderMode.OpenGL1DisplayLists;
            if (RenderMode.SelectedIndex == 2) Sledge.Settings.View.Renderer = Sledge.Settings.RenderMode.OpenGL1Immediate;

            Sledge.Settings.View.DisableWadTransparency = DisableWadTransparency.Checked;
            Sledge.Settings.View.DisableToolTextureTransparency = DisableToolTransparency.Checked;
            Sledge.Settings.View.GloballyDisableTransparency = GloballyDisableTransparency.Checked;

            // 2D Views
            Sledge.Settings.View.CrosshairCursorIn2DViews = CrosshairCursorIn2DViews.Checked;
            Sledge.Settings.Select.AutoSelectBox = AutoSelectBox.Checked;
            Sledge.Settings.Select.KeepVisgroupsWhenCloning = KeepVisgroupsWhenCloning.Checked;
            Sledge.Settings.View.ScrollWheelZoomMultiplier = ScrollWheelZoomMultiplier.Value;
            Sledge.Settings.View.SelectionBoxBackgroundOpacity = (int) SelectionBoxBackgroundOpacity.Value;

            Sledge.Settings.Select.DrawCenterHandles = DrawCenterHandles.Checked;
            Sledge.Settings.Select.CenterHandlesActiveViewportOnly = CenterHandlesActiveViewportOnly.Checked;
            Sledge.Settings.Select.CenterHandlesFollowCursor = CenterHandlesOnlyNearCursor.Checked;
            Sledge.Settings.Select.BoxSelectByCenterHandlesOnly = BoxSelectByHandlesOnly.Checked;
            Sledge.Settings.Select.ClickSelectByCenterHandlesOnly = ClickSelectByHandlesOnly.Checked;

            if (RotationStyle_SnapOnShift.Checked) Sledge.Settings.Select.RotationStyle = RotationStyle.SnapOnShift;
            if (RotationStyle_SnapOffShift.Checked) Sledge.Settings.Select.RotationStyle = RotationStyle.SnapOffShift;
            if (RotationStyle_SnapNever.Checked) Sledge.Settings.Select.RotationStyle = RotationStyle.SnapNever;

            if (SnapStyle_SnapOffAlt.Checked) Sledge.Settings.Select.SnapStyle = SnapStyle.SnapOffAlt;
            if (SnapStyle_SnapOnAlt.Checked) Sledge.Settings.Select.SnapStyle = SnapStyle.SnapOnAlt;

            Sledge.Settings.Select.ArrowKeysNudgeSelection = ArrowKeysNudgeSelection.Checked;
            Sledge.Settings.Select.NudgeUnits = NudgeUnits.Value;
            if (NudgeStyle_GridOffCtrl.Checked) Sledge.Settings.Select.NudgeStyle = NudgeStyle.GridOffCtrl;
            if (NudgeStyle_GridOnCtrl.Checked) Sledge.Settings.Select.NudgeStyle = NudgeStyle.GridOnCtrl;

            Grid.DefaultSize = int.Parse(Convert.ToString(DefaultGridSize.Text));
            Grid.HideSmallerThan = int.Parse(Convert.ToString(HideGridLimit.Value));
            Grid.HideSmallerOn = HideGridOn.Checked;
            Grid.HideFactor = int.Parse(Convert.ToString(HideGridFactor.Text));

            Grid.Background = GridBackgroundColour.BackColor;
            Grid.GridLines = GridColour.BackColor;
            Grid.ZeroLines = GridZeroAxisColour.BackColor;
            Grid.BoundaryLines = GridBoundaryColour.BackColor;
            Grid.Highlight1 = GridHighlight1Colour.BackColor;
            Grid.Highlight2 = GridHighlight2Colour.BackColor;
            Grid.Highlight1LineNum = (int) GridHighlight1Distance.Value;
            Grid.Highlight2UnitNum = int.Parse(Convert.ToString(GridHighlight2UnitNum.Text));
            Grid.Highlight1On = GridHighlight1On.Checked;
            Grid.Highlight2On = GridHighlight2On.Checked;

            // 3D Views
            Sledge.Settings.View.BackClippingPane = BackClippingPane.Value;
            Sledge.Settings.View.ModelRenderDistance = ModelRenderDistance.Value;
            Sledge.Settings.View.DetailRenderDistance = DetailRenderDistance.Value;

            Sledge.Settings.View.ForwardSpeed = ForwardSpeed.Value;
            Sledge.Settings.View.TimeToTopSpeed = TimeToTopSpeed.Value * 10m;
            Sledge.Settings.View.InvertX = InvertMouseX.Checked;
            Sledge.Settings.View.InvertY = InvertMouseY.Checked;

            Sledge.Settings.View.CameraFOV = (int) CameraFOV.Value;

            // Game Configurations
            // Build Programs
            // Steam
            Steam.SteamDirectory = SteamInstallDir.Text;
            Steam.SteamUsername = SteamUsername.Text;

            // Hotkeys

            // Save settings to database
            ReIndex();
            SettingsManager.Builds.Clear();
            SettingsManager.Builds.AddRange(_builds);
            SettingsManager.Games.Clear();
            SettingsManager.Games.AddRange(_games);

            SettingsManager.Write();

            Mediator.Publish(EditorMediator.SettingsChanged);
        }

        private void Apply(object sender, EventArgs e)
        {
            btnApplySettings.Enabled = false;
            btnApplyAndCloseSettings.Enabled = false;
            btnCancelSettings.Enabled = false;
            Application.DoEvents();
            Apply();
            UpdateData();
            btnApplySettings.Enabled = true;
            btnApplyAndCloseSettings.Enabled = true;
            btnCancelSettings.Enabled = true;
        }

        private void ApplyAndClose(object sender, MouseEventArgs e)
        {
            btnApplySettings.Enabled = false;
            btnApplyAndCloseSettings.Enabled = false;
            btnCancelSettings.Enabled = false;
            Apply();
            Close();
        }

        private void Close(object sender, MouseEventArgs e)
        {
            SettingsManager.Read();
            Close();
        }

        #endregion

        #region Specific Events

        private void TabChanged(object sender, EventArgs e)
        {
            GameTree.SelectedNode = null;
            BuildTree.SelectedNode = null;
            _selectedGame = null;
            _selectedBuild = null;
            UpdateSelectedGame();
            UpdateSelectedBuild();
        }

        private void BackClippingPaneChanged(object sender, EventArgs e)
        {
            BackClippingPaneLabel.Text = BackClippingPane.Value.ToString();
        }

        private void ForwardSpeedChanged(object sender, EventArgs e)
        {
            ForwardSpeedLabel.Text = ForwardSpeed.Value + @" units/sec";
        }

        private void TimeToTopSpeedChanged(object sender, EventArgs e)
        {
            TimeToTopSpeedLabel.Text = TimeToTopSpeed.Value / 10m + @" sec";
        }

        private void SteamInstallDirBrowseClicked(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog { SelectedPath = SteamInstallDir.Text })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    SteamInstallDir.Text = fbd.SelectedPath;
                }
            }
        }

        private void SteamUsernameChanged(object sender, EventArgs e)
        {
            SelectedGameUpdateSteamGames();
        }

	    private void SteamDirectoryChanged(object sender, EventArgs e)
        {
            UpdateSteamUsernames();
            SelectedGameUpdateSteamGames();
        }

        private void UpdateSteamUsernames()
        {
            SteamUsername.Items.Clear();
            var steamdir = Path.Combine(SteamInstallDir.Text, "steamapps");
            if (!Directory.Exists(steamdir)) return;
            var usernames = Directory.GetDirectories(steamdir).Select(Path.GetFileName);
            var ignored = new[] {"common", "downloading", "media", "sourcemods", "temp"};
            SteamUsername.Items.AddRange(usernames.Where(x => !ignored.Contains(x.ToLower())).OfType<object>().ToArray());
            var idx = SteamUsername.Items.IndexOf(SteamUsername.Text);
            if (SteamUsername.Items.Count > 0) SteamUsername.SelectedIndex = Math.Max(0, idx);
        }

        private void RemoveGameClicked(object sender, EventArgs e)
        {
            if (_selectedGame != null)
            {
                _games.Remove(_selectedGame);
                _selectedGame = null;
                UpdateSelectedGame();
                ReIndex();
                UpdateGameTree();
            }
        }

        private void AddGameClicked(object sender, EventArgs e)
        {
            _games.Add(new Game
            {
                ID = 0,
                Engine = Engine.Goldsource,
                Name = "New Game",
                BuildID = _builds.Select(x => x.ID).FirstOrDefault(),
                Autosave = true,
                MapDir = _games.Select(x => x.MapDir).FirstOrDefault() ?? "",
                AutosaveDir = _games.Select(x => x.AutosaveDir).FirstOrDefault() ?? "",
                DefaultLightmapScale = 1,
                DefaultTextureScale = 1,
                Fgds = new List<Fgd>(),
                Wads = new List<Wad>()
            });
            ReIndex();
            UpdateGameTree();
            var node = GameTree.Nodes.OfType<TreeNode>().SelectMany(x => x.Nodes.OfType<TreeNode>())
                .First(x => x.Name == (_games.Count - 1).ToString());
            GameTree.SelectedNode = node;
        }

        private void AddBuildClicked(object sender, EventArgs e)
        {
            _builds.Add(new Build
                            {
                                ID = 0,
                                Engine = Engine.Goldsource,
                                Name = "New Build"
                            });
            ReIndex();
            UpdateBuildTree();
            var node = BuildTree.Nodes.OfType<TreeNode>().SelectMany(x => x.Nodes.OfType<TreeNode>())
                .First(x => x.Name == (_builds.Count - 1).ToString());
            BuildTree.SelectedNode = node;
        }

        private void RemoveBuildClicked(object sender, EventArgs e)
        {
            if (_selectedBuild != null)
            {
                _builds.Remove(_selectedBuild);
                var replacementBuild = _builds.OrderBy(x => x.Engine == _selectedBuild.Engine ? 1 : 2).FirstOrDefault();
                var replace = replacementBuild == null ? 0 : replacementBuild.ID;
                _games.Where(x => x.BuildID == _selectedBuild.ID).ToList().ForEach(x => x.BuildID = replace);
                _selectedBuild = null;
                UpdateSelectedBuild();
                ReIndex();
                UpdateBuildTree();
            }
        }

        #endregion

        #region Selected Game

	    private Game _selectedGame;

        private void GameSelected(object sender, TreeViewEventArgs e)
        {
            var selection = e.Node;
            if (selection == null || selection.Parent == null)
            {
                // No node selected, or selected node is an engine
                _selectedGame = null;
            }
            else
            {
                // Get the selected game by ID
                _selectedGame = _games[int.Parse(selection.Name)];
            }
            UpdateSelectedGame();
        }

        private void UpdateSelectedGame()
        {
            GameSubTabs.Visible = RemoveGame.Enabled = _selectedGame != null;
            if (_selectedGame == null) return;
            SelectedGameName.Text = _selectedGame.Name;
            SelectedGameMapDir.Text = _selectedGame.MapDir;
            SelectedGameEnableAutosave.Checked = _selectedGame.Autosave;
            SelectedGameUseDiffAutosaveDir.Checked = _selectedGame.UseCustomAutosaveDir;
            SelectedGameAutosaveOnlyOnChange.Checked = _selectedGame.AutosaveOnlyOnChanged;
            SelectedGameDiffAutosaveDir.Text = _selectedGame.AutosaveDir;
            SelectedGameDefaultPointEnt.SelectedText = _selectedGame.DefaultPointEntity;
            SelectedGameDefaultBrushEnt.SelectedText = _selectedGame.DefaultBrushEntity;
            SelectedGameTextureScale.Value = _selectedGame.DefaultTextureScale;
            SelectedGameLightmapScale.Value = _selectedGame.DefaultLightmapScale;
            SelectedGameOverrideMapSize.Checked = _selectedGame.OverrideMapSize;
            var sizes = new[] { 4096, 8192, 16384, 32768, 65536 };
            SelectedGameOverrideSizeLow.SelectedIndex = Array.IndexOf(sizes, -_selectedGame.OverrideMapSizeLow);
            SelectedGameOverrideSizeHigh.SelectedIndex = Array.IndexOf(sizes, _selectedGame.OverrideMapSizeHigh);

            SelectedGameSteamInstall.Checked = _selectedGame.SteamInstall;

            SelectedGameMod.SelectedText = _selectedGame.ModDir;
            SelectedGameBase.SelectedText = _selectedGame.BaseDir;
            SelectedGameWonDir.Text = _selectedGame.WonGameDir;
            SelectedGameSteamDir.SelectedText = _selectedGame.SteamGameDir;

            SelectedGameAutosaveLimit.Value = _selectedGame.AutosaveLimit;
            if (_selectedGame.AutosaveLimit >= SelectedGameAutosaveLimit.Minimum && _selectedGame.AutosaveLimit <= SelectedGameAutosaveLimit.Maximum)
            {
                SelectedGameAutosaveLimit.Value = _selectedGame.AutosaveLimit;
            }
            else
            {
                SelectedGameAutosaveLimit.Value = 5;
            }
            if (_selectedGame.AutosaveTime >= SelectedGameAutosaveTime.Minimum && _selectedGame.AutosaveTime <= SelectedGameAutosaveTime.Maximum)
            {
                SelectedGameAutosaveTime.Value = _selectedGame.AutosaveTime;
            }
            else
            {
                SelectedGameAutosaveTime.Value = 5;
            }

            if (SelectedGameBuild.Items.Count > 0)
            {
                SelectedGameBuild.SelectedIndex = Math.Max(0, _builds.FindIndex(x => x.ID == _selectedGame.BuildID));
            }
            if (SelectedGameEngine.Items.Count > 0)
            {
                SelectedGameEngine.SelectedIndex = Math.Max(0, Enum.GetValues(typeof(Engine)).OfType<Engine>().ToList<Engine>().FindIndex(x => x == _selectedGame.Engine));
            }

            SelectedGameOverrideSizeHigh.Enabled = SelectedGameOverrideSizeLow.Enabled = SelectedGameOverrideMapSize.Checked;

            SelectedGameEngineChanged(null, null);
            SelectedGameUpdateSteamGames();
            SelectedGameUpdateFgds();
            SelectedGameUpdateWads();
        }

	    private void SelectedGameUpdateWads()
	    {
            SelectedGameWadList.Items.Clear();
            foreach (var wad in _selectedGame.Wads)
            {
                SelectedGameWadList.Items.Add(wad.Path);
            }
	    }

	    private void SelectedGameUpdateFgds()
	    {
            SelectedGameFgdList.Items.Clear();
	        foreach (var fgd in _selectedGame.Fgds)
	        {
	            SelectedGameFgdList.Items.Add(fgd.Path);
	        }

            SelectedGameDefaultPointEnt.Items.Clear();
            SelectedGameDefaultBrushEnt.Items.Clear();
            SelectedGameDetectedSizeHigh.Text = "";
            SelectedGameDetectedSizeLow.Text = "";

            try
            {
                var gd = GameDataProvider.GetGameDataFromFiles(_selectedGame.Fgds.Select(x => x.Path).Where(File.Exists));

                SelectedGameDefaultPointEnt.Items.AddRange(gd.Classes.Where(x => x.ClassType == ClassType.Point).Select(x => x.Name).OfType<object>().ToArray());
                var idx = SelectedGameDefaultPointEnt.Items.IndexOf(_selectedGame.DefaultPointEntity ?? "");
                if (idx < 0) idx = SelectedGameDefaultPointEnt.Items.IndexOf("info_player_start");
                if (idx < 0) idx = SelectedGameDefaultPointEnt.Items.IndexOf("light");
                if (SelectedGameDefaultPointEnt.Items.Count > 0) SelectedGameDefaultPointEnt.SelectedIndex = Math.Max(0, idx);

                SelectedGameDefaultBrushEnt.Items.AddRange(gd.Classes.Where(x => x.ClassType == ClassType.Solid).Select(x => x.Name).OfType<object>().ToArray());
                idx = SelectedGameDefaultBrushEnt.Items.IndexOf(_selectedGame.DefaultBrushEntity ?? "");
                if (idx < 0) idx = SelectedGameDefaultBrushEnt.Items.IndexOf("func_detail");
                if (idx < 0) idx = SelectedGameDefaultBrushEnt.Items.IndexOf("trigger_once");
                if (SelectedGameDefaultBrushEnt.Items.Count > 0) SelectedGameDefaultBrushEnt.SelectedIndex = Math.Max(0, idx);

                SelectedGameDetectedSizeHigh.Text = gd.MapSizeHigh.ToString(CultureInfo.InvariantCulture);
                SelectedGameDetectedSizeLow.Text = gd.MapSizeLow.ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error loading the provided FGDs, please ensure that you have loaded all dependant FGDs.\nError was: "+ex.Message);
            }
	    }

	    private void SelectedGameEngineChanged(object sender, EventArgs e)
        {
            if (_selectedGame == null || SelectedGameEngine.SelectedIndex < 0) return;
            var eng = (Engine) SelectedGameEngine.SelectedItem;
            var change = eng != _selectedGame.Engine;
            _selectedGame.Engine = eng;
            SelectedGameSteamInstall.Enabled = eng == Engine.Goldsource;
            if (eng == Engine.Goldsource && !SelectedGameSteamInstall.Checked)
            {
                lblGameWONDir.Visible = SelectedGameWonDir.Visible = SelectedGameDirBrowse.Visible = true;
                lblGameSteamDir.Visible = SelectedGameSteamDir.Visible = false;
                SelectedGameWonDir.Enabled = true;
                SelectedGameDirBrowse.Enabled = true;
                SelectedGameSteamDir.Enabled = false;
                SelectedGameWonDirChanged(null, null);
            }
            else
            {
                lblGameWONDir.Visible = SelectedGameWonDir.Visible = SelectedGameDirBrowse.Visible = false;
                lblGameSteamDir.Visible = SelectedGameSteamDir.Visible = true;
                SelectedGameWonDir.Enabled = false;
                SelectedGameDirBrowse.Enabled = false;
                SelectedGameSteamDir.Enabled = true;
                SelectedGameUpdateSteamGames();
            }
            if (change)
            {

                UpdateGameTree();
                var node = GameTree.Nodes.OfType<TreeNode>().SelectMany(x => x.Nodes.OfType<TreeNode>())
                    .First(x => x.Name == _games.IndexOf(_selectedGame).ToString());
                GameTree.SelectedNode = node;
            }
        }

        private void SelectedGameUpdateSteamGames()
        {
            if (_selectedGame == null) return;
            var steamdir = Path.Combine(SteamInstallDir.Text, "steamapps");
            var username = SteamUsername.Text;
            var userdir = Path.Combine(steamdir, username);
            var commondir = Path.Combine(steamdir, "common");
            var games = new List<string>();
            if (Directory.Exists(userdir)) games.AddRange(Directory.GetDirectories(userdir).Select(Path.GetFileName));
            if (Directory.Exists(commondir)) games.AddRange(Directory.GetDirectories(commondir).Select(Path.GetFileName));
            var includeGoldsource = new[]
                              {
                                  "counter-strike", "day of defeat", "deathmatch classic",
                                  "half-life", "half-life blue shift",
                                  "opposing force", "team fortress classic"
                              };
            var includeSource = new[]
                              {
                                  "alien swarm", "counter-strike global offensive",
                                  "counter-strike source", "day ofdefeat source",
                                  "dota 2 beta", "half-life 2", "half-life 2 deathmatch",
                                  "half-life 2episode one", "half-life 2 episode two",
                                  "half-life deathmatch source", "left 4 dead 2", "left 4dead", "lostcoast",
                                  "portal", "portal 2", "team fortress 2"
                              };
            SelectedGameSteamDir.Items.Clear();
            var eng = (Engine) SelectedGameEngine.SelectedItem;
            var include = eng == Engine.Goldsource ? includeGoldsource : includeSource;
            SelectedGameSteamDir.Items.AddRange(games.Where(x => include.Contains(x.ToLower())).Distinct().OrderBy(x => x.ToLower()).ToArray<object>());
            var idx = SelectedGameSteamDir.Items.IndexOf(_selectedGame.SteamGameDir ?? "");
            if (SelectedGameSteamDir.Items.Count > 0) SelectedGameSteamDir.SelectedIndex = Math.Max(0, idx);
        }

        private void SelectedGameWonDirChanged(object sender, EventArgs e)
        {
            if (SelectedGameEngine.SelectedIndex < 0 || SelectedGameSteamInstall.Checked) return;
            var eng = (Engine) SelectedGameEngine.SelectedItem;
            if (eng != Engine.Goldsource || SelectedGameSteamInstall.Checked) return;

            SelectedGameMod.Items.Clear();
            SelectedGameBase.Items.Clear();

            if (!Directory.Exists(SelectedGameWonDir.Text)) return;

            var mods = Directory.GetDirectories(SelectedGameWonDir.Text).Select(Path.GetFileName);
            var ignored = new[] { "gldrv", "logos", "logs", "errorlogs", "platform", "config" };

            var range = mods.Where(x => !ignored.Contains(x.ToLower())).OfType<object>().ToArray();
            SelectedGameMod.Items.AddRange(range);
            SelectedGameBase.Items.AddRange(range);

            var idx = SelectedGameMod.Items.IndexOf(_selectedGame.ModDir ?? "");
            if (SelectedGameMod.Items.Count > 0) SelectedGameMod.SelectedIndex = Math.Max(0, idx);

            idx = SelectedGameBase.Items.IndexOf(_selectedGame.BaseDir ?? "");
            if (SelectedGameBase.Items.Count > 0) SelectedGameBase.SelectedIndex = Math.Max(0, idx);
        }

        private void SelectedGameSteamDirChanged(object sender, EventArgs e)
        {
            if (SelectedGameEngine.SelectedIndex < 0 || !SelectedGameSteamInstall.Checked) return;
            var eng = (Engine) SelectedGameEngine.SelectedItem;
            if (eng == Engine.Goldsource && !SelectedGameSteamInstall.Checked) return;

            SelectedGameMod.Items.Clear();
            SelectedGameBase.Items.Clear();

            var dir = Path.Combine(SteamInstallDir.Text, "steamapps", SteamUsername.Text, SelectedGameSteamDir.Text);
            if (!Directory.Exists(dir)) dir = Path.Combine(SteamInstallDir.Text, "steamapps", "common", SelectedGameSteamDir.Text);
            if (!Directory.Exists(dir)) return;

            var mods = Directory.GetDirectories(dir).Select(Path.GetFileName);
            var ignored = new[] {"gldrv", "logos", "logs", "errorlogs", "platform", "config", "bin"};

            var range = mods.Where(x => !ignored.Contains(x.ToLower())).OfType<object>().ToArray();
            SelectedGameMod.Items.AddRange(range);
            SelectedGameBase.Items.AddRange(range);

            var idx = SelectedGameMod.Items.IndexOf(_selectedGame.ModDir ?? "");
            if (SelectedGameMod.Items.Count > 0) SelectedGameMod.SelectedIndex = Math.Max(0, idx);

            idx = SelectedGameBase.Items.IndexOf(_selectedGame.BaseDir ?? "");
            if (SelectedGameBase.Items.Count > 0) SelectedGameBase.SelectedIndex = Math.Max(0, idx);
        }

        private void SelectedGameNameChanged(object sender, EventArgs e)
        {
            if (_selectedGame == null) return;
            var idx = _games.IndexOf(_selectedGame).ToString(CultureInfo.InvariantCulture);
            var node = GameTree.Nodes.OfType<TreeNode>().SelectMany(x => x.Nodes.OfType<TreeNode>()).First(x => x.Name == idx);
            node.Text = SelectedGameName.Text;
        }

        private void SelectedGameDirBrowseClicked(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog { SelectedPath = SelectedGameWonDir.Text })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    SelectedGameWonDir.Text = fbd.SelectedPath;
                }
            }
        }

        private void SelectedGameMapDirBrowseClicked(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog { SelectedPath = SelectedGameMapDir.Text })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    SelectedGameMapDir.Text = fbd.SelectedPath;
                }
            }
        }

        private void SelectedGameDiffAutosaveDirBrowseClicked(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog { SelectedPath = SelectedGameDiffAutosaveDir.Text })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    SelectedGameDiffAutosaveDir.Text = fbd.SelectedPath;
                }
            }
        }

        private void SelectedGameAddFgdClicked(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "Forge Game Data files (*.fgd)|*.fgd", Multiselect = true })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (var fileName in ofd.FileNames)
                    {
                        _selectedGame.Fgds.Add(new Fgd { Path = fileName });
                    }
                    SelectedGameUpdateFgds();
                }
            }
        }

        private void SelectedGameRemoveFgdClicked(object sender, EventArgs e)
        {
            if (SelectedGameFgdList.SelectedIndex >= 0)
            {
                _selectedGame.Fgds.RemoveAt(SelectedGameFgdList.SelectedIndex);
                SelectedGameUpdateFgds();
            }
        }

        private void SelectedGameOverrideMapSizeChanged(object sender, EventArgs e)
        {
            SelectedGameOverrideSizeHigh.Enabled = SelectedGameOverrideSizeLow.Enabled = SelectedGameOverrideMapSize.Checked;
        }

        private void SelectedGameAddWadClicked(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "WAD files (*.wad)|*.wad", Multiselect = true })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (var fileName in ofd.FileNames)
                    {
                        _selectedGame.Wads.Add(new Wad { Path = fileName });
                    }
                    SelectedGameUpdateWads();
                }
            }
        }

        private void SelectedGameRemoveWadClicked(object sender, EventArgs e)
        {
            if (SelectedGameWadList.SelectedIndex >= 0)
            {
                _selectedGame.Wads.RemoveAt(SelectedGameWadList.SelectedIndex);
                SelectedGameUpdateWads();
            }
        }

        #endregion

        #region Selected Build

	    private Build _selectedBuild;

        private void BuildSelected(object sender, TreeViewEventArgs e)
        {
            var selection = e.Node;
            if (selection == null || selection.Parent == null)
            {
                // No node selected, or selected node is an engine
                _selectedBuild = null;
            }
            else
            {
                // Get the selected build by ID
                _selectedBuild = _builds[int.Parse(selection.Name)];
            }
            UpdateSelectedBuild();
        }

        private void SelectedBuildExeFolderBrowseClicked(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog { SelectedPath = SelectedBuildExeFolder.Text })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    SelectedBuildExeFolder.Text = fbd.SelectedPath;
                }
            }
        }

        private void SelectedBuildNameChanged(object sender, EventArgs e)
        {
            if (_selectedBuild == null) return;
            var idx = _builds.IndexOf(_selectedBuild).ToString();
            var node = BuildTree.Nodes.OfType<TreeNode>().SelectMany(x => x.Nodes.OfType<TreeNode>()).First(x => x.Name == idx);
            node.Text = SelectedBuildName.Text;
        }

        private void SelectedBuildEngineChanged(object sender, EventArgs e)
        {
            if (_selectedBuild == null) return;
            var eng = (Engine) SelectedBuildEngine.SelectedItem;
            var change = eng != _selectedBuild.Engine;
            _selectedBuild.Engine = eng;
            var gs = eng == Engine.Goldsource;
            SelectedBuildCsg.Enabled = SelectedBuildIncludeWads.Enabled = gs;
            if (change)
            {

                UpdateBuildTree();
                var node = BuildTree.Nodes.OfType<TreeNode>().SelectMany(x => x.Nodes.OfType<TreeNode>())
                    .First(x => x.Name == _builds.IndexOf(_selectedBuild).ToString());
                BuildTree.SelectedNode = node;
            }
        }

	    private void UpdateSelectedBuild()
	    {
            BuildSubTabs.Visible = RemoveBuild.Enabled = _selectedBuild != null;
            if (_selectedBuild == null) return;
            SelectedBuildName.Text = _selectedBuild.Name;
            SelectedBuildEngine.SelectedIndex = Math.Max(0, Enum.GetValues(typeof(Engine)).OfType<Engine>().ToList<Engine>().FindIndex(x => x == _selectedBuild.Engine));
            SelectedBuildExeFolder.Text = _selectedBuild.Path;
            SelectedBuildBsp.SelectedText = _selectedBuild.Bsp;
            SelectedBuildCsg.SelectedText = _selectedBuild.Csg;
            SelectedBuildVis.SelectedText = _selectedBuild.Vis;
            SelectedBuildRad.SelectedText = _selectedBuild.Rad;
            //SelectedBuildIncludeWads.Checked = _selectedBuild.IncludeWads;
	    }

        private void SelectedBuildPathChanged(object sender, EventArgs e)
        {
            var selBsp = _selectedBuild.Bsp;
            var selCsg = _selectedBuild.Csg;
            var selVis = _selectedBuild.Vis;
            var selRad = _selectedBuild.Rad;

            SelectedBuildBsp.Items.Clear();
            SelectedBuildCsg.Items.Clear();
            SelectedBuildVis.Items.Clear();
            SelectedBuildRad.Items.Clear();

            if (!Directory.Exists(SelectedBuildExeFolder.Text)) return;
            var dirs = Directory.GetFiles(SelectedBuildExeFolder.Text, "*.exe").Select(Path.GetFileName).ToList();
            if (dirs.Count == 0) return;
            var dira = dirs.ToArray();

            SelectedBuildBsp.Items.AddRange(dira);
            SelectedBuildCsg.Items.AddRange(dira);
            SelectedBuildVis.Items.AddRange(dira);
            SelectedBuildRad.Items.AddRange(dira);

            SelectedBuildBsp.SelectedIndex = dirs.IndexOf(selBsp);
            SelectedBuildCsg.SelectedIndex = dirs.IndexOf(selCsg);
            SelectedBuildVis.SelectedIndex = dirs.IndexOf(selVis);
            SelectedBuildRad.SelectedIndex = dirs.IndexOf(selRad);

            if (SelectedBuildBsp.SelectedIndex < 0) SelectedBuildBsp.SelectedIndex = Math.Max(0, dirs.FindIndex(x => x.ToLower().Contains("bsp")));
            if (SelectedBuildCsg.SelectedIndex < 0) SelectedBuildCsg.SelectedIndex = Math.Max(0, dirs.FindIndex(x => x.ToLower().Contains("csg")));
            if (SelectedBuildVis.SelectedIndex < 0) SelectedBuildVis.SelectedIndex = Math.Max(0, dirs.FindIndex(x => x.ToLower().Contains("vis")));
            if (SelectedBuildRad.SelectedIndex < 0) SelectedBuildRad.SelectedIndex = Math.Max(0, dirs.FindIndex(x => x.ToLower().Contains("rad")));
        }

	    #endregion
	}
}
