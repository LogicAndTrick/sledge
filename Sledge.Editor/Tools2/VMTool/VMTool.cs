using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.Editor.Actions;
using Sledge.Editor.History;
using Sledge.Editor.Properties;
using Sledge.Editor.Tools;
using Sledge.Editor.Tools2.DraggableTool;
using Sledge.Editor.Tools2.VMTool.Actions;
using Sledge.Rendering.Scenes;
using Sledge.Settings;
using View = Sledge.Settings.View;

namespace Sledge.Editor.Tools2.VMTool
{
    public class VMTool : BaseDraggableTool
    {
    //    private readonly VMSidebarPanel _controlPanel;
    //    private readonly VMErrorsSidebarPanel _errorPanel;

        private readonly VMPointsDraggableState _pointState;
        private readonly BoxDraggableState _boxState;

        private ShowPoints _showPoints;

        public VMTool()
        {
            //_controlPanel = new VMSidebarPanel();
            //_errorPanel = new VMErrorsSidebarPanel();

            _pointState = new VMPointsDraggableState(this);

            _boxState = new BoxDraggableState(this);
            _boxState.BoxColour = Color.Orange;
            _boxState.FillColour = Color.FromArgb(View.SelectionBoxBackgroundOpacity, Color.DodgerBlue);

            States.Add(_pointState);
            States.Add(_boxState);

            UseValidation = true;
        }

        public override Image GetIcon()
        {
            return Resources.Tool_VM;
        }

        public override string GetName()
        {
            return "VMTool";
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.VM;
        }

        public override string GetContextualHelp()
        {
            return Children.Where(x => x.Active).Select(x => x.GetContextualHelp()).FirstOrDefault() ?? "Select a VM mode for more information";
        }

        public override IEnumerable<KeyValuePair<string, Control>> GetSidebarControls()
        {
            //yield return new KeyValuePair<string, Control>(GetName(), _controlPanel);
            //yield return new KeyValuePair<string, Control>("VM Errors", _errorPanel);
            yield break;
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            switch (hotkeyMessage)
            {
                case HotkeysMediator.HistoryUndo:
                    if (Document.History.CanUndo()) Document.History.Undo();
                    else MessageBox.Show("Nothing to undo in the VM tool"); // todo pop the stack and so on?
                    return HotkeyInterceptResult.Abort;
                case HotkeysMediator.HistoryRedo:
                    if (Document.History.CanRedo()) Document.History.Redo();
                    return HotkeyInterceptResult.Abort;
                case HotkeysMediator.OperationsPaste:
                case HotkeysMediator.OperationsPasteSpecial:
                    return HotkeyInterceptResult.SwitchToSelectTool;
                case HotkeysMediator.SwitchTool:
                    if (parameters is HotkeyTool && (HotkeyTool)parameters == GetHotkeyToolType())
                    {
                        CycleShowPoints();
                        return HotkeyInterceptResult.Abort;
                    }
                    break;
            }
            return HotkeyInterceptResult.Continue;
        }

        public void PerformAction(VMAction action)
        {
            try
            {
                action.Redo(Document);
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var frames = st.GetFrames() ?? new StackFrame[0];
                var msg = "Action exception: " + action.Name + " (" + action + ")";
                foreach (var frame in frames)
                {
                    var method = frame.GetMethod();
                    msg += "\r\n    " + method.ReflectedType.FullName + "." + method.Name;
                }
                Logging.Logger.ShowException(new Exception(msg, ex), "Error performing action");
            }

            Document.History.AddHistoryItem(action);
        }

        public void PerformAndCommitAction(string name, IAction action)
        {
            Document.History.PopStack();
            Document.PerformAction(name, action);
            Document.History.PushStack("VM Tool");
        }

        public new void Invalidate()
        {
            base.Invalidate();
        }

        private void CycleShowPoints()
        {
            var side = (int)_showPoints;
            side = (side + 1) % (Enum.GetValues(typeof(ShowPoints)).Length);
            _showPoints = (ShowPoints)side;
        }

        private void SelectionChanged()
        {
            _pointState.SelectionChanged();
        }

        public override void ToolSelected(bool preventHistory)
        {
            Document.History.PushStack("VM Tool");

            SelectionChanged();

            Mediator.Subscribe(EditorMediator.SelectionChanged, this);
            Mediator.Subscribe(HotkeysMediator.VMStandardMode, this);
            Mediator.Subscribe(HotkeysMediator.VMScalingMode, this);
            Mediator.Subscribe(HotkeysMediator.VMFaceEditMode, this);

            base.ToolSelected(preventHistory);
        }

        public override void ToolDeselected(bool preventHistory)
        {
            Mediator.UnsubscribeAll(this);

            _pointState.Commit();
            _pointState.Clear();

            Document.History.PopStack(); // todo push history collection

            base.ToolDeselected(preventHistory);
        }
    }
}
