using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;

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

        public override Vector3 Origin => (State.Start + State.End) / 2;

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
            Oy.Publish("MapDocument:ToolStatus:UpdateText", label);
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
            foreach (var draggable in BoxHandles.ToList())
            {
                yield return draggable;
            }
            // 
        }

        public override void Click(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
        {
            State.Action = BoxAction.Idle;
        }

        public override bool CanDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
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

        public override void StartDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
        {
            State.Viewport = viewport;
            State.Action = BoxAction.Drawing;
            State.OrigStart = State.Start;
            State.OrigEnd = State.End;
            var st = RememberedDimensions == null ? Vector3.Zero : camera.GetUnusedCoordinate(RememberedDimensions.Start);
            var wid = RememberedDimensions == null ? Vector3.Zero : camera.GetUnusedCoordinate(RememberedDimensions.End - RememberedDimensions.Start);
            State.Start = Tool.SnapIfNeeded(camera.Expand(position) + st);
            State.End = State.Start + wid;
            base.StartDrag(viewport, camera, e, position);
        }

        public override void Drag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 lastPosition, Vector3 position)
        {
            State.End = Tool.SnapIfNeeded(camera.Expand(position)) + camera.GetUnusedCoordinate(State.End);
            base.Drag(viewport, camera, e, lastPosition, position);
        }

        public override void EndDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
        {
            State.Viewport = null;
            State.Action = BoxAction.Drawn;
            State.End = Tool.SnapIfNeeded(camera.Expand(position)) + camera.GetUnusedCoordinate(State.End);
            State.FixBounds();
            base.EndDrag(viewport, camera, e, position);
        }

        public override void Render(BufferBuilder builder)
        {
            if (ShouldDrawBox())
            {
                // Draw a box around the point
                var c = new Box(State.Start, State.End);

                const uint numVertices = 4 * 6;
                const uint numWireframeIndices = numVertices * 2;

                var points = new VertexStandard[numVertices];
                var indices = new uint[numWireframeIndices];

                var col = GetRenderBoxColour();
                var colour = new Vector4(col.R, col.G, col.B, 255) / 255;

                var vi = 0u;
                var wi = 0u;
                foreach (var face in c.GetBoxFaces())
                {
                    var offs = vi;

                    foreach (var v in face)
                    {
                        points[vi++] = new VertexStandard
                        {
                            Position = v,
                            Colour = colour,
                            Tint = Vector4.One
                        };
                    }

                    // Lines - [0 1] ... [n-1 n] [n 0]
                    for (uint i = 0; i < 4; i++)
                    {
                        indices[wi++] = offs + i;
                        indices[wi++] = offs + (i == 4 - 1 ? 0 : i + 1);
                    }
                }

                var groups = new[]
                {
                    new BufferGroup(PipelineType.Wireframe, CameraType.Perspective, 0, numWireframeIndices)
                };

                builder.Append(points, indices, groups);
            }
        }

        #region Rendering

        protected virtual bool ShouldDrawBox()
        {
            return State.Action == BoxAction.Drawing || State.Action == BoxAction.Drawn || State.Action == BoxAction.Resizing;
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

        public override void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, Graphics graphics)
        {
            if (ShouldDrawBox())
            {
                var start = camera.WorldToScreen(Vector3.Min(State.Start, State.End));
                var end = camera.WorldToScreen(Vector3.Max(State.Start, State.End));
                DrawBox(viewport, camera, graphics, start, end);
            }

            if (ShouldDrawBoxText())
            {
                var start = camera.WorldToScreen(Vector3.Min(State.Start, State.End));
                var end = camera.WorldToScreen(Vector3.Max(State.Start, State.End));
                DrawBoxText(viewport, camera, graphics, start, end);
            }
        }

        protected virtual void DrawBox(IViewport viewport, OrthographicCamera camera, Graphics graphics, Vector3 start, Vector3 end)
        {
            start = Vector3.Max(start, new Vector3(-100, -100, -100));
            end = Vector3.Min(end, new Vector3(viewport.Width + 100, viewport.Height + 100, 100));
            using (var b = new SolidBrush(GetRenderFillColour()))
            {
                graphics.FillRectangle(b, start.X, end.Y, end.X - start.X, start.Y - end.Y);
            }

            using (var p = new Pen(GetRenderBoxColour()))
            {
                graphics.DrawRectangle(p, start.X, end.Y, end.X - start.X, start.Y - end.Y);
            }
        }

        protected virtual void DrawBoxText(IViewport viewport, OrthographicCamera camera, Graphics graphics, Vector3 start, Vector3 end)
        {
            // Don't draw the text at all if the rectangle is entirely outside the viewport
            if (start.X > camera.Width || end.X < 0) return;
            if (start.Y < 0 || end.Y > camera.Height) return;

            // Find the width and height for the given projection
            var st = camera.Flatten(State.Start);
            var en = camera.Flatten(State.End);

            var widthText = (Math.Abs(Math.Round(en.X - st.X, 1))).ToString("0.##");
            var heightText = (Math.Abs(Math.Round(en.Y - st.Y, 1))).ToString("0.##");

            using (var f = new Font(FontFamily.GenericSansSerif, SystemFonts.DefaultFont.Size * 2))
            using (var b = new SolidBrush(GetRenderBoxColour()))
            {
                // Determine the size of the value strings
                var mWidth = graphics.MeasureString(widthText, f);
                var mHeight = graphics.MeasureString(heightText, f);

                const int padding = 6;

                // Ensure the text is clamped inside the viewport
                var vWidth = new Vector3((end.X + start.X - mWidth.Width) / 2, end.Y - mWidth.Height - padding, 0);
                vWidth = Vector3.Clamp(vWidth, Vector3.Zero,
                    new Vector3(camera.Width - mWidth.Width - padding, camera.Height - mHeight.Height - padding, 0));

                var vHeight = new Vector3(end.X + padding, (end.Y + start.Y - mHeight.Height) / 2, 0);
                vHeight = Vector3.Clamp(vHeight, new Vector3(0, mWidth.Height + padding, 0),
                    new Vector3(camera.Width - mWidth.Width - padding, camera.Height - mHeight.Height - padding, 0));

                // Draw the strings
                graphics.DrawString(widthText, f, b, vWidth.X, vWidth.Y);
                graphics.DrawString(heightText, f, b, vHeight.X, vHeight.Y);
            }
        }

        public override void Render(IViewport viewport, PerspectiveCamera camera, Graphics graphics)
        {
            //
        }

        #endregion
    }
}