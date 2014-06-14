using System.Collections.Generic;
using System.Drawing;
using Sledge.Editor.Tools.DisplacementTools;
using Sledge.Settings;
using Sledge.UI;
using Sledge.Editor.Properties;

namespace Sledge.Editor.Tools
{
    public class DisplacementTool : BaseTool
    {

        private readonly DisplacementForm _form;
        private readonly List<DisplacementSubTool> _tools;
        private DisplacementSubTool _currentTool;

        public DisplacementTool()
        {
            Usage = ToolUsage.View3D;
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
            if (_currentTool != null) _currentTool.ToolDeselected(false);
            _currentTool = tool;
            if (_currentTool != null) _currentTool.ToolSelected(false);
        }

        public override void DocumentChanged()
        {
            _tools.ForEach(x => x.SetDocument(Document));
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Texture;
        }

        public override string GetName()
        {
            return "Displacement Tool";
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.Displacement;
        }

        public override string GetContextualHelp()
        {
            // todo sub tools
            return "*Click* a displacement to select it.";
        }

        public override void ToolSelected(bool preventHistory)
        {
            _form.Show(Editor.Instance);
            Editor.Instance.Focus();
            Document.Selection.SwitchToFaceSelection();
            //todo Document.Update Display Lists(); (see texture tool)

            if (_currentTool != null) _currentTool.ToolSelected(preventHistory);
        }

        public override void ToolDeselected(bool preventHistory)
        {
            Document.Selection.SwitchToObjectSelection();
            _form.Hide();
            //todo Document.Update Display Lists(); (see texture tool)

            if (_currentTool != null) _currentTool.ToolDeselected(preventHistory);
        }

        public override void MouseDown(ViewportBase viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.MouseDown(viewport, e);
        }

        public override void KeyDown(ViewportBase viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.KeyDown(viewport, e);
        }

        public override void Render(ViewportBase viewport)
        {
            if (_currentTool != null) _currentTool.Render(viewport);
        }

        public override void MouseEnter(ViewportBase viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.MouseEnter(viewport, e);
        }

        public override void MouseLeave(ViewportBase viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.MouseLeave(viewport, e);
        }

        public override void MouseClick(ViewportBase viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.MouseClick(viewport, e);
        }

        public override void MouseDoubleClick(ViewportBase viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.MouseDoubleClick(viewport, e);
        }

        public override void MouseUp(ViewportBase viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.MouseUp(viewport, e);
        }

        public override void MouseWheel(ViewportBase viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.MouseWheel(viewport, e);
        }

        public override void MouseMove(ViewportBase viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.MouseMove(viewport, e);
        }

        public override void KeyPress(ViewportBase viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.KeyPress(viewport, e);
        }

        public override void KeyUp(ViewportBase viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.KeyUp(viewport, e);
        }

        public override void UpdateFrame(ViewportBase viewport, FrameInfo frame)
        {
            if (_currentTool != null) _currentTool.UpdateFrame(viewport, frame);
        }

        public override void PreRender(ViewportBase viewport)
        {
            if (_currentTool != null) _currentTool.PreRender(viewport);
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            switch (hotkeyMessage)
            {
                case HotkeysMediator.OperationsCopy:
                case HotkeysMediator.OperationsCut:
                case HotkeysMediator.OperationsPaste:
                case HotkeysMediator.OperationsPasteSpecial:
                case HotkeysMediator.OperationsDelete:
                    return HotkeyInterceptResult.Abort;
            }
            return HotkeyInterceptResult.Continue;
        }
    }
}
