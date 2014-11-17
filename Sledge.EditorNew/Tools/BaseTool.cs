using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK.Input;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.EditorNew.Documents;
using Sledge.EditorNew.UI.Viewports;
using Sledge.Extensions;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Structures;
using Sledge.Settings;

namespace Sledge.EditorNew.Tools
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
        /*
        protected Coordinate SnapToSelection(Coordinate c, IViewport2D vp)
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
        */
        protected Coordinate GetNudgeValue(Key key, bool ctrl)
        {
            if (!Select.ArrowKeysNudgeSelection) return null;
            var gridoff = Select.NudgeStyle == NudgeStyle.GridOffCtrl;
            var grid = (gridoff && !ctrl) || (!gridoff && ctrl);
            var val = grid ? Document.Map.GridSpacing : Select.NudgeUnits;
            switch (key)
            {
                case Key.Left:
                    return new Coordinate(-val, 0, 0);
                case Key.Right:
                    return new Coordinate(val, 0, 0);
                case Key.Up:
                    return new Coordinate(0, val, 0);
                case Key.Down:
                    return new Coordinate(0, -val, 0);
            }
            return null;
        }

        protected Document Document { get; set; }
        public IMapViewport Viewport { get; set; }
        public ToolUsage Usage { get; set; }

        public abstract IEnumerable<string> GetContexts();
        public abstract Image GetIcon();
        public abstract string GetName();
        public abstract HotkeyTool? GetHotkeyToolType();

        public virtual string GetNameTextKey()
        {
            return "Tools/" + GetName() + "/Name";
        }

        public virtual string GetContextualHelpTextKey()
        {
            return "Tools/" + GetName() + "/ContextualHelp";
        }

        public virtual IEnumerable<ToolSidebarControl> GetSidebarControls()
        {
            yield break;
        }

        protected BaseTool()
        {
            Viewport = null;
            Usage = ToolUsage.View2D;
        }

        public void SetDocument(Document document)
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

        public abstract void MouseEnter(IMapViewport viewport, ViewportEvent e);
        public abstract void MouseLeave(IMapViewport viewport, ViewportEvent e);
        public abstract void MouseDown(IMapViewport viewport, ViewportEvent e);
        public abstract void MouseClick(IMapViewport viewport, ViewportEvent e);
        public abstract void MouseDoubleClick(IMapViewport viewport, ViewportEvent e);
        public abstract void MouseUp(IMapViewport viewport, ViewportEvent e);
        public abstract void MouseWheel(IMapViewport viewport, ViewportEvent e);
        public abstract void MouseMove(IMapViewport viewport, ViewportEvent e);
        public abstract void DragStart(IMapViewport viewport, ViewportEvent e);
        public abstract void DragMove(IMapViewport viewport, ViewportEvent e);
        public abstract void DragEnd(IMapViewport viewport, ViewportEvent e);
        public abstract void KeyPress(IMapViewport viewport, ViewportEvent e);
        public abstract void KeyDown(IMapViewport viewport, ViewportEvent e);
        public abstract void KeyUp(IMapViewport viewport, ViewportEvent e);
        public abstract void UpdateFrame(IMapViewport viewport, Frame frame);
        public abstract void Render(IMapViewport viewport);

        public virtual void PositionChanged(IMapViewport viewport, ViewportEvent e)
        {
            return;
        }

        public virtual void PreRender(IMapViewport viewport)
        {
            return;
        }

        public virtual bool IsCapturingMouseWheel()
        {
            return false;
        }

        /// <summary>
        /// Intercepts a document hotkey. Returns false if the hotkey should not be executed.
        /// </summary>
        /// <param name="hotkeyMessage">The hotkey message</param>
        /// <param name="parameters">The hotkey parameters</param>
        /// <returns>False to prevent execution of the document hotkey</returns>
        public abstract HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters);

        /*
        public virtual void OverrideViewportContextMenu(ViewportContextMenu menu, IMapViewport vp, ViewportEvent e)
        {
            // Default: nothing...
        }
        */
    }

    public enum HotkeyInterceptResult
    {
        Continue,
        Abort,
        SwitchToSelectTool
    }

    public class ToolSidebarControl
    {
        public IControl Control { get; set; }
        public string TextKey { get; set; }
    }
}
