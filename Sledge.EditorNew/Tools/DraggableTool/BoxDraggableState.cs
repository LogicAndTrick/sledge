using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.EditorNew.UI.Viewports;
using EnableCap = OpenTK.Graphics.OpenGL.EnableCap;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace Sledge.EditorNew.Tools.DraggableTool
{
    public class BoxDraggableState : IDraggableState
    {
        public BaseTool Tool { get; set; }

        public Color BoxColour { get; set; }
        public Color FillColour { get; set; }
        internal BoxState State { get; set; }

        protected IDraggable[] BoxHandles { get; set; }

        protected TextPrinter _printer;
        protected Font _printerFont;

        public BoxDraggableState(BaseTool tool)
        {
            Tool = tool;
            State = new BoxState();

            _printer = new TextPrinter(TextQuality.Low);
            _printerFont = new Font(FontFamily.GenericSansSerif, 16, GraphicsUnit.Pixel);

            BoxHandles = new[]
            {
                new InternalBoxResizeHandle(this, ResizeHandle.TopLeft),
                new InternalBoxResizeHandle(this, ResizeHandle.TopRight),
                new InternalBoxResizeHandle(this, ResizeHandle.BottomLeft),
                new InternalBoxResizeHandle(this, ResizeHandle.BottomRight),
                
                new InternalBoxResizeHandle(this, ResizeHandle.Top),
                new InternalBoxResizeHandle(this, ResizeHandle.Left),
                new InternalBoxResizeHandle(this, ResizeHandle.Right),
                new InternalBoxResizeHandle(this, ResizeHandle.Bottom),

                new InternalBoxResizeHandle(this, ResizeHandle.Center), 
            };
        }

        public IEnumerable<IDraggable> GetDraggables(IViewport2D viewport)
        {
            if (State.Action == BoxAction.Idle || State.Action == BoxAction.Drawing) yield break;
            foreach (var draggable in BoxHandles)
            {
                yield return draggable;
            }
            // 
        }

        public bool CanDrag(IViewport2D viewport, ViewportEvent e, Coordinate position)
        {
            return true;
        }

        public void Highlight(IViewport2D viewport)
        {
            //
        }

        public void Unhighlight(IViewport2D viewport)
        {
            //
        }

        public void StartDrag(IViewport2D viewport, ViewportEvent e, Coordinate position)
        {
            State.Viewport = viewport;
            State.Action = BoxAction.Drawing;
            State.Start = Tool.SnapIfNeeded(viewport.Expand(position));
            State.End = State.Start;
        }

        public void Drag(IViewport2D viewport, ViewportEvent e, Coordinate lastPosition, Coordinate position)
        {
            State.End = Tool.SnapIfNeeded(viewport.Expand(position));
        }

        public void EndDrag(IViewport2D viewport, ViewportEvent e, Coordinate position)
        {
            State.Viewport = null;
            State.Action = BoxAction.Drawn;
            State.End = Tool.SnapIfNeeded(viewport.Expand(position)) + viewport.GetUnusedCoordinate(Coordinate.One * 100);
            State.FixBounds();
        }

        public void Render(IViewport2D viewport)
        {
            Render2D(viewport);
        }

        #region Rendering
        protected static void Coord(double x, double y, double z)
        {
            GL.Vertex3(x, y, z);
        }

        protected virtual bool ShouldDrawBox(IMapViewport viewport)
        {
            return State.Action == BoxAction.Drawing
                   || State.Action == BoxAction.Drawn
                   || State.Action == BoxAction.Resizing;
        }

        protected virtual bool ShouldDrawBoxText(IMapViewport viewport)
        {
            return ShouldDrawBox(viewport);
        }

        protected virtual Color GetRenderFillColour()
        {
            return FillColour;
        }

        protected virtual Color GetRenderBoxColour()
        {
            return BoxColour;
        }

        protected virtual void RenderBox(IMapViewport viewport, Coordinate start, Coordinate end)
        {
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(GetRenderFillColour());
            Coord(start.DX, start.DY, start.DZ);
            Coord(end.DX, start.DY, start.DZ);
            Coord(end.DX, end.DY, start.DZ);
            Coord(start.DX, end.DY, start.DZ);
            GL.End();

            if (Sledge.Settings.View.DrawBoxDashedLines)
            {
                GL.LineStipple(4, 0xAAAA);
                GL.Enable(EnableCap.LineStipple);
            }

            GL.Begin(PrimitiveType.LineLoop);
            GL.Color3(GetRenderBoxColour());
            Coord(start.DX, start.DY, start.DZ);
            Coord(end.DX, start.DY, start.DZ);
            Coord(end.DX, end.DY, start.DZ);
            Coord(start.DX, end.DY, start.DZ);
            GL.End();
            GL.Disable(EnableCap.LineStipple);
        }

        protected void RenderBoxText(IViewport2D viewport, Coordinate boxStart, Coordinate boxEnd)
        {
            if (!Sledge.Settings.View.DrawBoxText) return;

            var widthText = (Math.Round(boxEnd.X - boxStart.X, 1)).ToString("#.##");
            var heightText = (Math.Round(boxEnd.Y - boxStart.Y, 1)).ToString("#.##");

            var wid = _printer.Measure(widthText, _printerFont, new RectangleF(0, 0, viewport.Width, viewport.Height));
            var hei = _printer.Measure(heightText, _printerFont, new RectangleF(0, 0, viewport.Width, viewport.Height));

            boxStart = viewport.WorldToScreen(boxStart);
            boxEnd = viewport.WorldToScreen(boxEnd);

            var cx = (float)(boxStart.X + (boxEnd.X - boxStart.X) / 2);
            var cy = (float)(boxStart.Y + (boxEnd.Y - boxStart.Y) / 2);

            var wrect = new RectangleF(cx - wid.BoundingBox.Width / 2, viewport.Height - (float)boxEnd.Y - _printerFont.Height - 18, wid.BoundingBox.Width * 1.2f, wid.BoundingBox.Height);
            var hrect = new RectangleF((float)boxEnd.X + 18, viewport.Height - cy - hei.BoundingBox.Height * 0.75f, hei.BoundingBox.Width * 1.2f, hei.BoundingBox.Height);

            if (wrect.X < 10) wrect.X = 10;
            if (wrect.X + wrect.Width + 10 > viewport.Width) wrect.X = viewport.Width - 10 - wrect.Width;
            if (wrect.Y < 10) wrect.Y = 10;
            if (wrect.Y + wrect.Height + 10 > viewport.Height) wrect.Y = viewport.Height - 10 - wrect.Height;

            if (hrect.X < 10) hrect.X = 10;
            if (hrect.X + hrect.Width + 10 > viewport.Width) hrect.X = viewport.Width - 10 - hrect.Width;
            if (hrect.Y < 10) hrect.Y = 10;
            if (hrect.Y + hrect.Height + 10 > viewport.Height) hrect.Y = viewport.Height - 10 - hrect.Height;

            GL.Disable(EnableCap.CullFace);

            _printer.Begin();
            _printer.Print(widthText, _printerFont, BoxColour, wrect);
            _printer.Print(heightText, _printerFont, BoxColour, hrect);
            _printer.End();

            GL.Enable(EnableCap.CullFace);
        }

        protected virtual void Render2D(IViewport2D viewport)
        {
            if (State.Action == BoxAction.Idle) return;
            var box = new Box(State.Start, State.End);
            var start = viewport.Flatten(box.Start);
            var end = viewport.Flatten(box.End);
            if (ShouldDrawBox(viewport))
            {
                RenderBox(viewport, start, end);
            }
            if (ShouldDrawBoxText(viewport))
            {
                RenderBoxText(viewport, start, end);
            }
        }

        #endregion
    }
}