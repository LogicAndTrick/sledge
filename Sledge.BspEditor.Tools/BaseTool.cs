using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Viewports;
using Sledge.Shell.Input;

namespace Sledge.BspEditor.Tools
{
    public abstract class BaseTool : ITool, IOverlayRenderable
    {
        public enum ToolUsage
        {
            View2D,
            View3D,
            Both
        }

        public Image Icon => GetIcon();
        public string Name => GetName();

        public virtual bool IsInContext(IContext context)
        {
            return context.Get<MapDocument>("ActiveDocument") != null;
        }

        public Vector3 Snap(Vector3 c)
        {
            var gridData = Document.Map.Data.GetOne<GridData>();
            var snap = !KeyboardState.Alt && gridData?.SnapToGrid == true;
            var grid = gridData?.Grid;
            return snap && grid != null ? grid.Snap(c) : c;
        }

        public Vector3 SnapIfNeeded(Vector3 c)
        {
            return Snap(c);
        }

        public Vector3 SnapToSelection(Vector3 c, MapViewport vp)
        {
            var gridData = Document.Map.Data.GetOne<GridData>();
            var snap = !KeyboardState.Alt && gridData?.SnapToGrid == true;
            if (!snap) return c;

            var grid = gridData?.Grid;
            var snapped = grid != null ? grid.Snap(c) : c;

            if (Document.Selection.IsEmpty) return snapped;

            return c;
            // todo !snap

            //// Try and snap the the selection box center
            //var selBox = Document.Selection.GetSelectionBoundingBox();
            //var selCenter = vp.Flatten(selBox.Center);
            //if (DMath.Abs(selCenter.X - c.X) < selBox.Width / 10 && DMath.Abs(selCenter.Y - c.Y) < selBox.Height / 10) return selCenter;

            //var objects = Document.Selection.GetSelectedObjects().ToList();

            //// Try and snap to an object center
            //foreach (var mo in objects)
            //{
            //    if (!(mo is Entity) && !(mo is Solid)) continue;
            //    var center = vp.Flatten(mo.BoundingBox.Center);
            //    if (DMath.Abs(center.X - c.X) >= mo.BoundingBox.Width / 10) continue;
            //    if (DMath.Abs(center.Y - c.Y) >= mo.BoundingBox.Height / 10) continue;
            //    return center;
            //}

            //// Get all the edges of the selected objects
            //var lines = objects.SelectMany(x =>
            //{
            //    if (x is Entity) return x.BoundingBox.GetBoxLines();
            //    if (x is Solid) return ((Solid) x).Faces.SelectMany(f => f.GetLines());
            //    return new Line[0];
            //}).Select(x => new Line(vp.Flatten(x.Start), vp.Flatten(x.End))).ToList();

            //// Try and snap to an edge
            //var closest = snapped;
            //foreach (var line in lines)
            //{
            //    // if the line and the grid are in the same spot, return the snapped point
            //    if (line.ClosestPoint(snapped).EquivalentTo(snapped)) return snapped;

            //    // Test for corners and midpoints within a 10% tolerance
            //    var pointTolerance = (line.End - line.Start).VectorMagnitude() / 10;
            //    if ((line.Start - c).VectorMagnitude() < pointTolerance) return line.Start;
            //    if ((line.End - c).VectorMagnitude() < pointTolerance) return line.End;

            //    var center = (line.Start + line.End) / 2;
            //    if ((center - c).VectorMagnitude() < pointTolerance) return center;

            //    // If the line is closer to the grid point, return the line
            //    var lineSnap = line.ClosestPoint(c);
            //    if ((closest - c).VectorMagnitude() > (lineSnap - c).VectorMagnitude()) closest = lineSnap;
            //}
            //return closest;
        }

        protected Vector3? GetNudgeValue(Keys k)
        {
            var gridData = Document.Map.Data.GetOne<GridData>();
            var useGrid = !KeyboardState.Ctrl && gridData?.SnapToGrid != false;
            var grid = gridData?.Grid;
            var val = grid != null && !useGrid ? grid.AddStep(Vector3.Zero, Vector3.One) : Vector3.One;
            switch (k)
            {
                case Keys.Left:
                    return new Vector3(-val.X, 0, 0);
                case Keys.Right:
                    return new Vector3(val.X, 0, 0);
                case Keys.Up:
                    return new Vector3(0, val.Y, 0);
                case Keys.Down:
                    return new Vector3(0, -val.Y, 0);
            }
            return null;
        }

        public MapDocument Document { get; private set; }
        public MapViewport Viewport { get; set; }
        public ToolUsage Usage { get; set; }
        public bool Active { get; set; }

        protected bool UseValidation { get; set; }

        protected List<BaseTool> Children { get; private set; }

        public abstract Image GetIcon();
        public abstract string GetName();

        protected BaseTool()
        {
            Active = true;
            Viewport = null;
            Usage = ToolUsage.View2D;
            UseValidation = false;
            Children = new List<BaseTool>();

            Oy.Subscribe<IDocument>("Document:Activated", id => SetDocument(id as MapDocument));
            Oy.Subscribe<IContext>("Context:Changed", c => ContextChanged(c));
        }

        protected virtual void ContextChanged(IContext context)
        {

        }

        protected void SetDocument(MapDocument document)
        {
            Document = document;
            foreach (var t in Children) t.SetDocument(document);
            DocumentChanged();
        }
        
        private List<Subscription> _subscriptions;

        protected virtual IEnumerable<Subscription> Subscribe()
        {
            yield break;
        }

        public virtual Task ToolSelected()
        {
            _subscriptions = Subscribe().ToList();
            Invalidate();
            return Task.CompletedTask;
        }

        public virtual Task ToolDeselected()
        {
            _subscriptions?.ForEach(x => x.Dispose());
            Invalidate();
            return Task.CompletedTask;
        }

        protected virtual void DocumentChanged()
        {
            // Virtual
            Invalidate();
        }

        #region Viewport event listeners

        private bool ChildAction(Action<BaseTool, MapViewport, ViewportEvent> action, MapViewport viewport, ViewportEvent ev)
        {
            foreach (var child in Children.Where(x => x.Active))
            {
                action(child, viewport, ev);
                if (ev != null && ev.Handled) return true;
            }
            return false;
        }

        public virtual void MouseEnter(MapViewport viewport, ViewportEvent e)
        {
            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.MouseEnter(vp, ev), viewport, e)) return;
            if (viewport.Is2D) MouseEnter(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseEnter(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void MouseLeave(MapViewport viewport, ViewportEvent e)
        {
            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.MouseLeave(vp, ev), viewport, e)) return;
            if (viewport.Is2D) MouseLeave(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseLeave(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void MouseDown(MapViewport viewport, ViewportEvent e)
        {
            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.MouseDown(vp, ev), viewport, e)) return;
            if (viewport.Is2D) MouseDown(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseDown(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void MouseClick(MapViewport viewport, ViewportEvent e)
        {
            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.MouseClick(vp, ev), viewport, e)) return;
            if (viewport.Is2D) MouseClick(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseClick(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void MouseDoubleClick(MapViewport viewport, ViewportEvent e)
        {
            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.MouseDoubleClick(vp, ev), viewport, e)) return;
            if (viewport.Is2D) MouseDoubleClick(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseDoubleClick(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void MouseUp(MapViewport viewport, ViewportEvent e)
        {
            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.MouseUp(vp, ev), viewport, e)) return;
            if (viewport.Is2D) MouseUp(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseUp(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void MouseWheel(MapViewport viewport, ViewportEvent e)
        {
            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.MouseWheel(vp, ev), viewport, e)) return;
            if (viewport.Is2D) MouseWheel(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseWheel(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void MouseMove(MapViewport viewport, ViewportEvent e)
        {
            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.MouseMove(vp, ev), viewport, e)) return;
            if (viewport.Is2D) MouseMove(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseMove(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void DragStart(MapViewport viewport, ViewportEvent e)
        {
            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.DragStart(vp, ev), viewport, e)) return;
            if (viewport.Is2D) DragStart(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) DragStart(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void DragMove(MapViewport viewport, ViewportEvent e)
        {
            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.DragMove(vp, ev), viewport, e)) return;
            if (viewport.Is2D) DragMove(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) DragMove(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void DragEnd(MapViewport viewport, ViewportEvent e)
        {
            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.DragEnd(vp, ev), viewport, e)) return;
            if (viewport.Is2D) DragEnd(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) DragEnd(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void KeyPress(MapViewport viewport, ViewportEvent e)
        {
            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.KeyPress(vp, ev), viewport, e)) return;
            if (viewport.Is2D) KeyPress(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) KeyPress(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void KeyDown(MapViewport viewport, ViewportEvent e)
        {
            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.KeyDown(vp, ev), viewport, e)) return;
            if (viewport.Is2D) KeyDown(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) KeyDown(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void KeyUp(MapViewport viewport, ViewportEvent e)
        {
            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.KeyUp(vp, ev), viewport, e)) return;
            if (viewport.Is2D) KeyUp(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) KeyUp(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void UpdateFrame(MapViewport viewport, long frame)
        {
            if (!Active) return;
            foreach (var child in Children) child.UpdateFrame(viewport, frame);

            if (viewport.Is2D) UpdateFrame(viewport, viewport.Viewport.Camera as OrthographicCamera, frame);
            if (viewport.Is3D) UpdateFrame(viewport, viewport.Viewport.Camera as PerspectiveCamera, frame);
        }

        public virtual void PositionChanged(MapViewport viewport, ViewportEvent e)
        {
            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.PositionChanged(vp, ev), viewport, e)) return;
            if (viewport.Is2D) PositionChanged(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
        }

        public virtual void ZoomChanged(MapViewport viewport, ViewportEvent e)
        {
            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.ZoomChanged(vp, ev), viewport, e)) return;
            if (viewport.Is2D) ZoomChanged(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
        }

        protected virtual void MouseEnter(MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void MouseEnter(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void MouseLeave(MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void MouseLeave(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void MouseDown(MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void MouseDown(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void MouseClick(MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void MouseClick(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void MouseDoubleClick(MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void MouseDoubleClick(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void MouseUp(MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void MouseUp(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void MouseWheel(MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void MouseWheel(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void MouseMove(MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void MouseMove(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void DragStart(MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void DragStart(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void DragMove(MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void DragMove(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void DragEnd(MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void DragEnd(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void KeyPress(MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void KeyPress(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void KeyDown(MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void KeyDown(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void KeyUp(MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void KeyUp(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void UpdateFrame(MapViewport viewport, OrthographicCamera camera, long frame) { }
        protected virtual void UpdateFrame(MapViewport viewport, PerspectiveCamera camera, long frame) { }

        public virtual void PositionChanged(MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        public virtual void ZoomChanged(MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }

        #endregion

        public virtual bool IsCapturingMouseWheel()
        {
            return false;
        }

        #region Scene / rendering

        protected virtual void Invalidate()
        {
            // todo
        }

        public virtual void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, Graphics graphics)
        {
            if (!Active) return;
            foreach (var c in Children.Where(x => x.Active))
            {
                c.Render(viewport, camera, worldMin, worldMax, graphics);
            }
        }

        public virtual void Render(IViewport viewport, PerspectiveCamera camera, Graphics graphics)
        {
            if (!Active) return;
            foreach (var c in Children.Where(x => x.Active))
            {
                c.Render(viewport, camera, graphics);
            }
        }

        // protected virtual IEnumerable<SceneObject> GetSceneObjects()
        // {
        //     if (!Active) return new SceneObject[0];
        //     return Children.Where(x => x.Active).SelectMany(x => x.GetSceneObjects());
        // }

        #endregion
    }
}
