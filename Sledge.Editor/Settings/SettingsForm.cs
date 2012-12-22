using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Sledge.Database;
using Sledge.Database.Models;
using Sledge.Settings;
using System.Linq;

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

		    SelectedGameName.TextChanged += (s,e) => CheckNull(_selectedGame, x => x.Name = SelectedGameName.Text);
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
		}

	    private void UpdateData()
	    {
            _engines = Context.DBContext.GetAllEngines();
            _games = Context.DBContext.GetAllGames();
            _builds = Context.DBContext.GetAllBuilds();

            SelectedGameEngine.Items.Clear();
            SelectedGameEngine.Items.AddRange(_engines.Select(x => x.Name).ToArray());

            UpdateGameTree();
            UpdateBuildTree();
            UpdateSteamUsernames();
            SelectedGameUpdateSteamGames();
	    }

	    private void CheckNull<T>(T obj, Action<T> act) where T : class
        {
            if (obj != null) act(obj);
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

	    private void BindColourPicker(Control panel)
	    {
	        panel.Click += (sender, e) =>
	                           {
	                               var p = (Control) sender;
                                   using (var cpd = new ColorDialog { Color = p.BackColor })
                                   {
                                       if (cpd.ShowDialog() == DialogResult.OK) p.BackColor = cpd.Color;
                                   }
	                           };
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
            var newSettings = Serialise.SerialiseSettings().Select(s => new Setting {Key = s.Key, Value = s.Value});
            Context.DBContext.SaveAllSettings(newSettings);
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
            Close();
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
            try
            {
                SteamUsername.Items.Clear();
                var steamdir = Path.Combine(SteamInstallDir.Text, "steamapps");
                var usernames = Directory.GetDirectories(steamdir).Select(Path.GetFileName);
                var ignored = new[] {"common", "downloading", "media", "sourcemods", "temp"};
                SteamUsername.Items.AddRange(usernames.Where(x => !ignored.Contains(x.ToLower())).ToArray());
                var idx = SteamUsername.Items.IndexOf(SteamUsername.Text);
                SteamUsername.SelectedIndex = Math.Max(0, idx);
            }
            catch
            {
                // don't want to do anything if the directory doesn't exist
            }
        }

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
                SelectedGameSteamDirChanged(null, null);
            }
            SelectedGameUpdateSteamGames();
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
            SelectedGameMod.Items.Clear();
            var dir = Path.Combine(SteamInstallDir.Text, "steamapps", SteamUsername.Text, SelectedGameSteamDir.Text);
            if (!Directory.Exists(dir)) dir = Path.Combine(SteamInstallDir.Text, "steamapps", "common", SelectedGameSteamDir.Text);
            if (!Directory.Exists(dir)) return;
            var mods = Directory.GetDirectories(dir).Select(Path.GetFileName);
            var ignored = new[] { "gldrv", "logos", "logs", "errorlogs", "platform", "config", "bin" };
            SelectedGameMod.Items.AddRange(mods.Where(x => !ignored.Contains(x.ToLower())).ToArray());
            var idx = SelectedGameMod.Items.IndexOf(_selectedGame.ModDir ?? "");
            SelectedGameMod.SelectedIndex = Math.Max(0, idx);
        }

        private void TabChanged(object sender, EventArgs e)
        {
            GameTree.SelectedNode = null;
            _selectedGame = null;
            UpdateSelectedGame();
        }

        private void RemoveGameClicked(object sender, EventArgs e)
        {
            if (_selectedGame != null)
            {
                _games.Remove(_selectedGame);
                UpdateGameTree();
            }
        }

        private void AddGameClicked(object sender, EventArgs e)
        {
            _games.Add(new Game
                           {
                               ID = 0,
                               EngineID = _engines.First().ID,
                               Name = "New Game"
                           });
            UpdateGameTree();
            var node = GameTree.Nodes.OfType<TreeNode>().SelectMany(x => x.Nodes.OfType<TreeNode>())
                .First(x => x.Name == (_games.Count - 1).ToString());
            GameTree.SelectedNode = node;
        }

        private void SelectedGameNameChanged(object sender, EventArgs e)
        {
            if (_selectedGame == null) return;
            var idx = _games.IndexOf(_selectedGame).ToString();
            var node = GameTree.Nodes.OfType<TreeNode>().SelectMany(x => x.Nodes.OfType<TreeNode>()).First(x => x.Name == idx);
            node.Text = SelectedGameName.Text;
        }
	}
}
