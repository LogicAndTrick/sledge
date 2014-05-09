using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.QuickForms;
using Sledge.Settings;
using Sledge.Settings.Models;

namespace Sledge.Editor.Compiling
{
    public partial class CompileDialog : Form
    {
        private CompileSpecification _specification;
        private BuildProfile _profile;
        private CompilePreset _preset;
        private Build _build;

        private Size _simpleSize = new Size(320, 450);
        private Size _advancedSize = new Size(750, 550);

        public CompileDialog(Build build)
        {
            InitializeComponent();

            // Hide the panels
            AdvancedPanel.Size = Size.Empty;
            SimplePanel.Size = Size.Empty;

            // Open the default mode
            (Sledge.Settings.View.CompileDefaultAdvanced ? AdvancedPanel : SimplePanel).Dock = DockStyle.Fill;
            Size = (Sledge.Settings.View.CompileDefaultAdvanced ? _advancedSize : _simpleSize);

            _build = build;
            _specification = CompileSpecification.Specifications.FirstOrDefault(x => x.ID == build.Specification) ??
                             CompileSpecification.Specifications.FirstOrDefault() ??
                             new CompileSpecification {ID = "", Name = "No Specification Found"};

            Text = "Compile Map - " + _specification.Name;
            AddParameters();
            UpdateProfiles();
        }

        private void UpdateProfiles()
        {
            if (!_build.Profiles.Any())
            {
                _profile = CreateProfile("Default");
            }
            var profs = _build.Profiles;
            var idx = profs.IndexOf(_profile);
            if (idx < 0) idx = ProfileSelect.SelectedIndex;

            ProfileSelect.Items.Clear();
            PresetTable.Controls.Clear();
            PresetTable.RowStyles.Clear();

            if (_specification.Presets.Any())
            {
                PresetTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                PresetTable.Controls.Add(new Label
                {
                    Text = "Choose a preset to use for the compile:",
                    Dock = DockStyle.Top
                });

                foreach (var preset in _specification.Presets)
                {
                    PresetTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    var btn = new HeadingButton
                    {
                        HeadingText = preset.Name,
                        Text = preset.Description,
                        Dock = DockStyle.Top
                    };
                    var pre = preset;
                    btn.Click += (s, e) => UsePreset(pre);
                    PresetTable.Controls.Add(btn);
                }

                PresetTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                PresetTable.Controls.Add(new Label {Text = "Or select from a custom profile:", Dock = DockStyle.Top});
            }
            else
            {
                PresetTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                PresetTable.Controls.Add(new Label { Text = "Select a profile to use for the compile:", Dock = DockStyle.Top });
            }

            foreach (var profile in profs)
            {
                ProfileSelect.Items.Add(profile);
                PresetTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                var btn = new Button
                {
                    Text = profile.Name,
                    Height = 30,
                    Dock = DockStyle.Top
                };
                var pro = profile;
                btn.Click += (s, e) => UseProfile(pro);
                PresetTable.Controls.Add(btn);
            }
            if (idx < 0 || idx >= profs.Count) idx = 0;
            if (ProfileSelect.SelectedIndex != idx) ProfileSelect.SelectedIndex = idx;
        }

        private void UsePreset(CompilePreset preset)
        {
            _preset = preset;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void UseProfile(BuildProfile pro)
        {
            _profile = pro;
            UpdateParameters(pro);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void AddParameters()
        {
            var csg = _specification.GetTool("csg");
            if (csg != null)
            {
                CsgParameters.AddParameters(csg.Parameters);
                CsgParameters.SetDescription(csg.Description);
            }

            var bsp = _specification.GetTool("bsp");
            if (bsp != null)
            {
                BspParameters.AddParameters(bsp.Parameters);
                BspParameters.SetDescription(bsp.Description);
            }

            var vis = _specification.GetTool("vis");
            if (vis != null)
            {
                VisParameters.AddParameters(vis.Parameters);
                VisParameters.SetDescription(vis.Description);
            }

            var rad = _specification.GetTool("rad");
            if (rad != null)
            {
                RadParameters.AddParameters(rad.Parameters);
                RadParameters.SetDescription(rad.Description);
            }

            var shared = _specification.GetTool("shared");
            if (shared != null)
            {
                SharedParameters.AddParameters(shared.Parameters);
                SharedParameters.SetDescription(shared.Description);
            }
        }

        private void ProfileSelected(object sender, EventArgs e)
        {
            _profile = ProfileSelect.SelectedItem as BuildProfile;
            if (_profile == null) return;
            UpdateParameters(_profile);
        }

        private void UpdateParameters(BuildProfile prof)
        {
            RunCsgCheckbox.Checked = prof.RunCsg;
            RunBspCheckbox.Checked = prof.RunBsp;
            RunVisCheckbox.Checked = prof.RunVis;
            RunRadCheckbox.Checked = prof.RunRad;
            CsgParameters.SetCommands(prof.GeneratedCsgParameters ?? "", prof.AdditionalCsgParameters ?? "");
            BspParameters.SetCommands(prof.GeneratedBspParameters ?? "", prof.AdditionalBspParameters ?? "");
            VisParameters.SetCommands(prof.GeneratedVisParameters ?? "", prof.AdditionalVisParameters ?? "");
            RadParameters.SetCommands(prof.GeneratedRadParameters ?? "", prof.AdditionalRadParameters ?? "");
            SharedParameters.SetCommands(prof.GeneratedSharedParameters ?? "", prof.AdditionalSharedParameters ?? "");
        }

        private void RenameProfileButtonClicked(object sender, EventArgs e)
        {
            if (_profile == null) return;
            using (var qf = new QuickForm("Rename Build Profile").TextBox("Name", _profile.Name).OkCancel())
            {
                if (qf.ShowDialog() == DialogResult.OK)
                {
                    var name = qf.String("Name");
                    if (_build.Profiles.Any(x => String.Equals(name, x.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        MessageBox.Show("There is already a profile with that name, please type a unique name.", "Cannot rename profile");
                        name = null;
                    }
                    if (!String.IsNullOrWhiteSpace(name) && _profile.Name != name)
                    {
                        _profile.Name = name;
                        SettingsManager.Write();
                        UpdateProfiles();
                    }
                }
            }
        }

        private void DeleteProfileButtonClicked(object sender, EventArgs e)
        {
            if (_profile == null) return;
            if (MessageBox.Show("Are you sure you want to delete the '" + _profile.Name + "' profile?",
                    "Delete Build Profile", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _build.Profiles.Remove(_profile);
                SettingsManager.Write();
                UpdateProfiles();
            }
        }

        private void NewProfileButtonClicked(object sender, EventArgs e)
        {
            using (var qf = new QuickForm("New Build Profile").TextBox("Name").OkCancel())
            {
                if (qf.ShowDialog() == DialogResult.OK)
                {
                    var name = qf.String("Name");
                    if (_build.Profiles.Any(x => String.Equals(name, x.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        MessageBox.Show("There is already a profile with that name, please type a unique name.", "Cannot create profile");
                        name = null;
                    }
                    if (!String.IsNullOrWhiteSpace(name))
                    {
                        _profile = CreateProfile(name);
                        UpdateProfiles();
                    }
                }
            }
        }

        private BuildProfile CreateProfile(string name)
        {
            var prof = new BuildProfile
            {
                ID = _build.Profiles.Any() ? _build.Profiles.Max(x => x.ID) + 1 : 1,
                BuildID = _build.ID,
                Name = name,
                RunCsg = _specification.GetDefaultRun("csg"),
                RunBsp = _specification.GetDefaultRun("bsp"),
                RunVis = _specification.GetDefaultRun("vis"),
                RunRad = _specification.GetDefaultRun("rad"),
                GeneratedCsgParameters = _specification.GetDefaultParameters("csg"),
                GeneratedBspParameters = _specification.GetDefaultParameters("bsp"),
                GeneratedVisParameters = _specification.GetDefaultParameters("vis"),
                GeneratedRadParameters = _specification.GetDefaultParameters("rad"),
                GeneratedSharedParameters = _specification.GetDefaultParameters("shared"),
                AdditionalCsgParameters = "",
                AdditionalBspParameters = "",
                AdditionalVisParameters = "",
                AdditionalRadParameters = "",
                AdditionalSharedParameters = ""
            };
            _build.Profiles.Add(prof);
            SettingsManager.Write();
            return prof;
        }

        private BuildProfile SaveAsProfile(string name)
        {
            var prof = new BuildProfile
            {
                ID = _build.Profiles.Any() ? _build.Profiles.Max(x => x.ID) + 1 : 1,
                BuildID = _build.ID,
                Name = name,
                RunCsg = RunCsgCheckbox.Checked,
                RunBsp = RunBspCheckbox.Checked,
                RunVis = RunVisCheckbox.Checked,
                RunRad = RunRadCheckbox.Checked,
                GeneratedCsgParameters = CsgParameters.GeneratedCommands,
                GeneratedBspParameters = BspParameters.GeneratedCommands,
                GeneratedVisParameters = VisParameters.GeneratedCommands,
                GeneratedRadParameters = RadParameters.GeneratedCommands,
                GeneratedSharedParameters = SharedParameters.GeneratedCommands,
                AdditionalCsgParameters = CsgParameters.AdditionalCommands,
                AdditionalBspParameters = BspParameters.AdditionalCommands,
                AdditionalVisParameters = VisParameters.AdditionalCommands,
                AdditionalRadParameters = RadParameters.AdditionalCommands,
                AdditionalSharedParameters = SharedParameters.AdditionalCommands
            };
            _build.Profiles.Add(prof);
            SettingsManager.Write();
            return prof;
        }

        private void SaveProfileButtonClicked(object sender, EventArgs e)
        {
            if (_profile == null) return;
            _profile.RunCsg = RunCsgCheckbox.Checked;
            _profile.RunBsp = RunBspCheckbox.Checked;
            _profile.RunVis = RunVisCheckbox.Checked;
            _profile.RunRad = RunRadCheckbox.Checked;
            _profile.GeneratedCsgParameters = CsgParameters.GeneratedCommands;
            _profile.GeneratedBspParameters = BspParameters.GeneratedCommands;
            _profile.GeneratedVisParameters = VisParameters.GeneratedCommands;
            _profile.GeneratedRadParameters = RadParameters.GeneratedCommands;
            _profile.GeneratedSharedParameters = SharedParameters.GeneratedCommands;
            _profile.AdditionalCsgParameters = CsgParameters.AdditionalCommands;
            _profile.AdditionalBspParameters = BspParameters.AdditionalCommands;
            _profile.AdditionalVisParameters = VisParameters.AdditionalCommands;
            _profile.AdditionalRadParameters = RadParameters.AdditionalCommands;
            _profile.AdditionalSharedParameters = SharedParameters.AdditionalCommands;
            SettingsManager.Write();
        }

        private void SaveProfileAsButtonClicked(object sender, EventArgs e)
        {
            using (var qf = new QuickForm("Save Build Profile As...").TextBox("Name").OkCancel())
            {
                if (qf.ShowDialog() == DialogResult.OK)
                {
                    var name = qf.String("Name");
                    if (_build.Profiles.Any(x => String.Equals(name, x.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        MessageBox.Show("There is already a profile with that name, please type a unique name.", "Cannot create profile");
                        name = null;
                    }
                    if (!String.IsNullOrWhiteSpace(name))
                    {
                        _profile = SaveAsProfile(name);
                        UpdateProfiles();
                    }
                }
            }
        }

        private void UpdatePreview(object sender, EventArgs e)
        {
            ProfilePreview.Text = "";
            if (_profile == null) return;
            var str = "";
            if (RunCsgCheckbox.Checked)
            {
                str += _build.Csg + ' ' + (CsgParameters.GeneratedCommands
                                           + ' ' + CsgParameters.AdditionalCommands
                                           + ' ' + SharedParameters.GeneratedCommands
                                           + ' ' + SharedParameters.AdditionalCommands).Trim() + " <mapname>\r\n\r\n";
            }
            if (RunBspCheckbox.Checked)
            {
                str += _build.Bsp
                       + ' ' + (BspParameters.GeneratedCommands
                                + ' ' + BspParameters.AdditionalCommands
                                + ' ' + SharedParameters.GeneratedCommands
                                + ' ' + SharedParameters.AdditionalCommands).Trim() + " <mapname>\r\n\r\n";
            }
            if (RunVisCheckbox.Checked)
            {
                str += _build.Vis
                       + ' ' + (VisParameters.GeneratedCommands
                                + ' ' + VisParameters.AdditionalCommands
                                + ' ' + SharedParameters.GeneratedCommands
                                + ' ' + SharedParameters.AdditionalCommands).Trim() + " <mapname>\r\n\r\n";
            }
            if (RunRadCheckbox.Checked)
            {
                str += _build.Rad
                       + ' ' + (RadParameters.GeneratedCommands
                                + ' ' + RadParameters.AdditionalCommands
                                + ' ' + SharedParameters.GeneratedCommands
                                + ' ' + SharedParameters.AdditionalCommands).Trim() + " <mapname>\r\n\r\n";
            }
            ProfilePreview.Text = str;
        }

        public BuildProfile GetProfile()
        {
            if (_preset != null)
            {
                return new BuildProfile
                {
                    BuildID = _build.ID,
                    Name = _preset.Name,
                    RunCsg = _preset.RunCsg,
                    RunBsp = _preset.RunBsp,
                    RunVis = _preset.RunVis,
                    RunRad = _preset.RunRad,
                    GeneratedCsgParameters = _preset.Csg,
                    GeneratedBspParameters = _preset.Bsp,
                    GeneratedVisParameters = _preset.Vis,
                    GeneratedRadParameters = _preset.Rad
                };
            }
            return new BuildProfile
            {
                BuildID = _build.ID,
                Name = _profile == null ? "" : _profile.Name,
                RunCsg = RunCsgCheckbox.Checked,
                RunBsp = RunBspCheckbox.Checked,
                RunVis = RunVisCheckbox.Checked,
                RunRad = RunRadCheckbox.Checked,
                GeneratedCsgParameters = CsgParameters.GeneratedCommands,
                GeneratedBspParameters = BspParameters.GeneratedCommands,
                GeneratedVisParameters = VisParameters.GeneratedCommands,
                GeneratedRadParameters = RadParameters.GeneratedCommands,
                GeneratedSharedParameters = SharedParameters.GeneratedCommands,
                AdditionalCsgParameters = CsgParameters.AdditionalCommands,
                AdditionalBspParameters = BspParameters.AdditionalCommands,
                AdditionalVisParameters = VisParameters.AdditionalCommands,
                AdditionalRadParameters = RadParameters.AdditionalCommands,
                AdditionalSharedParameters = SharedParameters.AdditionalCommands
            };
        }

        private void SwitchToAdvanced(object sender, EventArgs e)
        {
            if (AdvancedPanel.Dock == DockStyle.Fill) return;
            SimplePanel.Dock = DockStyle.None;
            AdvancedPanel.Dock = DockStyle.Fill;
            _simpleSize = Size;
            Size = _advancedSize;
        }

        private void SwitchToSimple(object sender, EventArgs e)
        {
            if (SimplePanel.Dock == DockStyle.Fill) return;
            AdvancedPanel.Dock = DockStyle.None;
            SimplePanel.Dock = DockStyle.Fill;
            _advancedSize = Size;
            Size = _simpleSize;
        }
    }
}
