using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Rendering;
using Sledge.Editor.UI;
using Sledge.Extensions;
using Sledge.Rendering;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Settings;
using System.Drawing;
using Sledge.Rendering.Cameras;

namespace Sledge.Editor.Tools
{
    public abstract class BaseTool : IMediatorListener
    {
        public enum ToolUsage
        {
            View2D,
            View3D,
            Both
        }

        public Coordinate SnapIfNeeded(Coordinate c)
        {
            return Document.Snap(c);
        }

        protected Coordinate SnapToSelection(Coordinate c, MapViewport vp)
        {
            if (!Document.Map.SnapToGrid) return c;

            var snap = (Select.SnapStyle == SnapStyle.SnapOnAlt && KeyboardState.Alt) ||
                       (Select.SnapStyle == SnapStyle.SnapOffAlt && !KeyboardState.Alt);

            if (!snap) return c;

            var snapped = c.Snap(Document.Map.GridSpacing);
            if (Document.Selection.InFaceSelection || Document.Selection.IsEmpty()) return snapped;

            // Try and snap the the selection box center
            var selBox = Document.Selection.GetSelectionBoundingBox();
            var selCenter = vp.Flatten(selBox.Center);
            if (DMath.Abs(selCenter.X - c.X) < selBox.Width / 10 && DMath.Abs(selCenter.Y - c.Y) < selBox.Height / 10) return selCenter;

            var objects = Document.Selection.GetSelectedObjects().ToList();

            // Try and snap to an object center
            foreach (var mo in objects)
            {
                if (!(mo is Entity) && !(mo is Solid)) continue;
                var center = vp.Flatten(mo.BoundingBox.Center);
                if (DMath.Abs(center.X - c.X) >= mo.BoundingBox.Width / 10) continue;
                if (DMath.Abs(center.Y - c.Y) >= mo.BoundingBox.Height / 10) continue;
                return center;
            }

            // Get all the edges of the selected objects
            var lines = objects.SelectMany(x =>
            {
                if (x is Entity) return x.BoundingBox.GetBoxLines();
                if (x is Solid) return ((Solid) x).Faces.SelectMany(f => f.GetLines());
                return new Line[0];
            }).Select(x => new Line(vp.Flatten(x.Start), vp.Flatten(x.End))).ToList();

            // Try and snap to an edge
            var closest = snapped;
            foreach (var line in lines)
            {
                // if the line and the grid are in the same spot, return the snapped point
                if (line.ClosestPoint(snapped).EquivalentTo(snapped)) return snapped;

                // Test for corners and midpoints within a 10% tolerance
                var pointTolerance = (line.End - line.Start).VectorMagnitude() / 10;
                if ((line.Start - c).VectorMagnitude() < pointTolerance) return line.Start;
                if ((line.End - c).VectorMagnitude() < pointTolerance) return line.End;

                var center = (line.Start + line.End) / 2;
                if ((center - c).VectorMagnitude() < pointTolerance) return center;

                // If the line is closer to the grid point, return the line
                var lineSnap = line.ClosestPoint(c);
                if ((closest - c).VectorMagnitude() > (lineSnap - c).VectorMagnitude()) closest = lineSnap;
            }
            return closest;
        }

        protected Coordinate GetNudgeValue(Keys k)
        {
            if (!Select.ArrowKeysNudgeSelection) return null;
            var ctrl = KeyboardState.Ctrl;
            var gridoff = Select.NudgeStyle == NudgeStyle.GridOffCtrl;
            var grid = (gridoff && !ctrl) || (!gridoff && ctrl);
            var val = grid ? Document.Map.GridSpacing : Select.NudgeUnits;
            switch (k)
            {
                case Keys.Left:
                    return new Coordinate(-val, 0, 0);
                case Keys.Right:
                    return new Coordinate(val, 0, 0);
                case Keys.Up:
                    return new Coordinate(0, val, 0);
                case Keys.Down:
                    return new Coordinate(0, -val, 0);
            }
            return null;
        }

        protected Documents.Document Document { get; set; }
        public MapViewport Viewport { get; set; }
        public ToolUsage Usage { get; set; }

        protected bool UseValidation { get; set; }
        private bool _invalidated;
        private List<SceneObject> _currentObjects;

        public abstract Image GetIcon();
        public abstract string GetName();
        public abstract HotkeyTool? GetHotkeyToolType();
        public abstract string GetContextualHelp();

        public virtual IEnumerable<KeyValuePair<string, Control>> GetSidebarControls()
        {
            yield break;
        }

        protected BaseTool()
        {
            Viewport = null;
            Usage = ToolUsage.View2D;
            _invalidated = false;
            UseValidation = false;
            _currentObjects = new List<SceneObject>();
        }

        public void SetDocument(Documents.Document document)
        {
            Document = document;
            DocumentChanged();
        }

        public virtual void ToolSelected(bool preventHistory)
        {
            // Virtual
        }

        public virtual void ToolDeselected(bool preventHistory)
        {
            // Virtual
        }

        public virtual void DocumentChanged()
        {
            // Virtual
        }

        public virtual void Notify(string message, object data)
        {
            Mediator.ExecuteDefault(this, message, data);
        }

        public virtual void MouseEnter(MapViewport viewport, ViewportEvent e)
        {
            if (viewport.Is2D) MouseEnter(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseEnter(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void MouseLeave(MapViewport viewport, ViewportEvent e)
        {
            if (viewport.Is2D) MouseLeave(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseLeave(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void MouseDown(MapViewport viewport, ViewportEvent e)
        {
            if (viewport.Is2D) MouseDown(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseDown(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void MouseClick(MapViewport viewport, ViewportEvent e)
        {
            if (viewport.Is2D) MouseClick(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseClick(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void MouseDoubleClick(MapViewport viewport, ViewportEvent e)
        {
            if (viewport.Is2D) MouseDoubleClick(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseDoubleClick(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void MouseUp(MapViewport viewport, ViewportEvent e)
        {
            if (viewport.Is2D) MouseUp(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseUp(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void MouseWheel(MapViewport viewport, ViewportEvent e)
        {
            if (viewport.Is2D) MouseWheel(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseWheel(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void MouseMove(MapViewport viewport, ViewportEvent e)
        {
            if (viewport.Is2D) MouseMove(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) MouseMove(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void DragStart(MapViewport viewport, ViewportEvent e)
        {
            if (viewport.Is2D) DragStart(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) DragStart(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void DragMove(MapViewport viewport, ViewportEvent e)
        {
            if (viewport.Is2D) DragMove(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) DragMove(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void DragEnd(MapViewport viewport, ViewportEvent e)
        {
            if (viewport.Is2D) DragEnd(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) DragEnd(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void KeyPress(MapViewport viewport, ViewportEvent e)
        {
            if (viewport.Is2D) KeyPress(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) KeyPress(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void KeyDown(MapViewport viewport, ViewportEvent e)
        {
            if (viewport.Is2D) KeyDown(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) KeyDown(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void KeyUp(MapViewport viewport, ViewportEvent e)
        {
            if (viewport.Is2D) KeyUp(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
            if (viewport.Is3D) KeyUp(viewport, viewport.Viewport.Camera as PerspectiveCamera, e);
        }

        public virtual void UpdateFrame(MapViewport viewport, Frame frame)
        {
            Validate();
            if (viewport.Is2D) UpdateFrame(viewport, viewport.Viewport.Camera as OrthographicCamera, frame);
            if (viewport.Is3D) UpdateFrame(viewport, viewport.Viewport.Camera as PerspectiveCamera, frame);
        }

        public virtual void PositionChanged(MapViewport viewport, ViewportEvent e)
        {
            if (viewport.Is2D) PositionChanged(viewport, viewport.Viewport.Camera as OrthographicCamera, e);
        }

        public virtual void ZoomChanged(MapViewport viewport, ViewportEvent e)
        {
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
        protected virtual void UpdateFrame(MapViewport viewport, OrthographicCamera camera, Frame frame) { }
        protected virtual void UpdateFrame(MapViewport viewport, PerspectiveCamera camera, Frame frame) { }

        public virtual void PositionChanged(MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }
        public virtual void ZoomChanged(MapViewport viewport, OrthographicCamera camera, ViewportEvent e) { }

        public virtual bool IsCapturingMouseWheel()
        {
            return false;
        }

        protected void Invalidate()
        {
            _invalidated = true;
        }

        private void Validate()
        {
            if ((UseValidation && !_invalidated) || Document == null) return;
            _invalidated = false;

            foreach (var o in _currentObjects) Document.Scene.Remove(o);

            _currentObjects = GetSceneObjects().ToList();

            foreach (var o in _currentObjects) Document.Scene.Add(o);
        }

        protected virtual IEnumerable<SceneObject> GetSceneObjects()
        {
            yield break;
        }

        /// <summary>
        /// Intercepts a document hotkey. Returns false if the hotkey should not be executed.
        /// </summary>
        /// <param name="hotkeyMessage">The hotkey message</param>
        /// <param name="parameters">The hotkey parameters</param>
        /// <returns>False to prevent execution of the document hotkey</returns>
        public abstract HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters);

        public virtual void OverrideViewportContextMenu(ViewportContextMenu menu, MapViewport vp, ViewportEvent e)
        {
            // Default: nothing...
        }
    }

    public enum HotkeyInterceptResult
    {
        Continue,
        Abort,
        SwitchToSelectTool
    }
}
