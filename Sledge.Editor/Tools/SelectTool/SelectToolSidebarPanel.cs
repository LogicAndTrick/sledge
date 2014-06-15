using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.Editor.Brushes;
using Sledge.Editor.Tools.SelectTool.TransformationTools;
using Sledge.Settings;

namespace Sledge.Editor.Tools.SelectTool
{
    public partial class SelectToolSidebarPanel : UserControl
    {
        public delegate void ChangeTransformationToolEventHandler(object sender, Type transformationToolType);
        public delegate void ToggleShow3DWidgetsEventHandler(object sender, bool show);

        public event ChangeTransformationToolEventHandler ChangeTransformationTool;
        public event ToggleShow3DWidgetsEventHandler ToggleShow3DWidgets;

        protected virtual void OnChangeTransformationTool(Type transformationToolType)
        {
            if (ChangeTransformationTool != null)
            {
                ChangeTransformationTool(this, transformationToolType);
            }
        }

        protected virtual void OnToggleShow3DWidgets(bool show)
        {
            if (ToggleShow3DWidgets != null)
            {
                ToggleShow3DWidgets(this, show);
            }
        }

        public SelectToolSidebarPanel()
        {
            InitializeComponent();
            Show3DWidgetsCheckbox.Checked = Sledge.Settings.Select.Show3DSelectionWidgets;
        }

        private Type _selectedType;

        public void TransformationToolChanged(TransformationTool tt)
        {
            _selectedType = tt == null ? null : tt.GetType();
            SetCheckState();
        }

        private void SetCheckState()
        {
            if (_selectedType == null)
            {
                _selectedType = null;
                RotateModeCheckbox.Enabled = SkewModeCheckbox.Enabled = TranslateModeCheckbox.Enabled = false;
                RotateModeCheckbox.Checked = SkewModeCheckbox.Checked = TranslateModeCheckbox.Checked = false;
                return;
            }

            RotateModeCheckbox.Enabled = SkewModeCheckbox.Enabled = TranslateModeCheckbox.Enabled = true;

            if (_selectedType == typeof(ResizeTool))
            {
                RotateModeCheckbox.Checked = SkewModeCheckbox.Checked = false;
                TranslateModeCheckbox.Checked = true;
            }
            else if (_selectedType == typeof(RotateTool))
            {
                TranslateModeCheckbox.Checked = SkewModeCheckbox.Checked = false;
                RotateModeCheckbox.Checked = true;
            }
            else if (_selectedType == typeof(SkewTool))
            {
                RotateModeCheckbox.Checked = TranslateModeCheckbox.Checked = false;
                SkewModeCheckbox.Checked = true;
            }
        }

        private void TranslateModeChecked(object sender, EventArgs e)
        {
            if (TranslateModeCheckbox.Checked && _selectedType != typeof(ResizeTool)) OnChangeTransformationTool(typeof(ResizeTool));
            else SetCheckState();
        }

        private void RotateModeChecked(object sender, EventArgs e)
        {
            if (RotateModeCheckbox.Checked && _selectedType != typeof(RotateTool)) OnChangeTransformationTool(typeof(RotateTool));
            else SetCheckState();
        }

        private void SkewModeChecked(object sender, EventArgs e)
        {
            if (SkewModeCheckbox.Checked && _selectedType != typeof(SkewTool)) OnChangeTransformationTool(typeof(SkewTool));
            else SetCheckState();
        }

        private void Show3DWidgetsChecked(object sender, EventArgs e)
        {
            OnToggleShow3DWidgets(Show3DWidgetsCheckbox.Checked);
        }

        private void MoveToWorldButtonClicked(object sender, EventArgs e)
        {
            Mediator.Publish(HotkeysMediator.TieToWorld);
        }

        private void TieToEntityButtonClicked(object sender, EventArgs e)
        {
            Mediator.Publish(HotkeysMediator.TieToEntity);
        }
    }
}
