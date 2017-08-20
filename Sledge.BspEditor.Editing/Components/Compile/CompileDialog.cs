using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Sledge.BspEditor.Compile;
using Sledge.BspEditor.Editing.Components.Compile.Specification;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Components.Compile
{
    public sealed partial class CompileDialog : Form
    {
        private CompileSpecification _specification;
        private CompilePreset _preset;

        private Size _simpleSize = new Size(320, 450);
        private Size _advancedSize = new Size(750, 550);

        public CompileDialog(CompileSpecification specification)
        {
            InitializeComponent();

            _specification = specification;

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
    }
}
