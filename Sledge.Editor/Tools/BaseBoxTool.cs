using System;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.Graphics.Helpers;
using Sledge.UI;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Sledge.DataStructures.Geometric;

namespace Sledge.Editor.Tools
{
    public abstract class BaseBoxTool : BaseTool
    {
        // Enum/Class Declarations
        public enum BoxAction
        {
            ReadyToDraw,
            DownToDraw,
            Drawing,
            Drawn,
            ReadyToResize,
            DownToResize,
            Resizing
        }

        public enum ResizeHandle
        {
            TopLeft,    Top,     TopRight,
            Left,       Center,  Right,
            BottomLeft, Bottom,  BottomRight
        }

        public class BoxState
        {
            public BoxState()
            {
                ActiveViewport = null;
                Action = BoxAction.ReadyToDraw;
                Handle = ResizeHandle.Center;
                BoxStart = null;
                BoxEnd = null;
                MoveStart = null;
                PreTransformBoxStart = null;
                PreTransformBoxEnd = null;
                ClickStart = new Point(0, 0);
            }

            public bool IsValidAndApplicable(ViewportBase vp)
            {
                return (Action != BoxAction.DownToDraw
                        && Action != BoxAction.Drawing
                        && Action != BoxAction.DownToResize
                        && Action != BoxAction.Resizing)
                        || ActiveViewport == vp;
            }

            public void FixBoxBounds()
            {
                if (Action != BoxAction.Drawing && Action != BoxAction.Resizing) return;
                if (!(ActiveViewport is Viewport2D)) return;
                var vp = (Viewport2D) ActiveViewport;

                if (BoxStart.X > BoxEnd.X)
                {
                    var temp = BoxStart.X;
                    BoxStart.X = BoxEnd.X;
                    BoxEnd.X = temp;
                    var flat = vp.Flatten(Coordinate.UnitX);
                    if (flat.X == 1) SwapHandle("Left", "Right");
                    if (flat.Y == 1) SwapHandle("Top", "Bottom");
                }
                if (BoxStart.Y > BoxEnd.Y)
                {
                    var temp = BoxStart.Y;
                    BoxStart.Y = BoxEnd.Y;
                    BoxEnd.Y = temp;
                    var flat = vp.Flatten(Coordinate.UnitY);
                    if (flat.X == 1) SwapHandle("Left", "Right");
                    if (flat.Y == 1) SwapHandle("Top", "Bottom");
                }
                if (BoxStart.Z > BoxEnd.Z)
                {
                    var temp = BoxStart.Z;
                    BoxStart.Z = BoxEnd.Z;
                    BoxEnd.Z = temp;
                    var flat = vp.Flatten(Coordinate.UnitZ);
                    if (flat.X == 1) SwapHandle("Left", "Right");
                    if (flat.Y == 1) SwapHandle("Top", "Bottom");
                }
            }

            public void SwapHandle(string one, string two)
            {
                var str = Handle.ToString();
                if (str.Contains(one))
                {
                    Handle = (ResizeHandle) Enum.Parse(typeof (ResizeHandle), str.Replace(one, two));
                }
                else if (str.Contains(two))
                {
                    Handle = (ResizeHandle)Enum.Parse(typeof(ResizeHandle), str.Replace(two, one));
                }
            }

            public ViewportBase ActiveViewport { get; set; }
            public BoxAction Action { get; set; }
            public ResizeHandle Handle { get; set; }
            public Coordinate BoxStart { get; set; }
            public Coordinate BoxEnd { get; set; }
            public Coordinate MoveStart { get; set; }
            public Coordinate PreTransformBoxStart { get; set; }
            public Coordinate PreTransformBoxEnd { get; set; }
            public Point ClickStart { get; set; }
        }

        // Static Methods
        protected static Tuple<Coordinate, Coordinate> GetProperBoxCoordinates(Coordinate start, Coordinate end)
        {
            var newStart = new Coordinate(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y), Math.Min(start.Z, end.Z));
            var newEnd = new Coordinate(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y), Math.Max(start.Z, end.Z));
            return Tuple.Create(newStart, newEnd);
        }

        private static bool HandleHitTestPoint(decimal hitX, decimal hitY, decimal testX, decimal testY, decimal hitbox)
        {
            return (hitX >= testX - hitbox && hitX <= testX + hitbox && hitY >= testY - hitbox && hitY <= testY + hitbox);
        }

        private static bool HandleHitTestLine(decimal hitX, decimal hitY, decimal test1X, decimal test1Y, decimal test2X, decimal test2Y, decimal hitbox)
        {
            if (test1X != test2X && test1Y != test2Y) return false; // Only works on straight lines
            var sx = Math.Min(test1X, test2X);
            var sy = Math.Min(test1Y, test2Y);
            var ex = Math.Max(test1X, test2X);
            var ey = Math.Max(test1Y, test2Y);
            var hoz = test1Y == test2Y;
            return hoz
                       ? hitX >= sx && hitX <= ex && hitY >= sy - hitbox && hitY <= sy + hitbox
                       : hitY >= sy && hitY <= ey && hitX >= sx - hitbox && hitX <= sx + hitbox;
        }

        protected static ResizeHandle? GetHandle(Coordinate current, Coordinate boxStart, Coordinate boxEnd, decimal hitbox)
        {
            var start = new Coordinate(Math.Min(boxStart.X, boxEnd.X), Math.Min(boxStart.Y, boxEnd.Y), 0);
            var end = new Coordinate(Math.Max(boxStart.X, boxEnd.X), Math.Max(boxStart.Y, boxEnd.Y), 0);
            if (HandleHitTestPoint(current.X, current.Y, start.X, start.Y, hitbox)) return ResizeHandle.BottomLeft;
            if (HandleHitTestPoint(current.X, current.Y, end.X, start.Y, hitbox)) return ResizeHandle.BottomRight;
            if (HandleHitTestPoint(current.X, current.Y, start.X, end.Y, hitbox)) return ResizeHandle.TopLeft;
            if (HandleHitTestPoint(current.X, current.Y, end.X, end.Y, hitbox)) return ResizeHandle.TopRight;
            if (HandleHitTestLine(current.X, current.Y, start.X, start.Y, end.X, start.Y, hitbox)) return ResizeHandle.Bottom;
            if (HandleHitTestLine(current.X, current.Y, start.X, end.Y, end.X, end.Y, hitbox)) return ResizeHandle.Top;
            if (HandleHitTestLine(current.X, current.Y, start.X, start.Y, start.X, end.Y, hitbox)) return ResizeHandle.Left;
            if (HandleHitTestLine(current.X, current.Y, end.X, start.Y, end.X, end.Y, hitbox)) return ResizeHandle.Right;
            if (current.X > start.X && current.X < end.X && current.Y > start.Y && current.Y < end.Y) return ResizeHandle.Center;
            return null;
        }

        // Class Variables
        protected const decimal HandleWidth = 12;

        protected abstract Color BoxColour { get; }
        protected abstract Color FillColour { get; }
        internal BoxState State { get; set; }

        protected BaseBoxTool()
        {
            Usage = ToolUsage.Both;
            State = new BoxState();
        }

        protected virtual void OnBoxChanged()
        {
            State.FixBoxBounds();
            Mediator.Publish(EditorMediator.SelectionBoxChanged,
                             State.Action == BoxAction.ReadyToDraw
                                 ? Box.Empty
                                 : new Box(State.BoxStart, State.BoxEnd));
        }

        // Mouse Down
        public override void MouseDown(ViewportBase viewport, MouseEventArgs e)
        {
            if (viewport is Viewport3D) MouseDown3D((Viewport3D)viewport, e);
            if (e.Button != MouseButtons.Left) return;
            if (!(viewport is Viewport2D)) return;
            State.ClickStart = new Point(e.X, e.Y);
            var vp = (Viewport2D) viewport;
            switch (State.Action)
            {
                case BoxAction.ReadyToDraw:
                case BoxAction.Drawn:
                    LeftMouseDownToDraw(vp, e);
                    break;
                case BoxAction.ReadyToResize:
                    LeftMouseDownToResize(vp, e);
                    break;
            }
        }

        protected virtual void MouseDown3D(Viewport3D viewport, MouseEventArgs e)
        {
            // Virtual
        }

        protected virtual void LeftMouseDownToDraw(Viewport2D viewport, MouseEventArgs e)
        {
            State.ActiveViewport = viewport;
            State.Action = BoxAction.DownToDraw;
            State.BoxStart = SnapIfNeeded(viewport.Expand(viewport.ScreenToWorld(e.X, viewport.Height - e.Y)));
            State.BoxEnd = State.BoxStart;
            State.Handle = ResizeHandle.BottomLeft;
            OnBoxChanged();
        }

        protected virtual void LeftMouseDownToResize(Viewport2D viewport, MouseEventArgs e)
        {
            State.ActiveViewport = viewport;
            State.Action = BoxAction.DownToResize;
            State.MoveStart = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            State.PreTransformBoxStart = State.BoxStart;
            State.PreTransformBoxEnd = State.BoxEnd;
        }

        // Mouse Up
        public override void MouseUp(ViewportBase viewport, MouseEventArgs e)
        {
            if (viewport is Viewport3D) MouseUp3D((Viewport3D) viewport, e);
            Editor.Instance.CaptureAltPresses = false;
            if (e.Button != MouseButtons.Left) return;
            if (!(viewport is Viewport2D)) return;
            var vp = (Viewport2D)viewport;
            switch (State.Action)
            {
                case BoxAction.Drawing:
                    LeftMouseUpDrawing(vp, e);
                    break;
                case BoxAction.Resizing:
                    LeftMouseUpResizing(vp, e);
                    break;
                case BoxAction.DownToDraw:
                    LeftMouseClick(vp, e);
                    break;
                case BoxAction.DownToResize:
                    LeftMouseClickOnResizeHandle(vp, e);
                    break;
            }
        }

        protected virtual void MouseUp3D(Viewport3D viewport, MouseEventArgs e)
        {
            // Virtual
        }

        protected virtual void LeftMouseUpDrawing(Viewport2D viewport, MouseEventArgs e)
        {
            var coords = GetResizedBoxCoordinates(viewport, e);
            var corrected = GetProperBoxCoordinates(coords.Item1, coords.Item2);
            State.ActiveViewport = null;
            State.Action = BoxAction.Drawn;
            State.BoxStart = corrected.Item1;
            State.BoxEnd = corrected.Item2;
            OnBoxChanged();
        }

        protected virtual void LeftMouseUpResizing(Viewport2D viewport, MouseEventArgs e)
        {
            var coords = GetResizedBoxCoordinates(viewport, e);
            var corrected = GetProperBoxCoordinates(coords.Item1, coords.Item2);
            State.ActiveViewport = null;
            State.Action = BoxAction.Drawn;
            State.BoxStart = corrected.Item1;
            State.BoxEnd = corrected.Item2;
            OnBoxChanged();
        }

        protected virtual void LeftMouseClick(Viewport2D viewport, MouseEventArgs e)
        {
            State.ActiveViewport = null;
            State.Action = BoxAction.ReadyToDraw;
            State.BoxStart = null;
            State.BoxEnd = null;
            OnBoxChanged();
        }

        protected virtual void LeftMouseClickOnResizeHandle(Viewport2D vp, MouseEventArgs e)
        {
            State.Action = BoxAction.ReadyToResize;
        }

        // Mouse Move
        public override void MouseMove(ViewportBase viewport, MouseEventArgs e)
        {
            if (viewport is Viewport3D) MouseMove3D((Viewport3D) viewport, e);
            if (!(viewport is Viewport2D)) return;
            if (!State.IsValidAndApplicable(viewport)) return;
            if (Math.Abs(e.X - State.ClickStart.X) <= 2 && Math.Abs(e.Y - State.ClickStart.Y) <= 2) return;
            var vp = (Viewport2D) viewport;
            switch (State.Action)
            {
                case BoxAction.Drawing:
                case BoxAction.DownToDraw:
                    Editor.Instance.CaptureAltPresses = true;
                    MouseDraggingToDraw(vp, e);
                    break;
                case BoxAction.Drawn:
                case BoxAction.ReadyToResize:
                    MouseHoverWhenDrawn(vp, e);
                    break;
                case BoxAction.DownToResize:
                case BoxAction.Resizing:
                    Editor.Instance.CaptureAltPresses = true;
                    MouseDraggingToResize(vp, e);
                    break;
            }
        }

        protected virtual void MouseMove3D(Viewport3D viewport, MouseEventArgs e)
        {
            // Virtual
        }

        protected virtual void MouseDraggingToDraw(Viewport2D viewport, MouseEventArgs e)
        {
            State.Action = BoxAction.Drawing;
            var coords = GetResizedBoxCoordinates(viewport, e);
            State.BoxStart = coords.Item1;
            State.BoxEnd = coords.Item2;
            OnBoxChanged();
        }

        protected virtual void MouseDraggingToResize(Viewport2D viewport, MouseEventArgs e)
        {
            State.Action = BoxAction.Resizing;
            var coords = GetResizedBoxCoordinates(viewport, e);
            State.BoxStart = coords.Item1;
            State.BoxEnd = coords.Item2;
            OnBoxChanged();
        }

        protected virtual Cursor CursorForHandle(ResizeHandle handle)
        {
            switch (handle)
            {
                case ResizeHandle.TopLeft:
                case ResizeHandle.BottomRight:
                    return Cursors.SizeNWSE;
                case ResizeHandle.TopRight:
                case ResizeHandle.BottomLeft:
                    return Cursors.SizeNESW;
                case ResizeHandle.Top:
                case ResizeHandle.Bottom:
                    return Cursors.SizeNS;
                case ResizeHandle.Left:
                case ResizeHandle.Right:
                    return Cursors.SizeWE;
                case ResizeHandle.Center:
                    return Cursors.SizeAll;
                default:
                    return Cursors.Default;
            }
        }

        protected virtual void MouseHoverWhenDrawn(Viewport2D viewport, MouseEventArgs e)
        {
            var now = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            var start = viewport.Flatten(State.BoxStart);
            var end = viewport.Flatten(State.BoxEnd);
            var handle = GetHandle(now, start, end, HandleWidth / viewport.Zoom);
            if (handle.HasValue)
            {
                viewport.Cursor = CursorForHandle(handle.Value);
                State.Handle = handle.Value;
                State.Action = BoxAction.ReadyToResize;
                State.ActiveViewport = viewport;
            }
            else
            {
                viewport.Cursor = Cursors.Default;
                State.Action = BoxAction.Drawn;
                State.ActiveViewport = null;
            }
        }

        public override void MouseWheel(ViewportBase viewport, MouseEventArgs e)
        {
            // Nope.
        }

        protected Tuple<Coordinate, Coordinate> GetResizedBoxCoordinates(Viewport2D viewport, MouseEventArgs e)
        {
            if (State.Action != BoxAction.Resizing && State.Action != BoxAction.Drawing) return Tuple.Create(State.BoxStart, State.BoxEnd);
            var now = SnapIfNeeded(viewport.ScreenToWorld(e.X, viewport.Height - e.Y));
            var cstart = viewport.Flatten(State.BoxStart);
            var cend = viewport.Flatten(State.BoxEnd);
            switch (State.Handle)
            {
                case ResizeHandle.TopLeft:
                    cstart.X = now.X;
                    cend.Y = now.Y;
                    break;
                case ResizeHandle.Top:
                    cend.Y = now.Y;
                    break;
                case ResizeHandle.TopRight:
                    cend.X = now.X;
                    cend.Y = now.Y;
                    break;
                case ResizeHandle.Left:
                    cstart.X = now.X;
                    break;
                case ResizeHandle.Center:
                    var cdiff = cend - cstart;
                    cstart = State.PreTransformBoxStart + now - SnapIfNeeded(State.MoveStart);
                    cend = cstart + cdiff;
                    break;
                case ResizeHandle.Right:
                    cend.X = now.X;
                    break;
                case ResizeHandle.BottomLeft:
                    cstart.X = now.X;
                    cstart.Y = now.Y;
                    break;
                case ResizeHandle.Bottom:
                    cstart.Y = now.Y;
                    break;
                case ResizeHandle.BottomRight:
                    cend.X = now.X;
                    cstart.Y = now.Y;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            cstart = viewport.Expand(cstart) + viewport.GetUnusedCoordinate(State.BoxStart);
            cend = viewport.Expand(cend) + viewport.GetUnusedCoordinate(State.BoxEnd);
            return Tuple.Create(cstart, cend);
        }

        public override void KeyPress(ViewportBase viewport, KeyPressEventArgs e)
        {

        }

        public override void KeyDown(ViewportBase viewport, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    BoxDrawnConfirm(viewport);
                    break;
                case Keys.Escape:
                    BoxDrawnCancel(viewport);
                    break;
            }
        }

        public virtual void BoxDrawnConfirm(ViewportBase viewport)
        {
            if (State.ActiveViewport != null) State.ActiveViewport.Cursor = Cursors.Default;
            State.Action = BoxAction.ReadyToDraw;
            State.ActiveViewport = null;
        }

        public virtual void BoxDrawnCancel(ViewportBase viewport)
        {
            if (State.ActiveViewport != null) State.ActiveViewport.Cursor = Cursors.Default;
            State.Action = BoxAction.ReadyToDraw;
            State.ActiveViewport = null;
        }

        public override void KeyUp(ViewportBase viewport, KeyEventArgs e)
        {
            // Probably not needed
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

        public override void Render(ViewportBase viewport)
        {
            if (viewport is Viewport2D) Render2D((Viewport2D)viewport);
            if (viewport is Viewport3D) Render3D((Viewport3D)viewport);
        }

        protected virtual bool ShouldDrawBox()
        {
            return State.Action == BoxAction.Drawing
                   || State.Action == BoxAction.Drawn
                   || State.Action == BoxAction.ReadyToResize
                   || State.Action == BoxAction.DownToResize
                   || State.Action == BoxAction.Resizing;
        }

        protected virtual Color GetRenderFillColour()
        {
            return FillColour;
        }

        protected virtual Color GetRenderBoxColour()
        {
            return BoxColour;
        }

        protected virtual Color GetRenderResizeHoverColour()
        {
            return FillColour;
        }

        protected virtual void RenderBox(Viewport2D viewport, Coordinate start, Coordinate end)
        {
            GL.Begin(BeginMode.Quads);
            GL.Color4(GetRenderFillColour());
            Coord(start.DX, start.DY, start.DZ);
            Coord(end.DX, start.DY, start.DZ);
            Coord(end.DX, end.DY, start.DZ);
            Coord(start.DX, end.DY, start.DZ);
            GL.End();
            GL.Begin(BeginMode.LineLoop);
            GL.Color3(GetRenderBoxColour());
            Coord(start.DX, start.DY, start.DZ);
            Coord(end.DX, start.DY, start.DZ);
            Coord(end.DX, end.DY, start.DZ);
            Coord(start.DX, end.DY, start.DZ);
            GL.End();
        }

        protected virtual bool ShouldRenderResizeBox(Viewport2D viewport)
        {
            return State.ActiveViewport == viewport &&
                   (State.Action == BoxAction.ReadyToResize
                    || State.Action == BoxAction.DownToResize);
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
                    y1 = end.DY;
                    y2 = y1 - handleHeight;
                    break;
                case ResizeHandle.Top:
                    x1 = start.DX;
                    x2 = end.DX;
                    y1 = end.DY;
                    y2 = y1 - handleHeight;
                    break;
                case ResizeHandle.TopRight:
                    x1 = end.DX - handleWidth;
                    x2 = end.DX;
                    y1 = end.DY;
                    y2 = y1 - handleHeight;
                    break;
                case ResizeHandle.Left:
                    x1 = start.DX;
                    x2 = x1 + handleWidth;
                    y1 = end.DY;
                    y2 = start.DY;
                    break;
                case ResizeHandle.Center:
                    x1 = start.DX;
                    x2 = end.DX;
                    y1 = end.DY;
                    y2 = start.DY;
                    break;
                case ResizeHandle.Right:
                    x1 = end.DX - handleWidth;
                    x2 = end.DX;
                    y1 = end.DY;
                    y2 = start.DY;
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

        protected virtual void RenderResizeBox(Viewport2D viewport, Coordinate start, Coordinate end)
        {
            var box = GetRenderResizeBox(start, end);
            GL.Begin(BeginMode.Quads);
            GL.Color4(FillColour);
            double x1 = box[0], y1 = box[1], x2 = box[2], y2 = box[3];
            Coord(x1, y1, 0);
            Coord(x2, y1, 0);
            Coord(x2, y2, 0);
            Coord(x1, y2, 0);
            GL.End();
        }

        protected virtual void Render2D(Viewport2D viewport)
        {
            if (State.Action == BoxAction.ReadyToDraw || State.Action == BoxAction.DownToDraw) return;
            var start = viewport.Flatten(State.BoxStart);
            var end = viewport.Flatten(State.BoxEnd);
            if (ShouldDrawBox())
            {
                RenderBox(viewport, start, end);
            }
            if (ShouldRenderResizeBox(viewport))
            {
                RenderResizeBox(viewport, start, end);
            }
        }

        protected virtual bool ShouldDraw3DBox()
        {
            return State.Action != BoxAction.ReadyToDraw;
        }

        protected virtual void Render3DBox(Viewport3D viewport, Coordinate start, Coordinate end)
        {
            var box = new Box(start, end);
            TextureHelper.DisableTexturing();
            GL.Begin(BeginMode.Lines);
            GL.Color4(GetRenderBoxColour());
            foreach (var line in box.GetBoxLines())
            {
                Coord(line.Start);
                Coord(line.End);
            }
            GL.End();
            TextureHelper.EnableTexturing();
        }

        protected virtual void Render3D(Viewport3D viewport)
        {
            if (State.Action == BoxAction.ReadyToDraw || State.Action == BoxAction.DownToDraw) return;
            if (ShouldDraw3DBox())
            {
                Render3DBox(viewport, State.BoxStart, State.BoxEnd);
            }
        }
        #endregion

        public override void UpdateFrame(ViewportBase viewport)
        {
            //TODO: Drag-scrolling
        }

        public override void MouseEnter(ViewportBase viewport, EventArgs e)
        {
            if (State.ActiveViewport != null) State.ActiveViewport.Cursor = Cursors.Default;
        }

        public override void MouseLeave(ViewportBase viewport, EventArgs e)
        {
            if (State.ActiveViewport != null) State.ActiveViewport.Cursor = Cursors.Default;
        }

        protected bool GetSelectionBox(out Box boundingbox)
        {
            // If one of the dimensions has a depth value of 0, extend it out into infinite space
            // If two or more dimensions have depth 0, do nothing.

            var sameX = State.BoxStart.X == State.BoxEnd.X;
            var sameY = State.BoxStart.Y == State.BoxEnd.Y;
            var sameZ = State.BoxStart.Z == State.BoxEnd.Z;
            var start = State.BoxStart.Clone();
            var end = State.BoxEnd.Clone();
            var invalid = false;

            if (sameX)
            {
                if (sameY || sameZ) invalid = true;
                start.X = Decimal.MinValue;
                end.X = Decimal.MaxValue;
            }

            if (sameY)
            {
                if (sameZ) invalid = true;
                start.Y = Decimal.MinValue;
                end.Y = Decimal.MaxValue;
            }

            if (sameZ)
            {
                start.Z = Decimal.MinValue;
                end.Z = Decimal.MaxValue;
            }

            boundingbox = new Box(start, end);
            return !invalid;
        }
    }
}
