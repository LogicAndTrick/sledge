using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Properties;
using Sledge.Settings;

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

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage)
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
            get { return Color.FromArgb(128, Color.LightGray); }
        }

        public override void ToolSelected()
        {
            State.BoxStart = Document.Map.CordonBounds.Start;
            State.BoxEnd = Document.Map.CordonBounds.End;
            State.Action = BoxAction.Drawn;
        }

        protected override void LeftMouseUpResizing(Sledge.UI.Viewport2D viewport, System.Windows.Forms.MouseEventArgs e)
        {
            Document.Map.CordonBounds = new Box(State.BoxStart, State.BoxEnd);
            base.LeftMouseUpResizing(viewport, e);
        }

        protected override void Render2D(Sledge.UI.Viewport2D viewport)
        {
            if (State.Action == BoxAction.ReadyToDraw || State.Action == BoxAction.DownToDraw) return;
            var start = viewport.Flatten(State.BoxStart);
            var end = viewport.Flatten(State.BoxEnd);

            if (ShouldDrawBox())
            {
                var min = viewport.ScreenToWorld(0, 0);
                var max = viewport.ScreenToWorld(viewport.Width, viewport.Height);

                GL.Color4(Color.FromArgb(128, Color.Purple));
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

        protected override void LeftMouseDownToDraw(Sledge.UI.Viewport2D viewport, System.Windows.Forms.MouseEventArgs e)
        {
            //
        }

        protected override void MouseDraggingToDraw(Sledge.UI.Viewport2D viewport, System.Windows.Forms.MouseEventArgs e)
        {
            //
        }

        protected override void LeftMouseUpDrawing(Sledge.UI.Viewport2D viewport, System.Windows.Forms.MouseEventArgs e)
        {
            //
        }

        protected override void LeftMouseClick(Sledge.UI.Viewport2D viewport, System.Windows.Forms.MouseEventArgs e)
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
