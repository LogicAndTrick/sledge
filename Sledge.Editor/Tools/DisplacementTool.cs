using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Sledge.Editor.Tools.DisplacementTools;
using Sledge.UI;
using Sledge.Editor.Properties;
using Sledge.Editor.Editing;

namespace Sledge.Editor.Tools
{
    public class DisplacementTool : Base3DTool
    {

        private readonly DisplacementForm _form;
        private readonly List<DisplacementSubTool> _tools;
        private DisplacementSubTool _currentTool;

        public DisplacementTool()
        {
            _form = new DisplacementForm();
            _form.ToolSelected += DisplacementToolSelected;
            _tools = new List<DisplacementSubTool>();

            AddTool(new DisplacementTools.SelectTool());
            AddTool(new GeometryTool());
        }

        private void AddTool(DisplacementSubTool tool)
        {
            _form.AddTool(tool);
            _tools.Add(tool);
        }

        private void DisplacementToolSelected(object sender, DisplacementSubTool tool)
        {
            if (_currentTool != null) _currentTool.ToolDeselected();
            _currentTool = tool;
            if (_currentTool != null) _currentTool.ToolSelected();
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Texture;
        }

        public override string GetName()
        {
            return "Displacement Tool";
        }

        public override void ToolSelected()
        {
            _form.Show(Editor.Instance);
            Editor.Instance.Focus();
            Selection.SwitchToFaceSelection();
            Document.UpdateDisplayLists(true);

            if (_currentTool != null) _currentTool.ToolSelected();
        }

        public override void ToolDeselected()
        {
            Selection.SwitchToObjectSelection();
            _form.Hide();
            Document.UpdateDisplayLists();

            if (_currentTool != null) _currentTool.ToolDeselected();
        }

        public override void MouseDown(ViewportBase viewport, MouseEventArgs e)
        {
            if (_currentTool != null) _currentTool.MouseDown(viewport, e);
        }

        public override void KeyDown(ViewportBase viewport, KeyEventArgs e)
        {
            if (_currentTool != null) _currentTool.KeyDown(viewport, e);
        }

        public override void Render(ViewportBase viewport)
        {
            if (_currentTool != null) _currentTool.Render(viewport);
        }

        public override void MouseEnter(ViewportBase viewport, EventArgs e)
        {
            if (_currentTool != null) _currentTool.MouseEnter(viewport, e);
        }

        public override void MouseLeave(ViewportBase viewport, EventArgs e)
        {
            if (_currentTool != null) _currentTool.MouseLeave(viewport, e);
        }

        public override void MouseUp(ViewportBase viewport, MouseEventArgs e)
        {
            if (_currentTool != null) _currentTool.MouseUp(viewport, e);
        }

        public override void MouseWheel(ViewportBase viewport, MouseEventArgs e)
        {
            if (_currentTool != null) _currentTool.MouseWheel(viewport, e);
        }

        public override void MouseMove(ViewportBase viewport, MouseEventArgs e)
        {
            if (_currentTool != null) _currentTool.MouseMove(viewport, e);
        }

        public override void KeyPress(ViewportBase viewport, KeyPressEventArgs e)
        {
            if (_currentTool != null) _currentTool.KeyPress(viewport, e);
        }

        public override void KeyUp(ViewportBase viewport, KeyEventArgs e)
        {
            if (_currentTool != null) _currentTool.KeyUp(viewport, e);
        }

        public override void UpdateFrame(ViewportBase viewport)
        {
            if (_currentTool != null) _currentTool.UpdateFrame(viewport);
        }

        public override void PreRender(ViewportBase viewport)
        {
            if (_currentTool != null) _currentTool.PreRender(viewport);
        }
    }
}
