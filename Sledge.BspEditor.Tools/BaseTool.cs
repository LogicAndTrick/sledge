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
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;
using Sledge.Shell.Input;

namespace Sledge.BspEditor.Tools
{
    public abstract class BaseTool : ITool
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

        public Vector3 SnapIfNeeded(Vector3 c)
        {
            var gridData = GetDocument()?.Map.Data.GetOne<GridData>();
            var snap = !KeyboardState.Alt && gridData?.SnapToGrid == true;
            var grid = gridData?.Grid;
            return snap && grid != null ? grid.Snap(c) : c.Snap(1);
        }

        public Vector3 SnapToSelection(Vector3 c, OrthographicCamera camera)
        {
            var doc = GetDocument();
            var gridData = doc?.Map.Data.GetOne<GridData>();
            var snap = !KeyboardState.Alt && gridData?.SnapToGrid == true;
            if (doc == null || !snap) return c.Snap(1);

            var snapped = gridData.Grid?.Snap(c) ?? c.Snap(1);
            if (doc.Selection.IsEmpty) return snapped;

            // Try and snap the the selection box center
            var selBox = doc.Selection.GetSelectionBoundingBox();
            var selCenter = camera.Flatten(selBox.Center);
            if (Math.Abs(selCenter.X - c.X) < selBox.Width / 10 && Math.Abs(selCenter.Y - c.Y) < selBox.Height / 10) return selCenter;

            var objects = doc.Selection.ToList();

            // Try and snap to an object center
            foreach (var mo in objects)
            {
                if (mo is Group || mo is Root) continue;
                var center = camera.Flatten(mo.BoundingBox.Center);
                if (Math.Abs(center.X - c.X) >= mo.BoundingBox.Width / 10f) continue;
                if (Math.Abs(center.Y - c.Y) >= mo.BoundingBox.Height / 10f) continue;
                return center;
            }

            // Get all the edges of the selected objects
            var lines = objects
                .SelectMany(x => x.GetPolygons())
                .SelectMany(x => x.GetLines())
                .Select(x => new Line(camera.Flatten(x.Start), camera.Flatten(x.End)))
                .ToList();

            // Try and snap to an edge
            var closest = snapped;
            foreach (var line in lines)
            {
                // if the line and the grid are in the same spot, return the snapped point
                if (line.ClosestPoint(snapped).EquivalentTo(snapped)) return snapped;

                // Test for corners and midpoints within a 10% tolerance
                var pointTolerance = (line.End - line.Start).Length() / 10;
                if ((line.Start - c).Length() < pointTolerance) return line.Start;
                if ((line.End - c).Length() < pointTolerance) return line.End;

                var center = (line.Start + line.End) / 2;
                if ((center - c).Length() < pointTolerance) return center;

                // If the line is closer to the grid point, return the line
                var lineSnap = line.ClosestPoint(c);
                if ((closest - c).Length() > (lineSnap - c).Length()) closest = lineSnap;
            }
            return closest;
        }

        protected Vector3? GetNudgeValue(Keys k)
        {
            var gridData = GetDocument()?.Map.Data.GetOne<GridData>();
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
        
        private readonly WeakReference<MapDocument> _document;
        public MapDocument GetDocument() => _document.TryGetTarget(out var doc) ? doc : null;
        
        public ToolUsage Usage { get; protected set; }
        public bool Active { get; set; }

        protected List<BaseTool> Children { get; private set; }

        public abstract Image GetIcon();
        public abstract string GetName();

        protected BaseTool()
        {
            _document = new WeakReference<MapDocument>(null);
            Active = true;
            Usage = ToolUsage.View2D;
            Children = new List<BaseTool>();

            Oy.Subscribe<IDocument>("Document:Activated", id => SetDocument(id as MapDocument));
            Oy.Subscribe<IContext>("Context:Changed", c => ContextChanged(c));
        }

        protected virtual void ContextChanged(IContext context)
        {
            // Virtual
        }

        protected void SetDocument(MapDocument document)
        {
            _document.SetTarget(document);
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
            return Task.CompletedTask;
        }

        public virtual Task ToolDeselected()
        {
            _subscriptions?.ForEach(x => x.Dispose());
            return Task.CompletedTask;
        }

        protected virtual void DocumentChanged()
        {
            // Virtual
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

        public void MouseEnter(MapViewport viewport, ViewportEvent e)
        {
            var document = _document.TryGetTarget(out var doc) ? doc : null;
            if (document == null) return;

            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.MouseEnter(vp, ev), viewport, e)) return;

            if (viewport.Is2D) MouseEnter(document, viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseEnter(document, viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public void MouseLeave(MapViewport viewport, ViewportEvent e)
        {
            var document = _document.TryGetTarget(out var doc) ? doc : null;
            if (document == null) return;

            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.MouseLeave(vp, ev), viewport, e)) return;

            if (viewport.Is2D) MouseLeave(document, viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseLeave(document, viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public void MouseDown(MapViewport viewport, ViewportEvent e)
        {
            var document = _document.TryGetTarget(out var doc) ? doc : null;
            if (document == null) return;

            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.MouseDown(vp, ev), viewport, e)) return;

            if (viewport.Is2D) MouseDown(document, viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseDown(document, viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public void MouseClick(MapViewport viewport, ViewportEvent e)
        {
            var document = _document.TryGetTarget(out var doc) ? doc : null;
            if (document == null) return;

            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.MouseClick(vp, ev), viewport, e)) return;
            if (viewport.Is2D) MouseClick(document, viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseClick(document, viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public void MouseDoubleClick(MapViewport viewport, ViewportEvent e)
        {
            var document = _document.TryGetTarget(out var doc) ? doc : null;
            if (document == null) return;

            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.MouseDoubleClick(vp, ev), viewport, e)) return;

            if (viewport.Is2D) MouseDoubleClick(document, viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseDoubleClick(document, viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public void MouseUp(MapViewport viewport, ViewportEvent e)
        {
            var document = _document.TryGetTarget(out var doc) ? doc : null;
            if (document == null) return;

            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.MouseUp(vp, ev), viewport, e)) return;

            if (viewport.Is2D) MouseUp(document, viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseUp(document, viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public void MouseWheel(MapViewport viewport, ViewportEvent e)
        {
            var document = _document.TryGetTarget(out var doc) ? doc : null;
            if (document == null) return;

            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.MouseWheel(vp, ev), viewport, e)) return;

            if (viewport.Is2D) MouseWheel(document, viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseWheel(document, viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public void MouseMove(MapViewport viewport, ViewportEvent e)
        {
            var document = _document.TryGetTarget(out var doc) ? doc : null;
            if (document == null) return;

            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.MouseMove(vp, ev), viewport, e)) return;

            if (viewport.Is2D) MouseMove(document, viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseMove(document, viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public void DragStart(MapViewport viewport, ViewportEvent e)
        {
            var document = _document.TryGetTarget(out var doc) ? doc : null;
            if (document == null) return;

            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.DragStart(vp, ev), viewport, e)) return;

            if (viewport.Is2D) DragStart(document, viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) DragStart(document, viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public void DragMove(MapViewport viewport, ViewportEvent e)
        {
            var document = _document.TryGetTarget(out var doc) ? doc : null;
            if (document == null) return;

            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.DragMove(vp, ev), viewport, e)) return;

            if (viewport.Is2D) DragMove(document, viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) DragMove(document, viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public void DragEnd(MapViewport viewport, ViewportEvent e)
        {
            var document = _document.TryGetTarget(out var doc) ? doc : null;
            if (document == null) return;

            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.DragEnd(vp, ev), viewport, e)) return;

            if (viewport.Is2D) DragEnd(document, viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) DragEnd(document, viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public void KeyPress(MapViewport viewport, ViewportEvent e)
        {
            var document = _document.TryGetTarget(out var doc) ? doc : null;
            if (document == null) return;

            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.KeyPress(vp, ev), viewport, e)) return;

            if (viewport.Is2D) KeyPress(document, viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) KeyPress(document, viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public void KeyDown(MapViewport viewport, ViewportEvent e)
        {
            var document = _document.TryGetTarget(out var doc) ? doc : null;
            if (document == null) return;

            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.KeyDown(vp, ev), viewport, e)) return;

            if (viewport.Is2D) KeyDown(document, viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) KeyDown(document, viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public void KeyUp(MapViewport viewport, ViewportEvent e)
        {
            var document = _document.TryGetTarget(out var doc) ? doc : null;
            if (document == null) return;

            if (!Active) return;
            if (ChildAction((w, vp, ev) => w.KeyUp(vp, ev), viewport, e)) return;

            if (viewport.Is2D) KeyUp(document, viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) KeyUp(document, viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public void UpdateFrame(MapViewport viewport, long frame)
        {
            var document = _document.TryGetTarget(out var doc) ? doc : null;
            if (document == null) return;

            if (!Active) return;
            foreach (var child in Children) child.UpdateFrame(viewport, frame);

            if (viewport.Is2D) UpdateFrame(document, viewport, viewport.Viewport.Camera as OrthographicCamera, frame);
            if (viewport.Is3D) UpdateFrame(document, viewport, viewport.Viewport.Camera as PerspectiveCamera, frame);
        }

        public bool FilterHotkey(MapViewport viewport, string hotkey, Keys keys)
        {
            var document = _document.TryGetTarget(out var doc) ? doc : null;
            if (document == null) return false;

            if (!Active) return false;
            foreach (var child in Children)
            {
                if (child.FilterHotkey(viewport, hotkey, keys)) return true;
            }

            if (viewport.Is2D) return FilterHotkey(document, viewport, viewport.Viewport.Camera as OrthographicCamera, hotkey, keys);
            if (viewport.Is3D) return FilterHotkey(document, viewport, viewport.Viewport.Camera as PerspectiveCamera, hotkey, keys);
            return false;
        }

        protected virtual void MouseEnter(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void MouseEnter(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void MouseLeave(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void MouseLeave(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void MouseDown(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void MouseDown(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void MouseClick(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void MouseClick(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void MouseDoubleClick(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void MouseDoubleClick(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void MouseUp(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void MouseUp(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void MouseWheel(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void MouseWheel(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void MouseMove(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void MouseMove(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void DragStart(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void DragStart(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void DragMove(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void DragMove(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void DragEnd(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void DragEnd(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void KeyPress(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void KeyPress(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void KeyDown(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void KeyDown(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void KeyUp(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        protected virtual void KeyUp(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e) { }
        protected virtual void UpdateFrame(MapDocument document, MapViewport viewport, OrthographicCamera camera, long frame) { }
        protected virtual void UpdateFrame(MapDocument document, MapViewport viewport, PerspectiveCamera camera, long frame) { }
        protected virtual bool FilterHotkey(MapDocument document, MapViewport viewport, OrthographicCamera camera, string hotkey, Keys keys) => false;
        protected virtual bool FilterHotkey(MapDocument document, MapViewport viewport, PerspectiveCamera camera, string hotkey, Keys keys) => false;

        #endregion

        #region Scene / rendering

        public void Render(BufferBuilder builder, ResourceCollector resourceCollector)
        {
            var document = _document.TryGetTarget(out var doc) ? doc : null;
            if (document == null) return;

            if (!Active) return;

            Render(document, builder, resourceCollector);
        }

        public void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im)
        {
            var document = _document.TryGetTarget(out var doc) ? doc : null;
            if (document == null) return;

            if (!Active) return;

            Render(document, viewport, camera, worldMin, worldMax, im);
        }

        public void Render(IViewport viewport, PerspectiveCamera camera, I2DRenderer im)
        {
            var document = _document.TryGetTarget(out var doc) ? doc : null;
            if (document == null) return;

            if (!Active) return;

            Render(document, viewport, camera, im);
        }

        protected virtual void Render(MapDocument document, BufferBuilder builder, ResourceCollector resourceCollector)
        {
            foreach (var c in Children.Where(x => x.Active))
            {
                c.Render(document, builder, resourceCollector);
            }
        }

        protected virtual void Render(MapDocument document, IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im)
        {
            foreach (var c in Children.Where(x => x.Active))
            {
                c.Render(document, viewport, camera, worldMin, worldMax, im);
            }
        }

        protected virtual void Render(MapDocument document, IViewport viewport, PerspectiveCamera camera, I2DRenderer im)
        {
            foreach (var c in Children.Where(x => x.Active))
            {
                c.Render(document, viewport, camera, im);
            }
        }

        #endregion
    }
}
