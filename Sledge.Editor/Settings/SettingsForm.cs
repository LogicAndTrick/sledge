using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
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
	    private List<Engine> _engines; 
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
            SelectedGameEngine.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.EngineID = _engines[SelectedGameEngine.SelectedIndex].ID);
            SelectedGameBuild.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.BuildID = _builds[SelectedGameBuild.SelectedIndex].ID);
            SelectedGameSteamInstall.CheckedChanged += (s, e) => CheckNull(_selectedGame, x => x.SteamInstall = SelectedGameSteamInstall.Checked);
            SelectedGameWonDir.TextChanged += (s, e) => CheckNull(_selectedGame, x => x.WonGameDir = SelectedGameWonDir.Text);
            SelectedGameSteamDir.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.SteamGameDir = SelectedGameSteamDir.Text);
            SelectedGameMod.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.ModDir = SelectedGameMod.Text);
            SelectedGameMapDir.TextChanged += (s, e) => CheckNull(_selectedGame, x => x.MapDir = SelectedGameMapDir.Text);
            SelectedGameEnableAutosave.CheckedChanged += (s, e) => CheckNull(_selectedGame, x => x.Autosave = SelectedGameEnableAutosave.Checked);
            SelectedGameUseDiffAutosaveDir.CheckedChanged += (s, e) => CheckNull(_selectedGame, x => x.UseCustomAutosaveDir = SelectedGameUseDiffAutosaveDir.Checked);
            SelectedGameDiffAutosaveDir.TextChanged += (s, e) => CheckNull(_selectedGame, x => x.AutosaveDir = SelectedGameDiffAutosaveDir.Text);
            SelectedGameDefaultPointEnt.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.DefaultPointEntity = SelectedGameDefaultPointEnt.Text);
            SelectedGameDefaultBrushEnt.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.DefaultBrushEntity = SelectedGameDefaultBrushEnt.Text);
            SelectedGameTextureScale.ValueChanged += (s, e) => CheckNull(_selectedGame, x => x.DefaultTextureScale = SelectedGameTextureScale.Value);
            SelectedGameLightmapScale.ValueChanged += (s, e) => CheckNull(_selectedGame, x => x.DefaultLightmapScale = SelectedGameLightmapScale.Value);

            // Build Configurations
            SelectedBuildName.TextChanged += (s, e) => CheckNull(_selectedBuild, x => x.Name = SelectedBuildName.Text);
            SelectedBuildEngine.SelectedIndexChanged += (s, e) => CheckNull(_selectedBuild, x => x.EngineID = _engines[SelectedBuildEngine.SelectedIndex].ID);
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

            _engines = new List<Engine>(SettingsManager.Engines);
            _games = new List<Game>(SettingsManager.Games);
            _builds = new List<Build>(SettingsManager.Builds);

            ReIndex();

            SelectedGameEngine.Items.Clear();
            SelectedGameEngine.Items.AddRange(_engines.Select(x => x.Name).ToArray());

            SelectedBuildEngine.Items.Clear();
            SelectedBuildEngine.Items.AddRange(_engines.Select(x => x.Name).ToArray());

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
                _games[i].Fgds.ForEach(x => x.GameID = i + 1);
                _games[i].Wads.ForEach(x => x.GameID = i + 1);
            }

            for (var i = 0; i < _builds.Count; i++)
            {
                _builds[i].ID = i + 1;
            }
        }

	    private void UpdateBuildTree()
	    {
	        BuildTree.Nodes.Clear();
            foreach (var engine in _engines)
            {
                var eid = engine.ID;
                var node = BuildTree.Nodes.Add(engine.ID.ToString(), engine.Name);
                var list = _builds.Where(x => x.EngineID == eid).ToList();
                foreach (var build in list)
                {
                    var index = _builds.IndexOf(build);
                    node.Nodes.Add(index.ToString(), build.Name);
                }
            }
            BuildTree.ExpandAll();
            SelectedGameBuild.Items.Clear();
	        SelectedGameBuild.Items.AddRange(_builds.Select(x => x.Name).ToArray());
        }

	    private void UpdateGameTree()
	    {
            GameTree.Nodes.Clear();
	        foreach (var engine in _engines)
	        {
                var eid = engine.ID;
                var node = GameTree.Nodes.Add(engine.ID.ToString(), engine.Name);
	            var list = _games.Where(x => x.EngineID == eid).ToList();
	            foreach (var game in list)
	            {
	                var index = _games.IndexOf(game);
	                node.Nodes.Add(index.ToString(), game.Name);
	            }
	        }
            GameTree.ExpandAll();
	    }

        #endregion

        #region Load/Apply

	    private void SettingsFormLoad(object sender, EventArgs e)
        {
            // 2D Views
            CrosshairCursorIn2DViews.Checked = Sledge.Settings.View.CrosshairCursorIn2DViews;
            ArrowKeysNudgeSelection.Checked = Sledge.Settings.Select.ArrowKeysNudgeSelection;
            NudgeUnits.Value = Sledge.Settings.Select.NudgeUnits;
            RotationStyle_SnapOnShift.Checked = Sledge.Settings.Select.RotationStyle == RotationStyle.SnapOnShift;
            RotationStyle_SnapOffShift.Checked = Sledge.Settings.Select.RotationStyle == RotationStyle.SnapOffShift;
            RotationStyle_SnapNever.Checked = Sledge.Settings.Select.RotationStyle == RotationStyle.SnapNever;
            SnapStyle_SnapOffAlt.Checked = Sledge.Settings.Select.SnapStyle == SnapStyle.SnapOffAlt;
            SnapStyle_SnapOnAlt.Checked = Sledge.Settings.Select.SnapStyle == SnapStyle.SnapOnAlt;
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
            ForwardSpeed.Value = Sledge.Settings.View.ForwardSpeed;
            TimeToTopSpeed.Value = (int) (Sledge.Settings.View.TimeToTopSpeed / 10);
            InvertMouseX.Checked = Sledge.Settings.View.InvertX;
            InvertMouseY.Checked = Sledge.Settings.View.InvertY;
            BackClippingPane.Value = Sledge.Settings.View.BackClippingPane;
            BackClippingPane.Value = Sledge.Settings.View.BackClippingPane;

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
            // 2D Views
            Sledge.Settings.View.CrosshairCursorIn2DViews = CrosshairCursorIn2DViews.Checked;
            Sledge.Settings.Select.ArrowKeysNudgeSelection = ArrowKeysNudgeSelection.Checked;
            Sledge.Settings.Select.NudgeUnits = NudgeUnits.Value;
            if (RotationStyle_SnapOnShift.Checked) Sledge.Settings.Select.RotationStyle = RotationStyle.SnapOnShift;
            if (RotationStyle_SnapOffShift.Checked) Sledge.Settings.Select.RotationStyle = RotationStyle.SnapOffShift;
            if (RotationStyle_SnapNever.Checked) Sledge.Settings.Select.RotationStyle = RotationStyle.SnapNever;
            if (SnapStyle_SnapOffAlt.Checked) Sledge.Settings.Select.SnapStyle = SnapStyle.SnapOffAlt;
            if (SnapStyle_SnapOnAlt.Checked) Sledge.Settings.Select.SnapStyle = SnapStyle.SnapOnAlt;
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
            Sledge.Settings.View.ForwardSpeed = ForwardSpeed.Value;
            Sledge.Settings.View.TimeToTopSpeed = TimeToTopSpeed.Value * 10m;
            Sledge.Settings.View.InvertX = InvertMouseX.Checked;
            Sledge.Settings.View.InvertY = InvertMouseY.Checked;
            Sledge.Settings.View.BackClippingPane = BackClippingPane.Value;
            Sledge.Settings.View.BackClippingPane = BackClippingPane.Value;

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
            SteamUsername.Items.AddRange(usernames.Where(x => !ignored.Contains(x.ToLower())).ToArray());
            var idx = SteamUsername.Items.IndexOf(SteamUsername.Text);
            SteamUsername.SelectedIndex = Math.Max(0, idx);
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
                EngineID = _engines.First().ID,
                Name = "New Game",
                BuildID = _builds.Select(x => x.ID).FirstOrDefault(),
                Autosave = true,
                MapDir = _games.Select(x => x.MapDir).FirstOrDefault() ?? "",
                AutosaveDir = _games.Select(x => x.AutosaveDir).FirstOrDefault() ?? "",
                DefaultLightmapScale = 1,
                DefaultTextureScale = 1,
                Fgds = new List<Fgd>(),
                Wads = new List<Wad>(),
                Build = _builds.FirstOrDefault()
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
                                EngineID = _engines.First().ID,
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
                var replacementBuild = _builds.OrderBy(x => x.EngineID == _selectedBuild.EngineID ? 1 : 2).FirstOrDefault();
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
            SelectedGameDiffAutosaveDir.Text = _selectedGame.AutosaveDir;
            SelectedGameDefaultPointEnt.SelectedText = _selectedGame.DefaultPointEntity;
            SelectedGameDefaultBrushEnt.SelectedText = _selectedGame.DefaultBrushEntity;
            SelectedGameTextureScale.Value = _selectedGame.DefaultTextureScale;
            SelectedGameLightmapScale.Value = _selectedGame.DefaultLightmapScale;

            SelectedGameMod.SelectedText = _selectedGame.ModDir;
            SelectedGameWonDir.Text = _selectedGame.WonGameDir;
            SelectedGameSteamDir.SelectedText = _selectedGame.SteamGameDir;
            SelectedGameBuild.SelectedIndex = Math.Max(0, _builds.FindIndex(x => x.ID == _selectedGame.BuildID));
            SelectedGameEngine.SelectedIndex = Math.Max(0, _engines.FindIndex(x => x.ID == _selectedGame.EngineID));
            SelectedGameSteamInstall.Checked = _selectedGame.SteamInstall;

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
	        var gd = GameDataProvider.GetGameDataFromFiles(_selectedGame.Fgds.Select(x => x.Path));

            SelectedGameDefaultPointEnt.Items.Clear();
            SelectedGameDefaultPointEnt.Items.AddRange(gd.Classes.Where(x => x.ClassType == ClassType.Point).Select(x => x.Name).ToArray());
            var idx = SelectedGameDefaultPointEnt.Items.IndexOf(_selectedGame.DefaultPointEntity ?? "");
            if (idx < 0) idx = SelectedGameDefaultPointEnt.Items.IndexOf("info_player_start");
            if (idx < 0) idx = SelectedGameDefaultPointEnt.Items.IndexOf("light");
	        if (SelectedGameDefaultPointEnt.Items.Count > 0) SelectedGameDefaultPointEnt.SelectedIndex = Math.Max(0, idx);

            SelectedGameDefaultBrushEnt.Items.Clear();
            SelectedGameDefaultBrushEnt.Items.AddRange(gd.Classes.Where(x => x.ClassType == ClassType.Solid).Select(x => x.Name).ToArray());
            idx = SelectedGameDefaultBrushEnt.Items.IndexOf(_selectedGame.DefaultBrushEntity ?? "");
            if (idx < 0) idx = SelectedGameDefaultBrushEnt.Items.IndexOf("func_detail");
            if (idx < 0) idx = SelectedGameDefaultBrushEnt.Items.IndexOf("trigger_once");
            if (SelectedGameDefaultBrushEnt.Items.Count > 0) SelectedGameDefaultBrushEnt.SelectedIndex = Math.Max(0, idx);
	    }

	    private void SelectedGameEngineChanged(object sender, EventArgs e)
        {
            if (_selectedGame == null) return;
            var eng = _engines[SelectedGameEngine.SelectedIndex];
            var change = eng.ID != _selectedGame.EngineID;
            _selectedGame.EngineID = eng.ID;
            SelectedGameSteamInstall.Enabled = eng.Name == "Goldsource";
            if (eng.Name == "Goldsource" && !SelectedGameSteamInstall.Checked)
            {
                SelectedGameWonDir.Enabled = true;
                SelectedGameDirBrowse.Enabled = true;
                SelectedGameSteamDir.Enabled = false;
                SelectedGameWonDirChanged(null, null);
            }
            else
            {
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
            var eng = _engines[SelectedGameEngine.SelectedIndex];
            var include = eng.Name == "Goldsource" ? includeGoldsource : includeSource;
            SelectedGameSteamDir.Items.AddRange(games.Where(x => include.Contains(x.ToLower())).Distinct().OrderBy(x => x.ToLower()).ToArray());
            var idx = SelectedGameSteamDir.Items.IndexOf(_selectedGame.SteamGameDir ?? "");
            if (SelectedGameSteamDir.Items.Count > 0) SelectedGameSteamDir.SelectedIndex = Math.Max(0, idx);
        }

        private void SelectedGameWonDirChanged(object sender, EventArgs e)
        {
            if (SelectedGameEngine.SelectedIndex < 0) return;
            var eng = _engines[SelectedGameEngine.SelectedIndex];
            if (eng.Name != "Goldsource" || SelectedGameSteamInstall.Checked) return;
            SelectedGameMod.Items.Clear();
            if (!Directory.Exists(SelectedGameWonDir.Text)) return;
            var mods = Directory.GetDirectories(SelectedGameWonDir.Text).Select(Path.GetFileName);
            var ignored = new[] { "gldrv", "logos", "logs", "errorlogs", "platform", "config" };
            SelectedGameMod.Items.AddRange(mods.Where(x => !ignored.Contains(x.ToLower())).ToArray());
            var idx = SelectedGameMod.Items.IndexOf(_selectedGame.ModDir ?? "");
            SelectedGameMod.SelectedIndex = Math.Max(0, idx);
        }

        private void SelectedGameSteamDirChanged(object sender, EventArgs e)
        {
            if (SelectedGameEngine.SelectedIndex < 0) return;
            var eng = _engines[SelectedGameEngine.SelectedIndex];
            if (eng.Name == "Goldsource" && !SelectedGameSteamInstall.Checked) return;
            SelectedGameMod.Items.Clear();
            var dir = Path.Combine(SteamInstallDir.Text, "steamapps", SteamUsername.Text, SelectedGameSteamDir.Text);
            if (!Directory.Exists(dir)) dir = Path.Combine(SteamInstallDir.Text, "steamapps", "common", SelectedGameSteamDir.Text);
            if (!Directory.Exists(dir)) return;
            var mods = Directory.GetDirectories(dir).Select(Path.GetFileName);
            var ignored = new[] {"gldrv", "logos", "logs", "errorlogs", "platform", "config", "bin"};
            SelectedGameMod.Items.AddRange(mods.Where(x => !ignored.Contains(x.ToLower())).ToArray());
            var idx = SelectedGameMod.Items.IndexOf(_selectedGame.ModDir ?? "");
            SelectedGameMod.SelectedIndex = Math.Max(0, idx);
        }

        private void SelectedGameNameChanged(object sender, EventArgs e)
        {
            if (_selectedGame == null) return;
            var idx = _games.IndexOf(_selectedGame).ToString();
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
                        _selectedGame.Fgds.Add(new Fgd { GameID = _selectedGame.ID, Path = fileName });
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

        private void SelectedGameAddWadClicked(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "WAD files (*.wad)|*.wad", Multiselect = true })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (var fileName in ofd.FileNames)
                    {
                        _selectedGame.Wads.Add(new Wad { GameID = _selectedGame.ID, Path = fileName });
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
            var eng = _engines[SelectedBuildEngine.SelectedIndex];
            var change = eng.ID != _selectedBuild.EngineID;
            _selectedBuild.EngineID = eng.ID;
            var gs = eng.Name == "Goldsource";
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
            SelectedBuildEngine.SelectedIndex = Math.Max(0, _builds.FindIndex(x => x.ID == _selectedBuild.EngineID));
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
