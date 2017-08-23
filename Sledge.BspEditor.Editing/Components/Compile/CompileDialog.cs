using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.BspEditor.Compile;
using Sledge.BspEditor.Editing.Components.Compile.Profiles;
using Sledge.BspEditor.Editing.Components.Compile.Specification;
using Sledge.Common.Shell.Components;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Components.Compile
{
    public sealed partial class CompileDialog : Form
    {
        private CompileSpecification _specification;
        private readonly BuildProfileRegister _buildProfileRegister;
        private CompilePreset _preset;

        private Size _simpleSize = new Size(320, 450);
        private Size _advancedSize = new Size(750, 550);

        public CompileDialog(CompileSpecification specification, BuildProfileRegister buildProfileRegister)
        {
            InitializeComponent();

            _specification = specification;
            _buildProfileRegister = buildProfileRegister;

            // Hide the panels
            AdvancedPanel.Size = Size.Empty;
            SimplePanel.Size = Size.Empty;

            // Open the default mode
            SimplePanel.Dock = DockStyle.Fill;
            Size = _simpleSize;

            Text = _specification.Name;
            
            PresetTable.Controls.Clear();
            PresetTable.RowStyles.Clear();

            PopulatePresets();
            PopulateProfiles();
            PopulateTabs();

            btnAdvancedMode.Visible = false;
        }

        public IEnumerable<BatchArgument> SelectedBatchArguments
        {
            get
            {
                var batch = new List<BatchArgument>();
                if (_preset != null)
                {
                    foreach (var t in _specification.Tools)
                    {
                        if (_preset.ShouldRunTool(t.Name))
                        {
                            batch.Add(new BatchArgument {Name = t.Name, Arguments = _preset.GetArguments(t.Name)});
                        }
                    }
                }
                return batch;
            }
        }

        private void PopulatePresets()
        {
            PresetTable.Controls.Clear();
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
        }

        private void PopulateProfiles()
        {
            cmbProfile.Items.Clear();
            cmbProfile.Items.AddRange(_buildProfileRegister.GetProfiles(_specification.Name).Select(x => new ProfileWrapper(x)).ToArray<object>());
            if (cmbProfile.Items.Count > 0) cmbProfile.SelectedIndex = 0;
        }

        private void PopulateTabs()
        {
            pnlSteps.Controls.Clear();
            foreach (var page in ToolTabs.TabPages.OfType<TabPage>().ToList())
            {
                if (page != tabSteps) ToolTabs.TabPages.Remove(page);
            }

            foreach (var tool in _specification.Tools.OrderBy(x => x.Order))
            {
                if (!string.Equals(tool.Name, "Shared", StringComparison.InvariantCultureIgnoreCase))
                {
                    var cb = new CheckBox
                    {
                        Text = tool.Name,
                        Tag = tool,
                        Checked = tool.Enabled
                    };
                    pnlSteps.Controls.Add(cb);
                }

                var tab = new TabPage(tool.Name)
                {
                    Tag = tool
                };

                var bpp = new BuildParametersPanel {Dock = DockStyle.Fill};
                bpp.SetTool(tool);
                tab.Controls.Add(bpp);

                ToolTabs.TabPages.Add(tab);
            }
        }

        private void UsePreset(CompilePreset preset)
        {
            _preset = preset;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ProfileSelected(object sender, EventArgs e)
        {
            //_profile = ProfileSelect.SelectedItem as BuildProfile;
            //if (_profile == null) return;
            //UpdateParameters(_profile);
        }

        private void RenameProfileButtonClicked(object sender, EventArgs e)
        {

        }

        private void DeleteProfileButtonClicked(object sender, EventArgs e)
        {

        }

        private void NewProfileButtonClicked(object sender, EventArgs e)
        {

        }

        private void SaveProfileButtonClicked(object sender, EventArgs e)
        {

        }

        private void SaveProfileAsButtonClicked(object sender, EventArgs e)
        {

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

        private class ProfileWrapper
        {
            public BuildProfile Profile { get; set; }

            public ProfileWrapper(BuildProfile profile)
            {
                Profile = profile;
            }

            public override string ToString()
            {
                return Profile.Name;
            }
        }
    }
}
