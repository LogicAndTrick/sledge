using System.Collections.Generic;
using System.Drawing;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools.DisplacementTools;
using Sledge.Rendering;
using Sledge.Settings;
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

        public override void MouseDown(MapViewport viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.MouseDown(viewport, e);
        }

        public override void KeyDown(MapViewport viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.KeyDown(viewport, e);
        }

        public void Render(MapViewport viewport)
        {
            //if (_currentTool != null) _currentTool.Render(viewport);
        }

        public override void MouseEnter(MapViewport viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.MouseEnter(viewport, e);
        }

        public override void MouseLeave(MapViewport viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.MouseLeave(viewport, e);
        }

        public override void MouseClick(MapViewport viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.MouseClick(viewport, e);
        }

        public override void MouseDoubleClick(MapViewport viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.MouseDoubleClick(viewport, e);
        }

        public override void MouseUp(MapViewport viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.MouseUp(viewport, e);
        }

        public override void MouseWheel(MapViewport viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.MouseWheel(viewport, e);
        }

        public override void MouseMove(MapViewport viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.MouseMove(viewport, e);
        }

        public override void KeyPress(MapViewport viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.KeyPress(viewport, e);
        }

        public override void KeyUp(MapViewport viewport, ViewportEvent e)
        {
            if (_currentTool != null) _currentTool.KeyUp(viewport, e);
        }

        public override void UpdateFrame(MapViewport viewport, Frame frame)
        {
            if (_currentTool != null) _currentTool.UpdateFrame(viewport, frame);
        }

        public void PreRender(MapViewport viewport)
        {
            //if (_currentTool != null) _currentTool.PreRender(viewport);
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
