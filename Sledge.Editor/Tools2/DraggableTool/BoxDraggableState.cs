using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Extensions;
using Sledge.Editor.Rendering;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Rendering.Scenes.Renderables;
using EnableCap = OpenTK.Graphics.OpenGL.EnableCap;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace Sledge.Editor.Tools2.DraggableTool
{
    public class BoxDraggableState : IDraggableState
    {
        public BaseDraggableTool Tool { get; set; }

        public Color BoxColour { get; set; }
        public Color FillColour { get; set; }
        public Box RememberedDimensions { get; set; }
        internal BoxState State { get; set; }

        protected IDraggable[] BoxHandles { get; set; }

        protected TextPrinter _printer;
        protected Font _printerFont;

        public BoxDraggableState(BaseDraggableTool tool)
        {
            Tool = tool;
            State = new BoxState();

            _printer = new TextPrinter(TextQuality.Low);
            _printerFont = new Font(FontFamily.GenericSansSerif, 16, GraphicsUnit.Pixel);
            RememberedDimensions = null;

            CreateBoxHandles();
        }

        protected virtual void CreateBoxHandles()
        {
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

        public virtual IEnumerable<IDraggable> GetDraggables()
        {
            if (State.Action == BoxAction.Idle || State.Action == BoxAction.Drawing) yield break;
            foreach (var draggable in BoxHandles)
            {
                yield return draggable;
            }
            // 
        }

        public virtual void Click(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            State.Action = BoxAction.Idle;
        }

        public virtual bool CanDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            return true;
        }

        public virtual void Highlight(MapViewport viewport)
        {
            //
        }

        public virtual void Unhighlight(MapViewport viewport)
        {
            //
        }

        public virtual void StartDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            State.Viewport = viewport;
            State.Action = BoxAction.Drawing;
            State.OrigStart = State.Start;
            State.OrigEnd = State.End;
            var st = RememberedDimensions == null ? Coordinate.Zero : viewport.GetUnusedCoordinate(RememberedDimensions.Start);
            var wid = RememberedDimensions == null ? Coordinate.Zero : viewport.GetUnusedCoordinate(RememberedDimensions.End - RememberedDimensions.Start);
            State.Start = Tool.SnapIfNeeded(viewport.Expand(position) + st);
            State.End = State.Start + wid;
        }

        public virtual void Drag(MapViewport viewport, ViewportEvent e, Coordinate lastPosition, Coordinate position)
        {
            State.End = Tool.SnapIfNeeded(viewport.Expand(position)) + viewport.GetUnusedCoordinate(State.End);
        }

        public virtual void EndDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            State.Viewport = null;
            State.Action = BoxAction.Drawn;
            State.End = Tool.SnapIfNeeded(viewport.Expand(position)) + viewport.GetUnusedCoordinate(State.End);
            State.FixBounds();
        }

        public virtual IEnumerable<SceneObject> GetSceneObjects()
        {
            if (State.Action == BoxAction.Idle) yield break;
            var box = new Box(State.Start, State.End);
            if (ShouldDrawBox())
            {
                foreach (var face in box.GetBoxFaces())
                {
                    var verts = face.Select(x => new PositionVertex(new Position(x.ToVector3()), 0, 0)).ToList();
                    yield return new FaceElement(PositionType.World, Material.Flat(GetRenderFillColour()), verts)
                    {
                        RenderFlags = RenderFlags.Polygon,
                        CameraFlags = CameraFlags.Orthographic
                    };
                    yield return new FaceElement(PositionType.World, Material.Flat(GetRenderBoxColour()), verts)
                    {
                        RenderFlags = RenderFlags.Wireframe,
                        AccentColor = GetRenderBoxColour(),
                    };
                }
            }
            //if (ShouldDrawBoxText(viewport))
            //{
            //    RenderBoxText(viewport, start, end);
            //}
        }

        public virtual IEnumerable<Element> GetViewportElements(MapViewport viewport, PerspectiveCamera camera)
        {
            yield break;
        }

        public virtual IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            yield break;
        }

        #region Rendering

        protected virtual bool ShouldDrawBox()
        {
            return State.Action == BoxAction.Drawing
                   || State.Action == BoxAction.Drawn
                   || State.Action == BoxAction.Resizing;
        }

        protected virtual bool ShouldDrawBoxText()
        {
            return ShouldDrawBox();
        }

        protected virtual Color GetRenderFillColour()
        {
            return FillColour;
        }

        protected virtual Color GetRenderBoxColour()
        {
            return BoxColour;
        }

         //protected virtual void RenderBox(MapViewport viewport, Coordinate start, Coordinate end)
         //{
         //    GL.Begin(PrimitiveType.Quads);
         //    GL.Color4(GetRenderFillColour());
         //    Coord(start.DX, start.DY, start.DZ);
         //    Coord(end.DX, start.DY, start.DZ);
         //    Coord(end.DX, end.DY, start.DZ);
         //    Coord(start.DX, end.DY, start.DZ);
         //    GL.End();
         //
         //    if (Sledge.Settings.View.DrawBoxDashedLines)
         //    {
         //        GL.LineStipple(4, 0xAAAA);
         //        GL.Enable(EnableCap.LineStipple);
         //    }
         //
         //    GL.Begin(PrimitiveType.LineLoop);
         //    GL.Color3(GetRenderBoxColour());
         //    Coord(start.DX, start.DY, start.DZ);
         //    Coord(end.DX, start.DY, start.DZ);
         //    Coord(end.DX, end.DY, start.DZ);
         //    Coord(start.DX, end.DY, start.DZ);
         //    GL.End();
         //    GL.Disable(EnableCap.LineStipple);
         //}

        protected void RenderBoxText(MapViewport viewport, Coordinate boxStart, Coordinate boxEnd)
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

        #endregion
    }
}