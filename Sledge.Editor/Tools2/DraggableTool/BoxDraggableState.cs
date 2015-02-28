using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
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
        }

        public virtual IEnumerable<Element> GetViewportElements(MapViewport viewport, PerspectiveCamera camera)
        {
            yield break;
        }

        public virtual IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            if (!ShouldDrawBoxText()) yield break;

            foreach (var element in GetBoxTextElements(viewport, State.Start.ToVector3(), State.End.ToVector3()))
            {
                yield return element;
            }
        }

        protected IEnumerable<Element> GetBoxTextElements(MapViewport viewport, Vector3 worldStart, Vector3 worldEnd)
        {
            var st = viewport.Viewport.Camera.Flatten(worldStart);
            var en = viewport.Viewport.Camera.Flatten(worldEnd);

            var widthText = (Math.Abs(Math.Round(en.X - st.X, 1))).ToString("0.##");
            var heightText = (Math.Abs(Math.Round(en.Y - st.Y, 1))).ToString("0.##");

            var xval = viewport.Viewport.Camera.Expand(new Vector3((st.X + en.X) / 2, Math.Max(st.Y, en.Y), 0));
            yield return
                new TextElement(PositionType.World, xval, widthText, GetRenderBoxColour())
                {
                    AnchorX = 0.5f,
                    AnchorY = 1,
                    FontSize = 16,
                    ClampToViewport = true,
                    ScreenOffset = new Vector3(0, -10, 0)
                };

            var yval = viewport.Viewport.Camera.Expand(new Vector3(Math.Max(st.X, en.X), (st.Y + en.Y) / 2, 0));
            yield return
                new TextElement(PositionType.World, yval, heightText, GetRenderBoxColour())
                {
                    AnchorX = 0,
                    AnchorY = 0.5f,
                    FontSize = 16,
                    ClampToViewport = true,
                    ScreenOffset = new Vector3(10, 0, 0)
                };
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
            return ShouldDrawBox() && Sledge.Settings.View.DrawBoxText;
        }

        protected virtual Color GetRenderFillColour()
        {
            return FillColour;
        }

        protected virtual Color GetRenderBoxColour()
        {
            return BoxColour;
        }

        #endregion
    }
}