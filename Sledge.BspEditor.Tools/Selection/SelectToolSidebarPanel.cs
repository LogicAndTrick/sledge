using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using Sledge.Shell;

namespace Sledge.BspEditor.Tools.Selection
{
    [Export(typeof(ISidebarComponent))]
    [OrderHint("F")]
    [AutoTranslate]
    public partial class SelectToolSidebarPanel : UserControl, ISidebarComponent, IManualTranslate
    {
        public string Title { get; set; } = "Selection Tool";
        public object Control => this;

        public SelectToolSidebarPanel()
        {
            InitializeComponent();

            Oy.Subscribe<String>("SelectTool:TransformationModeChanged", x =>
            {
                if (Enum.TryParse(x, out SelectionBoxDraggableState.TransformationMode mode))
                {
                    TransformationToolChanged(mode);
                }
            });

            Oy.Subscribe<String>("SelectTool:SetShow3DWidgets", x =>
            {
                Show3DWidgetsCheckbox.Checked = x == "1";
            });
        }

        public void Translate(ITranslationStringProvider strings)
        {
            CreateHandle();
            var prefix = GetType().FullName;
            this.InvokeLater(() =>
            {
                Title = strings.GetString(prefix, "Title");
                lblMode.Text = strings.GetString(prefix, "Mode");
                TranslateModeCheckbox.Text = strings.GetString(prefix, "Translate");
                RotateModeCheckbox.Text = strings.GetString(prefix, "Rotate");
                SkewModeCheckbox.Text = strings.GetString(prefix, "Skew");
                Show3DWidgetsCheckbox.Text = strings.GetString(prefix, "Show3DWidgets");
                lblActions.Text = strings.GetString(prefix, "Actions");
                MoveToWorldButton.Text = strings.GetString(prefix, "MoveToWorld");
                MoveToEntityButton.Text = strings.GetString(prefix, "TieToEntity");
            });
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveTool", out SelectTool _);
        }

        private SelectionBoxDraggableState.TransformationMode _selectedType;

        public void TransformationToolChanged(SelectionBoxDraggableState.TransformationMode tt)
        {
            _selectedType = tt;
            SetCheckState();
        }

        private void SetCheckState()
        {
            if (_selectedType == SelectionBoxDraggableState.TransformationMode.Resize)
            {
                RotateModeCheckbox.Checked = SkewModeCheckbox.Checked = false;
                TranslateModeCheckbox.Checked = true;
            }
            else if (_selectedType == SelectionBoxDraggableState.TransformationMode.Rotate)
            {
                TranslateModeCheckbox.Checked = SkewModeCheckbox.Checked = false;
                RotateModeCheckbox.Checked = true;
            }
            else if (_selectedType == SelectionBoxDraggableState.TransformationMode.Skew)
            {
                RotateModeCheckbox.Checked = TranslateModeCheckbox.Checked = false;
                SkewModeCheckbox.Checked = true;
            }
        }

        private void TranslateModeChecked(object sender, EventArgs e)
        {
            if (TranslateModeCheckbox.Checked && _selectedType != SelectionBoxDraggableState.TransformationMode.Resize)
                Oy.Publish("SelectTool:TransformationModeChanged", "Resize");
            else
                SetCheckState();
        }

        private void RotateModeChecked(object sender, EventArgs e)
        {
            if (RotateModeCheckbox.Checked && _selectedType != SelectionBoxDraggableState.TransformationMode.Rotate)
                Oy.Publish("SelectTool:TransformationModeChanged", "Rotate");
            else
                SetCheckState();
        }

        private void SkewModeChecked(object sender, EventArgs e)
        {
            if (SkewModeCheckbox.Checked && _selectedType != SelectionBoxDraggableState.TransformationMode.Skew)
                Oy.Publish("SelectTool:TransformationModeChanged", "Skew");
            else
                SetCheckState();
        }

        private void Show3DWidgetsChecked(object sender, EventArgs e)
        {
            Oy.Publish("SelectTool:Show3DWidgetsChanged", Show3DWidgetsCheckbox.Checked ? "1" : "0");
        }

        private void MoveToWorldButtonClicked(object sender, EventArgs e)
        {
            Oy.Publish("Command:Run", new CommandMessage("BspEditor:Tools:MoveToWorld"));
        }

        private void TieToEntityButtonClicked(object sender, EventArgs e)
        {
            Oy.Publish("Command:Run", new CommandMessage("BspEditor:Tools:TieToEntity"));
        }
    }
}
