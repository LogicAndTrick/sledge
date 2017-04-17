using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;

namespace Sledge.BspEditor.Tools.Selection
{
    [Export(typeof(ISidebarComponent))]
    public partial class SelectToolSidebarPanel : UserControl, ISidebarComponent
    {
        public delegate void ChangeTransformationToolEventHandler(object sender, SelectionBoxDraggableState.TransformationMode transformationMode);
        public delegate void ToggleShow3DWidgetsEventHandler(object sender, bool show);

        public event ChangeTransformationToolEventHandler ChangeTransformationMode;
        public event ToggleShow3DWidgetsEventHandler ToggleShow3DWidgets;

        protected virtual void OnChangeTransformationMode(SelectionBoxDraggableState.TransformationMode transformationMode)
        {
            ChangeTransformationMode?.Invoke(this, transformationMode);
        }

        protected virtual void OnToggleShow3DWidgets(bool show)
        {
            ToggleShow3DWidgets?.Invoke(this, show);
        }

        public string Title => "Selection Tool";
        public object Control => this;

        public SelectToolSidebarPanel()
        {
            InitializeComponent();
            //Show3DWidgetsCheckbox.Checked = Sledge.Settings.Select.Show3DSelectionWidgets;
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
            if (TranslateModeCheckbox.Checked && _selectedType != SelectionBoxDraggableState.TransformationMode.Resize) OnChangeTransformationMode(SelectionBoxDraggableState.TransformationMode.Resize);
            else SetCheckState();
        }

        private void RotateModeChecked(object sender, EventArgs e)
        {
            if (RotateModeCheckbox.Checked && _selectedType != SelectionBoxDraggableState.TransformationMode.Rotate) OnChangeTransformationMode(SelectionBoxDraggableState.TransformationMode.Rotate);
            else SetCheckState();
        }

        private void SkewModeChecked(object sender, EventArgs e)
        {
            if (SkewModeCheckbox.Checked && _selectedType != SelectionBoxDraggableState.TransformationMode.Skew) OnChangeTransformationMode(SelectionBoxDraggableState.TransformationMode.Skew);
            else SetCheckState();
        }

        private void Show3DWidgetsChecked(object sender, EventArgs e)
        {
            OnToggleShow3DWidgets(Show3DWidgetsCheckbox.Checked);
        }

        private void MoveToWorldButtonClicked(object sender, EventArgs e)
        {
            //Mediator.Publish(HotkeysMediator.TieToWorld);
        }

        private void TieToEntityButtonClicked(object sender, EventArgs e)
        {
            //Mediator.Publish(HotkeysMediator.TieToEntity);
        }
    }
}
