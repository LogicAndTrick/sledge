using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK.Input;
using Sledge.DataStructures.Geometric;
using Sledge.EditorNew.Properties;
using Sledge.EditorNew.UI.Viewports;
using Sledge.Graphics.Helpers;
using OpenTK.Graphics.OpenGL;
using Sledge.Gui.Structures;
using Sledge.Settings;
using TextPrinter = OpenTK.Graphics.TextPrinter;
using TextQuality = OpenTK.Graphics.TextQuality;

namespace Sledge.EditorNew.Tools
{
    public abstract class BaseDraggableTool : BaseTool
    {
        public Stack<IDraggableState> States { get; set; }

        private IDraggable _currentDraggable;

        public override void MouseMove(IMapViewport viewport, ViewportEvent e)
        {
            if (!viewport.Is2D || e.Dragging) return;
            var vp = (IViewport2D)viewport;
            IDraggable drag = null;
            foreach (var state in States)
            {
                var drags = state.GetDraggables(vp).ToList();
                drags.Add(state);
                foreach (var draggable in drags)
                {
                    if (draggable.CanDrag(vp, e))
                    {
                        drag = draggable;
                        break;
                    }
                }
                if (drag != null) break;
            }
            if (drag != _currentDraggable)
            {
                if (_currentDraggable != null) _currentDraggable.Unhighlight(vp);
                _currentDraggable = drag;
                if (_currentDraggable != null) _currentDraggable.Highlight(vp);
            }
        }

        public override void DragStart(IMapViewport viewport, ViewportEvent e)
        {
            if (!viewport.Is2D || e.Button != MouseButton.Left) return;
            var vp = (IViewport2D)viewport;
            if (_currentDraggable == null) return;
            _currentDraggable.StartDrag(vp, e);
        }

        public override void DragMove(IMapViewport viewport, ViewportEvent e)
        {
            if (!viewport.Is2D || e.Button != MouseButton.Left) return;
            var vp = (IViewport2D)viewport;
            if (_currentDraggable == null) return;
            _currentDraggable.Drag(vp, e);
        }

        public override void DragEnd(IMapViewport viewport, ViewportEvent e)
        {
            if (!viewport.Is2D || e.Button != MouseButton.Left) return;
            var vp = (IViewport2D)viewport;
            if (_currentDraggable == null) return;
            _currentDraggable.EndDrag(vp, e);
        }

        public override void Render(IMapViewport viewport)
        {
            if (!viewport.Is2D) return;
            var vp = (IViewport2D) viewport;
            foreach (var state in States)
            {
                foreach (var draggable in state.GetDraggables(vp))
                {
                    if (draggable == _currentDraggable) continue;
                    draggable.Render(vp);
                }
                if (state != _currentDraggable) state.Render(vp);
            }
            if (_currentDraggable != null) _currentDraggable.Render(vp);
        }

        #region Unused (for now)
        public override void MouseDown(IMapViewport viewport, ViewportEvent e)
        {

        }

        public override void MouseUp(IMapViewport viewport, ViewportEvent e)
        {

        }

        public override void MouseEnter(IMapViewport viewport, ViewportEvent e)
        {

        }

        public override void MouseLeave(IMapViewport viewport, ViewportEvent e)
        {

        }

        public override void MouseClick(IMapViewport viewport, ViewportEvent e)
        {

        }

        public override void MouseDoubleClick(IMapViewport viewport, ViewportEvent e)
        {

        }

        public override void MouseWheel(IMapViewport viewport, ViewportEvent e)
        {

        }

        public override void KeyPress(IMapViewport viewport, ViewportEvent e)
        {

        }

        public override void KeyDown(IMapViewport viewport, ViewportEvent e)
        {

        }

        public override void KeyUp(IMapViewport viewport, ViewportEvent e)
        {

        }

        public override void UpdateFrame(IMapViewport viewport, Frame frame)
        {

        }

        public override void PreRender(IMapViewport viewport)
        {

        }
        #endregion
    }

    public interface IDraggable
    {
        bool CanDrag(IViewport2D viewport, ViewportEvent e);
        void Highlight(IViewport2D viewport);
        void Unhighlight(IViewport2D viewport);
        void StartDrag(IViewport2D viewport, ViewportEvent e);
        void Drag(IViewport2D viewport, ViewportEvent e);
        void EndDrag(IViewport2D viewport, ViewportEvent e);
        void Render(IViewport2D viewport);
    }

    public interface IDraggableState : IDraggable
    {
        IEnumerable<IDraggable> GetDraggables(IViewport2D viewport);
    }

    public class BoxDraggableState : IDraggableState
    {
        public enum BoxAction
        {
            Idle,
            Drawing,
            Drawn,
            Resizing
        }

        public enum ResizeHandle
        {
            TopLeft, Top, TopRight,
            Left, Center, Right,
            BottomLeft, Bottom, BottomRight
        }

        public class BoxState
        {
            public IMapViewport Viewport { get; set; }
            public BoxAction Action { get; set; }
            public ResizeHandle Handle { get; set; }
            public Coordinate OrigStart { get; set; }
            public Coordinate OrigEnd { get; set; }
            public Coordinate Start { get; set; }
            public Coordinate End { get; set; }
        }

        public Color BoxColour { get; set; }
        public Color FillColour { get; set; }
        internal BoxState State { get; set; }

        protected TextPrinter _printer;
        protected Font _printerFont;

        public BoxDraggableState()
        {
            State = new BoxState();

            _printer = new TextPrinter(TextQuality.Low);
            _printerFont = new Font(FontFamily.GenericSansSerif, 16, GraphicsUnit.Pixel);
        }

        IDraggable _d = new DraggableCoordinate();
        public IEnumerable<IDraggable> GetDraggables(IViewport2D viewport)
        {
            yield return _d;
            if (State.Action != BoxAction.Drawn) yield break;
            // 
        }

        public bool CanDrag(IViewport2D viewport, ViewportEvent e)
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

        public void StartDrag(IViewport2D viewport, ViewportEvent e)
        {
            State.Viewport = viewport;
            State.Action = BoxAction.Drawing;
            State.Start = viewport.Expand(viewport.ScreenToWorld(e.X, viewport.Height - e.Y));
            State.End = State.Start;
            State.Handle = ResizeHandle.BottomLeft;
        }

        public void Drag(IViewport2D viewport, ViewportEvent e)
        {
            State.End = viewport.Expand(viewport.ScreenToWorld(e.X, viewport.Height - e.Y));
        }

        public void EndDrag(IViewport2D viewport, ViewportEvent e)
        {
            State.Viewport = null;
            State.Action = BoxAction.Drawn;
            State.End = viewport.Expand(viewport.ScreenToWorld(e.X, viewport.Height - e.Y));
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
        protected static void Coord(Coordinate c)
        {
            GL.Vertex3(c.DX, c.DY, c.DZ);
        }

        //public override void Render(IMapViewport viewport)
        //{
        //    if (viewport.Is2D) Render2D((IViewport2D)viewport);
        //    if (viewport.Is3D) Render3D((IViewport3D)viewport);
        //}

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

        protected virtual Color GetRenderSnapHandleColour()
        {
            return BoxColour;
        }

        protected virtual Color GetRenderTextColour()
        {
            return BoxColour;
        }

        protected virtual Color GetRenderResizeHoverColour()
        {
            return FillColour;
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

        protected virtual bool ShouldRenderSnapHandle(IViewport2D viewport)
        {
            return State.Action == BoxAction.Resizing && State.Handle == ResizeHandle.Center;
        }

        protected virtual void RenderSnapHandle(IViewport2D viewport)
        {
            //var start = GetResizeOrigin(viewport);
            Coordinate start = null;
            if (start == null) return;
            const int size = 6;
            var dist = (double)(size / viewport.Zoom);

            var origin = start + viewport.Flatten(State.Start - State.OrigStart);
            GL.Begin(PrimitiveType.Lines);
            GL.Color4(GetRenderSnapHandleColour());
            Coord(origin.DX - dist, origin.DY + dist, 0);
            Coord(origin.DX + dist, origin.DY - dist, 0);
            Coord(origin.DX + dist, origin.DY + dist, 0);
            Coord(origin.DX - dist, origin.DY - dist, 0);
            GL.End();
        }

        protected virtual bool ShouldRenderResizeBox(IViewport2D viewport)
        {
            return false;
            //return State.Viewport == viewport &&
            //       (State.Action == BoxAction.ReadyToResize
            //        || State.Action == BoxAction.DownToResize);
        }

        protected virtual double[] GetRenderResizeBox(Coordinate start, Coordinate end)
        {
            var width = Math.Abs(start.DX - end.DX);
            var height = Math.Abs(start.DY - end.DY);
            double x1, y1, x2, y2;
            var handleWidth = width / 10d;
            var handleHeight = height / 10d;
            switch (State.Handle)
            {
                case ResizeHandle.TopLeft:
                    x1 = start.DX;
                    x2 = x1 + handleWidth;
                    y1 = end.DY - handleHeight;
                    y2 = end.DY;
                    break;
                case ResizeHandle.Top:
                    x1 = start.DX;
                    x2 = end.DX;
                    y1 = end.DY - handleHeight;
                    y2 = end.DY;
                    break;
                case ResizeHandle.TopRight:
                    x1 = end.DX - handleWidth;
                    x2 = end.DX;
                    y1 = end.DY - handleHeight;
                    y2 = end.DY;
                    break;
                case ResizeHandle.Left:
                    x1 = start.DX;
                    x2 = x1 + handleWidth;
                    y1 = start.DY;
                    y2 = end.DY;
                    break;
                case ResizeHandle.Center:
                    x1 = start.DX;
                    x2 = end.DX;
                    y1 = start.DY;
                    y2 = end.DY;
                    break;
                case ResizeHandle.Right:
                    x1 = end.DX - handleWidth;
                    x2 = end.DX;
                    y1 = start.DY;
                    y2 = end.DY;
                    break;
                case ResizeHandle.BottomLeft:
                    x1 = start.DX;
                    x2 = x1 + handleWidth;
                    y1 = start.DY;
                    y2 = y1 + handleHeight;
                    break;
                case ResizeHandle.Bottom:
                    x1 = start.DX;
                    x2 = end.DX;
                    y1 = start.DY;
                    y2 = y1 + handleHeight;
                    break;
                case ResizeHandle.BottomRight:
                    x1 = end.DX - handleWidth;
                    x2 = end.DX;
                    y1 = start.DY;
                    y2 = y1 + handleHeight;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new[] { x1, y1, x2, y2 };
        }

        protected virtual void RenderResizeBox(IViewport2D viewport, Coordinate start, Coordinate end)
        {
            var box = GetRenderResizeBox(start, end);
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(FillColour);
            double x1 = box[0], y1 = box[1], x2 = box[2], y2 = box[3];
            Coord(x1, y1, 0);
            Coord(x2, y1, 0);
            Coord(x2, y2, 0);
            Coord(x1, y2, 0);
            GL.End();
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
            var start = viewport.Flatten(State.Start);
            var end = viewport.Flatten(State.End);
            if (ShouldDrawBox(viewport))
            {
                RenderBox(viewport, start, end);
            }
            if (ShouldRenderSnapHandle(viewport))
            {
                RenderSnapHandle(viewport);
            }
            if (ShouldRenderResizeBox(viewport))
            {
                RenderResizeBox(viewport, start, end);
            }
            if (ShouldDrawBoxText(viewport))
            {
                RenderBoxText(viewport, start, end);
            }
        }

        protected virtual bool ShouldDraw3DBox()
        {
            return State.Action != BoxAction.Idle;
        }

        protected virtual void Render3DBox(IViewport3D viewport, Coordinate start, Coordinate end)
        {
            var box = new Box(start, end);
            TextureHelper.Unbind();
            GL.Begin(PrimitiveType.Lines);
            GL.Color4(GetRenderBoxColour());
            foreach (var line in box.GetBoxLines())
            {
                Coord(line.Start);
                Coord(line.End);
            }
            GL.End();
        }

        protected virtual void Render3D(IViewport3D viewport)
        {
            if (State.Action == BoxAction.Idle) return;
            if (ShouldDraw3DBox())
            {
                Render3DBox(viewport, State.Start, State.End);
            }
        }
        #endregion
    }

    public class DraggableCoordinate : IDraggable
    {
        private bool _highlighted;
        private Coordinate _position;

        public DraggableCoordinate()
        {
            _position = Coordinate.Zero;
        }

        public bool CanDrag(IViewport2D viewport, ViewportEvent e)
        {
            var pos = viewport.Flatten(_position);
            var point = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            var diff = (pos - point).Absolute();
            return diff.X < 5 && diff.Y < 5;
        }

        public void Highlight(IViewport2D viewport)
        {
            _highlighted = true;
        }

        public void Unhighlight(IViewport2D viewport)
        {
            _highlighted = false;
        }

        public void StartDrag(IViewport2D viewport, ViewportEvent e)
        {

        }

        public void Drag(IViewport2D viewport, ViewportEvent e)
        {
            _position = viewport.Expand(viewport.ScreenToWorld(e.X, viewport.Height - e.Y));
        }

        public void EndDrag(IViewport2D viewport, ViewportEvent e)
        {

        }

        public void Render(IViewport2D viewport)
        {
            var pos = viewport.Flatten(_position);
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(_highlighted ? Color.Red : Color.Green);
            GL.Vertex2((double)(pos.X - 2), (double)(pos.Y - 2));
            GL.Vertex2((double)(pos.X + 2), (double)(pos.Y - 2));
            GL.Vertex2((double)(pos.X + 2), (double)(pos.Y + 2));
            GL.Vertex2((double)(pos.X - 2), (double)(pos.Y + 2));
            GL.End();
        }
    }

    public class DummyDraggableTool : BaseDraggableTool
    {
        public DummyDraggableTool()
        {
            States = new Stack<IDraggableState>();

            var box = new BoxDraggableState();
            box.BoxColour = Color.Red;
            box.FillColour = Color.Purple;
            States.Push(box);
        }

        public override IEnumerable<string> GetContexts()
        {
            yield return "Dummy Draggable Tool";
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Clip;
        }

        public override string GetName()
        {
            return "Dummy Draggable Tool";
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return null;
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            return HotkeyInterceptResult.Continue;
        }
    }
}