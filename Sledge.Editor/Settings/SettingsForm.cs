using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.DataStructures.GameData;
using Sledge.Editor.Compiling;
using Sledge.Editor.Extensions;
using Sledge.Editor.UI;
using Sledge.Providers.GameData;
using Sledge.QuickForms;
using Sledge.Settings;
using System.Linq;
using Microsoft.Win32;
using Sledge.Settings.GameDetection.GameFiles;
using Sledge.Settings.Models;
using Hotkeys = Sledge.Settings.Hotkeys;

namespace Sledge.Editor.Settings
{
    /// <summary>
    /// Description of SettingsForm.
    /// </summary>
    public partial class SettingsForm : Form
    {
        private List<Game> _games;
        private List<Build> _builds;
        private List<Hotkey> _hotkeys; 

        public SettingsForm()
        {
            InitializeComponent();
            BindColourPicker(GridBackgroundColour);
            BindColourPicker(GridColour);
            BindColourPicker(GridZeroAxisColour);
            BindColourPicker(GridBoundaryColour);
            BindColourPicker(GridHighlight1Colour);
            BindColourPicker(GridHighlight2Colour);
            BindColourPicker(ViewportBackgroundColour);
            AddColourPresetButtons();
            AddFileTypeBoxes();

            UpdateData();

            BindConfigControls();
        }

        #region Colour Presets

        private struct ColourPreset
        {
            public string Name { get; set; }
            public Color Background { get; set; }
            public Color Grid { get; set; }
            public Color ZeroAxes { get; set; }
            public Color Boundaries { get; set; }
            public Color Highlight1 { get; set; }
            public Color Highlight2 { get; set; }
            public bool Highlight1Enabled { get; set; }
            public bool Highlight2Enabled { get; set; }
            public int Highlight1Count { get; set; }
            public int Highlight2Units { get; set; }
        }

        private static readonly ColourPreset[] Presets = new[]
        {
            new ColourPreset
            {
                Name = "Sledge Default",
                Background = Color.Black,
                Grid = Color.FromArgb(75, 75, 75),
                ZeroAxes = Color.FromArgb(0, 100, 100),
                Highlight1 = Color.FromArgb(115, 115, 115),
                Highlight1Count = 8,
                Highlight1Enabled = true,
                Highlight2 = Color.FromArgb(100, 46, 0),
                Highlight2Units = 1024,
                Highlight2Enabled = true,
                Boundaries = Color.Red
            },
            new ColourPreset
            {
                Name = "Hammer 3",
                Background = Color.Black,
                Grid = Color.FromArgb(81, 81, 81),
                ZeroAxes = Color.FromArgb(0, 100, 100),
                Highlight1 = Color.FromArgb(120, 120, 120),
                Highlight1Count = 8,
                Highlight1Enabled = true,
                Highlight2 = Color.FromArgb(120, 120, 120),
                Highlight2Units = 1024,
                Highlight2Enabled = true,
                Boundaries = Color.FromArgb(120, 120, 120)
            },
            new ColourPreset
            {
                Name = "Hammer 4",
                Background = Color.Black,
                Grid = Color.FromArgb(81, 81, 81),
                ZeroAxes = Color.FromArgb(0, 100, 100),
                Highlight1 = Color.FromArgb(109, 109, 109),
                Highlight1Count = 8,
                Highlight1Enabled = true,
                Highlight2 = Color.FromArgb(100, 46, 1),
                Highlight2Units = 1024,
                Highlight2Enabled = true,
                Boundaries = Color.FromArgb(100, 46, 1)
            },
            new ColourPreset
            {
                Name = "GtkRadiant",
                Background = Color.White,
                Grid = Color.FromArgb(191, 191, 191),
                ZeroAxes = Color.FromArgb(127, 127, 127),
                Highlight1 = Color.FromArgb(127, 127, 127),
                Highlight1Count = 8,
                Highlight1Enabled = false,
                Highlight2 = Color.FromArgb(127, 127, 127),
                Highlight2Units = 64,
                Highlight2Enabled = true,
                Boundaries = Color.FromArgb(127, 127, 127)
            },
            new ColourPreset
            {
                Name = "QuArK",
                Background = Color.FromArgb(232, 220, 184),
                Grid = Color.FromArgb(208, 208, 255),
                ZeroAxes = Color.FromArgb(166, 202, 240),
                Highlight1 = Color.FromArgb(184, 196, 255),
                Highlight1Count = 8,
                Highlight1Enabled = true,
                Highlight2 = Color.FromArgb(184, 196, 255),
                Highlight2Units = 1024,
                Highlight2Enabled = false,
                Boundaries = Color.FromArgb(166, 202, 240)
            },
            new ColourPreset
            {
                Name = "Unreal Editor",
                Background = Color.FromArgb(163, 163, 163),
                Grid = Color.FromArgb(145, 145, 145),
                ZeroAxes = Color.FromArgb(119, 119, 119),
                Highlight1 = Color.FromArgb(127, 127, 127),
                Highlight1Count = 8,
                Highlight1Enabled = true,
                Highlight2 = Color.FromArgb(127, 127, 127),
                Highlight2Units = 1024,
                Highlight2Enabled = false,
                Boundaries = Color.FromArgb(0, 0, 107)
            }
        };

        private void AddColourPresetButtons()
        {
            ColourPresetPanel.Controls.Clear();
            foreach (var cp in Presets)
            {
                var btn = new Button
                {
                    AutoSize = false,
                    Height = 23,
                    Width = 95,
                    Padding = new Padding(0),
                    Margin = new Padding(2),
                    Text = cp.Name
                };
                var preset = cp;
                btn.Click += (s, e) => SetColourPreset(preset);
                ColourPresetPanel.Controls.Add(btn);
            }
        }

        private void SetColourPreset(ColourPreset cp)
        {
            GridBackgroundColour.BackColor = cp.Background;
            GridColour.BackColor = cp.Grid;
            GridZeroAxisColour.BackColor = cp.ZeroAxes;
            GridBoundaryColour.BackColor = cp.Boundaries;

            GridHighlight1Colour.BackColor = cp.Highlight1;
            GridHighlight1Distance.Value = cp.Highlight1Count;
            GridHighlight1On.Checked = cp.Highlight1Enabled;

            GridHighlight2Colour.BackColor = cp.Highlight2;
            GridHighlight2UnitNum.SelectedItem = cp.Highlight2Units.ToString(CultureInfo.InvariantCulture);
            GridHighlight2On.Checked = cp.Highlight2Enabled;
        }

        #endregion

        #region File Types

        private void AddFileTypeBoxes()
        {
            AssociationsPanel.Controls.Clear();
            var supported = FileTypeRegistration.GetSupportedExtensions();
            var registered = FileTypeRegistration.GetRegisteredDefaultFileTypes().ToList();
            foreach (var ft in supported)
            {
                var def = registered.Contains(ft.Extension);
                var cb = new CheckBox
                {
                    AutoSize = true,
                    Checked = def,
                    Text = ft.Extension + " (" + ft.Description + ")",
                    Tag = ft
                };
                AssociationsPanel.Controls.Add(cb);
            }
        }

        #endregion

        #region Initialisation
        private void BindConfigControls()
        {
            // Game Configurations
            SelectedGameName.TextChanged += (s, e) => CheckNull(_selectedGame, x => x.Name = SelectedGameName.Text);
            SelectedGameEngine.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.Engine = (Engine) SelectedGameEngine.SelectedItem);
            SelectedGameBuild.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.BuildID = _builds[SelectedGameBuild.SelectedIndex].ID);
            SelectedGameWonDir.TextChanged += (s, e) => CheckNull(_selectedGame, x => x.GameInstallDir = SelectedGameWonDir.Text);
            SelectedGameMod.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.ModDir = SelectedGameMod.Text);
            SelectedGameBase.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.BaseDir = SelectedGameBase.Text);
            SelectedGameUseHDModels.CheckedChanged += (s, e) => CheckNull(_selectedGame, x => x.UseHDModels = SelectedGameUseHDModels.Checked);
            SelectedGameExecutable.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.Executable = SelectedGameExecutable.Text);
            SelectedGameRunArguments.TextChanged += (s, e) => CheckNull(_selectedGame, x => x.ExecutableParameters = SelectedGameRunArguments.Text);
            SelectedGameMapDir.TextChanged += (s, e) => CheckNull(_selectedGame, x => x.MapDir = SelectedGameMapDir.Text);
            SelectedGameEnableAutosave.CheckedChanged += (s, e) => CheckNull(_selectedGame, x => x.Autosave = SelectedGameEnableAutosave.Checked);
            SelectedGameUseDiffAutosaveDir.CheckedChanged += (s, e) => CheckNull(_selectedGame, x => x.UseCustomAutosaveDir = SelectedGameUseDiffAutosaveDir.Checked);
            SelectedGameDiffAutosaveDir.TextChanged += (s, e) => CheckNull(_selectedGame, x => x.AutosaveDir = SelectedGameDiffAutosaveDir.Text);
            SelectedGameAutosaveTime.ValueChanged += (s, e) => CheckNull(_selectedGame, x => x.AutosaveTime = (int) SelectedGameAutosaveTime.Value);
            SelectedGameAutosaveLimit.ValueChanged += (s, e) => CheckNull(_selectedGame, x => x.AutosaveLimit = (int)SelectedGameAutosaveLimit.Value);
            SelectedGameAutosaveOnlyOnChange.CheckedChanged += (s, e) => CheckNull(_selectedGame, x => x.AutosaveOnlyOnChanged = SelectedGameAutosaveOnlyOnChange.Checked);
            SelectedGameAutosaveTriggerFileSave.CheckedChanged += (s, e) => CheckNull(_selectedGame, x => x.AutosaveTriggerFileSave = SelectedGameAutosaveTriggerFileSave.Checked);
            SelectedGameDefaultPointEnt.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.DefaultPointEntity = SelectedGameDefaultPointEnt.Text);
            SelectedGameDefaultBrushEnt.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.DefaultBrushEntity = SelectedGameDefaultBrushEnt.Text);
            SelectedGameTextureScale.ValueChanged += (s, e) => CheckNull(_selectedGame, x => x.DefaultTextureScale = SelectedGameTextureScale.Value);
            SelectedGameLightmapScale.ValueChanged += (s, e) => CheckNull(_selectedGame, x => x.DefaultLightmapScale = SelectedGameLightmapScale.Value);
            SelectedGameBlacklistTextbox.TextChanged += (s, e) => CheckNull(_selectedGame, x => x.PackageBlacklist = SelectedGameBlacklistTextbox.Text.Trim());
            SelectedGameWhitelistTextbox.TextChanged += (s, e) => CheckNull(_selectedGame, x => x.PackageWhitelist = SelectedGameWhitelistTextbox.Text.Trim());
            SelectedGameIncludeFgdDirectoriesInEnvironment.CheckedChanged += (s, e) => CheckNull(_selectedGame, x => x.IncludeFgdDirectoriesInEnvironment = SelectedGameIncludeFgdDirectoriesInEnvironment.Checked);
            SelectedGameOverrideMapSize.CheckedChanged += (s, e) => CheckNull(_selectedGame, x => x.OverrideMapSize = SelectedGameOverrideMapSize.Checked);
            var sizes = new[] {4096, 8192, 16384, 32768, 65536};
            SelectedGameOverrideSizeLow.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.OverrideMapSizeLow = SelectedGameOverrideSizeLow.SelectedIndex < 0 ? 0 : -sizes[SelectedGameOverrideSizeLow.SelectedIndex]);
            SelectedGameOverrideSizeHigh.SelectedIndexChanged += (s, e) => CheckNull(_selectedGame, x => x.OverrideMapSizeHigh = SelectedGameOverrideSizeHigh.SelectedIndex < 0 ? 0 : sizes[SelectedGameOverrideSizeHigh.SelectedIndex]);

            // Build Configurations
            SelectedBuildName.TextChanged += (s, e) => CheckNull(_selectedBuild, x => x.Name = SelectedBuildName.Text);
            SelectedBuildEngine.SelectedIndexChanged += (s, e) => CheckNull(_selectedBuild, x => x.Engine = (Engine) SelectedBuildEngine.SelectedItem);
            SelectedBuildDontRedirectOutput.CheckedChanged += (s, e) => CheckNull(_selectedBuild, x => x.DontRedirectOutput = SelectedBuildDontRedirectOutput.Checked);
            SelectedBuildExeFolder.TextChanged += (s, e) => CheckNull(_selectedBuild, x => x.Path = SelectedBuildExeFolder.Text);
            SelectedBuildBsp.SelectedIndexChanged += (s, e) => CheckNull(_selectedBuild, x => x.Bsp = SelectedBuildBsp.Text);
            SelectedBuildCsg.SelectedIndexChanged += (s, e) => CheckNull(_selectedBuild, x => x.Csg = SelectedBuildCsg.Text);
            SelectedBuildVis.SelectedIndexChanged += (s, e) => CheckNull(_selectedBuild, x => x.Vis = SelectedBuildVis.Text);
            SelectedBuildRad.SelectedIndexChanged += (s, e) => CheckNull(_selectedBuild, x => x.Rad = SelectedBuildRad.Text);
            SelectedBuildIncludePathInEnvironment.CheckedChanged += (s, e) => CheckNull(_selectedBuild, x => x.IncludePathInEnvironment = SelectedBuildIncludePathInEnvironment.Checked);

            SelectedBuildWorkingDirTemp.CheckedChanged += (s, e) => { if (SelectedBuildWorkingDirTemp.Checked) CheckNull(_selectedBuild, x => x.WorkingDirectory = CompileWorkingDirectory.TemporaryDirectory); };
            SelectedBuildWorkingDirSame.CheckedChanged += (s, e) => { if (SelectedBuildWorkingDirSame.Checked) CheckNull(_selectedBuild, x => x.WorkingDirectory = CompileWorkingDirectory.SameDirectory); };
            SelectedBuildWorkingDirSub.CheckedChanged += (s, e) => { if (SelectedBuildWorkingDirSub.Checked) CheckNull(_selectedBuild, x => x.WorkingDirectory = CompileWorkingDirectory.SubDirectory); };

            SelectedBuildAfterCopyBsp.CheckedChanged += (s, e) => CheckNull(_selectedBuild, x => x.AfterCopyBsp = SelectedBuildAfterCopyBsp.Checked);
            SelectedBuildAfterRunGame.CheckedChanged += (s, e) => CheckNull(_selectedBuild, x => x.AfterRunGame = SelectedBuildAfterRunGame.Checked);
            SelectedBuildAskBeforeRun.CheckedChanged += (s, e) => CheckNull(_selectedBuild, x => x.AfterAskBeforeRun = SelectedBuildAskBeforeRun.Checked);

            SelectedBuildCopyBsp.CheckedChanged += (s, e) => CheckNull(_selectedBuild, x => x.CopyBsp = SelectedBuildCopyBsp.Checked);
            SelectedBuildCopyRes.CheckedChanged += (s, e) => CheckNull(_selectedBuild, x => x.CopyRes = SelectedBuildCopyRes.Checked);
            SelectedBuildCopyLin.CheckedChanged += (s, e) => CheckNull(_selectedBuild, x => x.CopyLin = SelectedBuildCopyLin.Checked);
            SelectedBuildCopyMap.CheckedChanged += (s, e) => CheckNull(_selectedBuild, x => x.CopyMap = SelectedBuildCopyMap.Checked);
            SelectedBuildCopyPts.CheckedChanged += (s, e) => CheckNull(_selectedBuild, x => x.CopyPts = SelectedBuildCopyPts.Checked);
            SelectedBuildCopyLog.CheckedChanged += (s, e) => CheckNull(_selectedBuild, x => x.CopyLog = SelectedBuildCopyLog.Checked);
            SelectedBuildCopyErr.CheckedChanged += (s, e) => CheckNull(_selectedBuild, x => x.CopyErr = SelectedBuildCopyErr.Checked);

            // Build Profiles
            SelectedBuildRunCsgCheckbox.CheckedChanged += (s, e) => CheckNull(_selectedProfile, x =>
            {
                if (_pauseProfileUpdates) return;
                x.RunCsg = SelectedBuildRunCsgCheckbox.Checked;
                UpdateSelectedBuildProfilePreview();
            });
            SelectedBuildRunBspCheckbox.CheckedChanged += (s, e) => CheckNull(_selectedProfile, x =>
            {
                if (_pauseProfileUpdates) return;
                x.RunBsp = SelectedBuildRunBspCheckbox.Checked;
                UpdateSelectedBuildProfilePreview();
            });
            SelectedBuildRunVisCheckbox.CheckedChanged += (s, e) => CheckNull(_selectedProfile, x =>
            {
                if (_pauseProfileUpdates) return;
                x.RunVis = SelectedBuildRunVisCheckbox.Checked;
                UpdateSelectedBuildProfilePreview();
            });
            SelectedBuildRunRadCheckbox.CheckedChanged += (s, e) => CheckNull(_selectedProfile, x =>
            {
                if (_pauseProfileUpdates) return;
                x.RunRad = SelectedBuildRunRadCheckbox.Checked;
                UpdateSelectedBuildProfilePreview();
            });

            SelectedBuildCsgParameters.ValueChanged += (s, e) => CheckNull(_selectedProfile, x =>
            {
                if (_pauseProfileUpdates) return;
                x.GeneratedCsgParameters = SelectedBuildCsgParameters.GeneratedCommands;
                x.AdditionalCsgParameters = SelectedBuildCsgParameters.AdditionalCommands;
                UpdateSelectedBuildProfilePreview();
            });
            SelectedBuildBspParameters.ValueChanged += (s, e) => CheckNull(_selectedProfile, x =>
            {
                if (_pauseProfileUpdates) return;
                x.GeneratedBspParameters = SelectedBuildBspParameters.GeneratedCommands;
                x.AdditionalBspParameters = SelectedBuildBspParameters.AdditionalCommands;
                UpdateSelectedBuildProfilePreview();
            });
            SelectedBuildVisParameters.ValueChanged += (s, e) => CheckNull(_selectedProfile, x =>
            {
                if (_pauseProfileUpdates) return;
                x.GeneratedVisParameters = SelectedBuildVisParameters.GeneratedCommands;
                x.AdditionalVisParameters = SelectedBuildVisParameters.AdditionalCommands;
                UpdateSelectedBuildProfilePreview();
            });
            SelectedBuildRadParameters.ValueChanged += (s, e) => CheckNull(_selectedProfile, x =>
            {
                if (_pauseProfileUpdates) return;
                x.GeneratedRadParameters = SelectedBuildRadParameters.GeneratedCommands;
                x.AdditionalRadParameters = SelectedBuildRadParameters.AdditionalCommands;
                UpdateSelectedBuildProfilePreview();
            });
            SelectedBuildSharedParameters.ValueChanged += (s, e) => CheckNull(_selectedProfile, x =>
            {
                if (_pauseProfileUpdates) return;
                x.GeneratedSharedParameters = SelectedBuildSharedParameters.GeneratedCommands;
                x.AdditionalSharedParameters = SelectedBuildSharedParameters.AdditionalCommands;
                UpdateSelectedBuildProfilePreview();
            });
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

            SelectedBuildSpecification.Items.Clear();
            SelectedBuildSpecification.Items.AddRange(CompileSpecification.Specifications.OfType<object>().ToArray());

            UpdateGameTree();
            UpdateBuildTree();
            SelectedGameUpdateSteamGames();

            _hotkeys = Hotkeys.GetHotkeys().Select(x => new Hotkey { ID = x.ID, HotkeyString = x.HotkeyString }).ToList();
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
        
        #region Controls

        private void AddHeading(string text)
        {
            var label = new Label
            {
                Font = new Font(Font, FontStyle.Bold),
                Text = text,
                AutoSize = true,
                Padding = new Padding(0, 5, 0, 5)
            };
            flowLayoutPanel1.Controls.Add(label);
        }

        private CheckBox AddSetting(Expression<Func<bool>> prop, string text)
        {
            var expression = (MemberExpression) prop.Body;
            var property = (PropertyInfo) expression.Member;
            var checkbox = new CheckBox
            {
                Text = text,
                AutoSize = true,
                Checked = (bool) property.GetValue(null, null),
                Tag = prop,
                Padding = new Padding(10, 0, 0, 0)
            };
            checkbox.CheckedChanged += (s, e) => property.SetValue(null, checkbox.Checked, null);
            flowLayoutPanel1.Controls.Add(checkbox);

            return checkbox;
        }

        private ComboBox AddSetting(Expression<Func<Enum>> prop, string text)
        {
            var expression = (MemberExpression)((UnaryExpression)prop.Body).Operand;
            var property = (PropertyInfo)expression.Member;
            var combo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 300
            };
            var vals = Enum.GetValues(property.PropertyType).OfType<Enum>().ToList();
            foreach (var val in vals)
            {
                combo.Items.Add(val.GetDescription());
            }
            combo.SelectedIndex = vals.IndexOf((Enum)property.GetValue(null, null));
            combo.SelectedIndexChanged += (s, e) => property.SetValue(null, vals[combo.SelectedIndex], null);
            var label = new Label
            {
                AutoSize = true,
                Text = text,
                Padding = new Padding(0, 5, 0, 5)
            };
            var panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true
            };
            panel.Controls.Add(label);
            panel.Controls.Add(combo);
            flowLayoutPanel1.Controls.Add(panel);

            return combo;
        }

        private NumericUpDown AddSetting(Expression<Func<decimal>> prop, decimal min, decimal max, int decimals, decimal increment, string text)
        {
            var expression = (MemberExpression)prop.Body;
            var property = (PropertyInfo)expression.Member;
            var updown = new NumericUpDown
            {
                Minimum = min,
                Maximum = max,
                DecimalPlaces = decimals,
                Increment = increment,
                Value = Convert.ToDecimal(property.GetValue(null, null)),
                Width = 50
            };
            updown.ValueChanged += (s, e) => property.SetValue(null, updown.Value, null);
            var label = new Label
            {
                AutoSize = true,
                Text = text,
                Padding = new Padding(0, 5, 0, 5)
            };
            var panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true
            };
            panel.Controls.Add(label);
            panel.Controls.Add(updown);
            flowLayoutPanel1.Controls.Add(panel);

            return updown;
        }

        private NumericUpDown AddSetting(Expression<Func<int>> prop, int min, int max, string text)
        {
            var expression = (MemberExpression)prop.Body;
            var property = (PropertyInfo)expression.Member;
            var updown = new NumericUpDown
            {
                Minimum = min,
                Maximum = max,
                DecimalPlaces = 0,
                Increment = 1,
                Value = Convert.ToDecimal(property.GetValue(null, null)),
                Width = 50
            };
            updown.ValueChanged += (s, e) => property.SetValue(null, (int) updown.Value, null);
            var label = new Label
            {
                AutoSize = true,
                Text = text,
                Padding = new Padding(0, 5, 0, 5)
            };
            var panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true
            };
            panel.Controls.Add(label);
            panel.Controls.Add(updown);
            flowLayoutPanel1.Controls.Add(panel);

            return updown;
        }

        private Panel AddSetting(Expression<Func<Color>> prop, string text)
        {
            var expression = (MemberExpression)prop.Body;
            var property = (PropertyInfo)expression.Member;
            var colour = new Panel
            {
                BackColor = (Color) property.GetValue(null, null),
                Height = 20,
                Width = 50,
                BorderStyle = BorderStyle.Fixed3D
            };
            colour.Click += (s, e) =>
            {
                using (var cpd = new ColorDialog { Color = colour.BackColor })
                {
                    if (cpd.ShowDialog() == DialogResult.OK)
                    {
                        colour.BackColor = cpd.Color;
                        property.SetValue(null, cpd.Color, null);
                    }
                }
            };
            var label = new Label
            {
                AutoSize = true,
                Text = text,
                Padding = new Padding(0, 5, 0, 5)
            };
            var panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true
            };
            panel.Controls.Add(label);
            panel.Controls.Add(colour);
            flowLayoutPanel1.Controls.Add(panel);

            return colour;
        }

        #endregion

        #region Load/Apply

        private void SettingsFormLoad(object sender, EventArgs e)
        {
            AddHeading("Object Creation");
            AddSetting(() => Sledge.Settings.Select.SwitchToSelectAfterCreation, "Switch to selection tool after brush creation");
            AddSetting(() => Sledge.Settings.Select.SwitchToSelectAfterEntity, "Switch to selection tool after entity creation");
            AddSetting(() => Sledge.Settings.Select.SelectCreatedBrush, "Automatically select created brush");
            AddSetting(() => Sledge.Settings.Select.SelectCreatedEntity, "Automatically select created entity");
            AddSetting(() => Sledge.Settings.Select.DeselectOthersWhenSelectingCreation, "Deselect other objects when automatically selecting created items");
            AddSetting(() => Sledge.Settings.Select.ResetBrushTypeOnCreation, "Reset to block brush type after creating brush");
            AddSetting(() => Sledge.Settings.Select.KeepVisgroupsWhenCloning, "Keep visgroups when cloning");

            AddHeading("Multiple Files");
            AddSetting(() => Sledge.Settings.View.LoadSession, "Load previously opened files on startup");
            AddSetting(() => Sledge.Settings.View.KeepCameraPositions, "Keep current camera positions when switching between maps");
            AddSetting(() => Sledge.Settings.View.KeepSelectedTool, "Keep current selected tool when switching between maps");

            AddHeading("Compiling");
            AddSetting(() => Sledge.Settings.View.CompileOpenOutput, "Open the output panel on compile start");
            AddSetting(() => Sledge.Settings.View.CompileDefaultAdvanced, "Use advanced compile dialog by default");

            AddHeading("Textures");
            AddSetting(() => Sledge.Settings.Select.ApplyTextureImmediately, "Apply texture immediately after browsing in the texture application tool");

            AddHeading("Rendering (these settings will be applied after Sledge is restarted)");
            AddSetting(() => Sledge.Settings.View.Renderer, "Renderer");
            AddSetting(() => Sledge.Settings.View.DisableWadTransparency, "Disable WAD texture transparency");
            AddSetting(() => Sledge.Settings.View.DisableToolTextureTransparency, "Disable tool texture transparency");
            AddSetting(() => Sledge.Settings.View.GloballyDisableTransparency, "Disable transparent textures globally");
            AddSetting(() => Sledge.Settings.View.DisableModelRendering, "Disable model rendering");
            AddSetting(() => Sledge.Settings.View.DisableSpriteRendering, "Disable sprite rendering");
            AddSetting(() => Sledge.Settings.View.DisableTextureFiltering, "Disable texture filtering (try this if textures render incorrectly)");
            AddSetting(() => Sledge.Settings.View.ForcePowerOfTwoTextureResizing, "Force non power of two textures to be resized (try this if only 64, 128, 256, 512, etc size textures work)");

            AddHeading("Center Handles");
            AddSetting(() => Sledge.Settings.Select.DrawCenterHandles, "Render brush center handles");
            AddSetting(() => Sledge.Settings.Select.CenterHandlesActiveViewportOnly, "Render center handles only in active viewport");
            AddSetting(() => Sledge.Settings.Select.CenterHandlesFollowCursor, "Render center handles only near cursor position");
            AddSetting(() => Sledge.Settings.Select.BoxSelectByCenterHandlesOnly, "Selection box selects by center handles only");
            AddSetting(() => Sledge.Settings.Select.ClickSelectByCenterHandlesOnly, "Clicking in 2D view selects by center handles only");

            AddHeading("Interaction");
            AddSetting(() => Sledge.Settings.Select.DoubleClick3DAction, "Action to perform when double-clicking an object in the 3D view");
            AddSetting(() => Sledge.Settings.Select.OpenObjectPropertiesWhenCreatingEntity, "Open object properties when creating an entity");

            AddHeading("2D Vertices");
            AddSetting(() => Sledge.Settings.View.Draw2DVertices, "Render vertices in 2D views");
            AddSetting(() => Sledge.Settings.View.VertexPointSize, 1, 10, "Vertex point size");
            AddSetting(() => Sledge.Settings.View.OverrideVertexColour, "Override vertex colour (defaults to the brush colour)");
            AddSetting(() => Sledge.Settings.View.VertexOverrideColour, "Vertex override colour");

            AddHeading("Selection Box");
            AddSetting(() => Sledge.Settings.Select.AutoSelectBox, "Automatically select when box is drawn");
            AddSetting(() => Sledge.Settings.View.DrawBoxText, "Draw selection box size in the viewport");
            AddSetting(() => Sledge.Settings.View.DrawBoxDashedLines, "Draw selection box with dashed lines");
            AddSetting(() => Sledge.Settings.View.ScrollWheelZoomMultiplier, 1.01m, 10, 2, 0.1m, "Scroll wheel zoom multiplier (default 1.20)");
            AddSetting(() => Sledge.Settings.View.SelectionBoxBackgroundOpacity, 0, 128, "Selection box background opacity");

            AddHeading("Camera");
            AddSetting(() => Sledge.Settings.View.Camera2DPanRequiresMouseClick, "Require mouse click to enable panning in 2D viewports when holding spacebar");
            AddSetting(() => Sledge.Settings.View.Camera3DPanRequiresMouseClick, "Require mouse click to enable free-look in 3D viewports when holding spacebar");

            AddHeading("Undo Stack");
            AddSetting(() => Sledge.Settings.Select.UndoStackSize, 1, 1000, "Undo stack size (caution: setting too high may result in an out of memory crash!)");
            AddSetting(() => Sledge.Settings.Select.SkipSelectionInUndoStack, "Fast-forward selection operations when performing undo/redo (selection, deselection)");
            AddSetting(() => Sledge.Settings.Select.SkipVisibilityInUndoStack, "Fast-forward visibility operations when performing undo/redo (visgroup visibility, quick show/hide)");

            // Integration
            SingleInstanceCheckbox.Checked = Sledge.Settings.View.SingleInstance;

            // 2D Views
            CrosshairCursorIn2DViews.Checked = Sledge.Settings.View.CrosshairCursorIn2DViews;
            DrawEntityNames.Checked = Sledge.Settings.View.DrawEntityNames;
            DrawEntityAngles.Checked = Sledge.Settings.View.DrawEntityAngles; //mxd

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
            ViewportBackgroundColour.BackColor = Sledge.Settings.View.ViewportBackground;
            BackClippingPlaneUpDown.Value = BackClippingPane.Value = Sledge.Settings.View.BackClippingPane;
            ModelRenderDistanceUpDown.Value = ModelRenderDistance.Value = Sledge.Settings.View.ModelRenderDistance;
            DetailRenderDistanceUpDown.Value = DetailRenderDistance.Value = Sledge.Settings.View.DetailRenderDistance;
            BackClippingPaneChanged(null, null);
            ModelRenderDistanceChanged(null, null);
            DetailRenderDistanceChanged(null, null);

            ForwardSpeedUpDown.Value = ForwardSpeed.Value = Sledge.Settings.View.ForwardSpeed;
            ForwardSpeedChanged(null, null);

            TimeToTopSpeed.Value = (int) (Sledge.Settings.View.TimeToTopSpeed / 100);
            TimeToTopSpeedUpDown.Value = Sledge.Settings.View.TimeToTopSpeed / 1000;
            TimeToTopSpeedChanged(null, null);

            InvertMouseX.Checked = Sledge.Settings.View.InvertX;
            InvertMouseY.Checked = Sledge.Settings.View.InvertY;
            MouseWheelMoveDistance.Value = Sledge.Settings.View.MouseWheelMoveDistance;

            CameraFOV.Value = Sledge.Settings.View.CameraFOV;

            // Game Configurations
            // Build Programs
            // Steam
            SteamInstallDir.Text = Steam.SteamDirectory;

            // Hotkeys
            UpdateHotkeyList();
        }

        private void Apply()
        {
            // Integration
            Sledge.Settings.View.SingleInstance = SingleInstanceCheckbox.Checked;

            // 2D Views
            Sledge.Settings.View.CrosshairCursorIn2DViews = CrosshairCursorIn2DViews.Checked;
            Sledge.Settings.View.DrawEntityNames = DrawEntityNames.Checked;
            Sledge.Settings.View.DrawEntityAngles = DrawEntityAngles.Checked; //mxd

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
            Sledge.Settings.View.ViewportBackground = ViewportBackgroundColour.BackColor;
            Sledge.Settings.View.BackClippingPane = BackClippingPane.Value;
            Sledge.Settings.View.ModelRenderDistance = ModelRenderDistance.Value;
            Sledge.Settings.View.DetailRenderDistance = DetailRenderDistance.Value;

            Sledge.Settings.View.ForwardSpeed = ForwardSpeed.Value;
            Sledge.Settings.View.TimeToTopSpeed = TimeToTopSpeed.Value * 100m;
            Sledge.Settings.View.InvertX = InvertMouseX.Checked;
            Sledge.Settings.View.InvertY = InvertMouseY.Checked;
            Sledge.Settings.View.MouseWheelMoveDistance = MouseWheelMoveDistance.Value;

            Sledge.Settings.View.CameraFOV = (int) CameraFOV.Value;

            // Game Configurations
            // Build Programs
            // Steam
            Steam.SteamDirectory = SteamInstallDir.Text;

            // Hotkeys
            SettingsManager.Hotkeys.Clear();
            SettingsManager.Hotkeys.AddRange(_hotkeys);
            Hotkeys.SetupHotkeys(SettingsManager.Hotkeys);
            
            // Save settings to database
            ReIndex();
            SettingsManager.Builds.Clear();
            SettingsManager.Builds.AddRange(_builds);
            SettingsManager.Games.Clear();
            SettingsManager.Games.AddRange(_games);

            SettingsManager.Write();

            // Update associations
            var assoc = AssociationsPanel.Controls
                .OfType<CheckBox>().Where(x => x.Checked)
                .Select(x => x.Tag).OfType<FileType>()
                .Select(x => x.Extension);
            FileTypeRegistration.RegisterDefaultFileTypes(assoc);

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
            Close();
        }

        private void SettingsFormClosed(object sender, FormClosedEventArgs e)
        {
            SettingsManager.Read();
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
            if (BackClippingPane.Value != BackClippingPlaneUpDown.Value) BackClippingPlaneUpDown.Value = BackClippingPane.Value;
        }

        private void BackClippingPlaneUpDownValueChanged(object sender, EventArgs e)
        {
            if (BackClippingPlaneUpDown.Value != BackClippingPane.Value) BackClippingPane.Value = (int) BackClippingPlaneUpDown.Value;
        }

        private void ModelRenderDistanceChanged(object sender, EventArgs e)
        {
            if (ModelRenderDistance.Value != ModelRenderDistanceUpDown.Value) ModelRenderDistanceUpDown.Value = ModelRenderDistance.Value;
        }

        private void ModelRenderDistanceUpDownValueChanged(object sender, EventArgs e)
        {
            if (ModelRenderDistanceUpDown.Value != ModelRenderDistance.Value) ModelRenderDistance.Value = (int)ModelRenderDistanceUpDown.Value;
        }

        private void DetailRenderDistanceChanged(object sender, EventArgs e)
        {
            if (DetailRenderDistance.Value != DetailRenderDistanceUpDown.Value) DetailRenderDistanceUpDown.Value = DetailRenderDistance.Value;
        }

        private void DetailRenderDistanceUpDownValueChanged(object sender, EventArgs e)
        {
            if (DetailRenderDistanceUpDown.Value != DetailRenderDistance.Value) DetailRenderDistance.Value = (int)DetailRenderDistanceUpDown.Value;
        }

        private void ForwardSpeedChanged(object sender, EventArgs e)
        {
            if (ForwardSpeed.Value != ForwardSpeedUpDown.Value) ForwardSpeedUpDown.Value = ForwardSpeed.Value;
        }

        private void ForwardSpeedUpDownValueChanged(object sender, EventArgs e)
        {
            if (ForwardSpeedUpDown.Value != ForwardSpeed.Value) ForwardSpeed.Value = (int)ForwardSpeedUpDown.Value;
        }

        private void TimeToTopSpeedChanged(object sender, EventArgs e)
        {
            if (TimeToTopSpeed.Value != TimeToTopSpeedUpDown.Value) TimeToTopSpeedUpDown.Value = TimeToTopSpeed.Value / 10m;
        }

        private void TimeToTopSpeedUpDownValueChanged(object sender, EventArgs e)
        {
            if (TimeToTopSpeedUpDown.Value != TimeToTopSpeed.Value) TimeToTopSpeed.Value = (int)(TimeToTopSpeedUpDown.Value * 10);
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

        private void SteamInstallDirAutoDectectClicked(object sender, EventArgs e)
        {
            // HKEY_CURRENT_USER\SOFTWARE\Valve\Steam
            using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam", false))
            {
                if (key != null)
                {
                    var value = key.GetValue("SteamPath") as string;
                    if (!String.IsNullOrWhiteSpace(value) && Directory.Exists(value))
                    {
                        SteamInstallDir.Text = value;
                    }
                }
            }
        }

        private void SteamUsernameChanged(object sender, EventArgs e)
        {
            SelectedGameUpdateSteamGames();
        }

        private void SteamDirectoryChanged(object sender, EventArgs e)
        {
            SelectedGameUpdateSteamGames();
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
                UseHDModels = true,
                AutosaveDir = _games.Select(x => x.AutosaveDir).FirstOrDefault() ?? "",
                DefaultLightmapScale = 1,
                DefaultTextureScale = 1,
                Fgds = new List<Fgd>(),
                AdditionalPackages = new List<string>()
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
            SelectedGameAutosaveTriggerFileSave.Checked = _selectedGame.AutosaveTriggerFileSave;
            SelectedGameDiffAutosaveDir.Text = _selectedGame.AutosaveDir;
            SelectedGameDiffAutosaveDir.Enabled = SelectedGameUseDiffAutosaveDir.Checked;
            SelectedGameDefaultPointEnt.SelectedText = _selectedGame.DefaultPointEntity;
            SelectedGameDefaultBrushEnt.SelectedText = _selectedGame.DefaultBrushEntity;
            SelectedGameTextureScale.Value = _selectedGame.DefaultTextureScale;
            SelectedGameLightmapScale.Value = _selectedGame.DefaultLightmapScale;
            SelectedGameBlacklistTextbox.Text = _selectedGame.PackageBlacklist ?? "";
            SelectedGameWhitelistTextbox.Text = _selectedGame.PackageWhitelist ?? "";
            SelectedGameIncludeFgdDirectoriesInEnvironment.Checked = _selectedGame.IncludeFgdDirectoriesInEnvironment;
            SelectedGameOverrideMapSize.Checked = _selectedGame.OverrideMapSize;
            var sizes = new[] { 4096, 8192, 16384, 32768, 65536 };
            SelectedGameOverrideSizeLow.SelectedIndex = Array.IndexOf(sizes, -_selectedGame.OverrideMapSizeLow);
            SelectedGameOverrideSizeHigh.SelectedIndex = Array.IndexOf(sizes, _selectedGame.OverrideMapSizeHigh);
            
            SelectedGameMod.SelectedText = _selectedGame.ModDir;
            SelectedGameBase.SelectedText = _selectedGame.BaseDir;
            SelectedGameUseHDModels.Checked = _selectedGame.UseHDModels;
            SelectedGameExecutable.SelectedText = _selectedGame.Executable;
            SelectedGameRunArguments.Text = _selectedGame.ExecutableParameters;
            SelectedGameWonDir.Text = _selectedGame.GameInstallDir;
            //SelectedGameSteamDir.SelectedText = _selectedGame.SteamGameDir;

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
            SelectedGameUpdateAdditionalPackages();
        }

        private void SelectedGameUpdateAdditionalPackages()
        {
            SelectedGameAdditionalPackageList.Items.Clear();
            foreach (var additionalPackage in _selectedGame.AdditionalPackages)
            {
                var item = new ListViewItem(new[]
                {
                    Path.GetFileName(additionalPackage),
                    additionalPackage
                }) {ToolTipText = additionalPackage};
                SelectedGameAdditionalPackageList.Items.Add(item);
            }
            SelectedGameAdditionalPackageList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void SelectedGameUpdateFgds()
        {
            SelectedGameFgdList.Items.Clear();
            foreach (var fgd in _selectedGame.Fgds)
            {
                var item = new ListViewItem(new[]
                {
                    Path.GetFileName(fgd.Path),
                    fgd.Path
                }) { ToolTipText = fgd.Path };
                SelectedGameFgdList.Items.Add(item);
            }
            SelectedGameFgdList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

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
                MessageBox.Show(
                    "There was an error loading the provided FGDs, please ensure that you have loaded all dependant FGDs and that all FGDs are valid.\n" +
                    "Check the location in the FGD file specified by the error message for more information.\n\n" +
                    "Error was: " + ex.Message,
                    "Error parsing FGD file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectedGameEngineChanged(object sender, EventArgs e)
        {
            if (_selectedGame == null || SelectedGameEngine.SelectedIndex < 0) return;
            var eng = (Engine) SelectedGameEngine.SelectedItem;
            var change = eng != _selectedGame.Engine;
            _selectedGame.Engine = eng;

            SelectedGameWonDirChanged(null, null);

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
            var detectedGames = Sledge.Settings.GameDetection.GameFiles.SteamGames.GetDetectedSteamGames(SteamInstallDir.Text);

            SteamGamesList.Items.Clear();
            foreach (var game in detectedGames)
            {
                SteamGamesList.Items.Add(new ListViewItem(game.AppId.ToString())
                {
                    SubItems = {game.Name, game.InstallPath}
                });
            }
            SteamGamesList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void SelectedGameWonDirChanged(object sender, EventArgs e)
        {
            if (SelectedGameEngine.SelectedIndex < 0) return;
            var eng = (Engine) SelectedGameEngine.SelectedItem;

            SelectedGameMod.Items.Clear();
            SelectedGameBase.Items.Clear();
            SelectedGameExecutable.Items.Clear();

            if (!Directory.Exists(SelectedGameWonDir.Text)) return;

            var mods = Directory.GetDirectories(SelectedGameWonDir.Text).Select(Path.GetFileName);
            var ignored = new[] { "gldrv", "logos", "logs", "errorlogs", "platform", "config" };

            var range = mods.Where(x => !ignored.Contains(x.ToLowerInvariant())).OfType<object>().ToArray();
            SelectedGameMod.Items.AddRange(range);
            SelectedGameBase.Items.AddRange(range);

            var exes = Directory.GetFiles(SelectedGameWonDir.Text, "*.exe").Select(Path.GetFileName);
            ignored = new[] { "sxuninst.exe", "utdel32.exe", "upd.exe", "hlds.exe", "hltv.exe" };
            range = exes.Where(x => !ignored.Contains(x.ToLowerInvariant())).OfType<object>().ToArray();
            SelectedGameExecutable.Items.AddRange(range);

            var idx = SelectedGameMod.Items.IndexOf(_selectedGame.ModDir ?? "");
            if (SelectedGameMod.Items.Count > 0) SelectedGameMod.SelectedIndex = Math.Max(0, idx);

            idx = SelectedGameBase.Items.IndexOf(_selectedGame.BaseDir ?? "");
            if (SelectedGameBase.Items.Count > 0) SelectedGameBase.SelectedIndex = Math.Max(0, idx);

            idx = SelectedGameExecutable.Items.IndexOf(_selectedGame.Executable ?? "");
            if (SelectedGameExecutable.Items.Count > 0) SelectedGameExecutable.SelectedIndex = Math.Max(0, idx);
        }

        private void SelectedGameNameChanged(object sender, EventArgs e)
        {
            if (_selectedGame == null) return;
            var idx = _games.IndexOf(_selectedGame).ToString(CultureInfo.InvariantCulture);
            var node = GameTree.Nodes.OfType<TreeNode>().SelectMany(x => x.Nodes.OfType<TreeNode>()).First(x => x.Name == idx);
            node.Text = SelectedGameName.Text;
        }

        private void SelectedGameUseDiffAutosaveDirChanged(object sender, EventArgs e)
        {
            SelectedGameDiffAutosaveDir.Enabled = SelectedGameUseDiffAutosaveDir.Checked;
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
            if (SelectedGameFgdList.SelectedIndices.Count > 0)
            {
                foreach (var idx in SelectedGameFgdList.SelectedIndices.OfType<int>().OrderByDescending(x => x).ToArray())
                {
                    _selectedGame.Fgds.RemoveAt(idx);
                }
                SelectedGameUpdateFgds();
            }
        }

        private void SelectedGameOverrideMapSizeChanged(object sender, EventArgs e)
        {
            SelectedGameOverrideSizeHigh.Enabled = SelectedGameOverrideSizeLow.Enabled = SelectedGameOverrideMapSize.Checked;
        }

        private void SelectedGameAddAdditionalPackageFileClicked(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "WAD files (*.wad)|*.wad", Multiselect = true })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (var fileName in ofd.FileNames)
                    {
                        _selectedGame.AdditionalPackages.Add(fileName);
                    }
                    SelectedGameUpdateAdditionalPackages();
                }
            }
        }

        private void SelectedGameAddAdditionalPackageFolderClicked(object sender, EventArgs e)
        {
            using (var ofd = new FolderBrowserDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _selectedGame.AdditionalPackages.Add(ofd.SelectedPath);
                    SelectedGameUpdateAdditionalPackages();
                }
            }
        }

        private void SelectedGameRemoveAdditionalPackageClicked(object sender, EventArgs e)
        {
            if (SelectedGameAdditionalPackageList.SelectedIndices.Count > 0)
            {
                foreach (var idx in SelectedGameAdditionalPackageList.SelectedIndices.OfType<int>().OrderByDescending(x => x).ToArray())
                {
                    _selectedGame.AdditionalPackages.RemoveAt(idx);
                }
                SelectedGameUpdateAdditionalPackages();
            }
        }

        #endregion

        #region Selected Build

        private Build _selectedBuild;
        private BuildProfile _selectedProfile;

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
            SelectedBuildCsg.Enabled = gs;
            if (change)
            {

                UpdateBuildTree();
                var node = BuildTree.Nodes.OfType<TreeNode>().SelectMany(x => x.Nodes.OfType<TreeNode>())
                    .First(x => x.Name == _builds.IndexOf(_selectedBuild).ToString());
                BuildTree.SelectedNode = node;
            }
        }

        private void SelectedBuildSpecificationChanged(object sender, EventArgs e)
        {
            var spec = SelectedBuildSpecification.SelectedItem as CompileSpecification;

            _selectedBuild.Specification = spec == null ? "" : spec.ID;

            SelectedBuildUpdateProfiles();
        }

        private bool _pauseProfileUpdates;

        private void SelectedBuildProfileChanged(object sender, EventArgs e)
        {
            if (_pauseProfileUpdates) return;
            _selectedProfile = SelectedBuildProfile.SelectedItem as BuildProfile;
            SelectedBuildUpdateCompileParameters(true);
        }

        private void SelectedBuildUpdateProfiles(bool addNew = false, string newName = "Default")
        {
            _pauseProfileUpdates = true;
            var setDefault = false;
            if (!_selectedBuild.Profiles.Any() || addNew)
            {
                _selectedProfile = new BuildProfile {ID = 1, BuildID = _selectedBuild.ID, Name = newName};
                _selectedBuild.Profiles.Add(_selectedProfile);
                setDefault = true;
            }

            SelectedBuildProfile.Items.Clear();
            foreach (var prof in _selectedBuild.Profiles)
            {
                SelectedBuildProfile.Items.Add(prof);
            }

            var idx = _selectedBuild.Profiles.IndexOf(_selectedProfile);
            if (idx < 0)
            {
                _selectedProfile = _selectedBuild.Profiles[0];
                idx = 0;
            }

            SelectedBuildProfile.SelectedIndex = idx;

            SelectedBuildUpdateCompileParameters(!setDefault);

            if (setDefault)
            {
                _selectedProfile.RunCsg = SelectedBuildRunCsgCheckbox.Checked;
                _selectedProfile.RunBsp = SelectedBuildRunBspCheckbox.Checked;
                _selectedProfile.RunVis = SelectedBuildRunVisCheckbox.Checked;
                _selectedProfile.RunRad = SelectedBuildRunRadCheckbox.Checked;
                _selectedProfile.GeneratedCsgParameters = SelectedBuildCsgParameters.GeneratedCommands;
                _selectedProfile.AdditionalCsgParameters = SelectedBuildCsgParameters.AdditionalCommands;
                _selectedProfile.GeneratedBspParameters = SelectedBuildBspParameters.GeneratedCommands;
                _selectedProfile.GeneratedBspParameters = SelectedBuildBspParameters.AdditionalCommands;
                _selectedProfile.GeneratedVisParameters = SelectedBuildVisParameters.GeneratedCommands;
                _selectedProfile.GeneratedVisParameters = SelectedBuildVisParameters.AdditionalCommands;
                _selectedProfile.GeneratedRadParameters = SelectedBuildRadParameters.GeneratedCommands;
                _selectedProfile.GeneratedRadParameters = SelectedBuildRadParameters.AdditionalCommands;
                _selectedProfile.GeneratedSharedParameters = SelectedBuildSharedParameters.GeneratedCommands;
                _selectedProfile.GeneratedSharedParameters = SelectedBuildSharedParameters.AdditionalCommands;
            }
            _pauseProfileUpdates = false;
        }

        private void UpdateSelectedBuild()
        {
            BuildSubTabs.Visible = RemoveBuild.Enabled = _selectedBuild != null;
            if (_selectedBuild == null) return;
            if (!_selectedBuild.Profiles.Contains(_selectedProfile))
            {
                _selectedProfile = _selectedBuild.Profiles.FirstOrDefault();
            }
            SelectedBuildName.Text = _selectedBuild.Name;
            SelectedBuildEngine.SelectedIndex = Math.Max(0, Enum.GetValues(typeof(Engine)).OfType<Engine>().ToList<Engine>().FindIndex(x => x == _selectedBuild.Engine));
            SelectedBuildSpecification.SelectedIndex = Math.Max(0, CompileSpecification.Specifications.FindIndex(x => x.ID == _selectedBuild.Specification));
            SelectedBuildDontRedirectOutput.Checked = _selectedBuild.DontRedirectOutput;
            SelectedBuildExeFolder.Text = _selectedBuild.Path;
            SelectedBuildBsp.SelectedText = _selectedBuild.Bsp;
            SelectedBuildCsg.SelectedText = _selectedBuild.Csg;
            SelectedBuildVis.SelectedText = _selectedBuild.Vis;
            SelectedBuildRad.SelectedText = _selectedBuild.Rad;
            SelectedBuildIncludePathInEnvironment.Checked = _selectedBuild.IncludePathInEnvironment;

            SelectedBuildWorkingDirTemp.Checked = _selectedBuild.WorkingDirectory == CompileWorkingDirectory.TemporaryDirectory;
            SelectedBuildWorkingDirSame.Checked = _selectedBuild.WorkingDirectory == CompileWorkingDirectory.SameDirectory;
            SelectedBuildWorkingDirSub.Checked = _selectedBuild.WorkingDirectory == CompileWorkingDirectory.SubDirectory;

            SelectedBuildAfterCopyBsp.Checked = _selectedBuild.AfterCopyBsp;
            SelectedBuildAfterRunGame.Checked = _selectedBuild.AfterRunGame;
            SelectedBuildAskBeforeRun.Checked = _selectedBuild.AfterAskBeforeRun;

            SelectedBuildCopyBsp.Checked = _selectedBuild.CopyBsp;
            SelectedBuildCopyRes.Checked = _selectedBuild.CopyRes;
            SelectedBuildCopyLin.Checked = _selectedBuild.CopyLin;
            SelectedBuildCopyMap.Checked = _selectedBuild.CopyMap;
            SelectedBuildCopyPts.Checked = _selectedBuild.CopyPts;
            SelectedBuildCopyLog.Checked = _selectedBuild.CopyLog;
            SelectedBuildCopyErr.Checked = _selectedBuild.CopyErr;
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

        private void SelectedBuildUpdateCompileParameters(bool setValues)
        {
            SelectedBuildCsgParameters.ClearParameters();
            SelectedBuildBspParameters.ClearParameters();
            SelectedBuildVisParameters.ClearParameters();
            SelectedBuildRadParameters.ClearParameters();
            SelectedBuildSharedParameters.ClearParameters();
            SelectedBuildRunCsgCheckbox.Checked = SelectedBuildRunBspCheckbox.Checked = SelectedBuildRunVisCheckbox.Checked = SelectedBuildRunRadCheckbox.Checked = false;

            var spec = CompileSpecification.Specifications.FirstOrDefault(x => x.ID == _selectedBuild.Specification);
            if (spec == null) return;

            var prof = SelectedBuildProfile.SelectedItem as BuildProfile;
            if (prof == null) return;

            SelectedBuildRunCsgCheckbox.Checked = spec.GetDefaultRun("csg");
            SelectedBuildRunBspCheckbox.Checked = spec.GetDefaultRun("bsp");
            SelectedBuildRunVisCheckbox.Checked = spec.GetDefaultRun("vis");
            SelectedBuildRunRadCheckbox.Checked = spec.GetDefaultRun("rad");

            var csg = spec.GetTool("csg");
            if (csg != null)
            {
                SelectedBuildCsgParameters.AddParameters(csg.Parameters);
                SelectedBuildCsgParameters.SetDescription(csg.Description);
            }

            var bsp = spec.GetTool("bsp");
            if (bsp != null)
            {
                SelectedBuildBspParameters.AddParameters(bsp.Parameters);
                SelectedBuildBspParameters.SetDescription(bsp.Description);
            }

            var vis = spec.GetTool("vis");
            if (vis != null)
            {
                SelectedBuildVisParameters.AddParameters(vis.Parameters);
                SelectedBuildVisParameters.SetDescription(vis.Description);
            }

            var rad = spec.GetTool("rad");
            if (rad != null)
            {
                SelectedBuildRadParameters.AddParameters(rad.Parameters);
                SelectedBuildRadParameters.SetDescription(rad.Description);
            }

            var shared = spec.GetTool("shared");
            if (shared != null)
            {
                SelectedBuildSharedParameters.AddParameters(shared.Parameters);
                SelectedBuildSharedParameters.SetDescription(shared.Description);
            }

            if (setValues)
            {
                SelectedBuildCsgParameters.SetCommands(prof.GeneratedCsgParameters ?? "", prof.AdditionalCsgParameters ?? "");
                SelectedBuildBspParameters.SetCommands(prof.GeneratedBspParameters ?? "", prof.AdditionalBspParameters ?? "");
                SelectedBuildVisParameters.SetCommands(prof.GeneratedVisParameters ?? "", prof.AdditionalVisParameters ?? "");
                SelectedBuildRadParameters.SetCommands(prof.GeneratedRadParameters ?? "", prof.AdditionalRadParameters ?? "");
                SelectedBuildSharedParameters.SetCommands(prof.GeneratedSharedParameters ?? "", prof.AdditionalSharedParameters ?? "");
                SelectedBuildRunCsgCheckbox.Checked = prof.RunCsg;
                SelectedBuildRunBspCheckbox.Checked = prof.RunBsp;
                SelectedBuildRunVisCheckbox.Checked = prof.RunVis;
                SelectedBuildRunRadCheckbox.Checked = prof.RunRad;
            }
        }

        private void SelectedBuildRenameProfileButtonClicked(object sender, EventArgs e)
        {
            if (_selectedBuild == null || _selectedProfile == null) return;
            using (var qf = new QuickForm("Rename Build Profile").TextBox("Name", _selectedProfile.Name).OkCancel())
            {
                if (qf.ShowDialog() == DialogResult.OK)
                {
                    var name = qf.String("Name");
                    if (_selectedBuild.Profiles.Any(x => String.Equals(name, x.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        MessageBox.Show("There is already a profile with that name, please type a unique name.", "Cannot rename profile");
                        name = null;
                    }
                    if (!String.IsNullOrWhiteSpace(name) && _selectedProfile.Name != name)
                    {
                        _selectedProfile.Name = name;
                        SelectedBuildUpdateProfiles();
                    }
                }
            }
        }

        private void SelectedBuildDeleteProfileButtonClicked(object sender, EventArgs e)
        {
            if (_selectedProfile == null) return;
            if (MessageBox.Show("Are you sure you want to delete the '" + _selectedProfile.Name + "' profile?",
                    "Delete Build Profile", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _selectedBuild.Profiles.Remove(_selectedProfile);
                SelectedBuildUpdateProfiles();
            }
        }

        private void SelectedBuildNewProfileButtonClicked(object sender, EventArgs e)
        {
            if (_selectedBuild == null) return;
            using (var qf = new QuickForm("New Build Profile").TextBox("Name").OkCancel())
            {
                if (qf.ShowDialog() == DialogResult.OK)
                {
                    var name = qf.String("Name");
                    if (_selectedBuild.Profiles.Any(x => String.Equals(name, x.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        MessageBox.Show("There is already a profile with that name, please type a unique name.", "Cannot create profile");
                        name = null;
                    }
                    if (!String.IsNullOrWhiteSpace(name))
                    {
                        SelectedBuildUpdateProfiles(true, name);
                    }
                }
            }
        }

        private void UpdateSelectedBuildProfilePreview()
        {
            SelectedBuildProfilePreview.Text = "";
            if (_selectedProfile == null || _selectedBuild == null) return;
            var str = "";
            if (SelectedBuildRunCsgCheckbox.Checked)
            {
                str += _selectedBuild.Csg + ' ' + (_selectedProfile.GeneratedCsgParameters
                                                   + ' ' + _selectedProfile.AdditionalCsgParameters
                                                   + ' ' + _selectedProfile.GeneratedSharedParameters
                                                   + ' ' + _selectedProfile.AdditionalSharedParameters).Trim() + " <mapname>\r\n\r\n";
            }
            if (SelectedBuildRunBspCheckbox.Checked)
            {
                str += _selectedBuild.Bsp + ' ' + (_selectedProfile.GeneratedBspParameters
                                                   + ' ' + _selectedProfile.AdditionalBspParameters
                                                   + ' ' + _selectedProfile.GeneratedSharedParameters
                                                   + ' ' + _selectedProfile.AdditionalSharedParameters).Trim() + " <mapname>\r\n\r\n";
            }
            if (SelectedBuildRunVisCheckbox.Checked)
            {
                str += _selectedBuild.Vis + ' ' + (_selectedProfile.GeneratedVisParameters
                                                   + ' ' + _selectedProfile.AdditionalVisParameters
                                                   + ' ' + _selectedProfile.GeneratedSharedParameters
                                                   + ' ' + _selectedProfile.AdditionalSharedParameters).Trim() + " <mapname>\r\n\r\n";
            }
            if (SelectedBuildRunRadCheckbox.Checked)
            {
                str += _selectedBuild.Rad + ' ' + (_selectedProfile.GeneratedRadParameters
                                                   + ' ' + _selectedProfile.AdditionalRadParameters
                                                   + ' ' + _selectedProfile.GeneratedSharedParameters
                                                   + ' ' + _selectedProfile.AdditionalSharedParameters).Trim() + " <mapname>\r\n\r\n";
            }
            SelectedBuildProfilePreview.Text = str;
        }

        #endregion

        #region Hotkeys

        private class HotkeyQuickFormItem : QuickForms.Items.QuickFormTextBox
        {
            public HotkeyQuickFormItem(string tbname, string value) : base(tbname, value)
            {
            }
            public override List<Control> GetControls(QuickForm qf)
            {
                var ctrls = base.GetControls(qf);
                ctrls.OfType<TextBox>().First().KeyDown += HotkeyDown;
                return ctrls;
            }

            private void HotkeyDown(object sender, KeyEventArgs e)
            {
                e.SuppressKeyPress = e.Handled = true;
                ((TextBox) sender).Text = KeyboardState.KeysToString(e.KeyData);
            }
        }
        private void UpdateHotkeyList()
        {
            HotkeyList.BeginUpdate();
            var idx = HotkeyList.SelectedIndices.Count == 0 ? 0 : HotkeyList.SelectedIndices[0];
            HotkeyList.Items.Clear();
            foreach (var hotkey in _hotkeys.OrderBy(x => x.ID))
            {
                var def = Hotkeys.GetHotkeyDefinition(hotkey.ID);
                HotkeyList.Items.Add(new ListViewItem(new[]
                                                          {
                                                              def.Name,
                                                              def.Description,
                                                              String.IsNullOrWhiteSpace(hotkey.HotkeyString)
                                                                  ? "<unassigned>"
                                                                  : hotkey.HotkeyString
                                                          }) {Tag = hotkey});
            }
            HotkeyList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            if (idx >= 0 && idx < HotkeyList.Items.Count) HotkeyList.Items[idx].Selected = true;
            HotkeyList.EndUpdate();

            HotkeyActionList.BeginUpdate();
            idx = HotkeyActionList.SelectedIndex;
            HotkeyActionList.Items.Clear();
            foreach (var def in Hotkeys.GetHotkeyDefinitions().OrderBy(x => x.ID))
            {
                HotkeyActionList.Items.Add(def);
            }
            if (idx < 0 || idx >= HotkeyActionList.Items.Count) idx = 0;
            HotkeyActionList.SelectedIndex = idx;
            HotkeyActionList.EndUpdate();
        }

        private void DeleteHotkey(Hotkey hk)
        {
            var others = _hotkeys.Where(x => x.ID == hk.ID && x != hk).ToList();
            if (others.Any()) _hotkeys.Remove(hk);
            else hk.HotkeyString = "";
            UpdateHotkeyList();
        }

        private void EditHotkey(Hotkey hk)
        {
            using (var qf = new QuickForm("Enter New Hotkey")
                .Item(new HotkeyQuickFormItem("Hotkey", hk.HotkeyString))
                .OkCancel())
            {
                if (qf.ShowDialog() != DialogResult.OK) return;
                var key = qf.String("Hotkey");
                if (String.IsNullOrWhiteSpace(key)) return;

                var conflict = _hotkeys.FirstOrDefault(x => x.HotkeyString == key && x != hk);
                if (conflict != null)
                {
                    if (MessageBox.Show(key + " is already assigned to \"" + Hotkeys.GetHotkeyDefinition(conflict.ID) + "\".\n" +
                                        "Continue anyway?", "Conflict Detected", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return;
                    }
                }

                hk.HotkeyString = key;
                UpdateHotkeyList();
            }
        }

        private void HotkeyListKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && HotkeyList.SelectedItems.Count == 1)
            {
                DeleteHotkey((Hotkey)HotkeyList.SelectedItems[0].Tag);
            }
            else if (e.KeyCode == Keys.Enter && HotkeyList.SelectedItems.Count == 1)
            {
                EditHotkey((Hotkey)HotkeyList.SelectedItems[0].Tag);
            }
        }

        private void HotkeyReassignButtonClicked(object sender, EventArgs e)
        {
            if (HotkeyList.SelectedItems.Count == 1)
            {
                EditHotkey((Hotkey)HotkeyList.SelectedItems[0].Tag);
            }
        }

        private void HotkeyRemoveButtonClicked(object sender, EventArgs e)
        {
            if (HotkeyList.SelectedItems.Count == 1)
            {
                DeleteHotkey((Hotkey)HotkeyList.SelectedItems[0].Tag);
            }
        }

        private void HotkeyAddButtonClicked(object sender, EventArgs e)
        {
            var key = HotkeyCombination.Text;
            if (HotkeyActionList.SelectedIndex <= 0 || String.IsNullOrWhiteSpace(key)) return;
            var conflict = _hotkeys.FirstOrDefault(x => x.HotkeyString == key);
            if (conflict != null)
            {
                if (MessageBox.Show(key + " is already assigned to \"" + Hotkeys.GetHotkeyDefinition(conflict.ID) + "\".\n" +
                                    "Continue anyway?", "Conflict Detected", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }
            var def = (HotkeyDefinition) HotkeyActionList.SelectedItem;
            var blank = _hotkeys.FirstOrDefault(x => x.ID == def.ID && String.IsNullOrWhiteSpace(x.HotkeyString));
            if (blank == null) _hotkeys.Add(new Hotkey { ID = def.ID, HotkeyString = key });
            else blank.HotkeyString = key;
            HotkeyCombination.Text = "";
            UpdateHotkeyList();
        }

        private void HotkeyCombinationKeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            e.Handled = true;
            HotkeyCombination.Text = KeyboardState.KeysToString(e.KeyData);
        }

        protected override bool ProcessTabKey(bool forward)
        {
            if (HotkeyCombination.Focused) return false;
            return base.ProcessTabKey(forward);
        }

        private void HotkeyResetButtonClicked(object sender, EventArgs e)
        {
            _hotkeys.Clear();
            foreach (var def in Hotkeys.GetHotkeyDefinitions())
            {
                foreach (var hk in def.DefaultHotkeys)
                {
                    _hotkeys.Add(new Hotkey {ID = def.ID, HotkeyString = hk});
                }
            }
            UpdateHotkeyList();
        }
        #endregion

        private void HotkeyListDoubleClicked(object sender, MouseEventArgs e)
        {

        }
    }
}
