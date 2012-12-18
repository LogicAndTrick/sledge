using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.Transformations;
using Sledge.Editor.Editing;
using Sledge.Editor.Properties;
using Sledge.Editor.Rendering;
using Sledge.UI;

namespace Sledge.Editor.Tools
{
    abstract class TransformationTool : BaseBoxTool
    {
        protected bool Transforming { get; set; }
        protected Matrix4d? CurrentTransform { get; set; }

        protected abstract Matrix4d? GetTransformationMatrix(Viewport2D viewport, MouseEventArgs mouseEventArgs);
        protected abstract bool RenderCircleHandles { get; }
        protected abstract bool AllowCenterHandle { get; }
        protected abstract bool FilterHandle(ResizeHandle handle);

        protected override Color BoxColour
        {
            get { return Color.Yellow; }
        }

        protected override Color FillColour
        {
            get { return Color.FromArgb(128, Color.Gray); }
        }

        public override void ToolSelected()
        {
            Document.UpdateSelectLists();
            UpdateBoxBasedOnSelection();
        }

        /// <summary>
        /// Once the resize has completed, we want to apply the current transform to the selection.
        /// </summary>
        /// <param name="viewport">The current viewport</param>
        /// <param name="e">The mouse event</param>
        protected override void LeftMouseUpResizing(Viewport2D viewport, MouseEventArgs e)
        {
            // Execute the transform on the selection
            var transformation = GetTransformationMatrix(viewport, e);
            if (transformation.HasValue)
            {
                ExecuteTransform(CreateMatrixMultTransformation(transformation.Value));
            }
            Document.EndSelectionTransform();
            State.ActiveViewport = null;
            State.Action = BoxAction.Drawn;
            Transforming = false;
            UpdateBoxBasedOnSelection();
        }

        /// <summary>
        /// As the mouse is dragged, update the transformation to the new values
        /// </summary>
        /// <param name="viewport">The active viewport</param>
        /// <param name="e">The mouse event</param>
        protected override void MouseDraggingToResize(Viewport2D viewport, MouseEventArgs e)
        {
            State.Action = BoxAction.Resizing;
            CurrentTransform = GetTransformationMatrix(viewport, e);
            if (!Transforming) // First drag event
            {
                Document.StartSelectionTransform();
                Transforming = true;
            }
            if (CurrentTransform.HasValue)
            {
                Document.SetSelectListTransform(CurrentTransform.Value);
            }
        }

        /// <summary>
        /// Get a list of handles and their standard offset positions
        /// </summary>
        /// <param name="start">The start coordinate of the box</param>
        /// <param name="end">The end coordinate of the box</param>
        /// <param name="zoom">The zoom value of the viewport</param>
        /// <param name="offset">The offset from the box bounds to place the handles</param>
        /// <returns>A list of handles for the box in tuple form: (Handle, X, Y)</returns>
        protected virtual IEnumerable<Tuple<ResizeHandle, decimal, decimal>> GetHandles(Coordinate start, Coordinate end, decimal zoom, decimal offset = 7)
        {
            var half = (end - start) / 2;
            var dist = offset / zoom;

            yield return Tuple.Create(ResizeHandle.TopLeft, start.X - dist, end.Y + dist);
            yield return Tuple.Create(ResizeHandle.TopRight, end.X + dist, end.Y + dist);
            yield return Tuple.Create(ResizeHandle.BottomLeft, start.X - dist, start.Y - dist);
            yield return Tuple.Create(ResizeHandle.BottomRight, end.X + dist, start.Y - dist);

            yield return Tuple.Create(ResizeHandle.Top, start.X + half.X, end.Y + dist);
            yield return Tuple.Create(ResizeHandle.Left, start.X - dist, start.Y + half.Y);
            yield return Tuple.Create(ResizeHandle.Right, end.X + dist, start.Y + half.Y);
            yield return Tuple.Create(ResizeHandle.Bottom, start.X + half.X, start.Y - dist);
        }

        /// <summary>
        /// When the mouse is hovering over the box, do collision tests against the handles and change the cursor if needed.
        /// </summary>
        /// <param name="viewport">The viewport</param>
        /// <param name="e">The mouse event</param>
        protected override void MouseHoverWhenDrawn(Viewport2D viewport, MouseEventArgs e)
        {
            var padding = 7 / viewport.Zoom;

            viewport.Cursor = Cursors.Default;
            State.Action = BoxAction.Drawn;
            State.ActiveViewport = null;

            var now = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            var start = viewport.Flatten(State.BoxStart);
            var end = viewport.Flatten(State.BoxEnd);

            var ccs = new Coordinate(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y), 0);
            var cce = new Coordinate(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y), 0);

            // Check center handle
            if (AllowCenterHandle && now.X > ccs.X && now.X < cce.X && now.Y > ccs.Y && now.Y < cce.Y)
            {
                State.Handle = ResizeHandle.Center;
                State.ActiveViewport = viewport;
                State.Action = BoxAction.ReadyToResize;
                viewport.Cursor = CursorForHandle(State.Handle);
                return;
            }

            // Check other handles
            foreach (var handle in GetHandles(start, end, viewport.Zoom).Where(x => FilterHandle(x.Item1)))
            {
                var x = handle.Item2;
                var y = handle.Item3;
                if (now.X < x - padding || now.X > x + padding || now.Y < y - padding || now.Y > y + padding) continue;
                State.Handle = handle.Item1;
                State.ActiveViewport = viewport;
                State.Action = BoxAction.ReadyToResize;
                viewport.Cursor = CursorForHandle(State.Handle);
                return;
            }
        }

        /// <summary>
        /// Render all the handles as squares or circles depending on class implementation
        /// </summary>
        /// <param name="viewport">The viewport to draw in</param>
        /// <param name="start">The start of the box</param>
        /// <param name="end">The end of the box</param>
        protected void RenderHandles(Viewport2D viewport, Coordinate start, Coordinate end)
        {
            var circles = RenderCircleHandles;

            // Get the filtered list of handles, and convert them to vector locations
            var z = (double)viewport.Zoom;
            var handles = GetHandles(start, end, viewport.Zoom)
                .Where(x => FilterHandle(x.Item1))
                .Select(x => new Vector2d((double)x.Item2, (double)x.Item3))
                .ToList();

            // Draw the insides of the handles in white
            GL.Color3(Color.White);
            foreach (var handle in handles)
            {
                GL.Begin(BeginMode.Polygon);
                if (circles) GLX.Circle(handle, 4, z, loop: true);
                else GLX.Square(handle, 4, z, true);
                GL.End();
            }

            // Draw the borders of the handles in black
            GL.Color3(Color.Black);
            GL.Begin(BeginMode.Lines);
            foreach (var handle in handles)
            {
                if (circles) GLX.Circle(handle, 4, z);
                else GLX.Square(handle, 4, z);
            }
            GL.End();
        }

        /// <summary>
        /// Returns true if the handles should be rendered, false otherwise
        /// </summary>
        /// <param name="viewport">The viewport to draw in</param>
        /// <returns>Whether or not to draw the handles</returns>
        protected bool ShouldRenderHandles(Viewport2D viewport)
        {
            // Only draw the handles if the box exists and we're not currently dragging the mouse
            return State.Action != BoxAction.ReadyToDraw
                   && State.Action != BoxAction.Resizing;
        }

        /// <summary>
        /// Returns true if the resize box should be rendered, false otherwise.
        /// </summary>
        /// <param name="viewport">The viewport to draw in</param>
        /// <returns>Whether or not the draw the resize box</returns>
        protected override bool ShouldRenderResizeBox(Viewport2D viewport)
        {
            // Only draw the resize box when we are ready to resize the center handle
            return State.Action == BoxAction.ReadyToResize
                && State.Handle == ResizeHandle.Center;
        }

        /// <summary>
        /// Renders the current transform if it exists, otherwise just renders the default view
        /// </summary>
        /// <param name="viewport">The viewport to render in</param>
        protected override void Render2D(Viewport2D viewport)
        {
            var start = viewport.Flatten(State.BoxStart);
            var end = viewport.Flatten(State.BoxEnd);

            Matrix4d mat;
            GL.GetDouble(GetPName.ProjectionMatrix, out mat);

            // If transforming in the viewport, push the matrix transformation to the stack
            if (viewport == State.ActiveViewport && State.Action == BoxAction.Resizing && CurrentTransform.HasValue)
            {
                start = viewport.Flatten(State.PreTransformBoxStart);
                end = viewport.Flatten(State.PreTransformBoxEnd);

                var dir = DisplayListGroup.GetMatrixFor(viewport.Direction);
                var inv = Matrix4d.Invert(dir);
                GL.MultMatrix(ref dir);
                var transform = CurrentTransform.Value;
                GL.MultMatrix(ref transform);
                GL.MultMatrix(ref inv);
            }
            
            if (ShouldDrawBox())
            {
                RenderBox(viewport, start, end);
            }

            if (ShouldRenderResizeBox(viewport))
            {
                RenderResizeBox(viewport, start, end);
            }

            // Restore the untransformed matrix
            GL.LoadMatrix(ref mat);

            if (ShouldRenderHandles(viewport))
            {
                RenderHandles(viewport, start, end);
            }
        }

        /// <summary>
        /// Updates the box based on the currently selected objects.
        /// </summary>
        protected virtual void UpdateBoxBasedOnSelection()
        {
            State.Action = BoxAction.ReadyToDraw;
            decimal x1 = Decimal.MaxValue, y1 = Decimal.MaxValue, z1 = Decimal.MaxValue;
            decimal x2 = Decimal.MinValue, y2 = Decimal.MinValue, z2 = Decimal.MinValue;
            foreach (var c in Selection.GetSelectedObjects())
            {
                var min = c.BoundingBox.Start;
                var max = c.BoundingBox.End;

                x1 = Math.Min(x1, min.X);
                y1 = Math.Min(y1, min.Y);
                z1 = Math.Min(z1, min.Z);

                x2 = Math.Max(x2, max.X);
                y2 = Math.Max(y2, max.Y);
                z2 = Math.Max(z2, max.Z);

                State.Action = BoxAction.Drawn;
            }
            State.BoxStart = new Coordinate(x1, y1, z1);
            State.BoxEnd = new Coordinate(x2, y2, z2);
        }

        /// <summary>
        /// Runs the transform on all the currently selected objects
        /// </summary>
        /// <param name="transform">The transformation to apply</param>
        protected void ExecuteTransform(IUnitTransformation transform)
        {
            foreach (var o in Selection.GetSelectedObjects().Where(o => o.Parent == null || !o.Parent.IsSelected))
            {
                o.Transform(transform);
            }
        }

        /// <summary>
        /// Convert a Matrix4d into a unit transformation object
        /// TODO: Move this somewhere better (extension method?)
        /// </summary>
        /// <param name="mat">The matrix to convert</param>
        /// <returns>The unit transformation representation of the matrix</returns>
        protected IUnitTransformation CreateMatrixMultTransformation(Matrix4d mat)
        {
            var dmat = new[]
                           {
                               (decimal) mat.M11, (decimal) mat.M21, (decimal) mat.M31, (decimal) mat.M41,
                               (decimal) mat.M12, (decimal) mat.M22, (decimal) mat.M32, (decimal) mat.M42,
                               (decimal) mat.M13, (decimal) mat.M23, (decimal) mat.M33, (decimal) mat.M43,
                               (decimal) mat.M14, (decimal) mat.M24, (decimal) mat.M34, (decimal) mat.M44,
                           };
            return new UnitMatrixMult(dmat);
        }

        // Prevent some operations from having effect

        protected override void LeftMouseDownToDraw(Viewport2D viewport, MouseEventArgs e)
        {
            // No drawing allowed
        }

        protected override void LeftMouseClick(Viewport2D viewport, MouseEventArgs e)
        {
            // Nothing
        }

        public override void BoxDrawnConfirm(ViewportBase viewport)
        {
            // Not allowed
        }

        public override void BoxDrawnCancel(ViewportBase viewport)
        {
            // Not allowed
        }
    }
}
