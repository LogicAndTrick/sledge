using System.Collections.Generic;
using System.Drawing;
using OpenTK.Input;
using Sledge.DataStructures.Geometric;
using Sledge.EditorNew.Actions.MapObjects.Operations;
using Sledge.EditorNew.Brushes;
using Sledge.EditorNew.Properties;
using Sledge.EditorNew.Tools.DraggableTool;
using Sledge.EditorNew.UI.Viewports;
using Sledge.Settings;

namespace Sledge.EditorNew.Tools.BrushTool
{
    public class BrushTool : BaseDraggableTool
    {
        private BoxDraggableState box;

        public BrushTool()
        {
            States = new Stack<IDraggableState>();

            box = new BoxDraggableState(this);
            box.BoxColour = Color.Turquoise;
            box.FillColour = Color.FromArgb(View.SelectionBoxBackgroundOpacity, Color.Green);
            States.Push(box);
        }

        public override IEnumerable<string> GetContexts()
        {
            yield return "Brush Tool";
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Brush;
        }

        public override string GetName()
        {
            return "Brush Tool";
        }

        public override void KeyDown(IMapViewport viewport, ViewportEvent e)
        {
            if (e.KeyValue == Key.Enter || e.KeyValue == Key.KeypadEnter)
            {
                Confirm();
            }
            else if (e.KeyValue == Key.Escape)
            {
                Cancel();
            }
            base.KeyDown(viewport, e);
        }

        private void Confirm()
        {
            if (box.State.Action != BoxAction.Drawn) return;
            var bbox = new Box(box.State.Start, box.State.End);
            var brush = new BlockBrush(); // todo brush selector
            var brushes = brush.Create(Document.Map.IDGenerator, bbox, null, 2);
            var add = new Create(Document.Map.WorldSpawn.ID, brushes);
            Document.PerformAction("Create " + brush.Name, add);
            box.State.Action = BoxAction.Idle;
        }

        private void Cancel()
        {
            box.State.Action = BoxAction.Idle;
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.Brush;
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            return HotkeyInterceptResult.Continue;
        }
    }
}