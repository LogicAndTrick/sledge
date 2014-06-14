using System.Drawing;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Properties;
using Sledge.Settings;
using Sledge.UI;

namespace Sledge.Editor.Tools
{
    public class CordonTool : BaseBoxTool
    {
        public override Image GetIcon()
        {
            return Resources.Tool_Cordon;
        }

        public override string GetName()
        {
            return "Cordon Tool";
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.Cordon;
        }

        public override string GetContextualHelp()
        {
            return "Manipulate the box to define the cordon bounds for the map.";
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            switch (hotkeyMessage)
            {
                case HotkeysMediator.OperationsPasteSpecial:
                case HotkeysMediator.OperationsPaste:
                    return HotkeyInterceptResult.SwitchToSelectTool;
            }
            return HotkeyInterceptResult.Continue;
        }

        protected override Color BoxColour
        {
            get { return Color.Red; }
        }

        protected override Color FillColour
        {
            get { return Color.FromArgb(Sledge.Settings.View.SelectionBoxBackgroundOpacity, Color.LightGray); }
        }

        public override void ToolSelected(bool preventHistory)
        {
            State.BoxStart = Document.Map.CordonBounds.Start;
            State.BoxEnd = Document.Map.CordonBounds.End;
            State.Action = BoxAction.Drawn;
        }

        protected override void LeftMouseUpResizing(Viewport2D viewport, ViewportEvent e)
        {
            Document.Map.CordonBounds = new Box(State.BoxStart, State.BoxEnd);
            base.LeftMouseUpResizing(viewport, e);
        }

        protected override void Render2D(Sledge.UI.Viewport2D viewport)
        {
            if (State.Action == BoxAction.ReadyToDraw || State.Action == BoxAction.DownToDraw) return;
            var start = viewport.Flatten(State.BoxStart);
            var end = viewport.Flatten(State.BoxEnd);

            if (ShouldDrawBox(viewport))
            {
                var min = viewport.ScreenToWorld(0, 0);
                var max = viewport.ScreenToWorld(viewport.Width, viewport.Height);

                GL.Color4(Color.FromArgb(128, Color.Orange));
                GL.Begin(BeginMode.Quads);

                Coord(min.DX, min.DY, 0);
                Coord(max.DX, min.DY, 0);
                Coord(max.DX, start.DY, 0);
                Coord(min.DX, start.DY, 0);

                Coord(min.DX, end.DY, 0);
                Coord(max.DX, end.DY, 0);
                Coord(max.DX, max.DY, 0);
                Coord(min.DX, max.DY, 0);

                Coord(min.DX, start.DY, 0);
                Coord(start.DX, start.DY, 0);
                Coord(start.DX, end.DY, 0);
                Coord(min.DX, end.DY, 0);

                Coord(end.DX, start.DY, 0);
                Coord(max.DX, start.DY, 0);
                Coord(max.DX, end.DY, 0);
                Coord(end.DX, end.DY, 0);

                GL.End();


                GL.LineWidth(2);
                GL.Begin(BeginMode.LineLoop);
                GL.Color3(GetRenderBoxColour());
                Coord(start.DX, start.DY, start.DZ);
                Coord(end.DX, start.DY, start.DZ);
                Coord(end.DX, end.DY, start.DZ);
                Coord(start.DX, end.DY, start.DZ);
                GL.End();
                GL.LineWidth(1);
            }

            if (ShouldRenderResizeBox(viewport))
            {
                RenderResizeBox(viewport, start, end);
            }
        }

        protected override void LeftMouseDownToDraw(Viewport2D viewport, ViewportEvent e)
        {
            //
        }

        protected override void MouseDraggingToDraw(Viewport2D viewport, ViewportEvent e)
        {
            //
        }

        protected override void LeftMouseUpDrawing(Viewport2D viewport, ViewportEvent e)
        {
            //
        }

        protected override void LeftMouseClick(Viewport2D viewport, ViewportEvent e)
        {
            //
        }

        public override void BoxDrawnConfirm(Sledge.UI.ViewportBase viewport)
        {
            //
        }

        public override void BoxDrawnCancel(Sledge.UI.ViewportBase viewport)
        {
            //
        }
    }
}
