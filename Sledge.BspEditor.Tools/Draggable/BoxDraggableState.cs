using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LogicAndTrick.Oy;
using OpenTK;
using Sledge.BspEditor.Rendering;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.BspEditor.Tools.Draggable
{
    public class BoxDraggableState : BaseDraggable, IDraggableState
    {
        public BaseDraggableTool Tool { get; set; }

        public Color BoxColour { get; set; }
        public Color FillColour { get; set; }
        public Box RememberedDimensions { get; set; }
        internal BoxState State { get; set; }
        public bool Stippled { get; set; }

        protected IDraggable[] BoxHandles { get; set; }

        public BoxDraggableState(BaseDraggableTool tool)
        {
            Tool = tool;
            State = new BoxState();
            State.Changed += BoxStateChanged;

            RememberedDimensions = null;

            CreateBoxHandles();
        }

        private void BoxStateChanged(object sender, EventArgs e)
        {
            var box = State.Action == BoxAction.Idle || State.Start == null || State.End == null ? Box.Empty : new Box(State.Start, State.End);

            var label = "";
            if (box != null && !box.IsEmpty()) label = box.Width.ToString("0") + " x " + box.Length.ToString("0") + " x " + box.Height.ToString("0");
            Oy.Publish("MapDocument:Status:UpdateText", label);
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

        public override void Click(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            State.Action = BoxAction.Idle;
        }

        public override bool CanDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            return true;
        }

        public override void Highlight(MapViewport viewport)
        {
            //
        }

        public override void Unhighlight(MapViewport viewport)
        {
            //
        }

        public override void StartDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            State.Viewport = viewport;
            State.Action = BoxAction.Drawing;
            State.OrigStart = State.Start;
            State.OrigEnd = State.End;
            var st = RememberedDimensions == null ? Coordinate.Zero : viewport.GetUnusedCoordinate(RememberedDimensions.Start);
            var wid = RememberedDimensions == null ? Coordinate.Zero : viewport.GetUnusedCoordinate(RememberedDimensions.End - RememberedDimensions.Start);
            State.Start = Tool.SnapIfNeeded(viewport.Expand(position) + st);
            State.End = State.Start + wid;
            base.StartDrag(viewport, e, position);
        }

        public override void Drag(MapViewport viewport, ViewportEvent e, Coordinate lastPosition, Coordinate position)
        {
            State.End = Tool.SnapIfNeeded(viewport.Expand(position)) + viewport.GetUnusedCoordinate(State.End);
            base.Drag(viewport, e, lastPosition, position);
        }

        public override void EndDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            State.Viewport = null;
            State.Action = BoxAction.Drawn;
            State.End = Tool.SnapIfNeeded(viewport.Expand(position)) + viewport.GetUnusedCoordinate(State.End);
            State.FixBounds();
            base.EndDrag(viewport, e, position);
        }

        public override IEnumerable<SceneObject> GetSceneObjects()
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
                        CameraFlags = CameraFlags.Orthographic,
                        ZIndex = -20 // Put this face underneath the grid because it's semi-transparent
                    };
                    yield return new FaceElement(PositionType.World, Material.Flat(GetRenderBoxColour()), verts)
                    {
                        RenderFlags = RenderFlags.Wireframe,
                        CameraFlags = CameraFlags.Perspective
                    };
                }
            }
        }

        public override IEnumerable<Element> GetViewportElements(MapViewport viewport, PerspectiveCamera camera)
        {
            yield break;
        }

        public override IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            if (ShouldDrawBox())
            {
                var box = new Box(State.Start, State.End);
                foreach (var face in box.GetBoxFaces())
                {
                    var verts = face.Select(x => new Position(x.ToVector3())).ToList();
                    yield return new LineElement(PositionType.World, GetRenderBoxColour(), verts)
                    {
                        Stippled = Stippled,
                        DepthTested = true
                    };
                }
            }

            if (ShouldDrawBoxText())
            {
                foreach (var element in GetBoxTextElements(viewport, State.Start.ToVector3(), State.End.ToVector3()))
                {
                    yield return element;
                }
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
            return ShouldDrawBox(); // todo! && Sledge.Settings.View.DrawBoxText;
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